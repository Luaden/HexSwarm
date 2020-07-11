using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Flags]
public enum Teams
{
    Player  = 1 << 0,
    AI1     = 1 << 1,
    AI2     = 1 << 2,
    AI3     = 1 << 3,
    AI4     = 1 << 4,
    AI5     = 1 << 5,
    AI6     = 1 << 6,
    AI7     = 1 << 7,
    AI8     = 1 << 8,
    AI9     = 1 << 9,
    AIS = AI1|AI2|AI3|AI4|AI5|AI6|AI7|AI8|AI9
}


public interface IPosAbilityDefault
{
    int ID { get; }
    string Name { get; }
    string Description { get; }
    Sprite MovementGrid { get; }
    Sprite DamageGrid { get; }
}

public interface IPosAblity : IPosAbilityDefault
{
    IEnumerable<Vector2> PossiblePlacements { get; }
    IReadOnlyDictionary<Vector2, HashSet<Vector2>> AttackZone { get; }
}

public interface IUnit
{
    string Name { get; }
    string Description { get; }
    TileBase Tile { get; }
    Sprite Icon { get; }
    Color Color { get; }
    Vector3Int Location { get; }
    IGrid World { get; }
    int UnitRange { get; }

    IReadOnlyList<IPosAbilityDefault> Abilites { get; }

    IPosAblity GeneratePosAblity(IPosAbilityDefault move);
    void ApplyAbility(IPosAbilityDefault move, Vector2 temp);
}
