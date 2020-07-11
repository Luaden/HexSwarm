using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class HashSetVector3Int : SerializableHashset<int> { }
[System.Serializable]
public class Ablity : IPosAblity
{
    [SerializeField] protected HashSetVector3Int starting;

    public IEnumerable<Vector3Int> PossiblePlacements => throw new System.NotImplementedException();

    public IReadOnlyDictionary<Vector3Int, HashSet<Vector3Int>> AttackZone => throw new System.NotImplementedException();

    public int ID => throw new System.NotImplementedException();

    public string Name => throw new System.NotImplementedException();

    public string Description => throw new System.NotImplementedException();

    public Sprite MovementGrid => throw new System.NotImplementedException();

    public Sprite DamageGrid => throw new System.NotImplementedException();
}
