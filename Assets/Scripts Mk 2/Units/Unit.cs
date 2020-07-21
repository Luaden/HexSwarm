using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Unit : IUnit
{
    public static int UnitIDAssigner = -1;
    
    [SerializeField]protected int id = -1;
    public int ID
    {
        get
        {
            if (id == -1)
                id = ++UnitIDAssigner;
            return id;
        }
    }

    [SerializeField] protected Sprite icon;
    public Sprite Icon { get => icon; set => icon = value; }

    public Unit() { if (ID == -1) throw new System.InvalidOperationException(); }
    public Unit(IUnit unit) : this()
    {
        id = unit.ID;
        name = unit.Name;
        description = unit.Description;
        icon = unit.Icon;
        ablities.AddRange(unit.Abilites.Select(X=>X as Ability));
    }

    [SerializeField] protected string name;
    public string Name => name;

    [SerializeField] protected string description;
    public string Description => description;

    public ITeam Team { get; set; }

    public Vector3Int Location { get; set; }

    [SerializeField] protected List<Ability> ablities = new List<Ability>(); 
    public IReadOnlyList<IAbility> Abilites => ablities;



    public IEnumerable<Vector3Int> CalcuateValidNewLocation(IAbility move)
    {
        return GameManager.Battlefield.GetNeighborCells(Location, move.MovementRange)
            .Select(X=>X.GridPosition);
    }

    public IEnumerable<Vector3Int> DiscoverHits(Vector3Int location, IAbility move, Direction direction = Direction.Zero)
    {
        return move.GetAttack(Direction.Zero, location);
    }
}
