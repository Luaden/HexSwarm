using System.Collections.Generic;
using UnityEngine;

public interface IUnit
{
    Units ID { get; }
    string Name { get; }
    string Description { get; }
    Sprite Icon { get; }
    ITeam Team { get; set; }
    Vector3Int Location { get; }

    IEnumerable<IAbility> Abilites { get; }
    IEnumerable<Vector3Int> CalcuateValidNewLocation(IAbility move);
    IEnumerable<ICell> DiscoverHits(Vector3Int location, IAbility move, Direction refernce = Direction.Zero);
}

