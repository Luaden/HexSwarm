using UnityEngine;
using UnityEngine.Tilemaps;

public class Cell : ICell
{
    public Vector3Int GridPosition { get; set; }
    public Vector3Int ThreeAxisPosition { get; set; }
    public Vector3 WorldPosition { get; set; }
    public IUnit Unit { get; set; }
    public TileBase Tile { get; set; }

    //I added this so it would compile - Luaden
    public Cell(Vector3Int gridPosition, Vector3Int threeAxisPosition, Vector3 worldPosition, IUnit unit, TileBase tile)
    {
        GridPosition = gridPosition;
        ThreeAxisPosition = threeAxisPosition;
        WorldPosition = worldPosition;
        Unit = unit;
        Tile = tile;
    }
}
