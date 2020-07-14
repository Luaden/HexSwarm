using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum MapShape
{
    Hexagon = 1,
    Rectangle = 2,
    Weird = 3,    
    Random = 4
}

public interface IBattlefieldManager
{
    IReadOnlyDictionary<Vector3Int,ICell> World { get; }
    IEnumerable<ICell> GetNeighborCells(ICell origin, int range = 1);
    IEnumerable<ICell> GetNeighborCells(Vector3Int origin, int range = 1);
    Vector3Int GetVectorByClick(Vector2 mouseScreenPos);
    void GenerateGrid(int gridHeight, MapShape mapShape);
    void HighlightGrid(IEnumerable<ICell> moveCells, IEnumerable<ICell> attackCells);
    void ClearHighlights();
    bool PlaceNewUnit(IUnit unit, Vector3Int position);
    bool MoveUnit(Vector3Int unitPosition, Vector3Int destination, ITeam team);
    void DestroyUnit(Vector3Int unitPosition);    
}
