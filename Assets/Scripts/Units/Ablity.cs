using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public abstract class Ablity : IPosAbilityDefault
{
    protected static int _ID;
    public HashSet<Vector3Int> movePattern;
    public HashSet<Vector3Int> attackPattern;
    public IEnumerable<Vector3Int> MovePattern { get; }
    public IEnumerable<Vector3Int> AttackPattern { get; }

    protected readonly int _iD;
    public int ID => _iD;

    [SerializeField] protected string name;
    public string Name => name;
    [SerializeField] protected string description;
    public string Description => description;
    [SerializeField] protected Sprite movementGrid;
    public Sprite MovementGrid => movementGrid;
    [SerializeField] protected Sprite damageGrid;
    public Sprite DamageGrid => damageGrid;

    public Ablity()
    {
        GenerateMoves();
        GenerateAttack();
        _iD = _ID++;
    }
    protected abstract HashSet<Vector3Int> GenerateMoves();
    protected abstract HashSet<Vector3Int> GenerateAttack();

    protected void GenerateRow(int Y, int Z, int xMin, int xMax, HashSet<Vector3Int> collection)
    {
        int currentX = xMin;
        while (currentX <= xMax)
            collection.Add(new Vector3Int(currentX++, Y, Z));
    }
    protected void GenerateHexagon(int radius, HashSet<Vector3Int> collection, int Z = 0)
    {
        GenerateRow(0,Z, -radius, radius, collection);
        for (int i = 1; i <= radius; i++)
        {
            int half = i / 2;
            int oddCorrection = i % 2;
            GenerateRow(i,Z, -radius + half, radius - half - oddCorrection, collection);
            GenerateRow(-i,Z, -radius + half, radius - half - oddCorrection, collection);
        }
    }

}
