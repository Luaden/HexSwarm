using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AttackPattern
{

    public IUnit Unit { get; }
    public IAbility Ability { get; }
    public Direction Direction { get; }
    public Vector3Int TargetLocation { get; }
    public IEnumerable<Vector3Int> Path { get; }
    public IEnumerable<Vector3Int> LocationsHit { get; }
    public int HitCount { get; }

    public AttackPattern(IUnit incUnit, IAbility incAbility, Direction direction, Vector3Int incTargetLocation, IEnumerable<Vector3Int> incPath, IEnumerable<Vector3Int> incLocationsHit)
    {
        Unit = incUnit;
        Ability = incAbility;
        Direction = direction;
        TargetLocation = incTargetLocation;
        Path = incPath;
        LocationsHit = incLocationsHit;
        HitCount = LocationsHit.Count();
    }
}
