using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : IPathfinder
{
    //Cached references
    protected BattlefieldManager battlefieldManager;
    protected GridManager gridManager;

    //State variables
    protected Dictionary<Vector3Int, Cell> world;
    protected List<Cell> cellsToEvaluate = new List<Cell>();
    protected HashSet<Cell> currentRoute;
    protected HashSet<Cell> newRoute;

    public Pathfinder(BattlefieldManager battlefieldManager, GridManager gridManager)
    {
        this.battlefieldManager = battlefieldManager;
        this.gridManager = gridManager;
    }

    public IEnumerable<Cell> AvoidUnitsPath(Cell origin, Cell destination)
    {
        GetFreshWorld();

        cellsToEvaluate = gridManager.GetNeighborCells(origin) as List<Cell>;

        return null;
    }

    public IEnumerable<Cell> DirectPath(Cell origin, Cell destination)
    {
        throw new System.NotImplementedException();
    }

    protected void GetFreshWorld() => world = battlefieldManager.World;
}

