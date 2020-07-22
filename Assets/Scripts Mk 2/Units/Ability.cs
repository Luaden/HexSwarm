using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[System.Serializable]
public class Ability : IAbility
{
    public static IReadOnlyDictionary<Direction, Func<Vector3Int, Vector3Int>> Transforms
        = new Dictionary<Direction, Func<Vector3Int, Vector3Int>>()
     {
            {Direction.Sixty,       (Vector3Int s)=>{ return new Vector3Int(s.y,s.x,s.z); }  },
            {Direction.One20,       (Vector3Int s)=>{ return new Vector3Int(s.z,s.x,s.y); }  },
            {Direction.One80,       (Vector3Int s)=>{ return new Vector3Int(s.z,s.y,s.x); }  },
            {Direction.Two40,       (Vector3Int s)=>{ return new Vector3Int(s.y,s.z,s.x); }  },
            {Direction.Threehundred,(Vector3Int s)=>{ return new Vector3Int(s.x,s.z,s.y); }  },
     };

    public static IEnumerable<Vector3Int> RotateAbility(Direction direction, IEnumerable<Vector3Int> ability)
    {
        Func<Vector3Int, Vector3Int> transform;
        if (Transforms.TryGetValue(direction, out transform))
            return ability.Select(X => transform(X));
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

