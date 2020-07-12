using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathfinder
{
    IEnumerable<Cell> DirectPath(Cell origin, Cell destination);

    IEnumerable<Cell> AvoidUnitsPath(Cell origin, Cell destination);
}
