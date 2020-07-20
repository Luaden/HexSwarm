using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendPattern
{


    public IUnit Unit { get; }
    public IAbility Ability { get; }
    public Vector3Int TargetLocation { get; }

    public DefendPattern(IUnit incUnit, IAbility incAbility, Vector3Int incTargetLocation)
    {
        Unit = incUnit;
        Ability = incAbility;
        TargetLocation = incTargetLocation;
    }
}
