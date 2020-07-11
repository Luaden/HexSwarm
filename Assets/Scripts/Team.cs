using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Team : ITeam
{
    public string Name { get; private set; }
    public Sprite Icon { get; private set; }
    public Color Color { get; private set; }
    public void UpdateColor(Color newColor) { Color = newColor; }
    public IGameManager Game => throw new NotImplementedException();

    public IEnumerable<IUnit> Units => throw new NotImplementedException();

    public bool hasMove => throw new NotImplementedException();

    public IEnumerable<Vector3Int> HighlightMove => throw new NotImplementedException();

    public IEnumerable<Vector3Int> HighlightAttack => throw new NotImplementedException();

    public IEnumerable<Vector3Int> HighlightOverlap => throw new NotImplementedException();

    public string Discription => throw new NotImplementedException();

    public Teams Type => throw new NotImplementedException();

    public void DoMove(IUnit unit, IPosAbilityDefault ablity, Vector3Int target)
    {
        throw new NotImplementedException();
    }

    public void EndTurn()
    {
        throw new NotImplementedException();
    }

    public bool hasUnitsAfterLosses(IEnumerable<IUnit> losses)
    {
        throw new NotImplementedException();
    }

    public void ResolveHighlight(IUnit unit, IPosAbilityDefault ablity, Vector3Int target)
    {
        throw new NotImplementedException();
    }

    public void StartTurn()
    {
        throw new NotImplementedException();
    }

    public void Undo()
    {
        throw new NotImplementedException();
    }
}
