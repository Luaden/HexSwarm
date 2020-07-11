using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Unit : IUnit
{
    [SerializeField] protected string name;

    [SerializeField] protected string description;

    [SerializeField] TileBase tile;

    [SerializeField] protected Sprite icon;

    [SerializeField] protected Color color;
    
    [SerializeField] protected GridManager world;

    [SerializeField] protected int unitRange;



    [SerializeField] Team member;
    public ITeam Member => member;

    public string Name => name;
    public string Description => description; 
    public TileBase Tile => tile;
    public Sprite Icon => icon;
    public Color Color => color;
    public Vector3Int Location { get; set; }
    public IGrid World => world;
    public int UnitRange => unitRange;


    [SerializeField] protected List<IPosAbilityDefault> abilites;
    public IReadOnlyList<IPosAbilityDefault> Abilites => abilites;

    public Unit() { }

    public Unit(Unit unit)
    {
        name = unit.Name;
        description = unit.Description;
        tile = unit.Tile;
        icon = unit.Icon;
        color = unit.Color;
        world = unit.world;
        unitRange = unit.unitRange;
        abilites = unit.abilites;
        member = unit.member;
    }

    public void ApplyAbility(IPosAbilityDefault move, Vector3Int temp)
    {
        Member.DoMove(this, move, temp);
    }

    public IEnumerable<Vector3Int> CalcuateValidNewLocation(IPosAbilityDefault move)
    {
        Vector3Int targetLocation = new Vector3Int(Location.x, Location.y - 1, Location.z);
        // if (CheckWorldForVector(targetlocation)
        //  return new Vector3Int[] {targetLocation};
        // else
         return new Vector3Int[0];
    }

    public IEnumerable<Vector3Int> DiscoverHits(Vector3Int location, IPosAbilityDefault move)
    {
        return new Vector3Int[0];
    }

}
