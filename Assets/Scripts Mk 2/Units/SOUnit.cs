using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SOUnit", menuName = "ScriptableObjects/SOUnit")]
public class SOUnit: ScriptableObject
{
    [SerializeField] protected Unit myUnit;

    public static implicit operator Unit(SOUnit unit) => unit.myUnit;
}
