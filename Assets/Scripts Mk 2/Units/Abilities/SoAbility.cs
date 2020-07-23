using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SOAbility", menuName = "ScriptableObjects/SOAbility")]
public class SOAbility : ScriptableObject
{
    [SerializeField] protected Ability myAbility;
    public static implicit operator Ability(SOAbility abily) => abily.myAbility;
}
