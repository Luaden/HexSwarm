using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Ability : IAbility
{
    [SerializeField] protected MoveHighlight defaultRanges;
    public IMoveHighlight DefaultRanges => defaultRanges;

    public int movementRange { get; private set; }
    public bool IsJump { get; private set; }

    public IEnumerable<Vector3Int> GetAttack(Direction direction, Vector3Int origin)
    {
        if (direction != Direction.Zero)
            Debug.LogError("not yet supported");

        return GameManager.Battlefield.GetNeighborCells(origin).Where(X=>X.Unit != default).Select(X=>X.Position);
    }
}

