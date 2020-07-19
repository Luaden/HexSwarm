using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITeam 
{
    string Name { get; }
    string Description { get; }
    Sprite Icon { get; }    
    Color PrimaryColor { get; }
    Color SecondaryColor { get; }
    Teams TeamNumber { get; }
    Vector3Int StartPosition { get; }
    bool MyTurn { get; }
    IEnumerable<IUnit> Units { get; }

    void StartTurn();    
    void EndTurn();
    void KillUnit(IUnit unit);
}
