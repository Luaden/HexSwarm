using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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
        currentRoute = new HashSet<Cell>();

        cellsToEvaluate = gridManager.GetNeighborCells(origin) as List<Cell>;
        Cell closestCell = origin;

        while (closestCell != destination)
        {
            cellsToEvaluate = gridManager.GetNeighborCells(closestCell) as List<Cell>;

            for (int i = 0; i < cellsToEvaluate.Count; i++)
            {
                if (EvaluateCellDistance(cellsToEvaluate[i].Position, destination.Position) < 
                    EvaluateCellDistance(closestCell.Position, destination.Position) && 
                    gridManager.CheckCanMove(cellsToEvaluate[i]))
                    closestCell = cellsToEvaluate[i];
            }

            currentRoute.Add(closestCell);
        }

        return currentRoute;
    }

    public IEnumerable<Cell> DirectPath(Cell origin, Cell destination)
    {
        GetFreshWorld();
        currentRoute = new HashSet<Cell>();

        cellsToEvaluate = gridManager.GetNeighborCells(origin) as List<Cell>;
        Cell closestCell = origin;

        while(closestCell != destination)
        {
            cellsToEvaluate = gridManager.GetNeighborCells(closestCell) as List<Cell>;

            for (int i = 0; i < cellsToEvaluate.Count; i++)
            {
                if (EvaluateCellDistance(cellsToEvaluate[i].Position, destination.Position) < 
                    EvaluateCellDistance(closestCell.Position, destination.Position))
                    closestCell = cellsToEvaluate[i];
            }

            currentRoute.Add(closestCell);
        }

        return currentRoute;
    }

    protected void GetFreshWorld() => world = battlefieldManager.World;

    protected float EvaluateCellDistance(Vector3Int position, Vector3Int destination) => Vector3Int.Distance(position, destination);

}

