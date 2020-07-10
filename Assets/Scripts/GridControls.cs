using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridControls : MonoBehaviour, IGrid
{
    [SerializeField] protected Grid mapGrid;
    [SerializeField] protected Tilemap mapTiles;
    [SerializeField] protected TileBase tile;
    [SerializeField] protected int gridHeight = 3;

    protected Camera mainCamera;
    protected HashSet<Vector3Int> gridCells;


    public void GenerateGrid(int gridHeight, TileBase tile)
    {
        Vector3Int cellPosition = new Vector3Int(0, 0, 0);

        for (int i = gridHeight; i >= -gridHeight; i--)
        {
            Vector3Int cellPositionModified = new Vector3Int(cellPosition.x, cellPosition.y = i, cellPosition.z);
            mapTiles.SetTile(cellPositionModified, tile);

            gridCells.Add(cellPositionModified);

            for (int j = gridHeight; j >= -gridHeight; j--)
            {
                cellPositionModified = new Vector3Int(cellPositionModified.x = j, cellPositionModified.y, cellPositionModified.z);
                mapTiles.SetTile(cellPositionModified, tile);

                gridCells.Add(cellPositionModified);
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

    public IEnumerable<Vector3Int> GetGridDimensions()
    {
        return gridCells;
    }

    public Vector3Int GetNeighborCells()
    {
        throw new System.NotImplementedException();
    }

    protected void Awake()
    {
        if (mapGrid == null)
            mapGrid = GetComponent<Grid>();
        if (mapTiles == null)
            mapTiles = GetComponentInChildren<Tilemap>();

        mainCamera = Camera.main;
        gridCells = new HashSet<Vector3Int>();
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
