using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy after having StartTurnCalled will for each unit
/// randomly pick to call the AIUnit's best or random move
/// </summary>
[System.Serializable]
public class Enemy : Team
{
    [SerializeField] GridManager gridManager;

    public override void StartTurn()
    {
        foreach(IUnit unit in units)
        {
            GetRandomMove(unit);
        }

        //HasMove = false;
        //EndTurn();
    }    

    public override void ResolveHighlight(IUnit unit, IPosAbilityDefault ablity, Vector3Int target)
    {

    }

    protected void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        
        IUnit tempUnit = gridManager.DebugGenerateUnit();

        units.Add(tempUnit);

        StartTurn();
    }
    
    protected void GetRandomMove(IUnit unit)
    {
        //Get game manager difficulty here;
        //subtract GameManager.Difficulty from Max;
        int moveToUse = Random.Range(0, 10);

        if(moveToUse > 0)
        {
            unit.CalcuateValidNewLocation(default);
            print("Used dumb ability");
        }
        else
        {
            unit.CalcuateValidNewLocation(default);
            print("Used smart ability");
        }
    }
}
