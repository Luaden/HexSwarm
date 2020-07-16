using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendPattern
{
    IUnit unit;
    IAbility ability;
    Vector3Int targetLocation;

    public IUnit Unit => unit;
    public IAbility Ability => ability;
    public Vector3Int TargetLocation => targetLocation;

    public DefendPattern(IUnit incUnit, IAbility incAbility, Vector3Int incTargetLocation)
    {
        unit = incUnit;
        ability = incAbility;
        targetLocation = incTargetLocation;
    }
}
