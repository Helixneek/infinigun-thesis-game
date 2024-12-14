using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BackupWaveFunctionCollapse : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private LevelConfigSO levelConfigFile;
    [SerializeField] private WFC_RoomPositions roomPositioner;

    [Header("Tiles")]
    [SerializeField] private Sprite emptyTile;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private WFC_TileTemplate[] tileTemplates;
    [SerializeField] private float tileWidth;
    [SerializeField] private float tileHeight;

    [Header("Grid")]
    [SerializeField] private int dimension;
    [SerializeField] private Transform gridParent;

    [Header("Room Control")]
    [SerializeField] private int maxRoomCount;
    [Tooltip("Whether to start the map generation from the center or from a random cell")]
    [SerializeField] private bool startFromCenter = true;
    [Tooltip("Maximum number of times to search for a direction before giving up")]
    [SerializeField] private int directionSearchMaxRetries = 10;

    //[Space]
    //[Range(1, 10)]
    //[SerializeField] private int difficultyLevel;
    //[SerializeField] private int maxEnemyRoom;
    //[SerializeField] private int maxTreasureRoom;

    [Header("Debug")]
    [SerializeField] private bool isDebug;

    // Tiles and their locations
    private List<WFC_Tile> _tiles = new List<WFC_Tile>();
    private Dictionary<Vector2, WFC_Tile> _tilesDictionary; // <Vector2, WFC_Tile>

    // Level difficulty
    private int _levelDifficulty;

    // Grid object
    private List<WFC_Cell> _grid;
    // Current room count
    private int _currentRoomCount;
    // Flag for first room
    private bool _firstRoom = true;
    // Type name of latest room
    private BackupRoomType _latestRoomType;

    // Temp copy of grid
    private List<WFC_Cell> _gridCopy;

    // Certain room coordinates
    private (int, int) _startRoomCoordinate;
    private (int, int) _endRoomCoordinate;
    private (int, int) _latestRoomCoordinate;

    // Max room count setup;
    private int _maxEnemyRoomCount = 0;
    private int _maxTreasureRoomCount = 0;
    private int _maxPuzzleRoomCount = 0;
    private int _maxTrapRoomCount = 0;
    private int _maxShopRoomCount = 0;

    // Room count tracking
    private int _currentEnemyRoomCount = 0;
    private int _currentTreasureRoomCount = 0;
    private int _currentPuzzleRoomCount = 0;
    private int _currentTrapRoomCount = 0;
    private int _currentShopRoomCount = 0;

    private List<int> _treasureRoomPositions;
    private List<int> _puzzleRoomPositions;
    private List<int> _trapRoomPositions;
    private List<int> _shopRoomPositions;

    private int[] _safeRoomPositions;

    private int _enemyRoomsSeries = 0;
    private bool _safeRoomNext = false;

    private void Start()
    {
        // Pre-preparation
        SetupLevelConfig();

        // Preparation
        CreateTiles();
        GenerateAdjacencyRules();
        InitializeGrid();

        // Cell collapse
        PerformWaveFunctionCollapse();
    }
    private void SetupLevelConfig()
    {
        // Set level difficulty
        _levelDifficulty = levelConfigFile.levelDifficulty;

        // Set the positions of safe rooms
        _safeRoomPositions = roomPositioner.GetSafeRoomPositions(_levelDifficulty);

        // Set max rooms
        _maxEnemyRoomCount = levelConfigFile.maxEnemyCount;
        _maxTreasureRoomCount = levelConfigFile.maxTreasureCount;
        _maxPuzzleRoomCount = levelConfigFile.maxPuzzleCount;
        _maxTrapRoomCount = levelConfigFile.maxTrapCount;
        _maxShopRoomCount = levelConfigFile.maxShopCount;
    }

    // Actually do the entire collapse process
    private void PerformWaveFunctionCollapse()
    {
        if(!isDebug)
        {
            for (int i = 0; i < dimension * dimension; i++)
            {
                //Debug.Log("WFC run " + i);
                DrawCellTiles();
                ProcessGridCopy();
                CollapseCell();
                RecalculateOptions();
            }
        }
        else
        {
            DrawCellTiles();
            ProcessGridCopy();
            CollapseCell();
            RecalculateOptions();
        }
        
    }

    // Create WFC_Tile objects
    private void CreateTiles()
    {
        for(int i = 0; i < tileTemplates.Length; i++)
        {
            Debug.Log("tile " + i + ": " + tileTemplates[i].name);
            //_tiles.Add(new WFC_Tile(tileTemplates[i].image, tileTemplates[i].up, tileTemplates[i].right, tileTemplates[i].down, tileTemplates[i].left));
        }
        
    }

    // Analyze each tile and generate their adjacency rules
    private void GenerateAdjacencyRules()
    {
        Debug.Log("Generating adjacency rules");

        foreach(WFC_Tile tile in _tiles)
        {
            tile.Analyze(_tiles.ToArray());
        }

        Debug.Log("Adjacency rules generated");
    }

    // Create initial grid of cells
    private void InitializeGrid()
    {
        Debug.Log("Initializing grid of " + (dimension * dimension) + " cells");
        Debug.Log("Tiles length: " + _tiles.Count);

        // Initialize WFC_Cell list
        _grid = new List<WFC_Cell>();

        // Make new cell for every square in the grid
        for(int i = 0; i < dimension * dimension; i++)
        {
            //Debug.Log("Making new cell");
            _grid.Add(new WFC_Cell(_tiles.Count));
            //Debug.Log("New cell options length: " + _grid[i].options.Count);
        }
    }

    // Draw the cells on screen
    private void DrawCellTiles()
    {
        if(gridParent.childCount > 0) DestroyYounglings();

        for(int j = 0; j < dimension; j++)
        {
            for(int i = 0; i < dimension; i++)
            {
                WFC_Cell cell = _grid[i + j * dimension];

                if(cell != null && cell.collapsed)
                {
                    //Debug.Log("Current cell being drawn, options length: " + cell.options.Count);
                    int index = cell.options[0];
                    GameObject obj = Instantiate(tilePrefab, new Vector3(i * tileWidth, j * tileHeight, 0), Quaternion.identity);
                    obj.name = $"Tile {i} {j}";
                    obj.transform.parent = gridParent;

                    //WFC_TilePrefab tile = obj.GetComponent<WFC_TilePrefab>();
                    //tile.spriteRenderer.sprite = tileTemplates[index].image;
                }
                // No actual option yet, still empty tile
                else
                {
                    //GameObject obj = Instantiate(tilePrefab, new Vector3(i * tileWidth, j * tileHeight, 0), Quaternion.identity);
                    //obj.name = $"Tile {i} {j}";
                    //obj.transform.parent = gridParent;

                    //WFC_TilePrefab tile = obj.GetComponent<WFC_TilePrefab>();
                    //tile.spriteRenderer.sprite = emptyTile;
                }
            }
        }
    }

    // Destroy the grid transform parent's children
    private void DestroyYounglings()
    {
        for (int i = gridParent.childCount - 1; i >= 0; i--)
        {
            Destroy(gridParent.GetChild(i).gameObject);
        }
    }

    // Process and filter the gridCopy list first before collapsing a cell
    private void ProcessGridCopy()
    {
        // Debugging
        if (_grid == null) Debug.Log("_grid is null");
        foreach (var cell in _grid)
        {
            if (cell == null) Debug.Log("Found a null cell");
            else if (cell.options == null) Debug.Log("Found a cell with null options");
        }

        // Find cell with least entropy
        _gridCopy = new List<WFC_Cell>(_grid);
        //_gridCopy = _grid.ToList();
        Debug.Log("Grid copy created with " + _gridCopy.Count + " cells");

        // Filter gridCopy so there is only noncollapsed cells
        _gridCopy = _gridCopy.Where(a => !a.collapsed).ToList();
        Debug.Log("Filtered grid copy has " + _gridCopy.Count + " non-collapsed cells");

        // Sort cells by length of options
        _gridCopy.Sort((a, b) => a.options.Count - b.options.Count);
        Debug.Log("Grid copy sorted by options count");

        // Stop function if gridCopy is empty
        if (_gridCopy.Count == 0)
        {
            Debug.Log("Grid copy is empty, stopping function");
            return;
        }

        // Find options with length longer than the lowest
        int len = _gridCopy[0].options.Count;
        int stopIndex = 0;

        for(int i = 0; i < _gridCopy.Count; i++)
        {
            if (_gridCopy[i].options.Count > len)
            {
                stopIndex = i;
                break;
            }
        }

        // If there are two or more options of the same length
        if (stopIndex > 0)
        {
            Debug.Log("Removing " + (_gridCopy.Count - stopIndex) + " cells with options count greater than " + len);
            _gridCopy.RemoveRange(stopIndex, _gridCopy.Count - stopIndex);
        }
        else
        {
            Debug.Log("No cells removed, all options count are " + len);
        }
    }
    
    // Collapse a cell from the gridCopy
    private void CollapseCell()
    {
        WFC_Cell cell = null;

        // Generate the first room
        // Coordinates has to be prepared beforehand because all subsequent cells will be generated from the other cells
        // Loop until the first room is succesfully generated
        while(_firstRoom)
        {
            int x, y;

            // Pick a coordinate
            // Map generation starts from center
            if (startFromCenter)
            {
                x = dimension / 2;
                y = dimension / 2;
                Debug.Log("Starting from center: (" + x + ", " + y + ")");
            }
            // Map generation starts from a random point
            else
            {
                x = UnityEngine.Random.Range(0, dimension);
                y = UnityEngine.Random.Range(0, dimension);
                Debug.Log("Starting from random point: (" + x + ", " + y + ")");
            }

            // Check whether the coordinates chosen is valid
            if (_gridCopy.Contains(_grid[x + y * dimension]))
            {
                cell = _grid[x + y * dimension];
                Debug.Log("Starting from random point: (" + x + ", " + y + ")");

                // Store the coordinates
                _startRoomCoordinate = (x, y);
                _latestRoomCoordinate = _startRoomCoordinate;
                Debug.Log("Set start room coordinate to (" + _startRoomCoordinate + ")");

                // Set the current room as an empty room
                // Do this by making a new options list and only putting empty room as the option
                Debug.Log("Setting current room as empty room at coordinates (" + x + ", " + y + ")");
                cell.options = new List<int> { 1 };
                Debug.Log("Options list for cell at (" + x + ", " + y + ") set to: " + string.Join(", ", cell.options));
                //foreach(var op in cell.options)
                //{
                //    Debug.Log("Options list for cell at (" + x + ", " + y + ") set to: " + op);
                //}

                // Set the last room type as empty
                _latestRoomType = BackupRoomType.Empty;
                Debug.Log("Last room type set to Empty");

                // Increase room count counter
                _currentRoomCount++;
                Debug.Log("Room count counter increased to " + _currentRoomCount);

                // Set first room to false
                _firstRoom = false;
                Debug.Log("First room flag set to false");

                // Collapse the cell
                cell.collapsed = true;
                Debug.Log("Cell at (" + x + ", " + y + ") collapsed");

                DebugShowGridListOptions(_grid);

                break;
            }
        }
        
        // Generate subsequent rooms
        // Also check if the current room is the last room or not
        if(!_firstRoom && _currentRoomCount < maxRoomCount)
        {
            int x = _latestRoomCoordinate.Item1;
            int y = _latestRoomCoordinate.Item2;

            // The direction to move
            int newDirection = SearchForNewDirection(x, y);

            // Choose the new cell using newDirection
            switch(newDirection)
            {
                // Up
                case 0:
                    cell = _grid[(x + (y - 1) * dimension)];
                    _latestRoomCoordinate = (x, y - 1);
                    break;

                // Right
                case 1:
                    cell = _grid[((x + 1) + y * dimension)];
                    _latestRoomCoordinate = (x + 1, y);
                    break;

                // Down
                case 2:
                    cell = _grid[(x + (y + 1) * dimension)];
                    _latestRoomCoordinate = (x, y + 1);
                    break;

                // Left
                case 3:
                    cell = _grid[((x - 1) + y * dimension)];
                    _latestRoomCoordinate = (x - 1, y);
                    break;

                default:
                    Debug.Log("[CollapseCell] How?");
                    break;
            }

            // Choose room type
            // Same method as choosing direction, we constantly loop until we get a valid room
            bool validRoom = false;
            int roomIndex = 1;

            
            // Pick a random safe room position
            //roomIndex = safePos[UnityEngine.Random.Range(0, safePos.Length)];

            while (!validRoom && _currentRoomCount < maxRoomCount - 1)
            {
                // TODO: Pick room type based on set intervals and difficulty level
                switch(_levelDifficulty)
                {
                    // Level 1
                    // Enemy - Safe - Enemy - Safe
                    case 1:
                        // If the new room's index is in the safe positions
                        // It is a safe room
                        if(_safeRoomPositions.Contains(_currentRoomCount))
                        {
                            roomIndex = (int)PickSafeRoomType();

                            _safeRoomPositions = _safeRoomPositions.Where(val => val != _currentRoomCount).ToArray();
                        }
                        // Otherwise, it's an enemy room
                        else
                        {
                            roomIndex = (int)BackupRoomType.Enemy;

                            //var weights = new (int, int)[]
                            //{
                            //    ((int)RoomType.Empty, 70),
                            //    ((int)RoomType.Puzzle, 15),
                            //    ((int)RoomType.Trap, 15)
                            //};
                            //var rng = new WeightedRNG(weights)
                            //roomIndex = rng.GetRandomIntItem();

                        }
                        break;
                }

                // Pick the room type based on the final roomIndex
                switch (roomIndex)
                {
                    // Empty room
                    case 1:
                        _latestRoomType = BackupRoomType.Empty;
                        validRoom = true;
                        break;

                    // Enemy room
                    case 2:
                        _latestRoomType = BackupRoomType.Enemy;
                        _currentEnemyRoomCount++;
                        validRoom = true;
                        break;

                    // Treasure room
                    case 3:
                        _latestRoomType = BackupRoomType.Treasure;
                        _currentTreasureRoomCount++;
                        validRoom = true;
                        break;

                    // Puzzle room
                    case 4: 
                        _latestRoomType = BackupRoomType.Puzzle;
                        _currentPuzzleRoomCount++;
                        validRoom = true;
                        break;

                    // Trap room
                    case 5:
                        _latestRoomType = BackupRoomType.Trap;
                        _currentTrapRoomCount++;
                        validRoom = true;
                        break;

                    // Shop room
                    case 6:
                        _latestRoomType = BackupRoomType.Shop;
                        _currentShopRoomCount++;
                        validRoom = true;
                        break;

                    default:
                        Debug.LogError("CollapseCell: Error when choosing room type");
                        break;
                }
            }

            // Assign new room type to cell
            // Set the final room as a boss room
            if (_currentRoomCount == maxRoomCount - 1)
            {
                cell.options = new List<int> { (int) BackupRoomType.Boss };

                // Set final room coordinate
                _endRoomCoordinate = _latestRoomCoordinate;

                Debug.Log("Final room coords: " + _endRoomCoordinate);
            }
            // Set the current room depending on the room type
            else
            {
                cell.options = new List<int> { roomIndex };
            }

            // Increment room count
            _currentRoomCount++;

            // Collapse the cell
            cell.collapsed = true;
        }

        // Generate walls
        // Only done if enough rooms have been made
        else if(!_firstRoom && _currentRoomCount >= maxRoomCount)
        {
            foreach(WFC_Cell gridcell in _gridCopy)
            {
                // Set cell options to only walls
                gridcell.options = new List<int> { 0 };

                // Set cell as collapsed
                gridcell.collapsed = true;
            }
        }
    }

    // Recalculate entropy and plan the next generation step
    private void RecalculateOptions()
    {
        List<WFC_Cell> nextGrid = new List<WFC_Cell>(new WFC_Cell[_grid.Count]);

        for(int j = 0; j < dimension; j++)
        {
            for(int i = 0; i < dimension; i++)
            {
                int index = i + j * dimension;

                //DebugShowGridListOptions(_grid);

                // If the cell is already collapsed, just set the next tile to itself
                if(_grid[index].collapsed)
                {
                    nextGrid[index] = _grid[index];

                    //WFC_Cell newCell = new WFC_Cell(0);
                    //newCell.collapsed = true;
                    //newCell.options = new List<int> { _grid[index].options[0] }; // Ensure options list has at least one element
                    //nextGrid[index] = newCell;

                    Debug.Log("[RecalculateOptions] Initial grid options: " + _grid[index].options.Count);
                    Debug.Log("[RecalculateOptions] Collapsed next grid options: " + nextGrid[index].options.Count);

                }
                // Else, calculate
                else
                {
                    List<int> options = Enumerable.Range(0, _tiles.Count).ToList();

                    // UP DIRECTION
                    // Ignore topmost row
                    if (j > 0)
                    {
                        WFC_Cell up = _grid[i + (j - 1) * dimension];
                        List<int> validOptions = new List<int>();

                        // Find list of possible options
                        foreach(int option in up.options)
                        {
                            // Choose rule for going down (since this current cell is connecting to the top)
                            List<int> valid = _tiles[option].down;

                            // Merge the lists
                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        if (validOptions.Count > 0)
                        {
                            CheckValid(options, validOptions);
                        }
                    }

                    // RIGHT DIRECTION
                    // Ignore topmost row
                    if (i < dimension - 1)
                    {
                        WFC_Cell right = _grid[(i + 1) + j * dimension];
                        List<int> validOptions = new List<int>();

                        // Find list of possible options
                        foreach (int option in right.options)
                        {
                            // Choose rule for going left (since this current cell is connecting to the right)
                            List<int> valid = _tiles[option].left;

                            // Merge the lists
                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        if (validOptions.Count > 0)
                        {
                            CheckValid(options, validOptions);
                        }
                    }

                    // DOWN DIRECTION
                    // Ignore topmost row
                    if (j < dimension - 1)
                    {
                        WFC_Cell down = _grid[i + (j + 1) * dimension];
                        List<int> validOptions = new List<int>();

                        // Find list of possible options
                        foreach (int option in down.options)
                        {
                            // Choose rule for going up (since this current cell is connecting to the down)
                            List<int> valid = _tiles[option].up;

                            // Merge the lists
                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        if (validOptions.Count > 0)
                        {
                            CheckValid(options, validOptions);
                        }
                    }

                    // LEFT DIRECTION
                    // Ignore topmost row
                    if (i > 0)
                    {
                        WFC_Cell left = _grid[(i - 1) + j * dimension];
                        List<int> validOptions = new List<int>();

                        // Find list of possible options
                        foreach (int option in left.options)
                        {
                            // Choose rule for going right (since this current cell is connecting to the left)
                            List<int> valid = _tiles[option].right;

                            // Merge the lists
                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        if (validOptions.Count > 0)
                        {
                            CheckValid(options, validOptions);
                        }
                    }

                    // Set the next grid cell
                    nextGrid[index] = new WFC_Cell(options.Count);
                    nextGrid[index].options = options;
                }   
            }
        }

        Debug.Log("[RecalculateOptions] Next grid cell check: " + nextGrid[0]);
        Debug.Log("[RecalculateOptions] Initial grid cell check: " + _grid[0]);

        // Paste in the new grid
        _grid = new List<WFC_Cell>(nextGrid);
    }

    // Pick the safe room type
    private BackupRoomType PickSafeRoomType()
    {
        BackupRoomType result = BackupRoomType.Wall;
        int[] choices = new int[] { (int)BackupRoomType.Treasure, (int)BackupRoomType.Puzzle, (int)BackupRoomType.Trap, (int)BackupRoomType.Shop};

        // Keep repeating until a valid room type is obtained
        while(result == BackupRoomType.Wall)
        {
            int index = UnityEngine.Random.Range(0, choices.Length);

            switch (choices[index])
            {
                case (int)BackupRoomType.Treasure:
                    if(_currentTreasureRoomCount >= _maxTreasureRoomCount)
                    {
                        continue;
                    }

                    result = BackupRoomType.Treasure;
                    break;

                case (int)BackupRoomType.Puzzle:
                    if(_currentPuzzleRoomCount >= _maxPuzzleRoomCount)
                    {
                        continue;
                    }

                    result = BackupRoomType.Puzzle;
                    break;

                case (int)BackupRoomType.Trap:
                    if (_currentTrapRoomCount >= _maxTrapRoomCount)
                    {
                        continue;
                    }

                    result = BackupRoomType.Trap;
                    break;

                case (int)BackupRoomType.Shop:
                    if (_currentShopRoomCount >= _maxShopRoomCount)
                    {
                        continue;
                    }

                    result = BackupRoomType.Shop;
                    break;
            }
        }

        return result;
    }

    private void CheckValid(List<int> options, List<int> validOptions)
    {
        //for(int i = 0; i < options.Count; i++)
        //{
        //    int element = options[i];
        //    if(!validOptions.Contains(element))
        //    {
        //        options.RemoveAt(i);
        //        i--;
        //    }
        //}

        options = options.Where(option => validOptions.Contains(option)).ToList();
    }

    // Look around current cell and see which direction is a valid place for a new cell
    private int SearchForNewDirection(int x, int y)
    {
        int result = -1;
        int i = 0;

        // Check if the new direction already has another room or not
        while (result < 0)
        {
            if (i > directionSearchMaxRetries)
            {
                Debug.Log("SearchForNewDirections: Max tries reached");
                break;
            }

            int temp = UnityEngine.Random.Range(0, 4);

            switch (temp)
            {
                // Up direction
                case 0:
                    // Check if the current y is at the edge or not
                    // Don't use this direction of it is
                    if (y == 0)
                    {
                        continue;
                    }

                    // Check if the new direction is empty or not
                    if (_gridCopy.Contains(_grid[(x + (y - 1) * dimension)]) && (y - 1) >= 0)
                    {
                        // Check if the tiles on the left and right are empty or not
                        // This is done so that the rooms aren't clustered together most of the time

                        // Only 1 tile is required if we're adding at the edge of the grid
                        // So we're not trying to read outside the grid
                        if (x == 0 || x == dimension - 1)
                        {
                            if ( ((x + 1) < dimension && _gridCopy.Contains(_grid[((x + 1) + (y - 1) * dimension)]) ) ||
                                ( (x - 1) >= 0 && _gridCopy.Contains(_grid[((x - 1) + (y - 1) * dimension)])) )
                            {
                                result = temp;
                            }
                        }
                        else
                        {
                            // Both are required for everywhere else
                            bool leftValid = (x - 1) >= 0 && _gridCopy.Contains(_grid[(x - 1) + (y - 1) * dimension]);
                            bool rightValid = (x + 1) < dimension && _gridCopy.Contains(_grid[(x + 1) + (y - 1) * dimension]);

                            if (leftValid && rightValid)
                            {
                                result = temp;
                            }
                        }

                    }

                    break;

                // Right direction
                case 1:
                    // Check if the current X is at the edge or not
                    if (x == dimension - 1)
                    {
                        continue;
                    }

                    // Check if the new direction is empty or not
                    if (_gridCopy.Contains(_grid[((x + 1) + y * dimension)]) && (x + 1) < dimension)
                    {

                        result = temp;

                        // Check if the tiles on the top and bottom are empty or not
                        // This is done so that the rooms aren't clustered together most of the time

                        // Only 1 tile is required if we're adding at the edge of the grid
                        // So we're not trying to read outside the grid
                        if (y == 0 || y == dimension - 1)
                        {
                            if ( ((y - 1) >= 0 && _gridCopy.Contains(_grid[((x + 1) + (y - 1) * dimension)]) ) ||
                                ( (y + 1) < dimension && _gridCopy.Contains(_grid[((x + 1) + (y + 1) * dimension)])) )
                            {
                                result = temp;
                            }
                        }
                        else
                        {
                            // Both are required for everywhere else
                            bool downValid = (y - 1) >= 0 && _gridCopy.Contains(_grid[((x + 1) + (y - 1) * dimension)]);
                            bool upValid = (y + 1) < dimension && _gridCopy.Contains(_grid[((x + 1) + (y + 1) * dimension)]);

                            if (upValid && downValid)
                            {
                                result = temp;
                            }
                        }
                    }

                    break;

                // Down
                case 2:
                    // Check if the current y is at the edge or not
                    if (y == dimension - 1)
                    {
                        continue;
                    }

                    // Check if the new direction is empty or not
                    if (_gridCopy.Contains(_grid[(x + (y + 1) * dimension)]) && (y + 1) < dimension)
                    {

                        // Check if the tiles on the left and right are empty or not
                        // This is done so that the rooms aren't clustered together most of the time

                        // Only 1 tile is required if we're adding at the edge of the grid
                        // So we're not trying to read outside the grid
                        if (x == 0 || x == dimension - 1)
                        {
                            if ( ((x + 1) < dimension && _gridCopy.Contains(_grid[((x + 1) + (y + 1) * dimension)]) ) ||
                                ( (x - 1) >= 0 && _gridCopy.Contains(_grid[((x - 1) + (y + 1) * dimension)])) )
                            {
                                result = temp;
                            }
                        }
                        else
                        {
                            // Both are required for everywhere else
                            bool leftValid = (x - 1) >= 0 && _gridCopy.Contains(_grid[(x - 1) + (y + 1) * dimension]);
                            bool rightValid = (x + 1) < dimension && _gridCopy.Contains(_grid[(x + 1) + (y + 1) * dimension]);

                            if (leftValid && rightValid)
                            {
                                result = temp;
                            }
                        }
                    }

                    break;

                // Left
                case 3:
                    // Check if the current X is at the edge or not
                    if (x == 0)
                    {
                        continue;
                    }

                    // Check if the new direction is empty or not
                    if (_gridCopy.Contains(_grid[((x - 1) + y * dimension)]) && (x - 1) >= 0)
                    {

                        result = temp;

                        // Check if the tiles on the top and bottom are empty or not
                        // This is done so that the rooms aren't clustered together most of the time

                        // Only 1 tile is required if we're adding at the edge of the grid
                        // So we're not trying to read outside the grid
                        if (y == 0 || y == dimension - 1)
                        {
                            if ( ((y + 1) < dimension && _gridCopy.Contains(_grid[((x - 1) + (y + 1) * dimension)]) ) ||
                                ( (y - 1) >= 0 && _gridCopy.Contains(_grid[((x - 1) + (y - 1) * dimension)])) )
                            {
                                result = temp;
                            }
                        }
                        else
                        {
                            // Both are required for everywhere else
                            bool downValid = (y - 1) >= 0 && _gridCopy.Contains(_grid[((x - 1) + (y - 1) * dimension)]);
                            bool upValid = (y + 1) < dimension && _gridCopy.Contains(_grid[((x - 1) + (y + 1) * dimension)]);

                            if (upValid && downValid)
                            {
                                result = temp;
                            }
                        }
                    }

                    break;

                default:
                    Debug.Log("SearchForNewDirections: wtf");
                    break;
            }

            i++;
        }

        return result;
    }

    // TEST FUNCTION: Create a grid of filled tiles
    private void CreateFilledGrid()
    {
        for (int x = 0; x < dimension; x++)
        {
            for (int y = 0; y < dimension; y++)
            {
                // Create tile object
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.transform.parent = gridParent;

                // Get WFC_TilePrefab object
                //WFC_TilePrefab newTile = spawnedTile.GetComponent<WFC_TilePrefab>();
                //newTile.spriteRenderer.sprite = emptyTile;
               
                // Add WFC_Tile from object to dictionary
                //_tilesDictionary[new Vector2(x, y)] = newTile.tile;

            }
        }
    }

    private bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < dimension && y >= 0 && y < dimension;
    }

    private void DebugShowGridListOptions(List<WFC_Cell> grid)
    {
        Debug.Log("[DEBUG] Showing grid list options");

        int gridSize = (int)Math.Sqrt(grid.Count);
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                int index = i * gridSize + j;
                Debug.Log("[DEBUG] Collapsed: " + grid[index].collapsed + " | Options: " + grid[index].options.Count + " ");
            }
            Debug.Log("");
        }
    }

    public void AdvanceStep()
    {
        if(!isDebug) 
        {
            Debug.Log("[AdvanceStep] Debug is off, button will do nothing");
            return; 
        }

        Debug.Log("[AdvanceStep] Continue WFC");
        PerformWaveFunctionCollapse();
    }
}

public enum BackupRoomType
{
    Wall = 0,
    Empty = 1,
    Enemy = 2,
    Treasure = 3,
    Puzzle = 4,
    Trap = 5,
    Shop = 6,
    Boss = 7
}
