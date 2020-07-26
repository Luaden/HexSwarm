using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITeam 
{
    string Name { get; }
    string Description { get; }
    Sprite Icon { get; }    
    Teams TeamNumber { get; }
    Vector3Int StartPosition { get; }
    IEnumerable<IUnit> Units { get; }
    void StartTurn();
    void NextMove(float deltaTime);
    void EndTurn();
    void RemoveUnit(IUnit unit);
    void AddNewUnit(IUnit unit);
}
