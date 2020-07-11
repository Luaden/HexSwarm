using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class GameManager : MonoBehaviour, IGameManager
{
    //Editor variables
    [SerializeField] protected GridManager gridManager;

    //Cached references
    protected BattlefieldManager battlefieldManager;

    public int turnCounter => throw new NotImplementedException();

    public int levelCounter => throw new NotImplementedException();

    public Queue<ITeam> ActiveTeams => throw new NotImplementedException();

    public IUnit DisplayedUnit => throw new NotImplementedException();

    public void AnimateMove()
    {
        throw new NotImplementedException();
    }

    public void EndTurn()
    {
        throw new NotImplementedException();
    }

    public void InspectUnitAt(Vector3Int location)
    {
        throw new NotImplementedException();
    }

    public void NewGame()
    {
        throw new NotImplementedException();
    }

    public bool PerformMove(IUnit unit, IPosAbilityDefault ablity, Vector3Int target)
    {
        throw new NotImplementedException();
    }

    public void StartLevel()
    {
        throw new NotImplementedException();
    }

    public bool Undo()
    {
        throw new NotImplementedException();
    }

    protected void Awake()
    {
        if (gridManager == null)
            gridManager = FindObjectOfType<GridManager>();
    }

    protected void Update()
    {

    }

    bool IGameManager.EndTurn()
    {
        throw new NotImplementedException();
    }
}
