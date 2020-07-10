using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IGrid 
{
    Vector3Int GetNeighborCells();

    IEnumerable<Vector3Int> GetGridDimensions();

    Vector3Int GetCellByClick();

    void GenerateGrid(int gridHeight, TileBase tile);

}
