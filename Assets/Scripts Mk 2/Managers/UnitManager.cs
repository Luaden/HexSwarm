using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class UnitManager : ISerializationCallbackReceiver
{
    [SerializeField] protected List<SOUnit> EditorList = new List<SOUnit>();
    protected Dictionary<Units, SOUnit> createUnits = new Dictionary<Units, SOUnit>();

    public Unit this[Units key]
    {
        get
        {
            SOUnit retrieved;
            createUnits.TryGetValue(key, out retrieved);
            return retrieved;
        }
    }

    public void OnAfterDeserialize()
    {
        createUnits.Clear();
        foreach (SOUnit type in EditorList.Where(x=>(x!= default)&&((Unit)x) != default))
            if(createUnits.ContainsKey(((Unit)type).ID))
                Debug.Log(string.Format("entry {0} has duplicate key {1}", EditorList.FindIndex(x => ((Unit)x).ID == ((Unit)type).ID), ((Unit)type).ID));
            else
                createUnits.Add(((Unit)type).ID, type);
    }

    public void OnBeforeSerialize()
    {
        EditorList.Clear();
        EditorList.AddRange(createUnits.Values);
        EditorList.Add(default);
    }
}
