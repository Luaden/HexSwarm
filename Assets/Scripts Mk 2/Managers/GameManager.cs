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
    public UnitManager UnitManager => unitManager;


    protected readonly Queue<ITeam> activeTeams = new Queue<ITeam>();
    protected PlayerTeam player1;
    protected int turnCounter;
    protected int levelCounter;
    protected IAbility selectedAbility;
    protected Vector3Int end;
    protected Vector3Int secondEnd;

    public int TurnCounter => turnCounter;
    public int LevelCounter => levelCounter;
    public Team Player1 => player1;
    public Queue<ITeam> ActiveTeams => activeTeams;
    public int GridSize { get; set; }

    public IUnit DisplayedUnit { get; protected set; }
    public IAbility SelectedAbility => DisplayedUnit?.Abilites.ElementAtOrDefault(SelectedUnitPanel.LastSelected); 

    public Vector3Int GetMousePosition() => Battlefield.GetVectorByClick(Input.mousePosition);

    public void InspectUnitUnderMouse()
    {
        ICell selectedcell;
        if (!Battlefield.World.TryGetValue(GetMousePosition(), out selectedcell))
            return;

        if (selectedcell.Unit == default)
            return;

        if (DisplayedUnit == selectedcell.Unit)
            return;

        DisplayedUnit = selectedcell.Unit;
        SelectedUnitPanel.UpdateUI(DisplayedUnit);
    }

    public void ResolveHighlight(Vector3Int mousePos)
    {
        Stack<Vector3Int> pathEnds = new Stack<Vector3Int>();
                
        IEnumerable<Vector3Int> path = Pathing.FindPath(
            DisplayedUnit.Location,
            mousePos,
            !selectedAbility.IsJump,
            selectedAbility.MovementRange);
        if (path == null)
            return;

        if(path.Count() > 1)
        {
            foreach (Vector3Int location in path)
                pathEnds.Push(location);

            end = pathEnds.Pop();
            secondEnd = pathEnds.Pop();
        }

        if(path.Count() == 1)
        {
            foreach (Vector3Int location in path)
                pathEnds.Push(location);

            end = DisplayedUnit.Location;
            secondEnd = pathEnds.Pop();
        }

        if (path.Count() == 0)
            return;
        
        Direction angleOfAttack = GetDirection(secondEnd, end);

        Queue<ICell> moveCells = new Queue<ICell>();
        ICell cell;

        foreach (Vector3Int location in path)
        {
            Battlefield.World.TryGetValue(location, out cell);
            moveCells.Enqueue(cell);
        }            
        
        IEnumerable<ICell> attackCells = selectedAbility.GetAttack(angleOfAttack, end);

        Battlefield.HighlightGrid(moveCells, attackCells);

    }

    public bool NewGame()
    {
        levelCounter = 0;
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

        List<IUnit> deaths = new List<IUnit>();
        Stack<Vector3Int> pathEnds = new Stack<Vector3Int>();

        if (path.Count() > 1)
        {
            foreach (Vector3Int location in path)
                pathEnds.Push(location);

            end = pathEnds.Pop();
            secondEnd = pathEnds.Pop();
        }

        if (path.Count() == 1)
        {
            foreach (Vector3Int location in path)
                pathEnds.Push(location);

            end = unit.Location;
            secondEnd = pathEnds.Pop();
        }

        if (path.Count() == 0)
            return false;

        IEnumerable<ICell> attackLocations = ability.GetAttack(direction, end);

        foreach (ICell cell in attackLocations)
            if (cell.Unit != null && cell.Unit.Team.TeamNumber == Teams.Player)
                deaths.Add(cell.Unit);

        if(path.Count() > 1)
            Battlefield.MoveUnit(unit.Location, target, path);

        ResolveDeaths(deaths, unit);
        //Battlefield.ClearHighlights();
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
        //if (activeTeams.Count == 0)
        //    return;
        //PlayerTeam currentPlayer = activeTeams.Peek() as PlayerTeam;
        //currentPlayer?.GetMouseInput();
    }

    public Direction GetDirection(Vector3Int secondTolast, Vector3Int lastPostion)
    {
        if (secondTolast.x < lastPostion.x && secondTolast.y > lastPostion.y)
            return Direction.Sixty;
        if (secondTolast.x == lastPostion.x && secondTolast.y > lastPostion.y)
            return Direction.One20;
        if (secondTolast.x < lastPostion.x && secondTolast.y == lastPostion.y)
            return Direction.One80;
        if (secondTolast.x > lastPostion.x && secondTolast.y < lastPostion.y)
            return Direction.Two40;
        if (secondTolast.x == lastPostion.x && secondTolast.y < lastPostion.y)
            return Direction.Threehundred;
        return Direction.Zero;
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

    protected void GenerateTeam(Team team, Unit template, Vector3Int centerPoint, int radius = 0)
    {
        GenerateUnitForTeam(team, template, centerPoint);
        foreach (ICell cell in Battlefield.GetNeighborCells(centerPoint, radius))
            GenerateUnitForTeam(team,
            template,
            cell.GridPosition);
    }
}
