using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IGrid 
{
    IEnumerable<Cell> GetNeighborCells(Cell origin, int range = 1);

    Vector3Int GetCellByClick(Vector2 mouseScreenPos);

    void GenerateGrid(int gridHeight, TileBase tile);

    void HighlightGrid(IEnumerable<Cell> tilesToHighlight);

    void ClearHighlightedTiles();

}
