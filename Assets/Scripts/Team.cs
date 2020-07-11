using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Team : MonoBehaviour, ITeam
{
    public string Discription { get; protected set; }
    public string Name { get; protected set; }
    public Sprite Icon { get; protected set; }
    public Color Color { get; protected set; }
    public Teams Type { get; protected set; }
    public IGameManager GameManager { get; protected set; }
    public bool hasMove { get; protected set; }
    public int RemainingUnits => units.Count;


    protected HashSet<IUnit> units = new HashSet<IUnit>();
    public IEnumerable<IUnit> Units => units;

    protected HashSet<Vector3Int> highlightMove;
    public IEnumerable<Vector3Int> HighlightMove => highlightMove;
    protected HashSet<Vector3Int> highlightAttack;
    public IEnumerable<Vector3Int> HighlightAttack => highlightAttack;
    protected HashSet<Vector3Int> highlightOverlap;
    public IEnumerable<Vector3Int> HighlightOverlap => highlightOverlap;

    public string Description => throw new System.NotImplementedException();

    public TileBase Tile => throw new System.NotImplementedException();

    public bool HasMove => throw new System.NotImplementedException();

    public void UpdateColor(Color newColor) { Color = newColor; }

    public bool DoMove(IUnit unit, IPosAbilityDefault ablity, Vector3Int target)
    {
        if (!hasMove)
            return false;
        return GameManager.PerformMove(unit, ablity, target);
    }

    public bool EndTurn()
    {
        if (!hasMove)
            return false;
        hasMove = false;
        return GameManager.EndTurn();
    }

    public bool HasUnitsAfterLosses(IEnumerable<IUnit> losses)
    {
        foreach (IUnit loss in losses)
            if (this.units.Contains(loss))
                this.units.Remove(loss);
        return this.units.Count > 0;
    }
    

    public virtual void ResolveHighlight(IUnit unit, IPosAbilityDefault ablity, Vector3Int target)
    {
        hasMove = true;
    }

    public bool Undo()
    {
        if (!hasMove)
            return false;
        return GameManager.Undo();
    }

    bool ITeam.EndTurn()
    {
        return GameManager.EndTurn();
    }

    public void StartTurn()
    {
        throw new System.NotImplementedException();
    }


    void ITeam.DoMove(IUnit unit, IPosAbilityDefault ablity, Vector3Int target)
    {
        throw new System.NotImplementedException();
    }
}
