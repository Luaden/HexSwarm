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

        ResolveAttack(ability, unit, direction, target);

        return true;
    }

    protected void ResolveAttack(IAbility move,IUnit unit, Direction direction, Vector3Int location)
    {
        List<ICell> deaths = new List<ICell>();
        IEnumerable<ICell> attackLocations = move.GetAttack(direction, location);

        foreach (ICell cell in attackLocations)
            if ((cell.Unit != null && cell.Unit.ID != unit.ID)||
                (cell.Unit == null && move.IsSpawnVoid))
                deaths.Add(cell);

        ResolveDeaths(deaths, unit, move.IsSpawnVoid || move.IsSpawn);
    }

    [ContextMenu("New Level")]
    public bool StartLevel()
    {
        activeTeams.Clear();
        UnitAVController.Nuke();

        GridSize = Mathf.RoundToInt((9 + levelCounter) * ConfigManager.GameDifficulty);
        Battlefield.GenerateGrid(GridSize, ConfigManager.MapShape);

        SpawnTeams();
        //EndTurn();
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

    public void EndPlayerTurn()
    {
        if ((activeTeams.Peek().TeamNumber & Teams.AIS) != default)
            return;
        EndTurn();
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
        //if (activeTeams.Peek() == player1)
        //    EndTurn();
    }

    public void RemoveTeam(ITeam team)
    {
        if (team == player1)
        {
            Loss();
            return;
        }
        else if (activeTeams.Count == 2)
        {
            Win();
            return;
        }

        ITeam currentTeam = activeTeams.Dequeue();

        if (currentTeam != team)
            activeTeams.Enqueue(currentTeam);
        while (activeTeams.Peek() != currentTeam)
        {
            ITeam nextTeam = activeTeams.Dequeue();
            if (nextTeam != team)
                activeTeams.Enqueue(nextTeam);
        }
    }

    protected void ResolveDeaths(IEnumerable<ICell> deaths, IUnit unit, bool spawnReplacements)
    {
        foreach (ICell cell in deaths)
        {
            if (cell.Unit != default)
                cell.Unit.Team.RemoveUnit(cell.Unit);

            if (spawnReplacements)
                GenerateUnitForTeam(unit.Team, unit, cell.GridPosition);
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
        SpawnPlayerTeam();

        for (int i = 1; i < Mathf.RoundToInt((teamCountPerLevel + levelCounter) * ConfigManager.GameDifficulty); i++)
        {
            SpawnAITeam(i);
        }
    }
    protected void SpawnPlayerTeam()
    {
        while (true)
        {
            Vector3Int spawnLocation = GetSpawnLocation();
            if (ValidateSpawnLocation(spawnLocation))
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
            Vector3Int spawnLocation = GetSpawnLocation();
            if (ValidateSpawnLocation(spawnLocation))
            { 
                TestAITeam ai = new TestAITeam(this, "AI" + i, "Tank wielding maniac.", UnitManager[Units.Infantry].Icon, (Teams)i,
                spawnLocation,
                new HashSet<IUnit>());
                activeTeams.Enqueue(ai);
                Debug.Log(ai.Name);

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
