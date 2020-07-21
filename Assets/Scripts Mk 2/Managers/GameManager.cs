using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

//[RequireComponent(typeof(ConfigManager))]
public class GameManager : MonoBehaviour, IGameManager
{
    public static IBattlefieldManager Battlefield { get; protected set; }
    public static Pathfinder Pathing { get; protected set; }
    public static SelectedUnitPanel SelectedUnitPanel { get; protected set; }
    public static TurnOrderDisplay TurnOrderDisplay { get; protected set; }
    public static UnitAVController UnitAVController { get; protected set; }

    [SerializeField] protected string battlefieldName; 

    protected Player player1;
    public Team Player1 => player1;

    protected int turnCounter;
    public int TurnCounter => turnCounter;
    protected int levelCounter;
    public int LevelCounter => levelCounter;

    protected readonly Queue<ITeam> activeTeams = new Queue<ITeam>();
    public Queue<ITeam> ActiveTeams => activeTeams;

    public IUnit DisplayedUnit { get; protected set; }

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
        if (SelectedUnitPanel == null)
            SelectedUnitPanel = FindObjectOfType<SelectedUnitPanel>();
    }

    protected void Start()
    {
        //if (Battlefield == null)
        //    Battlefield = string.IsNullOrWhiteSpace(battlefieldName)
        //        ? FindObjectOfType<BattlefieldManager>()
        //        : GameObject.Find(battlefieldName).GetComponent<BattlefieldManager>();
        if (Battlefield == null)
            Battlefield = FindObjectOfType<BattlefieldManager>();

        if (TurnOrderDisplay == null)
            TurnOrderDisplay = FindObjectOfType<TurnOrderDisplay>();

        if (Pathing == null)
            Pathing = new Pathfinder(Battlefield as BattlefieldManager);

        if (UnitAVController == null)
            UnitAVController = FindObjectOfType<UnitAVController>();

        NewGame();
    }

    protected void Update()
    {
        if (activeTeams.Count == 0)
            return;
        Player currentPlayer = activeTeams.Peek() as Player;
        currentPlayer?.GetMouseInput();
    }

    public Vector3Int GetMousePosition() => Battlefield.GetVectorByClick(Input.mousePosition);

    public void InspectUnitUnderMouse()
    {
        ICell selectedcell;
        if (!Battlefield.World.TryGetValue(GetMousePosition(), out selectedcell))
            return;

        if (selectedcell.Unit == default)
            return;

        if (DisplayedUnit != selectedcell.Unit)
            SelectedUnitPanel.UpdateUI(selectedcell.Unit);

        DisplayedUnit = selectedcell.Unit;
    }

    public bool NewGame()
    {
        levelCounter = 0;
        turnCounter = 0;

        StartLevel();
        return true;
    }

    public bool PerformMove(IUnit unit, IAbility ablity, Vector3Int target, IEnumerable<Vector3Int> path = default)
    {
        if (unit.Team != activeTeams.Peek())
            return false;

        Battlefield.MoveUnit(unit.Location, target);

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
            corspe.Team.RemoveUnit(corspe);
            teamsWithLosses.Add(oldTeam);

            if ((activeTeams.Peek().TeamNumber & Teams.Player) == 0)
                GenerateUnitForTeam(unit.Team, unit, corspe.Location);
        }
    }

    protected void GenerateUnitForTeam(ITeam team, IUnit template, Vector3Int location)
    {
        Unit newUnit = new Unit(template);
        team.AddNewUnit(newUnit);
        Battlefield.PlaceNewUnit(newUnit, location);

    }

    public bool StartLevel()
    {
        ////TODO: refine with difficulty/config
        //int gridRange = levelCounter + 10;

        //Battlefield.GenerateGrid(gridRange, MapShape.Hexagon);
        //activeTeams.Clear();



        //EndTurn();


        return true;
    }


    protected void GenerateTeam(Player team, Unit template, Vector3Int centerPoint, int radius = 0)
    {
        GenerateUnitForTeam(team, template, centerPoint);
        foreach (ICell cell in Battlefield.GetNeighborCells(centerPoint, radius))
            GenerateUnitForTeam(team,
            template,
            cell.Position);
    }


}
