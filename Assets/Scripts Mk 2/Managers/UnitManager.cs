using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitManager", menuName = "ScriptableObjects/UnitManager")]
public class UnitManager : ScriptableObject, IScriptableCollection<SOUnit>
{
    [SerializeField] protected List<SOUnit> EditorList;
    protected Dictionary<Units, SOUnit> createdUnits;
    IReadOnlyDictionary<Units, SOUnit> CreatedUnits {
        get
        {
            if (createdUnits == default)
                SetInfo(EditorList);
            return createdUnits;
        }
    }

    [ContextMenu("ReloadFromList")]
    protected void reloadFromList()
    {
        SetInfo(EditorList);
    }

    protected void Awake()
    {
        if (EditorList == default)
            return;
        SetInfo(EditorList);
    }

    public void SetInfo(IEnumerable<SOUnit> units)
    {
        EditorList = EditorList.ToList();
        createdUnits = EditorList.ToDictionary(X => ((Unit)X).ID);
    }

    public Unit this[Units key]
    {
        get
        {
            SOUnit retrieved;
            CreatedUnits.TryGetValue(key, out retrieved);
            return retrieved;
        }
    }
}
