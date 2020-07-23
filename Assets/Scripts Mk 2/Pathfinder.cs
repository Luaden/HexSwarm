using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor.Animations;
using UnityEngine;

public class Pathfinder
{
    //Cached references
    protected BattlefieldManager battlefieldManager;
    protected Dictionary<Vector3Int, PathfindingCell> closedSet = new Dictionary<Vector3Int, PathfindingCell>();
    protected Dictionary<Vector3Int, PathfindingCell> openSet = new Dictionary<Vector3Int, PathfindingCell>();
    protected Dictionary<Vector3Int, PathfindingCell> allCells = new Dictionary<Vector3Int, PathfindingCell>();
    protected HashSet<PathfindingCell> pfCells = new HashSet<PathfindingCell>();

    public Pathfinder(BattlefieldManager battlefieldManager)
    {
        this.battlefieldManager = battlefieldManager;
    }

    public void InitPathfinder()
    {
        allCells.Clear(); 

        foreach (KeyValuePair<Vector3Int, ICell> entry in battlefieldManager.World)
        {
            if (!allCells.ContainsKey(entry.Key))
            {
                PathfindingCell pfCell = new PathfindingCell(entry.Key);
                allCells.Add(entry.Key, pfCell);

                continue;
            }
        }
    }

      protected void ClearPathfindingCells()
    {
        foreach(KeyValuePair<Vector3Int, PathfindingCell> entry in allCells)
        {
            entry.Value.FCost = 0;
            entry.Value.GCost = 0;
            entry.Value.HCost = 0;
            entry.Value.Parent = null;
        }
    }

    public IEnumerable<Vector3Int> FindTraversableArea(Vector3Int originVector, int maxRange)
    {

        IEnumerable<Vector3Int> totalArea = battlefieldManager.GetNeighborCells(originVector, maxRange) as IEnumerable<Vector3Int>;
        HashSet<Vector3Int> traversableArea = new HashSet<Vector3Int>();

        foreach(Vector3Int destination in totalArea)
        {
            IEnumerable<Vector3Int> openSet = FindPath(originVector, destination, true, maxRange);

            foreach (Vector3Int location in openSet)
                if (!traversableArea.Contains(location))
                    traversableArea.Add(location);
        }

        return traversableArea;
    }

    public IEnumerable<Vector3Int> FindPath(Vector3Int originVector, Vector3Int destinationVector, bool avoidUnits = true, int maxRange = int.MaxValue)
    {        
        InitPathfinder();  

        closedSet.Clear();
        openSet.Clear();
        ClearPathfindingCells();

        PathfindingCell origin;
        PathfindingCell destination;
        allCells.TryGetValue(originVector, out origin);

        if (!allCells.TryGetValue(destinationVector, out destination))
            return null;

        

        origin.FCost = 0;
        openSet.Add(origin.Location, origin);

        while(openSet.Count > 0)
        {
            PathfindingCell currentCell = null;

            foreach(KeyValuePair<Vector3Int, PathfindingCell> entry in openSet)
            {
                if (currentCell == null)
                {
                    currentCell = entry.Value;
                    continue;
                }
                
                if(currentCell.FCost > entry.Value.FCost)
                {
                    currentCell = entry.Value;
                }
            }
            
            closedSet.Add(currentCell.Location, currentCell);
            openSet.Remove(currentCell.Location);

            if (currentCell == destination)
                break;

            ICell worldCell;
            battlefieldManager.World.TryGetValue(currentCell.Location, out worldCell);

            foreach(PathfindingCell cell in 
                        GetPathFindingCells(battlefieldManager.GetNeighborCells(worldCell)))
            {
                if (closedSet.ContainsKey(cell.Location))
                    continue;

                if(avoidUnits && CellIsBlocked(cell))
                {
                    closedSet.Add(cell.Location, cell);
                    continue;
                }

                if(!openSet.ContainsKey(cell.Location))
                {
                    cell.GCost = currentCell.GCost + 1;
                    cell.HCost = EvaluateCellDistance(cell, destination);
                    cell.CalculateFCost();
                    cell.Parent = currentCell;
                    openSet.Add(cell.Location, cell);
                }
                if (openSet.ContainsKey(cell.Location))
                {
                    PathfindingCell oldCheck;
                    openSet.TryGetValue(cell.Location, out oldCheck);
                    if(oldCheck.FCost < cell.FCost)
                    {
                        openSet.Remove(cell.Location);
                        openSet.Add(cell.Location, cell);
                    }
                }                    
            }                
        }

        return CalculatePath(destination, maxRange);
    }

    protected IEnumerable<Vector3Int> CalculatePath(PathfindingCell endCell, int maxRange)
    {
        Stack<PathfindingCell> unrestrictedPath = new Stack<PathfindingCell>();
        Queue<PathfindingCell> rangeRestrictedPath = new Queue<PathfindingCell>();
        PathfindingCell currentCell = endCell;

        unrestrictedPath.Push(currentCell);
        while (currentCell.Parent != null)
        {
            unrestrictedPath.Push(currentCell.Parent);
            currentCell = currentCell.Parent;
        }

        int pathCount = unrestrictedPath.Count;

        for(int i = 0; i < pathCount; i++)
        {
            rangeRestrictedPath.Enqueue(unrestrictedPath.Pop());
            if (i == maxRange)
                break;
        }

        return rangeRestrictedPath.Select(x => x.Location);
    }

    protected IEnumerable<PathfindingCell> GetPathFindingCells(IEnumerable<ICell> cells)
    {
        pfCells.Clear();

        foreach(ICell cell in cells)
        {                           
            pfCells.Add(GetPathFindingCell(cell));
        }

        return pfCells;
    }

    protected PathfindingCell GetPathFindingCell(ICell cell)
    {
        PathfindingCell pfCell;

        allCells.TryGetValue(cell.GridPosition, out pfCell);
        return pfCell;
    }

    protected bool CellIsBlocked(ICell cell) => cell.Unit != null ? true : false;

    protected bool CellIsBlocked(PathfindingCell cell)
    {
        ICell worldCell;
        battlefieldManager.World.TryGetValue(cell.Location, out worldCell);

        if (worldCell.Unit != null)
            return true;

        return false;
    }

    protected float EvaluateCellDistance(ICell origin, ICell destination) =>
        Mathf.Abs(origin.GridPosition.x - destination.GridPosition.x) + Mathf.Abs(origin.GridPosition.y - destination.GridPosition.y);

    protected float EvaluateCellDistance(PathfindingCell origin, PathfindingCell destination) => 
        Mathf.Abs(origin.Location.x - destination.Location.x) + Mathf.Abs(origin.Location.y - destination.Location.y);

    #region Old Pathfinder
    //protected List<ICell> cellsToEvaluate = new List<ICell>();
    //protected List<ICell> orderedEvaluatedCells = new List<ICell>();
    //protected List<ICell> neighborCells = new List<ICell>();
    //protected List<ICell> blockedCells = new List<ICell>();
    //protected List<ICell> currentRoute = new List<ICell>();
    //protected HashSet<ICell> newRoute;
    //protected ICell closestCell;
    //protected ICell lastClosestCell;

    //public IEnumerable<ICell> AvoidUnitsPath(ICell origin, ICell destination)
    //{
    //    GetFreshWorld();
    //    currentRoute = new List<ICell>();

    //    //gets a direct path
    //    cellsToEvaluate = DirectPath(origin, destination) as List<ICell>;
    //    blockedCells = new List<ICell>();
    //    closestCell = origin;

    //    //checks each cell for blocked path, adds to evaluatedCells if not blocked.
    //    while (closestCell != destination)
    //    {
    //        for (int i = 0; i < cellsToEvaluate.Count; i++)
    //        {
    //            while (CellIsBlocked(cellsToEvaluate[i]))
    //            {
    //                blockedCells.Add(cellsToEvaluate[i]);

    //                cellsToEvaluate[i] = FindPathAround(cellsToEvaluate[i - 1], destination);

    //                if (cellsToEvaluate[i] == cellsToEvaluate[i - 2])
    //                {
    //                    blockedCells.Add(cellsToEvaluate[i - 1]);
    //                    orderedEvaluatedCells.Remove(cellsToEvaluate[i - 1]);
    //                    i--;
    //                }

    //                cellsToEvaluate = DirectPath(cellsToEvaluate[i], destination) as List<ICell>;
    //            }

    //            orderedEvaluatedCells.Add(cellsToEvaluate[i]);
    //            closestCell = cellsToEvaluate[i];
    //        }
    //    }

    //    return orderedEvaluatedCells;
    //}

    //public IEnumerable<Vector3Int> AvoidUnitsPath(Vector3Int origin, Vector3Int destination)
    //{
    //    ICell originCell;
    //    ICell destinationCell;

    //    GetFreshWorld();
    //    world.TryGetValue(origin, out originCell);
    //    world.TryGetValue(destination, out destinationCell);

    //    return AvoidUnitsPath(originCell, destinationCell) as IEnumerable<Vector3Int>;
    //}

    //private ICell FindPathAround(ICell origin, ICell destination)
    //{
    //    neighborCells = new List<ICell>();

    //    neighborCells = battlefieldManager.GetNeighborCells(origin) as List<ICell>;

    //    for (int i = 0; i < neighborCells.Count; i++)
    //    {
    //        if (CellIsBlocked(neighborCells[i]) || blockedCells.Contains(neighborCells[i]))
    //        {
    //            blockedCells.Add(neighborCells[i]);
    //            neighborCells.RemoveAt(i);
    //        }
    //    }

    //    closestCell = neighborCells[0];

    //    for (int i = 1; i < neighborCells.Count; i++)
    //    {
    //        if (EvaluateCellDistance(neighborCells[i], destination) < EvaluateCellDistance(closestCell, destination))
    //        {
    //            closestCell = neighborCells[i];
    //        }
    //    }

    //    return closestCell;
    //}

    //public IEnumerable<ICell> DirectPath(ICell origin, ICell destination)
    //{
    //    GetFreshWorld();
    //    currentRoute = new List<ICell>();

    //    cellsToEvaluate = battlefieldManager.GetNeighborCells(origin) as List<ICell>;
    //    cellsToEvaluate.Remove(origin);
    //    closestCell = origin;

    //    while (closestCell != destination)
    //    {
    //        cellsToEvaluate = battlefieldManager.GetNeighborCells(closestCell) as List<ICell>;

    //        for (int i = 0; i < cellsToEvaluate.Count; i++)
    //        {
    //            if (EvaluateCellDistance(cellsToEvaluate[i], destination) <
    //                EvaluateCellDistance(closestCell, destination))
    //                closestCell = cellsToEvaluate[i];
    //        }

    //        currentRoute.Add(closestCell);
    //    }

    //    return currentRoute;
    //}

    //public IEnumerable<Vector3Int> DirectPath(Vector3Int origin, Vector3Int destination)
    //{
    //    ICell originCell;
    //    ICell destinationCell;

    //    GetFreshWorld();
    //    world.TryGetValue(origin, out originCell);
    //    world.TryGetValue(destination, out destinationCell);

    //    return DirectPath(originCell, destinationCell) as IEnumerable<Vector3Int>;
    //}
    #endregion
}