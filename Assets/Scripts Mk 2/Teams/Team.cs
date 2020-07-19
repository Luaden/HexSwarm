using System.Collections.Generic;
using UnityEngine;

public abstract class Team : ITeam
{
    protected GameManager gameManager;
    protected BattlefieldManager battlefieldManager;
    protected HashSet<IUnit> units = new HashSet<IUnit>();
    protected HashSet<IUnit> unitsUnmoved = new HashSet<IUnit>();

    public string Name { get; set;  }
    public string Description { get; set; }
    public Sprite Icon { get; set; }
    public Color PrimaryColor { get; set; }
    public Color SecondaryColor { get; set; }
    public Teams TeamNumber { get; set; }
    public bool MyTurn { get; set; }
    public IEnumerable<IUnit> Units { get => units; }
    public Vector3Int StartPosition { get; set; }

    public void StartTurn()
    {
        MyTurn = true;

        while(MyTurn)
        {
            TakeTurn();
        }
    }

    public virtual void EndTurn()
    {
        MyTurn = false;
        //Tell the game manager this team is done.
    }

    public void KillUnit(IUnit unit)
    {
        try
        {
            units.Remove(unit);
            battlefieldManager.DestroyUnit(unit.Location);
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

    protected virtual void TakeTurn()
    {
        return;
    }
}
