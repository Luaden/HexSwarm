using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TestTeam : Team
{
    [SerializeField] protected bool outOfGuys = false;

    public new int RemainingUnits => (outOfGuys)?0:1;
    public new bool HasUnitsAfterLosses(IEnumerable<IUnit> losses)
    {
        return outOfGuys;
    }
}

