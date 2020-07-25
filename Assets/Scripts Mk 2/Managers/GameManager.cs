using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;


public class GameManager : MonoBehaviour, IGameManager
{
    public static IBattlefieldManager Battlefield { get; protected set; }
    public static Pathfinder Pathing { get; protected set; }
    public static ConfigManager ConfigManager { get; protected set; }
    public static SelectedUnitPanel SelectedUnitPanel { get; protected set; }
    public static TurnOrderDisplay TurnOrderDisplay { get; protected set; }
    public static UnitAVController UnitAVController { get; protected set; }

    [SerializeField] protected UnitManager unitManager;
    
    [Header("Team Spawn Variables")]
    [SerializeField] protected int teamCountPerLevel;
    [SerializeField] protected int rangeFromNeighbors;

    [Header("Level Variables")]
    [SerializeField] protected int levelCounter;
    [SerializeField] protected int gridSize;
    public UnitManager UnitManager => unitManager;


    protected readonly Queue<ITeam> activeTeams = new Queue<ITeam>();
    protected PlayerTeam player1;
    protected int turnCounter;
    
    protected IAbility selectedAbility;
    protected Vector3Int end;
    protected Vector3Int secondEnd;
    protected Vector3Int spawnLocation;
    

    public int TurnCounter => turnCounter;
    public int LevelCounter => levelCounter;
    public Team Player1 => player1;
    public Queue<ITeam> ActiveTeams => activeTeams;
    public int GridSize { get => gridSize; set => gridSize = value; }

    public IUnit DisplayedUnit { get; protected set; }
    public IAbility SelectedAbility => DisplayedUnit?.Abilites.ElementAtOrDefault(SelectedUnitPanel.LastSelected); 

    public static Vector3Int GetMousePosition() => Battlefield.GetVectorByClick(Input.mousePosition);

    public static ICell GetCellUnderMouse()
    {
        ICell selectedcell;
        Battlefield.World.TryGetValue(GetMousePosition(), out selectedcell);
        return selectedcell;
    }

    public void InspectUnitUnderMouse()
    {
        ICell selectedcell;
        if ((selectedcell = GetCellUnderMouse()) == default)
            return;

        if (selectedcell.Unit == default)
            return;

        if (DisplayedUnit == selectedcell.Unit)
            return;

        DisplayedUnit = selectedcell.Unit;
        SelectedUnitPanel.UpdateUI(DisplayedUnit);
    }

    public bool NewGame()
    {
        levelCounter = 1;
        turnCounter = 0;

        StartLevel();
        return true;
    }

    public bool PerformMove(IUnit unit, IAbility ability, Direction direction, Vector3Int target, IEnumerable<Vector3Int> path)
    {
        if (unit.Team != activeTeams.Peek())
            return false;
        if (path == null)
            return false;

        if (unit.Location != target)
            Battlefield.MoveUnit(unit.Location, target, path);

        List<IUnit> deaths = new List<IUnit>();
        IEnumerable<ICell> attackLocations = ability.GetAttack(direction, target);

        foreach (ICell cell in attackLocations)
            if (cell.Unit != null && cell.Unit.Team.TeamNumber == Teams.Player)
                deaths.Add(cell.Unit);

        ResolveDeaths(deaths, unit);

        return true;
    }

    protected void ResolveAttack(IAbility move, Direction direction, Vector3Int location)
    {
        List<IUnit> deaths = new List<IUnit>();
        IEnumerable<ICell> attackLocations = move.GetAttack(direction, end);

        foreach (ICell cell in attackLocations)
            if (cell.Unit != null && cell.Unit.Team.TeamNumber == Teams.Player)
                deaths.Add(cell.Unit);

    }

    [ContextMenu("New Level")]
    public bool StartLevel()
    {
        activeTeams.Clear();
        UnitAVController.Nuke();

        GridSize = Mathf.RoundToInt((9 + levelCounter) * ConfigManager.GameDifficulty);
        Battlefield.GenerateGrid(GridSize, ConfigManager.MapShape);

        SpawnTeams();
        Debug.Log("Current active team: " + activeTeams.Peek().Name);
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

    public void ClearActiveUnit()
    {
        selectedAbility = null;
        DisplayedUnit = null;

        Battlefield.ClearHighlights();
        SelectedUnitPanel.UpdateUI(null);
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
        //PlayerTeam currentPlayer = activeTeams.Peek() as PlayerTeam;
        //currentPlayer?.GetMouseInput();
        if (activeTeams.Peek() == player1)
            EndTurn();
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

            if ((activeTeams.Peek().TeamNumber & Teams.Player) == Teams.Player)
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

    protected void GenerateTeam(Team team, Unit template, Vector3Int centerPoint, int radius = 1)
    {
        GenerateUnitForTeam(team, template, centerPoint);
        foreach (ICell cell in Battlefield.GetNeighborCells(centerPoint, radius))
            GenerateUnitForTeam(team,
            template,
            cell.GridPosition);
    }

    protected void SpawnTeams()
    {
        for (int i = 1; i < Mathf.RoundToInt((teamCountPerLevel + levelCounter) * ConfigManager.GameDifficulty); i++)
        {
            if (i == 1)
            {
                SpawnPlayerTeam();
                continue;
            }

            SpawnAITeam(i);
        }
    }
    protected void SpawnPlayerTeam()
    {
        while (true)
        {
            if (ValidateSpawnLocation(GetSpawnLocation()))
            {
                player1 = new PlayerTeam(this, "Player1", "Grey Goo", UnitManager[Units.Nanos].Icon, Teams.Player,
                spawnLocation,
                new HashSet<IUnit>());
                activeTeams.Enqueue(player1);

                GenerateTeam(player1, UnitManager[Units.Nanos], player1.StartPosition);
                break;
            }
        }
    }

    protected void SpawnAITeam(int i)
    {
        while (true)
        {
            if (ValidateSpawnLocation(GetSpawnLocation()))
            {               

                TestAITeam ai = new TestAITeam(this, "AI" + i, "Tank wielding maniac.", UnitManager[Units.Infantry].Icon, (Teams)i,
                spawnLocation,
                new HashSet<IUnit>());
                activeTeams.Enqueue(ai);

                GenerateTeam(ai, UnitManager[Units.Infantry], ai.StartPosition);
                break;
            }
        }
    }

    protected Vector3Int GetSpawnLocation()
    {
        int i = UnityEngine.Random.Range(0, Battlefield.World.Count + 1);
        int j = 0;

        foreach (KeyValuePair<Vector3Int, ICell> entry in Battlefield.World)
        {
            if (i == j && entry.Value.Unit == null)
            {
                spawnLocation = entry.Key;
                break;
            }

            j++;
        }

        return spawnLocation;
    }

    protected bool ValidateSpawnLocation(Vector3Int checkLocation)
    {
        IEnumerable<ICell> neighbors = Battlefield.GetNeighborCells(checkLocation, rangeFromNeighbors);

        if (neighbors.Count() < Battlefield.GetNeighborCells(new Vector3Int(0, 0, 0), rangeFromNeighbors).Count())
            return false;

        foreach (ICell neighbor in neighbors)
            if (neighbor.Unit != null)
                return false;

        return true;
    }
}
