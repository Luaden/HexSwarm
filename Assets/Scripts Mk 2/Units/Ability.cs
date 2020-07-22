using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Ability : IAbility
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Sprite Icon { get; set; }
    
    [SerializeField] protected MoveHighlight defaultRanges;
    public IMoveHighlight DefaultRanges => defaultRanges;

    [SerializeField] protected int movementRange;
    public int MovementRange { get; private set; }
    public bool IsJump { get; private set; }



    public IEnumerable<Vector3Int> GetAttack(Direction direction, Vector3Int origin)
    {
        if (direction != Direction.Zero)
            Debug.LogError("not yet supported");

        return GameManager.Battlefield.GetNeighborCells(origin).Where(X=>X.Unit != default).Select(X=>X.GridPosition);
    }
}

