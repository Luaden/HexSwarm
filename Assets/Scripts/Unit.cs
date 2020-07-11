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
    
    [SerializeField] protected Vector3Int location;

    [SerializeField] protected IGrid world;

    [SerializeField] protected int unitRange;

    [SerializeField] protected IReadOnlyList<IPosAbilityDefault> abilites;

    public string Name => name;
    public string Description => description; 
    public TileBase Tile => tile;
    public Sprite Icon => icon;
    public Color Color => color;
    public Vector3Int Location => location;
    public IGrid World => world;
    public int UnitRange => unitRange;
    public IReadOnlyList<IPosAbilityDefault> Abilites => abilites;

    void IUnit.ApplyAbility(IPosAbilityDefault move, Vector2 temp)
    {
        throw new System.NotImplementedException();
    }

    IPosAblity IUnit.GeneratePosAblity(IPosAbilityDefault move)
    {
        throw new System.NotImplementedException();
    }    
}
