using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ThreeAxisExtensions;

[System.Serializable]
public class Ability : IAbility
{
    [SerializeField] protected Abilitys Id;
    public Abilitys ID => Id;
    [SerializeField] protected string name;
    public string Name { get => name; set => name = value;}
    [SerializeField] protected string description;
    public string Description { get => description; set => description = value; }
    [SerializeField] protected Sprite icon;
    public Sprite Icon { get => icon; set => icon = value; }


    [SerializeField] protected SODamageRange ThreeAxisAttack;
    public bool IsSpawn => ThreeAxisAttack.IsSpawn;
    public bool IsSpawnVoid => ThreeAxisAttack.IsSpawnVoid;
    [SerializeField] protected SOMovementRange ThreeAxisMove;
    public bool NeedsClearLand => ThreeAxisMove.NeedsClearSpace;
    public bool IsJump => ThreeAxisMove.IsJump;
    public int MovementRange => ThreeAxisMove.MovementRange;

    public IEnumerable<ICell> GetMoves(Vector3Int gridOrigin, Direction direction = Direction.Zero)
    {
        return GameManager.Battlefield.GetValidCells(gridOrigin,
            ThreeAxisMove.Rotate(direction)
            );
    }
    public IEnumerable<ICell> GetAttack(Direction direction, Vector3Int gridOrigin)
    {
        return GameManager.Battlefield.GetValidCells(gridOrigin, 
            ThreeAxisAttack.Rotate(direction)
            );
    }

}

