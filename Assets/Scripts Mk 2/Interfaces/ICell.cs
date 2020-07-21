using UnityEngine;
using UnityEngine.Tilemaps;


public interface ICell
{
    Vector3Int GridPosition { get; }
    Vector3 WorldPosition { get; }
    IUnit Unit { get; set; }
    TileBase Tile { get; set; }
}

