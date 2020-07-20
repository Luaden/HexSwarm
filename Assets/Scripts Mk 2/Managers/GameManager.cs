using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ConfigManager), typeof(Pathfinder))]
public class GameManager : MonoBehaviour, IGameManager
{
    public static IBattlefieldManager Battlefield { get; private set; }
    public static Pathfinder Pathing { get; private set; }
    //public readonly Player Player1 { get; private set; } 

    protected int turnCounter;
    public int TurnCounter => turnCounter;
    protected int levelCounter;
    public int LevelCounter => levelCounter;

    protected readonly Queue<ITeam> activeTeams = new Queue<ITeam>();
    public Queue<ITeam> ActiveTeams => throw new System.NotImplementedException();

    protected Unit displayedUnit; 
    public IUnit DisplayedUnit => displayedUnit;

    public void EndTurn()
    {
        if (activeTeams.Peek() == )
            turnCounter++;
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
