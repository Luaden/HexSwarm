using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[RequireComponent(typeof(ConfigManager), typeof(Pathfinder))]
public class GameManager : MonoBehaviour, IGameManager
{
    public static IBattlefieldManager Battlefield { get; protected set; }
    public static Pathfinder Pathing { get; protected set; }
    public static SelectedUnitPanel SelectedUnitPanel { get; protected set; }
    public static TurnOrderDisplay TurnOrderDisplay { get; protected set; }

    [SerializeField] protected string battlefieldName; 

    protected readonly Team player1;
    public Team Player1 => player1;

    protected int turnCounter;
    public int TurnCounter => turnCounter;
    protected int levelCounter;
    public int LevelCounter => levelCounter;

    protected readonly Queue<ITeam> activeTeams = new Queue<ITeam>();
    public Queue<ITeam> ActiveTeams => activeTeams;

    protected Unit displayedUnit; 
    public IUnit DisplayedUnit => displayedUnit;

    public bool EndTurn()
    {
        if (activeTeams.Peek() == Player1)
            turnCounter++;

        activeTeams.Enqueue(activeTeams.Dequeue());
        activeTeams.Peek().StartTurn();
        TurnOrderDisplay.UpdateUI(this);

        return true;
    }

    private bool Win()
    {
        this.levelCounter++;
        StartLevel();
        return true;
    }

    private bool Loss()
    {
        this.levelCounter--;
        StartLevel();
        return true;
    }

    protected void Awake()
    {
        if (Battlefield == null)
            Battlefield = string.IsNullOrWhiteSpace(battlefieldName)
                ?FindObjectOfType<BattlefieldManager>()
                :GameObject.Find(battlefieldName).GetComponent<BattlefieldManager>();
        if (TurnOrderDisplay == null)
            TurnOrderDisplay = FindObjectOfType<TurnOrderDisplay>();

        if (Pathing == null)
            Pathing = new Pathfinder(Battlefield as BattlefieldManager);
    }

    protected void Start() { NewGame(); }




    public void InspectUnitUnderMouse()
    {
        throw new System.NotImplementedException();
    }

    public bool NewGame()
    {
        levelCounter = 0;
        turnCounter = 0;

        StartLevel();
        return true;
    }

    public bool PerformMove(IUnit unit, IAbility ablity, Vector3Int target)
    {
        if (unit.Team != activeTeams.Peek())
            return false;

        Battlefield.MoveUnit(unit.Location, target, default);

        IEnumerable<ICell> neighbors = Battlefield.GetNeighborCells(unit.Location);
        List<IUnit> deaths = neighbors.Select(X => X.Unit).Where(X=>X!=default).Where(X=>X.Team!=unit.Team).ToList();

         ResolveDeaths(deaths, unit);
        return true;
    }

    protected void RemoveTeam(ITeam team)
    {
        if (team == player1)
        {
            Loss();
            return;
        }
    }

    protected void ResolveDeaths(IEnumerable<IUnit> deaths, IUnit unit)
    {
        HashSet<ITeam> teamsWithLosses = new HashSet<ITeam>();

        foreach (IUnit corspe in deaths)
        {
            ITeam oldTeam = corspe.Team;
            Vector3Int oldLocation = corspe.Location;
            corspe.Team.KillUnit(corspe);
            teamsWithLosses.Add(oldTeam);

            if ((activeTeams.Peek().TeamNumber & Teams.Player) == 0)
                GenerateUnitForTeam(unit.Team, unit, corspe.Location);
        }
        //while (activeTeams.Peek() != currentTurn)
        //{
        //    ITeam teamToResolve = activeTeams.Dequeue();
        //    if (teamToResolve.HasUnitsAfterLosses(deaths))
        //        activeTeams.Enqueue(teamToResolve);
        //}

        //if ((activeTeams.Peek().Type & Teams.Player) == 0)
        //    foreach (IUnit freshKill in deaths)
        //        GenerateUnitForTeam(unit.Member, unit as Unit, freshKill.Location);
    }

    protected void GenerateUnitForTeam(ITeam team, IUnit template, Vector3Int location)
    {
        Unit newUnit = default;//new Unit(template);
        throw new System.NotImplementedException();
        Battlefield.PlaceNewUnit(newUnit, location);
        //team.GetUnit(newUnit);
    }

    public bool StartLevel()
    {
        //TODO: refine with difficulty/config
        int gridRange = levelCounter + 10;

        Battlefield.GenerateGrid(gridRange, MapShape.Hexagon);
        activeTeams.Clear();

        return true;
    }



}
