using System.Collections.Generic;
using UnityEngine;

public class Enemy : Team
{

    [SerializeField] IGameManager gameManager;
    [SerializeField] IGrid gridManager;


    public override void StartTurn()
    {
        foreach(IUnit unit in units)
        {
            GetRandomMove(unit);
        }

        HasMove = false;
        EndTurn();
    }    

    public override void ResolveHighlight(IUnit unit, IPosAbilityDefault ablity, Vector3Int target)
    {

    }

    protected void Awake()
    {
        units = new List<IUnit>();
    }

    protected void GetRandomMove(IUnit unit)
    {
        //Get game manager difficulty here;
        //subtract GameManager.Difficulty from Max;
        int moveToUse = Random.Range(0, 10);

        if(moveToUse > 0)
        {
            //Random unit ability.
        }
        else
        {
            //Best unit ability.
        }
    }

}
