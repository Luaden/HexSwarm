using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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

    void GenerateRing()
    {

    }
}
