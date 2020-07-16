using Old;
using System.Collections.Generic;
using UnityEngine;

public class AttackPattern
{
    IUnit unit;
    IAbility ability;
    Vector3Int targetLocation;
    List<Vector3Int> locationsHit;

    public IUnit Unit => unit;
    public IAbility Ability => ability;
    public Vector3Int TargetLocation => targetLocation;
    public List<Vector3Int> LocationsHit => locationsHit;

    public AttackPattern(IUnit incUnit, IAbility incAbility, Vector3Int incTargetLocation, List<Vector3Int> incLocationsHit)
    {
        unit = incUnit;
        ability = incAbility;
        targetLocation = incTargetLocation;
        locationsHit = incLocationsHit;
    }
}
