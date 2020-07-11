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
    [SerializeField] protected TileBase highlightTile;
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
    
    public void GenerateGrid(int gridHeight, TileBase tile)
    {
        for (int i = gridHeight; i >= -gridHeight; i--)
        {
            for (int j = gridHeight; j >= -gridHeight; j--)
            {
                Vector3Int cellPositionModified = new Vector3Int(j, i, 0);
                groundTiles.SetTile(cellPositionModified, tile);  

                if (!world.ContainsKey(cellPositionModified))
                    world.Add(cellPositionModified, new Cell(cellPositionModified, null, tile));
            }
        }
    }

    public Vector3Int GetCellByClick(Vector2 mouseScreenPos)
    {
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        
        Vector3Int mouseCellPos = mapGrid.WorldToCell(mouseWorldPos);

        return mouseCellPos;
    }

    public bool CheckWorldForVector(Vector3Int location)
    {
        return world.ContainsKey(location);
    }

    public IEnumerable<ICell> GetNeighborCells(Cell origin, int range = 1)
    {
        //Need logic here that gets neighbor cells in a hexagonal pattern;
        return new List<Cell>();
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

    public void PaintUnitTile(Vector3Int cellToPaint, TileBase tileToPaint) => unitTiles.SetTile(cellToPaint, tileToPaint);

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

        DebugGenerateGrid();
    }

    protected void DebugGenerateGrid()
    {
        GenerateGrid(gridHeight, tile);
    }

    [ContextMenu("Delete Grid")]
    protected void DebugDeleteGrid()
    {
        GenerateGrid(gridHeight, null);
    }

    [ContextMenu("Generate Unit")]
    protected void DebugGenerateUnit()
    {
        battlefieldManager.PlaceNewUnit(new Unit(templateUnit), unitSpawnPos);
    }

    //Make OnMouseDown if cells have colliders.
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

    protected void GetMouseCell()
    {
        currentLocation = GetCellByClick(Input.mousePosition);
        battlefieldManager.World.TryGetValue(currentLocation, out selectedCell);
    }

    protected void GetUnitFromCell(Cell selectedCell)
    {
        if (selectedCell == null)
            return;

        selectedUnit = selectedCell.Unit;
        selectedUnitPanel.UpdateUI(selectedUnit);
    }

    protected bool CheckCanMove(Cell selectedCell) => canMove = selectedCell.Unit == default ? true : false;

    protected void MoveUnit()
    {
        if (selectedUnit == null)
            return;

        GetMouseCell(); // assigns new Selected Cell
        if(CheckCanMove(selectedCell))
        {
            Debug.Log("Unit named: " + selectedUnit.Name + " is moving to " + selectedCell.Position);
            battlefieldManager.MoveUnit(selectedUnit.Location, selectedCell.Position);
            selectedUnit = null;
        }
    }


}
