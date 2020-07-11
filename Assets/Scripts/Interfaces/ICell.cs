using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface ICell 
{
    Vector3Int Position { get; }
    IUnit Unit { get; set; }
    TileBase Tile { get; set; }

}
