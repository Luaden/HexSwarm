using UnityEngine;
using UnityEngine.Tilemaps;

public class Cell : ICell
{
    public Vector3Int Position { get; set; }
    public IUnit Unit { get; set; }
    public TileBase Tile { get; set; }

    //I added this so it would compile - Luaden
    public Cell(Vector3Int position, IUnit unit, TileBase tile)
    {
        Position = position;
        Unit = unit;
        Tile = tile;
    }
}
