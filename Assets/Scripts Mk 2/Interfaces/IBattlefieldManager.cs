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
    void HighlightGrid(IEnumerable<Vector3Int> moveCells, IEnumerable<Vector3Int> attackCells);
    void HighlightGrid(IEnumerable<Vector3Int> moveCells, IEnumerable<ICell> attackCells);
    void HighlightGrid(IEnumerable<ICell> moveCells, IEnumerable<Vector3Int> attackCells);
    void HighlightGrid(IEnumerable<ICell> moveCells, IEnumerable<ICell> attackCells);
    void HighlightGrid(IEnumerable<ICell> moveCells);
    void HighlightSelectedUnit(ICell selectedUnits);
    void HighlightPossibleAttacks(IEnumerable<ICell> possibleAttacks);
    void HighlightUnmovedUnits(IEnumerable<ICell> unmovedUnits);
    void HighlightPossibleAttacks(IEnumerable<Vector3Int> possibleAttacks);
    void HighlightUnmovedUnits(IEnumerable<Vector3Int> unmovedUnits);
    void HighlightSelectedUnit(Vector3Int selectedUnits);



    void ClearHighlights();
    void ClearSelectedUnitHighlight();
    void PlaceNewUnit(IUnit unit, Vector3Int position);
    void MoveUnit(Vector3Int unitPosition, Vector3Int destination, IEnumerable<Vector3Int> path = null);
    void DestroyUnit(Vector3Int unitPosition);
    Vector3 GetWorldLocation(Vector3Int location);
    ICell GetValidCell(Vector3Int worldLocation);
    IEnumerable<ICell> GetValidCells(Vector3Int gridOrigin, IEnumerable<Vector3Int> worldOffsets);
}
