using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : IUnit
{
    public string Name => throw new System.NotImplementedException();

    public string Description => throw new System.NotImplementedException();

    public Sprite Icon => throw new System.NotImplementedException();

    public ITeam Team { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public Vector3Int Location { get; set; }

    public IReadOnlyList<IAbility> Abilites => throw new System.NotImplementedException();

    public IEnumerable<Vector3Int> CalcuateValidNewLocation(IAbility move)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerable<Vector3Int> DiscoverHits(Vector3Int location, IAbility move)
    {
        throw new System.NotImplementedException();
    }
}
