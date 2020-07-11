using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class GridManager : MonoBehaviour, IGrid
{
    //Editor/debug variables
    [SerializeField] protected Unit tempUnit = new Unit();
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

    //Cached references
    protected Camera mainCamera;
    protected Dictionary<Vector3Int, Cell> world;
    protected HashSet<Cell> highlightedCells;
    protected BattlefieldManager battlefieldManager;


    public void GenerateGrid(int gridHeight, TileBase tile)
    {
        Vector3Int cellPosition = new Vector3Int(0, 0, 0);
        Cell checkCell;

        for (int i = gridHeight; i >= -gridHeight; i--)
        {
            Vector3Int cellPositionModified = new Vector3Int(cellPosition.x, cellPosition.y = i, cellPosition.z);
            groundTiles.SetTile(cellPositionModified, tile);

            world.TryGetValue(cellPositionModified, out checkCell);

            if (checkCell == null)
                world.Add(cellPositionModified, new Cell(cellPositionModified, null, tile));

            for (int j = gridHeight; j >= -gridHeight; j--)
            {
                cellPositionModified = new Vector3Int(cellPositionModified.x = j, cellPositionModified.y, cellPositionModified.z);
                groundTiles.SetTile(cellPositionModified, tile);

                world.TryGetValue(cellPositionModified, out checkCell);

                if (checkCell == null)
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
        battlefieldManager.PlaceNewUnit(tempUnit, unitSpawnPos);
        print("Unit generated at: " + unitSpawnPos);
    }

    //Make OnMouseDown if cells have colliders.
    protected void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetMouseCell();
            CheckCellForUnit(selectedCell);
        }

        if (Input.GetMouseButtonDown(1))
        {
            CheckMoveUnit();
        }
    }

    protected void GetMouseCell()
    {
        currentLocation = GetCellByClick(Input.mousePosition);
        battlefieldManager.World.TryGetValue(currentLocation, out selectedCell);
    }

    protected void CheckCellForUnit(Cell selectedCell)
    {
        if (selectedCell == null)
            return;

        if (selectedCell.Unit != null)
        {
            selectedUnit = selectedCell.Unit;
            selectedUnitPanel.UpdateUI(selectedUnit);

            if (selectedCell.Unit != null)
                print("Selected: " + selectedUnit.Name);

            return;
        }
    }

    protected void CheckMoveUnit()
    {
        if (selectedUnit == null)
            return;

        GetMouseCell();

        if (selectedCell.Unit != null)
            return;

        battlefieldManager.MoveUnit(selectedUnit.Location, selectedCell.Position);
    }

}
