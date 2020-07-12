using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class GridManager : MonoBehaviour, IGrid
{
    //Editor/debug variables
    [SerializeField] protected Unit templateUnit;
    [SerializeField] protected Vector3Int unitSpawnPos;

    //Editor variables

    [SerializeField] protected Grid mapGrid;
    [SerializeField] protected Tilemap groundTiles;
    [SerializeField] protected Tilemap highlightTiles;
    [SerializeField] protected Tilemap unitTiles;
    [SerializeField] protected TileBase tile;
    public TileBase BasicTile => tile;
    [SerializeField] protected TileBase highlightTile;
    public TileBase HighlightTile => highlightTile;

    [SerializeField] protected int gridHeight = 3;

    //State variables
    protected Vector3Int currentLocation;
    protected bool canMove;

    //Cached references
    protected Dictionary<Vector3Int, Cell> world;
    protected HashSet<Cell> highlightedCells;
    protected BattlefieldManager battlefieldManager;
    protected List<Cell> neighbors;
    protected Camera mainCamera;

    public BattlefieldManager BattlefieldManager => battlefieldManager;

    public void Clear()
    {
        groundTiles.ClearAllTiles();
        highlightTiles.ClearAllTiles();
        unitTiles.ClearAllTiles();
        world.Clear();
    }

    #region MapGeneration
    public void GenerateGrid(int gridHeight, TileBase tile)
    {
        Clear();
        GenerateHexagon(gridHeight, tile);
    }

    protected void GenerateHexagon(int radius, TileBase tile)
    {
        GenerateRow(0, -radius, radius, tile);
        for (int i = 1; i <= radius; i++)
        {
            int half = i / 2;
            int oddCorrection = i % 2;
            GenerateRow(i, -radius + half, radius - half - oddCorrection, tile);
            GenerateRow(-i, -radius + half, radius - half - oddCorrection, tile);
        }
    }

    protected void GenerateRow(int Y, int xMin, int xMax, TileBase tile)
    {
        int currentX = xMin;
        while (currentX <= xMax)
            GenerateCell(new Vector3Int(currentX++, Y, 0), tile);
    }

    protected void GenerateCell(Vector3Int pos, TileBase tile)
    {
        Cell newCell = new Cell(pos, default, tile);
        world.Add(pos, newCell);
        groundTiles.SetTile(pos, tile);
    }

    //protected void GenerateSquare(int radius, TileBase tile)
    //{
    //    GenerateRow(0, -radius,radius, tile);
    //    for (int i = 1; i < radius; i++)
    //    {
    //        GenerateRow(i, -radius, radius, tile);
    //        GenerateRow(-i, -radius, radius, tile);
    //    }
    //}
    #endregion

    #region Cell/Vector Check

    public bool CheckWorldForVector(Vector3Int location)
    {
        return world.ContainsKey(location);
    }

    public bool CheckCanMove(Cell selectedCell) => canMove = selectedCell.Unit == default ? true : false;
    #endregion

    public void PaintUnitTile(Vector3Int cellToPaint, TileBase tileToPaint) => unitTiles.SetTile(cellToPaint, tileToPaint);

    #region Debug

    [ContextMenu("Generate Unit")]
    public IUnit DebugGenerateUnit()
    {
        Unit newUnit = new Unit(templateUnit);

        battlefieldManager.PlaceNewUnit(newUnit, unitSpawnPos);
        return newUnit;
    }


    //[ContextMenu("GenerateGrid")]
    protected void DebugGenerateGrid()
    {
        GenerateGrid(gridHeight, tile);
    }
    #endregion

    #region Mouse
    public Vector3Int GetCellByClick(Vector2 mouseScreenPos)
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        Vector3Int mouseCellPos = mapGrid.WorldToCell(mouseWorldPos);

        return mouseCellPos;
    }
    #endregion
    public IEnumerable<Cell> GetNeighborCells(Vector3Int origin, int range = 1)
    {
        neighbors = new List<Cell>();

        int y = origin.y;
        int xMax = origin.x + range;
        int xMin = origin.x - range;

        GetNeighborCellRow(y, xMin, xMax);

        for (int i = 1; i <= range; i++)
        {
            int half = i / 2;
            print(half);
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

        return neighbors;
    }
    public IEnumerable<Cell> GetNeighborCells(Cell origin, int range = 1)
    {
        print(origin.Position);
        return GetNeighborCells(origin.Position, range);
    }

    protected void GetNeighborCellRow(int y, int xMin, int xMax)
    {
        Vector3Int currentLoc;
        Cell currentCell;
        for (int i = xMin; i <= xMax; i++)
        {
            currentLoc = new Vector3Int(i, y, 0);
            if (world.TryGetValue(currentLoc, out currentCell))
                neighbors.Add(currentCell);
        }
    }

    public void HighlightGrid(IEnumerable<Cell> tilesToHighlight)
    {
        foreach(Cell cell in tilesToHighlight)
        {
            highlightTiles.SetTile(cell.Position, highlightTile);
            highlightedCells.Add(cell);
        }
    }

    public void ClearHighlightedTiles()
    {
        foreach (Cell cell in highlightedCells)
        {
            highlightTiles.SetTile(cell.Position, null);
        }

        highlightedCells.Clear();
    }


    protected void Awake()
    {
        if (mapGrid == null)
            mapGrid = GetComponent<Grid>();
        if (groundTiles == null)
            groundTiles = GetComponentInChildren<Tilemap>();


        mainCamera = Camera.main;
        world = new Dictionary<Vector3Int, Cell>();
        highlightedCells = new HashSet<Cell>();

        battlefieldManager = new BattlefieldManager(world, this);

        //DebugGenerateGrid();
    }



}
