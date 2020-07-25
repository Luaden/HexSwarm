using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using ThreeAxisExtensions;

[CreateAssetMenu(fileName = "SOMovementRange", menuName = "ScriptableObjects/SOMovementRange")]
public class SOMovementRange : ScriptableObject, IRange<Vector3Int>
{
    [SerializeField] protected bool rotates = false;
    public bool Rotates => rotates;
    [SerializeField] protected bool isJump = false;
    public bool IsJump => isJump;
    [SerializeField] protected bool needsClearSpace = true;
    public bool NeedsClearSpace => needsClearSpace;
    [SerializeField] protected int movementRange = 2;
    public int MovementRange => movementRange;
    [SerializeField] protected List<Vector3Int> ThreeAxisRange;

    public IEnumerator<Vector3Int> GetEnumerator()
    {
        return ThreeAxisRange.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ThreeAxisRange.GetEnumerator();
    }
    [ContextMenu("AppendRing")]
    void AppendRing()
    {
        HashSet<Vector3Int> finalAppends = new HashSet<Vector3Int>();
        if (movementRange <= 0)
            return;
        RightRange full = new RightRange(1, movementRange, true);
        foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
        {
            finalAppends.UnionWith(full.Rotate(direction));
        }
        if (movementRange <= 1)
            return;
        RightRange remove = new RightRange(1, movementRange-1,true);
        foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
        {
            finalAppends.ExceptWith(remove.Rotate(direction));
        }
        ThreeAxisRange.AddRange(finalAppends);
    }
    [ContextMenu("AppendHexagon")]
    void AppendHexagon()
    {
        HashSet<Vector3Int> finalAppends = new HashSet<Vector3Int>();
        finalAppends.Add(new Vector3Int(0, 0, 0));
        RightRange range = new RightRange(1, movementRange,true);
        foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
        {
            finalAppends.UnionWith(range.Rotate(direction));
        }
        ThreeAxisRange.AddRange(finalAppends);
    }
}
