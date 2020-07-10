﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IGrid 
{
    IEnumerable<ICell> GetNeighborCells(Vector3Int origin, int range = 1);

    Vector3Int GetCellByClick();

    void GenerateGrid(int gridHeight, TileBase tile);

}
