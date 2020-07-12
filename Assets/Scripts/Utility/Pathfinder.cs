using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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
    protected List<Cell> orderedEvaluatedCells = new List<Cell>();
    protected List<Cell> neighborCells = new List<Cell>();
    protected List<Cell> blockedCells = new List<Cell>();
    protected List<Cell> currentRoute = new List<Cell>();
    protected HashSet<Cell> newRoute;
    protected Cell closestCell;
    protected Cell lastClosestCell;
    


    public Pathfinder(BattlefieldManager battlefieldManager, GridManager gridManager)
    {
        this.battlefieldManager = battlefieldManager;
        this.gridManager = gridManager;
    }

    public IEnumerable<Cell> AvoidUnitsPath(Cell origin, Cell destination)
    {
        GetFreshWorld();
        currentRoute = new List<Cell>();

        //gets a direct path
        cellsToEvaluate = DirectPath(origin, destination) as List<Cell>;
        blockedCells = new List<Cell>();
        closestCell = origin;

        Debug.Log("There are :" + cellsToEvaluate.Count + " left to count.");

        //checks each cell for blocked path, adds to evaluatedCells if not blocked.
        while (closestCell != destination)
        {
            for (int i = 0; i < cellsToEvaluate.Count; i++)
            {
                Debug.Log(cellsToEvaluate[i]);
                Debug.Log("There are :" + (cellsToEvaluate.Count - i) + " left to count.");

                while (CellIsBlocked(cellsToEvaluate[i]))
                {
                    blockedCells.Add(cellsToEvaluate[i]);                    
                    
                    cellsToEvaluate[i] = FindPathAround(cellsToEvaluate[i -1], destination);

                    if (cellsToEvaluate[i] == cellsToEvaluate[i-2])
                    {
                        blockedCells.Add(cellsToEvaluate[i - 1]);
                        orderedEvaluatedCells.Remove(cellsToEvaluate[i - 1]);
                        i--;
                    }

                    cellsToEvaluate = DirectPath(cellsToEvaluate[i], destination) as List<Cell>;
                }

                orderedEvaluatedCells.Add(cellsToEvaluate[i]);
                closestCell = cellsToEvaluate[i];
            }
        }

        return orderedEvaluatedCells;
    }

    private Cell FindPathAround(Cell origin, Cell destination)
    {
        neighborCells = new List<Cell>();

        neighborCells = gridManager.GetNeighborCells(origin) as List<Cell>;

        for(int i = 0; i < neighborCells.Count; i++)
        {
            if (CellIsBlocked(neighborCells[i]) || blockedCells.Contains(neighborCells[i]))
            {
                blockedCells.Add(neighborCells[i]);
                neighborCells.RemoveAt(i);
            }                
        }

        closestCell = neighborCells[0];

        for(int i = 1; i < neighborCells.Count; i++)
        {
            if(EvaluateCellDistance(neighborCells[i], destination) < EvaluateCellDistance(closestCell, destination))
            {
                closestCell = neighborCells[i];
            }
        }

        return closestCell;
    }

    public IEnumerable<Cell> DirectPath(Cell origin, Cell destination)
    {
        Debug.Log("Got a direct path");
        GetFreshWorld();
        currentRoute = new List<Cell>();

        cellsToEvaluate = gridManager.GetNeighborCells(origin) as List<Cell>;
        cellsToEvaluate.Remove(origin);
        closestCell = origin;

        while(closestCell != destination)
        {
            cellsToEvaluate = gridManager.GetNeighborCells(closestCell) as List<Cell>;

            for (int i = 0; i < cellsToEvaluate.Count; i++)
            {
                if (EvaluateCellDistance(cellsToEvaluate[i], destination) < 
                    EvaluateCellDistance(closestCell, destination))
                    closestCell = cellsToEvaluate[i];
            }

            currentRoute.Add(closestCell);
        }

        return currentRoute;
    }

    protected bool CellIsBlocked(Cell cell) => cell.Unit != null ? true: false;

    protected void GetFreshWorld() => world = battlefieldManager.World;

    protected float EvaluateCellDistance(Cell position, Cell destination) => Vector3Int.Distance(position.Position, destination.Position);

}

