using Old;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AttackPattern
{
    IUnit unit;
    IAbility ability;
    Vector3Int targetLocation;

    public IUnit Unit => unit;
    public IAbility Ability => ability;
    public Vector3Int TargetLocation => targetLocation;
    public IEnumerable<Vector3Int> LocationsHit { get; private set; }
    public int HitCount { get; private set; }

    public AttackPattern(IUnit incUnit, IAbility incAbility, Vector3Int incTargetLocation, IEnumerable<Vector3Int> incLocationsHit)
    {
        unit = incUnit;
        ability = incAbility;
        targetLocation = incTargetLocation;
        LocationsHit = incLocationsHit;
        HitCount = LocationsHit.Count();
    }
}
