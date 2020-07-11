using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridControls : MonoBehaviour, IGrid
{
    [SerializeField] protected Grid mapGrid;
    [SerializeField] protected Tilemap groundTiles;
    [SerializeField] protected Tilemap highlightTiles;
    [SerializeField] protected TileBase tile;
    [SerializeField] protected TileBase highlightTile;
    [SerializeField] protected int gridHeight = 3;

    protected Camera mainCamera;
    protected HashSet<Cell> world;
    protected HashSet<Cell> highlightedCells;


    public void GenerateGrid(int gridHeight, TileBase tile)
    {
        Vector3Int cellPosition = new Vector3Int(0, 0, 0);

        for (int i = gridHeight; i >= -gridHeight; i--)
        {
            Vector3Int cellPositionModified = new Vector3Int(cellPosition.x, cellPosition.y = i, cellPosition.z);
            groundTiles.SetTile(cellPositionModified, tile);

            world.Add(new Cell(cellPositionModified, null, tile));

            for (int j = gridHeight; j >= -gridHeight; j--)
            {
                cellPositionModified = new Vector3Int(cellPositionModified.x = j, cellPositionModified.y, cellPositionModified.z);
                groundTiles.SetTile(cellPositionModified, tile);

                world.Add(new Cell(cellPositionModified, null, tile));
            }
        }
    }

    public Vector3Int GetCellByClick()
    {
        Vector2 mouseScreenPos = Input.mousePosition;
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        
        Vector3Int mouseCellPos = mapGrid.WorldToCell(mouseWorldPos);

        return mouseCellPos;
    }

    public IEnumerable<ICell> GetNeighborCells(Cell origin, int range = 1)
    {
        return world;
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
        world = new HashSet<Cell>();
        highlightedCells = new HashSet<Cell>();
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
}
