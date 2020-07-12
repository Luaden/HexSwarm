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
    [SerializeField] protected SelectedUnitPanel selectedUnitPanel;
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
    protected Cell selectedCell;
    protected IUnit selectedUnit;
    protected bool canMove;

    //Cached references
    protected Camera mainCamera;
    protected Dictionary<Vector3Int, Cell> world;
    protected HashSet<Cell> highlightedCells;
    protected BattlefieldManager battlefieldManager;
    protected List<Cell> neighbors;

    public BattlefieldManager BattlefieldManager => battlefieldManager;

    public void Clear()
    {
        groundTiles.ClearAllTiles();
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

    #region MouseClickFunctions
    public Vector3Int GetCellByClick(Vector2 mouseScreenPos)
    {
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        
        Vector3Int mouseCellPos = mapGrid.WorldToCell(mouseWorldPos);

        return mouseCellPos;
    }

    protected void GetMouseCell()
    {
        currentLocation = GetCellByClick(Input.mousePosition);
        battlefieldManager.World.TryGetValue(currentLocation, out selectedCell);
    }
    #endregion

    #region Cell/Vector Check

    public bool CheckWorldForVector(Vector3Int location)
    {
        return world.ContainsKey(location);
    }

    public void GetUnitFromCell(Cell selectedCell)
    {
        if (selectedCell == null)
            return;

        selectedUnit = selectedCell.Unit;
        selectedUnitPanel.UpdateUI(selectedUnit);
    }

    public bool CheckCanMove(Cell selectedCell) => canMove = selectedCell.Unit == default ? true : false;
    #endregion

    #region Unit Functions
    protected void MoveUnit()
    {
        if (selectedUnit == null)
            return;

        GetMouseCell(); // assigns new Selected Cell
        if (CheckCanMove(selectedCell))
        {
            Debug.Log("Unit named: " + selectedUnit.Name + " is moving to " + selectedCell.Position);
            battlefieldManager.MoveUnit(selectedUnit.Location, selectedCell.Position);
            selectedUnit = null;
        }
    }

    public void PaintUnitTile(Vector3Int cellToPaint, TileBase tileToPaint) => unitTiles.SetTile(cellToPaint, tileToPaint);

    #endregion

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

    public IEnumerable<Cell> GetNeighborCells(Cell origin, int range = 1)
    {
        print(origin.Position);
        neighbors = new List<Cell>();

        int y = origin.Position.y;
        int xMax = origin.Position.x + range;
        int xMin = origin.Position.x - range;

        GetNeighborCellRow(y, xMin, xMax);

        for (int i = 1; i <= range; i++)
        {
            int half = i / 2;
            print(half);
            int oddCorrection = i % 2;
            
            if(Mathf.Abs(y) % 2 > 0)
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

    protected void GetNeighborCellRow(int y, int xMin, int xMax)
    {
        Vector3Int currentLoc;
        Cell currentCell;

        for (int i = xMin; i <= xMax; i++)
        {
            currentLoc = new Vector3Int(i, y, 0);
            world.TryGetValue(currentLoc, out currentCell);
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
        if (selectedUnitPanel == null)
            selectedUnitPanel = FindObjectOfType<SelectedUnitPanel>();

        mainCamera = Camera.main;
        world = new Dictionary<Vector3Int, Cell>();
        highlightedCells = new HashSet<Cell>();

        battlefieldManager = new BattlefieldManager(world, this);

        //DebugGenerateGrid();
    }

    protected void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetMouseCell(); // assigns SelectedCell
            GetUnitFromCell(selectedCell); //Looks at SelectedCell and assigns SelectedUnit to the Unit in the cell.
        }

        if (Input.GetMouseButtonDown(1))
        {
            MoveUnit();
        }
    }

}
