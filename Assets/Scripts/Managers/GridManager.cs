using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour, IGrid
{
    [SerializeField] protected SelectedUnitPanel selectedUnitPanel;
    [SerializeField] protected Grid mapGrid;
    [SerializeField] protected Tilemap groundTiles;
    [SerializeField] protected Tilemap highlightTiles;
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

        battlefieldManager = new BattlefieldManager(world);
    }

    [ContextMenu("Generate Grid")]
    protected void DebugGenerateGrid()
    {
        GenerateGrid(gridHeight, tile);
    }

    [ContextMenu("Delete Grid")]
    protected void DebugDeleteGrid()
    {
        GenerateGrid(gridHeight, null);
    }

    //Make OnMouseDown if cells have colliders.
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetMouseCell();
        }

        if (Input.GetMouseButtonDown(1))
        {
            CheckMoveUnit();
        }
    }

    private void GetMouseCell()
    {
        currentLocation = GetCellByClick(Input.mousePosition);
        battlefieldManager.World.TryGetValue(currentLocation, out selectedCell);

        if(selectedCell != null)
            print(selectedCell.Position);
    }

    private void CheckCellForUnit(Cell selectedCell)
    {
        if (selectedCell == null)
            return;

        if (selectedCell.Unit != null)
        {
            selectedUnitPanel.UpdateUI(selectedCell.Unit);
            selectedUnit = selectedCell.Unit;
            return;
        }
    }

    private void CheckMoveUnit()
    {
        if (selectedUnit == null)
            return;

        GetMouseCell();

        if (selectedCell.Unit != null)
            return;

        battlefieldManager.MoveUnit(selectedUnit.Location, selectedCell.Position);
    }

}
