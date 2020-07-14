using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITeam2 
{
    string Name { get; }
    string Description { get; }
    Sprite Icon { get; }    
    Color Color { get; }
    Teams TeamNumber { get; }
    bool MyTurn { get; }
    IGameManager GameManager { get; }
    IEnumerable<IUnit> Units { get; }

    void StartTurn();    
    void EndTurn();
    void KillUnit(IUnit unit);
}
