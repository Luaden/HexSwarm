using Old;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AttackPattern
{

    public IUnit Unit { get; }
    public IAbility Ability { get; }
    public Vector3Int TargetLocation { get; }
    public IEnumerable<Vector3Int> LocationsHit { get; }
    public int HitCount { get; }

    public AttackPattern(IUnit incUnit, IAbility incAbility, Vector3Int incTargetLocation, IEnumerable<Vector3Int> incLocationsHit)
    {
        Unit = incUnit;
        Ability = incAbility;
        TargetLocation = incTargetLocation;
        LocationsHit = incLocationsHit;
        HitCount = LocationsHit.Count();
    }
}
