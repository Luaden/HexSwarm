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
    
    [SerializeField] protected IGrid world;

    [SerializeField] protected int unitRange;

    [SerializeField] protected IReadOnlyList<IPosAbilityDefault> abilites;

    public string Name => name;
    public string Description => description; 
    public TileBase Tile => tile;
    public Sprite Icon => icon;
    public Color Color => color;
    public Vector3Int Location { get; set; }
    public IGrid World => world;
    public int UnitRange => unitRange;
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
    }

    void IUnit.ApplyAbility(IPosAbilityDefault move, Vector2 temp)
    {
        throw new System.NotImplementedException();
    }

    IPosAblity IUnit.GeneratePosAblity(IPosAbilityDefault move)
    {
        throw new System.NotImplementedException();
    }    
}
