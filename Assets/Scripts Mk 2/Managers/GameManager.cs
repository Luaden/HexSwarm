using UnityEngine;
using System.Collections.Generic;

public class GameManager : IGameManager
{
    protected int turnCounter;
    public int TurnCounter => turnCounter;

    public int LevelCounter => throw new System.NotImplementedException();

    public Queue<ITeam> ActiveTeams => throw new System.NotImplementedException();

    public IUnit DisplayedUnit => throw new System.NotImplementedException();

    public void EndTurn()
    {
        throw new System.NotImplementedException();
    }

    public void InspectUnitUnderMouse()
    {
        throw new System.NotImplementedException();
    }

    public void NewGame()
    {
        throw new System.NotImplementedException();
    }

    public bool PerformMove(IUnit unit, IAbility ablity, Vector3Int target)
    {
        throw new System.NotImplementedException();
    }

    public void StartLevel()
    {
        throw new System.NotImplementedException();
    }
}
