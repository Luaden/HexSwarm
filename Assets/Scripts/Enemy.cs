using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : Team
{

    [SerializeField] IGameManager gameManager;
    [SerializeField] IGrid gridManager;


    public override void StartTurn()
    {
        //For each unit in list, move randomly.
    }

    public void EndTurn()
    {

    }

    public void DoMove(IUnit unit, IPosAbilityDefault ablity, Vector3Int target)
    {

    }

    public override void ResolveHighlight(IUnit unit, IPosAbilityDefault ablity, Vector3Int target)
    {

    }

    protected void Awake()
    {
        units = new List<IUnit>();
    }

    protected void GetRandomMove()
    {

    }

}
