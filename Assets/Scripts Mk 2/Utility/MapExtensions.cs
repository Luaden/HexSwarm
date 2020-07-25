using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;
using UnityEngine;

namespace MapExtensions
{
    public static class MapExtensions
    {
        public static void PaintTiles(this Tilemap map, IEnumerable<Vector3Int> fillList, Tile fill)
        {
            foreach (Vector3Int location in fillList)
                map.SetTile(location, fill);
        }
    }
}
