using System.Collections.Generic;
using UnityEngine;

public abstract class Team : ITeam
{
    protected GameManager gameManager;
    protected BattlefieldManager battlefieldManager;
    protected HashSet<IUnit> units = new HashSet<IUnit>();
    protected HashSet<IUnit> unitsUnmoved = new HashSet<IUnit>();

    public string Name { get; set; }
    public string Description { get; set; }
    public Sprite Icon { get; set; }
    public Teams TeamNumber { get; set; }
    public IEnumerable<IUnit> Units { get => units; }
    public Vector3Int StartPosition { get; set; }

    public virtual void StartTurn()
    {
 
    }

    public virtual void EndTurn()
    {
        
    }

    public void AddNewUnit(IUnit unit)
    {
        units.Add(unit);
        unit.Team = this;
    }

    public void RemoveUnit(IUnit unit)
    {
        if(units.Contains(unit))
        {
            unit.Team = null;
            units.Remove(unit);
        }        
    }
}
