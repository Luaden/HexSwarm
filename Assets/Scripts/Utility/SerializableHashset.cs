using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class SerializableHashset<T> : HashSet<T>, ISerializationCallbackReceiver
{
    [SerializeField] List<T> entries = new List<T>();

    public void OnAfterDeserialize()
    {
        this.Clear();
        this.UnionWith(entries);
    }

    public void OnBeforeSerialize()
    {
        if (this.Count == entries.Count)
            return;
        entries.Clear();
        entries.AddRange(this);
    }
}

