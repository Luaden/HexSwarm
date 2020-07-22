using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[System.Serializable]
public class Ability : IAbility
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Sprite Icon { get; set; }
    
    public static IReadOnlyDictionary<int, Quaternion> Transforms
        = new Dictionary<int, Quaternion>()
     {
            {(int)Direction.Sixty,       Quaternion.AngleAxis(60,new Vector3(0.6f,0.6f,0.6f))},
            {(int)Direction.One20,       Quaternion.AngleAxis(120,new Vector3(0.6f,0.6f,0.6f))},
            {(int)Direction.One80,       Quaternion.AngleAxis(180,new Vector3(0.6f,0.6f,0.6f))},
            {(int)Direction.Two40,       Quaternion.AngleAxis(240,new Vector3(0.6f,0.6f,0.6f))},
            {(int)Direction.Threehundred,Quaternion.AngleAxis(300,new Vector3(0.6f,0.6f,0.6f))},
     };


    public static IEnumerable<Vector3Int> RotateAbility(Direction direction, IEnumerable<Vector3Int> ability)
    {
        Quaternion transform;
        if (Transforms.TryGetValue((int)direction, out transform))
            return ability.Select(X => Vector3Int.RoundToInt(transform * X));
        return ability;
    }

    [SerializeField] protected MoveHighlight defaultRanges;
    public IMoveHighlight DefaultRanges => defaultRanges;
    [SerializeField] protected int movementRange;
    public int MovementRange => movementRange;
    [SerializeField] protected bool isJump;
    public bool IsJump => isJump;

    public IEnumerable<ICell> GetAttack(Direction direction, Vector3Int gridOrigin)
    {
        return GameManager.Battlefield.GetValidCells(gridOrigin, RotateAbility(direction, defaultRanges.DamageRange));
    }
}

