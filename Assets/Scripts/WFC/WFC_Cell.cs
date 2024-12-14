using System.Collections.Generic;
using System.Linq;

public class WFC_Cell
{
    public bool collapsed;
    public List<int> options;
    public bool first;

    public WFC_Cell(int value)
    {
        collapsed = false;

        options = new List<int>(Enumerable.Range(0, value));

        first = false;
    }

    public WFC_Cell(List<int> values)
    {
        collapsed = false;

        options = new List<int>(values);
    }
}
