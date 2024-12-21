using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveFunctionCollapse : MonoBehaviour
{
    [Header("Config")]
    public LevelConfigSO levelConfigFile;
    [SerializeField] private WFC_RoomPositions roomPositioner;
    [SerializeField] private WFC_Drawer roomDrawer;

    [Space]
    [SerializeField] private int maxWFCRetries = 10;

    [Header("Tiles")]
    [SerializeField] private WFC_TileTemplate[] tileTemplates;

    [Header("Grid")]
    [SerializeField] private int dimension;
    [SerializeField] private Transform gridParent;

    [Header("Room Control")]
    [Tooltip("Whether to start the map generation from the center or from a random cell")]
    [SerializeField] private bool startFromCenter = true;
    [Tooltip("Maximum number of times to search for a direction before giving up")]
    [SerializeField] private int maxDirectionRetries = 10;
    [Tooltip("Maximum number of times to search for a branch before giving up")]
    [SerializeField] private int maxBranchRetries = 10;

    [Space]
    [SerializeField] private bool generateWallRooms = false;

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
    private RoomType _latestRoomType;

    // Temp copy of grid
    private List<WFC_Cell> _gridCopy;

    // Certain room coordinates
    private (int, int) _startRoomCoordinate;
    private (int, int) _endRoomCoordinate;
    private (int, int) _latestRoomCoordinate;

    // Max room count setup;
    private int _maxRoomCount;
    private int _maxSafeRoomCount;

    private int _maxEmptyRoomCount = 0;
    private int _maxEnemyRoomCount = 0;
    private int _maxTreasureRoomCount = 0;
    private int _maxPuzzleRoomCount = 0;
    private int _maxTrapRoomCount = 0;
    private int _maxShopRoomCount = 0;

    // Branch tracking
    // UP, RIGHT, DOWN, LEFT
    private bool[] _activeBranches = new bool[4] { false, false, false, false};
    private int[] _branchRoomCount = new int[4] { 0, 0, 0, 0};
    private (int, int)[] _currentBranchRoomCoordinate = new (int, int)[4];

    private int _trueBranchDirection = 0;
    private int _deadBranchDirection = 0;
    private int _maxTrueBranchRoomCount = 0;
    private int _maxDeadBranchRoomCount = 0;

    private RoomType _lastTrueBranchRoom;
    private RoomType _lastDeadBranchRoom;

    // Room count tracking
    private int _currentSafeRoomCount;

    private int _currentEmptyRoomCount = 0;
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

    private int _safeRoomGaps;
    private int _trueRoomsSinceSafe = 0;
    private int _deadRoomsSinceSafe = 0;

    private int _wfcRetryAttempt = 0;

    private GameObject _player;
    private PlayerSpawn _playerSpawn;

    [HideInInspector] public WFC_Cell firstCell;

    private bool somethingFuckedUp = false;

    private void Start()
    {
        SetupPlayer();

        // Pre-preparation
        SetupLevelConfig();

        // Preparation
        CreateTiles();
        GenerateAdjacencyRules();
        InitializeGrid();
        CreateInitialBranches();

        // Cell collapse
        PerformWaveFunctionCollapse();
    }

    private void SetupPlayer()
    {
        // Find player
        _player = GameObject.FindGameObjectWithTag("Player");

        // If there is no player object detected
        if (_player == null)
        {
            // Clone PlayerClone object from singleton
            _player = Instantiate(PlayerDataManager.Instance.PlayerClone, Vector3.zero, Quaternion.identity);
            _player.SetActive(true);
        }

        _playerSpawn = _player.GetComponent<PlayerSpawn>();

    }

    private void SetupLevelConfig()
    {
        // Set level difficulty
        _levelDifficulty = levelConfigFile.levelDifficulty;

        // Set the positions of safe rooms
        //_safeRoomPositions = roomPositioner.GetSafeRoomPositions(_levelDifficulty);
        _safeRoomGaps = levelConfigFile.safeRoomGaps;

        // Set branch max rooms
        _maxTrueBranchRoomCount = levelConfigFile.trueBranchLength;
        _maxDeadBranchRoomCount = levelConfigFile.deadBranchLength;

        // Set max rooms
        _maxRoomCount = levelConfigFile.maxRoomCount;
        _maxSafeRoomCount = levelConfigFile.maxRoomCount - (levelConfigFile.maxEnemyCount);

        _maxEmptyRoomCount = levelConfigFile.maxRoomCount - (_maxEnemyRoomCount + _maxTreasureRoomCount + _maxPuzzleRoomCount + _maxTrapRoomCount + _maxShopRoomCount + 1);
        _maxEnemyRoomCount = levelConfigFile.maxEnemyCount;
        _maxTreasureRoomCount = levelConfigFile.maxTreasureCount;
        _maxPuzzleRoomCount = levelConfigFile.maxPuzzleCount;
        _maxTrapRoomCount = levelConfigFile.maxTrapCount;
        _maxShopRoomCount = levelConfigFile.maxShopCount;
    }

    public void WFCDEbug()
    {
        //if(!isDebug) return;

        if (_gridCopy != null && _gridCopy.Count <= 0)
        {
            roomDrawer.StartDrawer(dimension, _grid, firstCell, gridParent);
            _playerSpawn.SetPlayerSpawn();

            Debug.Log("[DEBUG] WFC COMPLETE");
            return;
        }

        Debug.Log("[DEBUG] Processing grid copy");
        ProcessGridCopy();
        System.Threading.Thread.Sleep(1);

        Debug.Log("[DEBUG] Collapsing cell");
        CollapseCell();
        System.Threading.Thread.Sleep(1);

        Debug.Log("[DEBUG] Recalculating options");
        RecalculateOptions();
        System.Threading.Thread.Sleep(1);

        CheckIfSomethingFuckedUp();
    }

    // Actually do the entire collapse process
    private void PerformWaveFunctionCollapse()
    {
        while (_wfcRetryAttempt < maxWFCRetries)
        {
            try
            {
                for (int i = 0; i < dimension * dimension; i++)
                {
                    //Debug.Log("WFC run " + i);
                    ProcessGridCopy();
                    CollapseCell();
                    RecalculateOptions();

                    CheckIfSomethingFuckedUp();

                    if (i == dimension * dimension - 1 && _gridCopy.Count <= 0) // Last collapse
                    {
                        //Debug.Log("WFC completed");
                        roomDrawer.StartDrawer(dimension, _grid, firstCell, gridParent);
                        _playerSpawn.SetPlayerSpawn();

                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[PerformWaveFunctionCollapse] Exception caught: " + e);

                _wfcRetryAttempt++;
                Debug.LogError($"Retry {_wfcRetryAttempt}: {e.Message}");
                //if (_wfcRetryAttempt >= maxWFCRetries) throw;

                // Restart
                DestroyYounglings();

                CreateTiles();
                GenerateAdjacencyRules();
                InitializeGrid();
                CreateInitialBranches();

                PerformWaveFunctionCollapse();
            }
        }

    }

    // Create WFC_Tile objects
    private void CreateTiles()
    {
        _tiles = new List<WFC_Tile>();

        for(int i = 0; i < tileTemplates.Length; i++)
        {
            //Debug.Log("tile " + i + ": " + tileTemplates[i].name);
            _tiles.Add(new WFC_Tile(tileTemplates[i].roomData, tileTemplates[i].up, tileTemplates[i].right, tileTemplates[i].down, tileTemplates[i].left));
        }
        
    }

    // Analyze each tile and generate their adjacency rules
    private void GenerateAdjacencyRules()
    {
        //Debug.Log("Generating adjacency rules");

        foreach(WFC_Tile tile in _tiles)
        {
            tile.Analyze(_tiles.ToArray());
        }

        //Debug.Log("Adjacency rules generated");
    }

    // Create initial grid of cells
    private void InitializeGrid()
    {
        //Debug.Log("Initializing grid of " + (dimension * dimension) + " cells");
        //Debug.Log("Tiles length: " + _tiles.Count);

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
        // Find cell with least entropy
        _gridCopy = _grid.Where(a => !a.collapsed).OrderBy(a => a.options.Count).ToList();
        Debug.Log("Grid copy created with " + _gridCopy.Count + " non-collapsed cells");

        // Stop function if gridCopy is empty
        if (_gridCopy.Count <= 0)
        {
            Debug.Log("Grid copy is empty, stopping function");
            return;
        }

        // If there are two or more options of the same length
        int len = _gridCopy[0].options.Count;
        int stopIndex = _gridCopy.FindIndex(a => a.options.Count > len);

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
        WFC_Cell cell = _grid[0];

        // FIRST ROOM
        if (_firstRoom)
        {
            int x = startFromCenter ? dimension / 2 : UnityEngine.Random.Range(0, dimension);
            int y = startFromCenter ? dimension / 2 : UnityEngine.Random.Range(0, dimension);

            //Debug.Log($"Grid Copy Count: {_gridCopy.Count}");

            if (_gridCopy.Contains(_grid[x + y * dimension]))
            {
                cell = _grid[x + y * dimension];
                _startRoomCoordinate = _latestRoomCoordinate = (x, y);
                cell.options = new List<int> { 1 };
                _latestRoomType = RoomType.Empty;

                _currentEmptyRoomCount++;
                _currentRoomCount++;
                _currentSafeRoomCount++;
                _firstRoom = false;

                cell.collapsed = true;
                cell.first = true;

                // Set the initial coords of each branch
                _currentBranchRoomCoordinate = new (int, int)[4] {
                    (x, y), // UP
                    (x, y), // RIGHT
                    (x, y), // DOWN
                    (x, y)  // LEFT
                };
            }
        }

        // NON-FIRST ROOMS
        else if (!_firstRoom && _currentRoomCount < _maxRoomCount)
        {
            int trueCount = _branchRoomCount[_trueBranchDirection];
            int deadCount = _branchRoomCount[_deadBranchDirection];

            // TRUE BRANCH
            if (trueCount < _maxTrueBranchRoomCount)
            {
                int x = _currentBranchRoomCoordinate[_trueBranchDirection].Item1;
                int y = _currentBranchRoomCoordinate[_trueBranchDirection].Item2;

                int newDirection = 0;

                // Get the new direction
                newDirection = SearchForNewDirection(x, y);

                // Set the values according to the results
                switch (newDirection)
                {
                    case 0: cell = _grid[x + (y - 1) * dimension]; _latestRoomCoordinate = (x, y - 1); break;
                    case 1: cell = _grid[(x + 1) + y * dimension]; _latestRoomCoordinate = (x + 1, y); break;
                    case 2: cell = _grid[x + (y + 1) * dimension]; _latestRoomCoordinate = (x, y + 1); break;
                    case 3: cell = _grid[(x - 1) + y * dimension]; _latestRoomCoordinate = (x - 1, y); break;
                    default: Debug.Log("[CollapseCell] How?"); somethingFuckedUp = true; break;
                }

                // Put boss room as the final room
                if (trueCount == _maxTrueBranchRoomCount - 1)
                {
                    cell.options = new List<int> { (int)RoomType.Boss };
                    _endRoomCoordinate = _latestRoomCoordinate;
                    _lastTrueBranchRoom = RoomType.Boss;
                }
                // Collapse cell normally
                else
                {
                    // If its the first room in a branch, it'll always be in the same direction as the branch
                    if (_branchRoomCount[_trueBranchDirection] <= 0)
                    {
                        newDirection = _trueBranchDirection;
                    }

                    // Set as regular room
                    int roomIndex = DetermineRoomType();
                    cell.options = new List<int> { roomIndex };

                    _lastTrueBranchRoom = (RoomType)roomIndex;
                }

                _currentBranchRoomCoordinate[_trueBranchDirection] = _latestRoomCoordinate;

                _currentRoomCount++;
                _branchRoomCount[_trueBranchDirection]++;
                cell.collapsed = true;

            }

            // DEAD BRANCH
            else if (deadCount < _maxDeadBranchRoomCount)
            {
                int x = _currentBranchRoomCoordinate[_deadBranchDirection].Item1;
                int y = _currentBranchRoomCoordinate[_deadBranchDirection].Item2;

                int newDirection = 0;

                // Get the new direction
                newDirection = SearchForNewDirection(x, y);

                // Set the values according to the results
                switch (newDirection)
                {
                    case 0: cell = _grid[x + (y - 1) * dimension]; _latestRoomCoordinate = (x, y - 1); break;
                    case 1: cell = _grid[(x + 1) + y * dimension]; _latestRoomCoordinate = (x + 1, y); break;
                    case 2: cell = _grid[x + (y + 1) * dimension]; _latestRoomCoordinate = (x, y + 1); break;
                    case 3: cell = _grid[(x - 1) + y * dimension]; _latestRoomCoordinate = (x - 1, y); break;
                    default: Debug.Log("[CollapseCell] How?"); break;
                }

                // Put treasure room as the final room
                if (deadCount == _maxDeadBranchRoomCount - 1)
                {
                    cell.options = new List<int> { (int)RoomType.Treasure };
                    _lastDeadBranchRoom = RoomType.Treasure;

                }
                // Collapse cell normally
                else
                {
                    // If its the first room in a branch, it'll always be in the same direction as the branch
                    if (_branchRoomCount[_deadBranchDirection] <= 0)
                    {
                        newDirection = _deadBranchDirection;
                    }

                    // Set as regular room
                    int roomIndex = DetermineRoomType();
                    cell.options = new List<int> { roomIndex };

                    _lastDeadBranchRoom = (RoomType)roomIndex;
                }

                _currentBranchRoomCoordinate[_deadBranchDirection] = _latestRoomCoordinate;

                _currentRoomCount++;
                _branchRoomCount[_deadBranchDirection]++;
                cell.collapsed = true;
            }
        }
        // WALLS
        else if (!_firstRoom && _currentRoomCount >= _maxRoomCount)
        {
            foreach (WFC_Cell gridcell in _gridCopy)
            {
                gridcell.options = new List<int> { 0 };
                gridcell.collapsed = true;
            }
        }

        //Debug.Log("[COLLAPSE CELL] Room " + _currentRoomCount + " type: " + _latestRoomType);
    }

    private int DetermineRoomType()
    {
        int roomIndex = 1;
        bool validRoom = false;

        //Debug.Log("[DetermineRoomType] Safe rooms positions: " + _safeRoomPositions.Length);

        if(!validRoom && _currentRoomCount < _maxRoomCount - 1)
        {
            // TRUE BRANCH
            // Do this if true branch isnt done yet
            if (_branchRoomCount[_trueBranchDirection] < _maxTrueBranchRoomCount)
            {
                // We ran out of empty rooms, just generate enemy rooms
                if(_currentSafeRoomCount >= _maxSafeRoomCount)
                {
                    roomIndex = (int)RoomType.Enemy;
                    _trueRoomsSinceSafe++;
                }

                // If it hasnt been _safeRoomGaps amount of rooms
                // Set as enemy
                else if (_trueRoomsSinceSafe < _safeRoomGaps)
                {
                    roomIndex = (int)RoomType.Enemy;
                    _trueRoomsSinceSafe++;
                }
                // Set as safe room
                else
                {
                    roomIndex = (int)PickSafeRoomType();
                    _trueRoomsSinceSafe = 0;
                }

                validRoom = ValidateRoomType(roomIndex);
            }
            // DEAD BRANCH
            // Do this if true branch is done
            else if(_branchRoomCount[_deadBranchDirection] < _maxDeadBranchRoomCount)
            {
                // We ran out of empty rooms, just generate enemy rooms
                if (_currentSafeRoomCount >= _maxSafeRoomCount)
                {
                    roomIndex = (int)RoomType.Enemy;
                    _trueRoomsSinceSafe++;
                }

                // If it hasnt been _safeRoomGaps amount of rooms
                // Set as enemy
                else if (_deadRoomsSinceSafe < _safeRoomGaps)
                {
                    roomIndex = (int)RoomType.Enemy;
                    _deadRoomsSinceSafe++;
                }
                // Set as safe room
                else
                {
                    roomIndex = (int)PickSafeRoomType();
                    _deadRoomsSinceSafe = 0;
                }

                validRoom = ValidateRoomType(roomIndex);
            }
        }

        return roomIndex;
    }

    private bool ValidateRoomType(int roomIndex)
    {
        switch (roomIndex)
        {
            case 1: _latestRoomType = RoomType.Empty; _currentEmptyRoomCount++; _currentSafeRoomCount++; return true;
            case 2: _latestRoomType = RoomType.Enemy; _currentEnemyRoomCount++; return true;
            case 3: _latestRoomType = RoomType.Treasure; _currentTreasureRoomCount++; _currentSafeRoomCount++; return true;
            case 4: _latestRoomType = RoomType.Puzzle; _currentPuzzleRoomCount++; _currentSafeRoomCount++; return true;
            case 5: _latestRoomType = RoomType.Trap; _currentTrapRoomCount++; _currentSafeRoomCount++; return true;
            case 6: _latestRoomType = RoomType.Shop; _currentShopRoomCount++; _currentSafeRoomCount++; return true;
            default: Debug.LogError("CollapseCell: Error when choosing room type"); somethingFuckedUp = true; return false;
        }
    }

    // Recalculate entropy and plan the next generation step
    private void RecalculateOptions()
    {
        List<WFC_Cell> nextGrid = new List<WFC_Cell>(new WFC_Cell[_grid.Count]);

        for (int j = 0; j < dimension; j++)
        {
            for (int i = 0; i < dimension; i++)
            {
                int index = i + j * dimension;
                WFC_Cell currentCell = _grid[index];

                if (currentCell.collapsed)
                {
                    nextGrid[index] = currentCell;
                }
                else
                {
                    HashSet<int> options = new HashSet<int>(Enumerable.Range(0, _tiles.Count));

                    if (j > 0) // UP
                        FilterOptions(options, _grid[i + (j - 1) * dimension].options, dir => _tiles[dir].down);

                    if (i < dimension - 1) // RIGHT
                        FilterOptions(options, _grid[(i + 1) + j * dimension].options, dir => _tiles[dir].left);

                    if (j < dimension - 1) // DOWN
                        FilterOptions(options, _grid[i + (j + 1) * dimension].options, dir => _tiles[dir].up);

                    if (i > 0) // LEFT
                        FilterOptions(options, _grid[(i - 1) + j * dimension].options, dir => _tiles[dir].right);

                    nextGrid[index] = new WFC_Cell(options.Count) { options = options.ToList() };
                }
            }
        }

        //Debug.Log("[RecalculateOptions] Next grid cell check: " + nextGrid[0]);
        //Debug.Log("[RecalculateOptions] Initial grid cell check: " + _grid[0]);

        _grid = nextGrid;
    }

    private void FilterOptions(HashSet<int> options, List<int> neighborOptions, Func<int, List<int>> getValidOptions)
    {
        HashSet<int> validOptions = new HashSet<int>();
        foreach (int option in neighborOptions)
        {
            validOptions.UnionWith(getValidOptions(option));
        }
        options.IntersectWith(validOptions);
    }

    // Pick the safe room type
    private RoomType PickSafeRoomType()
    {
        int maxTries = 10;
        int tries = 0;

        var roomCounts = new Dictionary<RoomType, int>
        {
            { RoomType.Empty, _currentEmptyRoomCount },
            { RoomType.Puzzle, _currentPuzzleRoomCount },
            { RoomType.Trap, _currentTrapRoomCount },
            { RoomType.Shop, _currentShopRoomCount }
        };
        var maxRoomCounts = new Dictionary<RoomType, int>
        {
            { RoomType.Empty, _maxEmptyRoomCount },
            { RoomType.Puzzle, _maxPuzzleRoomCount },
            { RoomType.Trap, _maxTrapRoomCount },
            { RoomType.Shop, _maxShopRoomCount }
        };
        var choices = new[] { RoomType.Empty, RoomType.Puzzle, RoomType.Trap, RoomType.Shop };

        RoomType result = RoomType.Wall;

        while (result == RoomType.Wall)
        {
            var index = UnityEngine.Random.Range(0, choices.Length);
            var roomType = choices[index];
            if (roomCounts[roomType] < maxRoomCounts[roomType])
            {
                result = roomType;
            }

            if(tries++ > maxTries)
            {
                Debug.LogError("[PickSafeRoomType] Max tries reached, setting room to enemy");
                result = RoomType.Enemy;
                break;
            }
        }

        return result;
    }

    private void CheckValid(List<int> options, List<int> validOptions)
    {
        options = options.Where(option => validOptions.Contains(option)).ToList();
    }

    private void CreateInitialBranches() {
        // Create initial index array
        List<int> directions = new List<int> { 0, 1, 2, 3 };

        // Create true branch
        int trueIndex = UnityEngine.Random.Range(0, directions.Count);
        _activeBranches[trueIndex] = true;
        _trueBranchDirection = trueIndex;
        directions.Remove(trueIndex);

        // Create dead branch
        int deadIndex = UnityEngine.Random.Range(0, directions.Count);
        _activeBranches[deadIndex] = true;
        _deadBranchDirection = deadIndex;
    }

    private T[] ChooseRandomElements<T>(T[] arr, int count) {
        if(count > arr.Length) {
            Debug.LogError("[ChooseRandomElements] Count is bigger than array length");
        }

        var random = new System.Random();

        return arr.OrderBy(a => random.Next()).Take(count).ToArray();
    }

    // Choose a branch thats going to be used
    private int ChooseBranch() {
        int result = -1;
        int i = 0;

        while(result < 0) {
            if(i > maxDirectionRetries) 
            {
                Debug.Log("[ChooseBranch] Max tries reached");
                somethingFuckedUp = true;
                break;
            }

            int temp = UnityEngine.Random.Range(0, 4);

            if(_activeBranches[temp] == true) {
                result = temp;
                break;
            }

            i++;
        }

        return result;
    }

    // Look around current cell and see which direction is a valid place for a new cell
    private int SearchForNewDirection(int x, int y)
    {
        int result = -1;
        int i = 0;

        Debug.Log($"Search attempts: {i}, Current Room Count: {_currentRoomCount}");

        // Check if the new direction already has another room or not
        while (result < 0)
        {
            if (i > maxDirectionRetries)
            {
                Debug.LogWarning("[SearchForNewDirections] Max tries reached");
                somethingFuckedUp = true;
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
                    Debug.LogError("SearchForNewDirections: wtf");
                    break;
            }

            i++;
        }

        return result;
    }

    private int FindLargestIndex(int[] arr) {
        if(arr == null || arr.Length == 0) {
            Debug.LogError("Array is empty or null");
            return -1;
        }

        int maxIndex = 0;
        int maxValue = arr[0];

        for(int i = 1; i < arr.Length; i++) {
            if(arr[i] > maxValue) {
                maxValue = arr[i];
                maxIndex = i;
            }
        }

        return maxIndex;
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

    private void CheckIfSomethingFuckedUp()
    {
        if(somethingFuckedUp)
        {
            Debug.LogError("Something Fucked Up");
            somethingFuckedUp = false;

            // Restart WFC
            DestroyYounglings();

            CreateTiles();
            GenerateAdjacencyRules();
            InitializeGrid();
            CreateInitialBranches();

            PerformWaveFunctionCollapse();
        }
    }
}

public enum RoomType
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
