using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

//[RequireComponent(typeof(ConfigManager))]
public class GameManager : MonoBehaviour, IGameManager
{
    public static IBattlefieldManager Battlefield { get; protected set; }
    public static Pathfinder Pathing { get; protected set; }
    public static ConfigManager ConfigManager { get; protected set; }
    public static SelectedUnitPanel SelectedUnitPanel { get; protected set; }
    public static TurnOrderDisplay TurnOrderDisplay { get; protected set; }
    public static UnitAVController UnitAVController { get; protected set; }
    
    protected readonly Queue<ITeam> activeTeams = new Queue<ITeam>();
    protected Player player1;
    protected int turnCounter;
    protected int levelCounter;
    protected IAbility selectedAbility;

    public int TurnCounter => turnCounter;
    public int LevelCounter => levelCounter;
    public Team Player1 => player1;
    public Queue<ITeam> ActiveTeams => activeTeams;
    public int GridSize { get; set; }

    public IUnit DisplayedUnit { get; protected set; }
    public IAbility SelectedAbility { get => selectedAbility; set => selectedAbility = value; }


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

    public bool PerformMove(IUnit unit, IAbility ablity, Vector3Int target, IEnumerable<Vector3Int> path)
    {
        if (unit.Team != activeTeams.Peek())
            return false;

        IEnumerable<ICell> neighbors = Battlefield.GetNeighborCells(target);
        List<IUnit> deaths = neighbors.Select(X => X.Unit).Where(X => X != default).Where(X => X.Team != unit.Team).ToList();

        Battlefield.MoveUnit(unit.Location, target, path);

        ResolveDeaths(deaths, unit);
        return true;
    }

    public bool StartLevel()
    {
        GridSize = Mathf.RoundToInt((10 + levelCounter) * ConfigManager.GameDifficulty);
        Battlefield.GenerateGrid(GridSize, ConfigManager.MapShape);

        activeTeams.Clear();

        EndTurn();
        return true;
    }

    public bool EndTurn()
    {
        if (activeTeams.Peek() == Player1)
            turnCounter++;

        activeTeams.Enqueue(activeTeams.Dequeue());
        activeTeams.Peek().StartTurn();
        TurnOrderDisplay.UpdateUI(this);

        return true;
    }

    protected void Awake()
    {
        if (Battlefield == null)
            Battlefield = FindObjectOfType<BattlefieldManager>();

        if (TurnOrderDisplay == null)
            TurnOrderDisplay = FindObjectOfType<TurnOrderDisplay>();

        if (Pathing == null)
            Pathing = new Pathfinder(Battlefield as BattlefieldManager);

        if (UnitAVController == null)
            UnitAVController = FindObjectOfType<UnitAVController>();

        if (SelectedUnitPanel == null)
            SelectedUnitPanel = FindObjectOfType<SelectedUnitPanel>();

        if (ConfigManager == null)
            ConfigManager = ConfigManager.instance;
    }
    protected void Start() => NewGame();

    protected void Update()
    {
        if (activeTeams.Count == 0)
            return;
        Player currentPlayer = activeTeams.Peek() as Player;
        currentPlayer?.GetMouseInput();
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

        foreach (IUnit corpse in deaths)
        {
            ITeam oldTeam = corpse.Team;
            Vector3Int oldLocation = corpse.Location;
            corpse.Team.RemoveUnit(corpse);
            teamsWithLosses.Add(oldTeam);

            Battlefield.DestroyUnit(corpse.Location);

            if ((activeTeams.Peek().TeamNumber & Teams.Player) == 0)
                GenerateUnitForTeam(unit.Team, unit, corpse.Location);
        }
    }

    protected void GenerateUnitForTeam(ITeam team, IUnit template, Vector3Int location)
    {
        Unit newUnit = new Unit(template);
        team.AddNewUnit(newUnit);
        Battlefield.PlaceNewUnit(newUnit, location);
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

    protected void GenerateTeam(Player team, Unit template, Vector3Int centerPoint, int radius = 0)
    {
        GenerateUnitForTeam(team, template, centerPoint);
        foreach (ICell cell in Battlefield.GetNeighborCells(centerPoint, radius))
            GenerateUnitForTeam(team,
            template,
            cell.GridPosition);
    }

    protected void GetUnitAbility(int abilityIndex) => selectedAbility = DisplayedUnit.Abilites[abilityIndex];
}
