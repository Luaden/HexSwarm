using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Team : MonoBehaviour, ITeam
{
    [SerializeField] GameManager gameManager;
    [SerializeField] protected string teamName;
    [SerializeField] protected string description;
    [SerializeField] protected Sprite icon;
    [SerializeField] protected TileBase tile;

    protected List<IUnit> units = new List<IUnit>();

    public string Name { get => teamName; }
    public string Description { get => description; }
    public Sprite Icon { get => icon; }
    public TileBase Tile { get => tile; }
    public IGameManager GameManager { get => gameManager; }
    public IEnumerable<IUnit> Units { get => units; }
    public IEnumerable<Vector3Int> HighlightMove { get; set; }
    public IEnumerable<Vector3Int> HighlightAttack { get; set; }
    public IEnumerable<Vector3Int> HighlightOverlap { get; set; }

    //Get and Set
    public Color Color { get; set; }
    public Teams Type { get; set; }
    public bool HasMove { get; set; }

    public virtual void StartTurn()
    {

    }

    public virtual void Undo()
    {
        return;
    }

    public virtual void EndTurn()
    {
        
    }
    
    public virtual void DoMove(IUnit unit, IPosAbilityDefault ablity, Vector3Int target)
    {
        //This needs to not have no parameters.
    }

    public virtual void ResolveHighlight(IUnit unit, IPosAbilityDefault ablity, Vector3Int target)
    {
        // This needs to take in a collection of Vector3Ints
    }

    public bool HasUnitsAfterLosses(IEnumerable<IUnit> deadUnits)
    {
        return false;
    }
}
