using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendPattern
{


    public IUnit Unit { get; }
    public IAbility Ability { get; }
    public IEnumerable<Vector3Int> Path { get; }
    public Vector3Int TargetLocation { get; }

    public DefendPattern(IUnit incUnit, IAbility incAbility, IEnumerable<Vector3Int> incPath, Vector3Int incTargetLocation)
    {
        Unit = incUnit;
        Ability = incAbility;
        Path = incPath;
        TargetLocation = incTargetLocation;
    }
}
