using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Team : ITeam
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Sprite Icon { get; set; }
    public Color Color { get; set; }
    public Teams TeamNumber { get; set; }
    public bool MyTurn { get; set; }
    public IGameManager GameManager { get; set; }
    public IEnumerable<IUnit> Units { get => units; }

    protected List<IUnit> units;

    public void StartTurn() => MyTurn = true;

    public void EndTurn()
    {
        //Tell the game manager this team is done.
    }

    public void KillUnit(IUnit unit)
    {
        try
        {
            units.Remove(unit);
        }
        catch
        {
            Debug.LogError("Team " + Name + " was asked to remove a unit it did not own in unit list.");
        }

        if(units.Count <= 0)
            TeamEliminated();
    }

    protected void TeamEliminated()
    {
        //Tell game manager this team is eliminated.
    }

}
