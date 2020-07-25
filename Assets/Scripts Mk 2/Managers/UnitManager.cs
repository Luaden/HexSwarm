using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class UnitManager
{
    [SerializeField] protected List<SOUnit> EditorList;
    protected Dictionary<Units, SOUnit> createUnits;

    public void Awake()
    {
        EditorList = GameObject.FindObjectsOfType<SOUnit>().ToList();
        createUnits = EditorList.ToDictionary(X=>((Unit)X).ID);
    }

    public Unit this[Units key]
    {
        get
        {
            SOUnit retrieved;
            createUnits.TryGetValue(key, out retrieved);
            return retrieved;
        }
    }
}
