using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace Old
{
    public class Cell : ICell
    {
        protected Vector3Int position;

        public Vector3Int Position => position;
        public IUnit Unit { get; set; }
        public TileBase Tile { get; set; }

        public Cell(Vector3Int position, IUnit unit, TileBase tile)
        {
            this.position = position;
            Unit = unit;
            Tile = tile;
        }


    }
}
