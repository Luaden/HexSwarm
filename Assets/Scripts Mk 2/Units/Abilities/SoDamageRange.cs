using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

[CreateAssetMenu(fileName = "SODamgeRange", menuName = "ScriptableObjects/SODamgeRange")]
public class SODamgeRange : ScriptableObject, IRange<Vector3Int>
{
    [SerializeField] protected bool rotates = false;
    public bool Rotates => rotates;
    [SerializeField] protected bool isSpawn;
    public bool IsSpawn => isSpawn;
    [SerializeField] protected List<Vector3Int> ThreeAxisRange;

    public IEnumerator<Vector3Int> GetEnumerator()
    {
        return ThreeAxisRange.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ThreeAxisRange.GetEnumerator();
    }
}
