using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Team : ITeam
{
    public string Discription { get; protected set; }
    public string Name { get; protected set; }
    public Sprite Icon { get; protected set; }
    public Color Color { get; protected set; }
    public Teams Type { get; protected set; }
    public IGameManager Game { get; protected set; }
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

    public void UpdateColor(Color newColor) { Color = newColor; }

    public void DoMove(IUnit unit, IPosAbilityDefault ablity, Vector3Int target)
    {
        Game.PerformMove(unit, ablity, target);
    }

    public void EndTurn()
    {
        hasMove = false;
        Game.EndTurn();
    }

    public bool HasUnitsAfterLosses(IEnumerable<IUnit> losses)
    {
        foreach (IUnit loss in losses)
            if (this.units.Contains(loss))
                this.units.Remove(loss);
        return this.units.Count > 0;
    }

    public void ResolveHighlight(IUnit unit, IPosAbilityDefault ablity, Vector3Int target)
    {
        throw new NotImplementedException();
    }

    public void StartTurn()
    {
        hasMove = true;
    }

    public void Undo()
    {
        Game.Undo();
    }
}
