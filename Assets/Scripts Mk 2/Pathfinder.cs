using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    //Cached references
    protected BattlefieldManager battlefieldManager;

    //State variables
    protected IReadOnlyDictionary<Vector3Int, ICell> world;
    protected List<ICell> cellsToEvaluate = new List<ICell>();
    protected List<ICell> orderedEvaluatedCells = new List<ICell>();
    protected List<ICell> neighborCells = new List<ICell>();
    protected List<ICell> blockedCells = new List<ICell>();
    protected List<ICell> currentRoute = new List<ICell>();
    protected HashSet<ICell> newRoute;
    protected ICell closestCell;
    protected ICell lastClosestCell;



    public Pathfinder(BattlefieldManager battlefieldManager) => this.battlefieldManager = battlefieldManager;

    public IEnumerable<ICell> AvoidUnitsPath(ICell origin, ICell destination)
    {
        GetFreshWorld();
        currentRoute = new List<ICell>();

        //gets a direct path
        cellsToEvaluate = DirectPath(origin, destination) as List<ICell>;
        blockedCells = new List<ICell>();
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

                    cellsToEvaluate[i] = FindPathAround(cellsToEvaluate[i - 1], destination);

                    if (cellsToEvaluate[i] == cellsToEvaluate[i - 2])
                    {
                        blockedCells.Add(cellsToEvaluate[i - 1]);
                        orderedEvaluatedCells.Remove(cellsToEvaluate[i - 1]);
                        i--;
                    }

                    cellsToEvaluate = DirectPath(cellsToEvaluate[i], destination) as List<ICell>;
                }

                orderedEvaluatedCells.Add(cellsToEvaluate[i]);
                closestCell = cellsToEvaluate[i];
            }
        }

        return orderedEvaluatedCells;
    }

    public IEnumerable<Vector3Int> AvoidUnitsPath(Vector3Int origin, Vector3Int destination)
    {
        ICell originCell;
        ICell destinationCell;

        GetFreshWorld();
        world.TryGetValue(origin, out originCell);
        world.TryGetValue(destination, out destinationCell);

        return AvoidUnitsPath(originCell, destinationCell) as IEnumerable<Vector3Int>;
    }

    private ICell FindPathAround(ICell origin, ICell destination)
    {
        neighborCells = new List<ICell>();

        neighborCells = battlefieldManager.GetNeighborCells(origin) as List<ICell>;

        for (int i = 0; i < neighborCells.Count; i++)
        {
            if (CellIsBlocked(neighborCells[i]) || blockedCells.Contains(neighborCells[i]))
            {
                blockedCells.Add(neighborCells[i]);
                neighborCells.RemoveAt(i);
            }
        }

        closestCell = neighborCells[0];

        for (int i = 1; i < neighborCells.Count; i++)
        {
            if (EvaluateCellDistance(neighborCells[i], destination) < EvaluateCellDistance(closestCell, destination))
            {
                closestCell = neighborCells[i];
            }
        }

        return closestCell;
    }

    public IEnumerable<ICell> DirectPath(ICell origin, ICell destination)
    {
        GetFreshWorld();
        currentRoute = new List<ICell>();

        cellsToEvaluate = battlefieldManager.GetNeighborCells(origin) as List<ICell>;
        cellsToEvaluate.Remove(origin);
        closestCell = origin;

        while (closestCell != destination)
        {
            cellsToEvaluate = battlefieldManager.GetNeighborCells(closestCell) as List<ICell>;

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

    public IEnumerable<Vector3Int> DirectPath(Vector3Int origin, Vector3Int destination)
    {
        ICell originCell;
        ICell destinationCell;

        GetFreshWorld();
        world.TryGetValue(origin, out originCell);
        world.TryGetValue(destination, out destinationCell);

        return DirectPath(originCell, destinationCell) as IEnumerable<Vector3Int>;        
    }

    protected bool CellIsBlocked(ICell cell) => cell.Unit != null ? true : false;

    protected void GetFreshWorld() => world = battlefieldManager.World;

    protected float EvaluateCellDistance(ICell position, ICell destination) => Vector3Int.Distance(position.Position, destination.Position);
}