using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Unit : IUnit
{
    public static int UnitIDAssigner = 1;
    
    [SerializeField]protected Units id = Units.empty;
    public Units ID => id;

    [SerializeField] protected Sprite icon;
    public Sprite Icon { get => icon; set => icon = value; }

    public Unit() { }
    public Unit(IUnit unit) : this()
    {
        id = unit.ID;
        name = unit.Name;
        description = unit.Description;
        icon = unit.Icon;
        ablities.AddRange(((Unit)unit).ablities);
    }

    [SerializeField] protected string name;
    public string Name => name;

    [SerializeField] protected string description;
    public string Description => description;

    public ITeam Team { get; set; }

    public Vector3Int Location { get; set; }

    [SerializeField] protected List<SOAbility> ablities = new List<SOAbility>(); 
    public IEnumerable<IAbility> Abilites => ablities.Select(X=>((IAbility)(Ability)X));



    public IEnumerable<Vector3Int> CalcuateValidNewLocation(IAbility move)
    {
        return GameManager.Battlefield.GetNeighborCells(Location, move.MovementRange)
            .Select(X=>X.GridPosition);
    }

    public IEnumerable<ICell> DiscoverHits(Vector3Int location, IAbility move, Direction direction = Direction.Zero)
    {
        return move.GetAttack(direction, location).Where(x => x.Unit != default);
    }
}
