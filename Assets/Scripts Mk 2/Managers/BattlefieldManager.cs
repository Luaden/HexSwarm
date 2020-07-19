using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


public class BattlefieldManager : MonoBehaviour, IBattlefieldManager
{
    [SerializeField] protected Tile[] groundTiles;
    [SerializeField] protected Tile[] highlightTiles;
    [SerializeField] protected Tilemap groundMap;
    [SerializeField] protected Tilemap highlightMap;

    protected Grid mapGrid;
    protected Dictionary<Vector3Int, ICell> world;
    protected List<ICell> highlightedCells;
    protected List<ICell> neighbors;
    protected ICell checkCell;
    protected ICell fromCell;
    protected ICell toCell;

    public IReadOnlyDictionary<Vector3Int, ICell> World => world;

    #region Map Generation
    public void GenerateGrid(int gridHeight, MapShape mapShape = MapShape.Hexagon)
    {
        Clear();

        if (mapShape == MapShape.Hexagon)
            GenerateHexagon(gridHeight);
    }
    public void Clear()
    {
        groundMap.ClearAllTiles();
        highlightMap.ClearAllTiles();
        world.Clear();
    }

    protected void GenerateHexagon(int radius)
    {
        //Add randomization for ground types somewhere using the line below.
        Tile tile = groundTiles[0];
        GenerateRow(0, -radius, radius, tile);
        for (int i = 1; i <= radius; i++)
        {
            int half = i / 2;
            int oddCorrection = i % 2;
            GenerateRow(i, -radius + half, radius - half - oddCorrection, tile);
            GenerateRow(-i, -radius + half, radius - half - oddCorrection, tile);
        }
    }

    protected void GenerateRow(int Y, int xMin, int xMax, Tile tile)
    {
        int currentX = xMin;
        while (currentX <= xMax)
            GenerateCell(new Vector3Int(currentX++, Y, 0), tile);
    }

    protected void GenerateCell(Vector3Int pos, Tile tile)
    {
        Cell newCell = new Cell(pos, default, tile);

        world.Add(pos, newCell);
        groundMap.SetTile(pos, tile);
    }
    #endregion

    #region Get Neighbors
    public IEnumerable<ICell> GetNeighborCells(ICell origin, int range = 1)
    {
        return GetNeighborCells(origin.Position, range);
    }

    public IEnumerable<ICell> GetNeighborCells(Vector3Int origin, int range = 1)
    {
        neighbors = new List<ICell>();

        int y = origin.y;
        int xMax = origin.x + range;
        int xMin = origin.x - range;

        GetNeighborCellRow(y, xMin, xMax);

        for (int i = 1; i <= range; i++)
        {
            int half = i / 2;
            int oddCorrection = i % 2;

            if (Mathf.Abs(y) % 2 > 0)
            {
                GetNeighborCellRow(y - i, xMin + half + oddCorrection, xMax - half);
                GetNeighborCellRow(y + i, xMin + half + oddCorrection, xMax - half);
            }
            else
            {
                GetNeighborCellRow(y - i, xMin + half, xMax - half - oddCorrection);
                GetNeighborCellRow(y + i, xMin + half, xMax - half - oddCorrection);
            }
        }

        ICell originCell;
        world.TryGetValue(origin, out originCell);
        neighbors.Remove(originCell);
        return neighbors;
    }

    protected void GetNeighborCellRow(int y, int xMin, int xMax)
    {
        Vector3Int currentLoc;
        ICell currentCell;
        for (int i = xMin; i <= xMax; i++)
        {
            currentLoc = new Vector3Int(i, y, 0);
            if (world.TryGetValue(currentLoc, out currentCell))
                neighbors.Add(currentCell as Cell);
        }
    }
    #endregion

    #region Highlight Cells
    public void HighlightGrid(IEnumerable<ICell> moveCells, IEnumerable<ICell> attackCells)
    {
        ClearHighlights();

        foreach (ICell cell in moveCells)
        {
            if (attackCells.Contains(cell))
                highlightMap.SetTile(cell.Position, highlightTiles[2]);
            else
                highlightMap.SetTile(cell.Position, highlightTiles[0]);

            highlightedCells.Add(cell);
        }

        foreach (ICell cell in attackCells)
        {
            if (!moveCells.Contains(cell))
                highlightMap.SetTile(cell.Position, highlightTiles[0]);

            highlightedCells.Add(cell);
        }
    }

    public void ClearHighlights()
    {
        foreach (ICell cell in highlightedCells)
        {
            highlightMap.SetTile(cell.Position, null);
        }

        highlightedCells.Clear();
    }
    #endregion

    #region Unit Control
    public bool PlaceNewUnit(IUnit unit, Vector3Int position)
    {
        World.TryGetValue(position, out checkCell);

        if (checkCell.Unit != null)
            return false;

        checkCell.Unit = unit;
        //Whatever implements the visual representation for units needs to go here.
        return true;
    }

    public bool MoveUnit(Vector3Int unitPosition, Vector3Int destination, ITeam team)
    {
        World.TryGetValue(unitPosition, out fromCell);
        World.TryGetValue(destination, out toCell);

        if (team.TeamNumber != fromCell.Unit.Team.TeamNumber)
            return false;

        if (toCell.Unit != null)
            return false;

        toCell.Unit = fromCell.Unit;
        (toCell.Unit as Unit).Location = destination;
        fromCell.Unit = null;

        //Whatever implements the visual representation for units needs to go here.
        return true;
    }

    public void DestroyUnit(Vector3Int unitPosition)
    {
        World.TryGetValue(unitPosition, out checkCell);

        //Whatever implements the visual representation for units needs to go here.
        checkCell.Unit = null;
    }

    public void DestroyUnits(IEnumerable<Vector3Int> unitPositions)
    {
        foreach(Vector3Int unit in unitPositions)
            DestroyUnit(unit);
    }

    #endregion

    public Vector3Int GetVectorByClick(Vector2 mouseScreenPos)
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        Vector3Int mouseCellPos = mapGrid.WorldToCell(mouseWorldPos);

        return mouseCellPos;
    }

    protected void Awake()
    {
        mapGrid = GetComponent<Grid>();
    }

}

