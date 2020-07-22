using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum MapShape
{
    Hexagon = 0,
    Rectangle = 1,
    Weird = 2,    
    Random = 3
}

public interface IBattlefieldManager
{
    IReadOnlyDictionary<Vector3Int,ICell> World { get; }
    IEnumerable<ICell> GetNeighborCells(ICell origin, int range = 1);
    IEnumerable<ICell> GetNeighborCells(Vector3Int origin, int range = 1);
    Vector3Int GetVectorByClick(Vector2 mouseScreenPos);
    void GenerateGrid(int gridHeight, MapShape mapShape);
    void HighlightGrid(IEnumerable<ICell> moveCells, IEnumerable<ICell> attackCells);
    void HighlightGrid(IEnumerable<ICell> moveCells);
    void ClearHighlights();
    void PlaceNewUnit(IUnit unit, Vector3Int position);
    void MoveUnit(Vector3Int unitPosition, Vector3Int destination);
    void DestroyUnit(Vector3Int unitPosition);
    Vector3 GetWorldLocation(Vector3Int location);
    IEnumerable<ICell> GetValidCells(Vector3Int gridOrigin, IEnumerable<Vector3Int> worldOffsets);
}
