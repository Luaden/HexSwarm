using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Unit : IUnit
{
    [SerializeField] protected string name;
    public string Name => name;

    [SerializeField] protected string description;
    public string Description => description;

    [SerializeField] protected Sprite icon;
    public Sprite Icon => throw new System.NotImplementedException();

    public ITeam Team { get; set; }

    public Vector3Int Location { get; set; }

    protected List<Ability> ablities; 
    public IReadOnlyList<IAbility> Abilites => ablities;

    public IEnumerable<Vector3Int> CalcuateValidNewLocation(IAbility move)
    {
        return GameManager.Battlefield.GetNeighborCells(Location, move.movementRange)
            .Select(X=>X.Position);
    }

    public IEnumerable<Vector3Int> DiscoverHits(Vector3Int location, IAbility move, Direction direction = Direction.Zero)
    {
        return move.GetAttack(Direction.Zero, location);
    }
}
