using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Cell : ICell
{
    protected Vector3Int position;
    protected IUnit unit;
    protected TileBase tile;

    public Vector3Int Position => position;
    public IUnit Unit => unit;
    public TileBase Tile => tile;

    public Cell(Vector3Int position, IUnit unit, TileBase tile)
    {
        this.position = position;
        this.unit = unit;
        this.tile = tile;
    }

    
}
