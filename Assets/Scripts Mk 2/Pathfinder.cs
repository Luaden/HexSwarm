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
            return System.Array.Empty<Vector3Int>();

        

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
        Vector3.Distance(origin.GridPosition, destination.GridPosition);

    protected float EvaluateCellDistance(PathfindingCell origin, PathfindingCell destination) => 
        Vector3.Distance(origin.Location, destination.Location);   
}