using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using MapExtensions;

public class BattlefieldManager : MonoBehaviour, IBattlefieldManager
{
    [SerializeField, Header("Ground")] protected Tilemap groundMap;
    [SerializeField] protected Tile[] groundTiles;
    [SerializeField, Header("Movement")] protected Tilemap moveHighlightMap;
    [SerializeField] protected Tile moveTile;
    [SerializeField, Header("Attacks")] protected Tilemap possibleAttackHighlightMap;
    [SerializeField] protected Tile possibleAttackTile;
    [SerializeField, Header("Attacks")] protected Tilemap attackHighlightMap;
    [SerializeField] protected Tile attackTile;
    [SerializeField, Header("Unmoved Units")] protected Tilemap unmovedUnitsHighlightMap;
    [SerializeField] protected Tile unmovedUnitsHighlightTile;
    [SerializeField, Header("Unmoved Units")] protected Tilemap selectedUnitsHighlightMap;
    [SerializeField] protected Tile selectedUnitsHighlightTile;

    protected Grid mapGrid;
    protected Dictionary<Vector3Int, ICell> gridLookup = new Dictionary<Vector3Int, ICell>();
    protected Dictionary<Vector3Int, ICell> threeAxisLookup = new Dictionary<Vector3Int, ICell>();
    protected List<ICell> neighbors;
    protected ICell checkCell;
    protected ICell fromCell;
    protected ICell toCell;

    public IReadOnlyDictionary<Vector3Int, ICell> World => gridLookup;

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
        moveHighlightMap.ClearAllTiles();
        gridLookup.Clear();
        threeAxisLookup.Clear();
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

    protected void GenerateCell(Vector3Int gridPos, Tile tile)
    {
        int newX = gridPos.x - gridPos.y / 2;
        if (gridPos.y % 2 == -1)
            newX++;
        int newZ = -(newX + gridPos.y);
        Vector3Int threeAxisPosition = new Vector3Int(newX, gridPos.y, newZ);
        Cell newCell = new Cell(gridPos, threeAxisPosition, GetWorldLocation(gridPos), default, tile);

        gridLookup.Add(gridPos, newCell);
        threeAxisLookup.Add(threeAxisPosition, newCell);

        groundMap.SetTile(gridPos, tile);
    }
    #endregion

    #region Get Neighbors
    public IEnumerable<ICell> GetNeighborCells(ICell origin, int range = 1)
    {
        return GetNeighborCells(origin.GridPosition, range);
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
        gridLookup.TryGetValue(origin, out originCell);
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
            if (gridLookup.TryGetValue(currentLoc, out currentCell))
                neighbors.Add(currentCell as Cell);
        }
    }
    #endregion

    #region Highlight Cells
    public void HighlightGrid(IEnumerable<ICell> moveCells, IEnumerable<ICell> attackCells){ HighlightGrid(moveCells, attackCells.Select(X => X.GridPosition));}
    public void HighlightGrid(IEnumerable<Vector3Int> moveCells, IEnumerable<ICell> attackCells) { HighlightGrid(moveCells, attackCells.Select(X => X.GridPosition)); }
    public void HighlightGrid(IEnumerable<ICell> moveCells, IEnumerable<Vector3Int> attackCells) {HighlightGrid(moveCells.Select(X=>X.GridPosition), attackCells);}
    public void HighlightGrid(IEnumerable<ICell> moveCells) { HighlightGrid(moveCells.Select(X => X.GridPosition)); }
    public void HighlightPossibleAttacks(IEnumerable<ICell> possibleAttacks) { HighlightPossibleAttacks(possibleAttacks.Select(x => x.GridPosition)); }
    public void HighlightUnmovedUnits(IEnumerable<ICell> unmovedUnits) { HighlightUnmovedUnits(unmovedUnits.Select(x => x.GridPosition)); }
    public void HighlightSelectedUnit(ICell selectedUnits) { HighlightSelectedUnit(selectedUnits.GridPosition); }


    public void HighlightGrid(IEnumerable<Vector3Int> moveCells, IEnumerable<Vector3Int> attackCells)
    {
        HighlightGrid(moveCells);
        attackHighlightMap.PaintTiles(attackCells, attackTile);
    }

    public void HighlightGrid(IEnumerable<Vector3Int> moveCells)
    {
        ClearHighlights();
        moveHighlightMap.PaintTiles(moveCells, moveTile);
    }
   
    public void HighlightPossibleAttacks(IEnumerable<Vector3Int> possibleAttacks)
    {
        possibleAttackHighlightMap.ClearAllTiles();
        possibleAttackHighlightMap.PaintTiles(possibleAttacks, possibleAttackTile);
    }

    public void HighlightUnmovedUnits(IEnumerable<Vector3Int> unmovedUnits)
    {
        unmovedUnitsHighlightMap.ClearAllTiles();
        unmovedUnitsHighlightMap.PaintTiles(unmovedUnits, unmovedUnitsHighlightTile);
    }

    public void HighlightSelectedUnit(Vector3Int selectedUnits)
    {
        selectedUnitsHighlightMap.ClearAllTiles();
        selectedUnitsHighlightMap.SetTile(selectedUnits, selectedUnitsHighlightTile);
    }

    public void ClearHighlights()
    {
        moveHighlightMap.ClearAllTiles();
        attackHighlightMap.ClearAllTiles();
    }

    public void ClearSelectedUnitHighlight() => selectedUnitsHighlightMap.ClearAllTiles();
    #endregion

    #region Unit Control
    public bool CheckForUnit(Vector3Int location)
    {
        ICell cell;
        gridLookup.TryGetValue(location, out cell);

        if (cell.Unit != null)
            return true;
        return false;
    }

    public void PlaceNewUnit(IUnit unit, Vector3Int position)
    {
        World.TryGetValue(position, out checkCell);

        checkCell.Unit = unit;
        (unit as Unit).Location = position;

        GameManager.UnitAVController.PlaceNewUnit(unit as Unit);
    }

    public void MoveUnit(Vector3Int unitPosition, Vector3Int destination, IEnumerable<Vector3Int> path)
    {
        World.TryGetValue(unitPosition, out fromCell);
        World.TryGetValue(destination, out toCell);

        toCell.Unit = fromCell.Unit;
        (toCell.Unit as Unit).Location = destination;
        fromCell.Unit = null;

        GameManager.UnitAVController.MoveUnit(toCell.Unit as Unit, path);
    }

    public void DestroyUnit(Vector3Int unitPosition)
    {
        World.TryGetValue(unitPosition, out checkCell);

        GameManager.UnitAVController.DestroyUnit(checkCell.Unit as Unit);
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

    public Vector3 GetWorldLocation(Vector3Int location)
    {
        return mapGrid.CellToWorld(location);
    }


    protected void Awake() => mapGrid = GetComponent<Grid>();

    public ICell GetValidCell(Vector3Int worldLocation)
    {
        ICell location;
        threeAxisLookup.TryGetValue(worldLocation, out location);
        return location;
    }

    public IEnumerable<ICell> GetValidCells(Vector3Int gridOrigin, IEnumerable<Vector3Int> worldOffsets)
    {
        List<ICell> foundcells = new List<ICell>();
        ICell origin;
        if (!gridLookup.TryGetValue(gridOrigin, out origin))
            return System.Array.Empty<ICell>();

        foreach(Vector3Int worldOffset in worldOffsets)
        {
            ICell target;
            if (threeAxisLookup.TryGetValue(origin.ThreeAxisPosition + worldOffset, out target))
                foundcells.Add(target);
        }
        return foundcells;
    }


}

