using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

public class GameManager : MonoBehaviour, IGameManager
{
    //Editor variables
    [SerializeField] protected GridManager gridManager;
    [SerializeField] protected TurnOrderDisplay turnOrderDisplay;
    [SerializeField] protected int totalTeamCount;
    [SerializeField] protected Enemy tempEnemy;
    [SerializeField] protected Pathfinder pathFinder;

    [SerializeField] protected List<Sprite> teamSprites = new List<Sprite>();
    [SerializeField] protected Player Player1;
    [SerializeField] protected List<Unit> playerOneUnitTemplates = new List<Unit>();
    [SerializeField] protected Player Player2;
    [SerializeField] protected List<Unit> playerTwoTemplates = new List<Unit>();
    [SerializeField] protected SelectedUnitPanel selectedUnitPanel;


    protected BattlefieldManager battlefieldManager;

    //Cached references
    public BattlefieldManager BattlefieldManager => battlefieldManager;
    public GridManager GridManager => gridManager;

    [SerializeField] protected int turnCounter;
    public int TurnCounter => turnCounter;

    [SerializeField] protected int levelCounter;
    public int LevelCounter => levelCounter;

    [SerializeField] protected Queue<ITeam> activeTeams = new Queue<ITeam>();
    public Queue<ITeam> ActiveTeams => activeTeams;

    [SerializeField] protected IUnit displayedUnit;
    public IUnit DisplayedUnit => displayedUnit;

    public void AnimateMove()
    {
        throw new NotImplementedException();
    }

    public void EndTurn()
    {
        if (activeTeams.Peek() == Player1)
            turnCounter++;

        activeTeams.Enqueue(activeTeams.Dequeue());
        activeTeams.Peek().StartTurn();
        turnOrderDisplay.UpdateUI(activeTeams);

        if (!Player1.HasUnitsAfterLosses(new IUnit[0]))
        {
            Loss();
            return;
        }

        if (activeTeams.Count == 1)
        {
            Win();
            return;
        }

    }
    protected void Loss()
    {
        this.levelCounter--;
        StartLevel();
    }
    protected void Win()
    {
        this.levelCounter++;
        StartLevel();
    }



    public Vector3Int GetMousePosition() => gridManager.GetCellByClick(Input.mousePosition);


    public void InspectUnitUnderMouse()
    {
        Cell selectedcell;
        if (!battlefieldManager.World.TryGetValue(GetMousePosition(), out selectedcell))
            return;

        if (selectedcell.Unit == default)
            return;

        if (displayedUnit != selectedcell.Unit)
            selectedUnitPanel.UpdateUI(selectedcell.Unit);

        displayedUnit = selectedcell.Unit;
    }

    [ContextMenu("NewGame")]
    public void NewGame()
    {
        levelCounter = 0;
        turnCounter = 0;

        StartLevel();
    }

    public bool PerformMove(IUnit unit, IPosAbilityDefault ablity, Vector3Int target)
    {
        if (unit.Member != activeTeams.Peek())
            return false;

        BattlefieldManager.MoveUnit(unit.Location, target);

        IEnumerable<Cell> neighbors = gridManager.GetNeighborCells(unit.Location);

        List<IUnit> deaths = neighbors.Select(X => X.Unit).Where(X=>X!=default).Where(X=>X.Member !=unit.Member) .ToList();

        ResolveDeaths(deaths, unit);

        return true;
    }

    protected void ResolveDeaths(IEnumerable<IUnit> deaths, IUnit unit)
    {
        ITeam currentTurn = activeTeams.Dequeue();
        activeTeams.Enqueue(currentTurn);

        foreach (IUnit corspe in deaths)
            BattlefieldManager.DestroyUnits(corspe.Location);

        while(activeTeams.Peek() != currentTurn)
        {
            ITeam teamToResolve = activeTeams.Dequeue();
            if (teamToResolve.HasUnitsAfterLosses(deaths))
                activeTeams.Enqueue(teamToResolve);
        }

        if ((activeTeams.Peek().Type & Teams.Player) == 0)
            foreach (IUnit freshKill in deaths)
                GenerateUnitForTeam(unit.Member, unit as Unit, freshKill.Location);
    }

    protected void GenerateUnitForTeam(ITeam team, Unit template, Vector3Int location)
    {
        Unit newUnit = new Unit(template);
        BattlefieldManager.PlaceNewUnit(newUnit, location);
        team.GetUnit(newUnit);
    }



    public void StartLevel()
    {
        int Gridrange = levelCounter + 2;

        gridManager.GenerateGrid(Gridrange, gridManager.BasicTile);
        activeTeams.Clear();


        Player2 = new Player(this, "Player2", "Gooier", teamSprites[1], default);
        activeTeams.Enqueue(Player2);


        GenerateTeam(Player2,
             playerTwoTemplates[0],
             new Vector3Int(UnityEngine.Random.Range(-Gridrange / 4, Gridrange / 4), Gridrange / 2, 0),
             Gridrange / 4);

        GenerateUnitForTeam(Player2,
            playerTwoTemplates[0],
            new Vector3Int(0, +Gridrange / 2, 0));

        Player1 = new Player(this, "Player1", "First Goo", teamSprites[0], default);
        activeTeams.Enqueue(Player1);

        GenerateTeam(Player1,
            playerOneUnitTemplates[0],
            new Vector3Int(UnityEngine.Random.Range(-Gridrange / 4, Gridrange/4 ), -Gridrange / 2, 0),
            Gridrange/4);

        EndTurn();
    }

    protected void GenerateTeam(Player team,Unit template, Vector3Int centerPoint, int radius = 0)
    {
        foreach(Cell cell in gridManager.GetNeighborCells(centerPoint,radius))
                    GenerateUnitForTeam(team,
                    template,
                    cell.Position);
    }

    protected void Update()
    {
        if (activeTeams.Count > 0)
            activeTeams.Peek().Update();
    }

    [ContextMenu("EndPlayer1")]
    protected void EndPlayer1Turn()
    {
        if (Player1 == activeTeams.Peek())
            EndTurn();
    }

    [ContextMenu("EndPlayer2")]
    protected void EndPlayer2Turn()
    {
        if (Player2 == activeTeams.Peek())
            EndTurn();
    }

    public bool Undo()
    {
        throw new NotImplementedException();
    }

    protected void Awake()
    {
        if (selectedUnitPanel == null)
            selectedUnitPanel = FindObjectOfType<SelectedUnitPanel>();
        if (gridManager == null)
            gridManager = FindObjectOfType<GridManager>();
        if (turnOrderDisplay == null)
            turnOrderDisplay = FindObjectOfType<TurnOrderDisplay>();
    }

    protected void Start()
    {
        //BuildTeams(totalTeamCount);
        //EndTurn();

        battlefieldManager = gridManager.BattlefieldManager;
        pathFinder = new Pathfinder(BattlefieldManager, gridManager);
    }

    protected void BuildTeams(int totalTeams)
    {

        for (int i = 0; i < totalTeams; i++)
        {
            ITeam newEnemy = new Enemy(tempEnemy);
            activeTeams.Enqueue(newEnemy);
            print(activeTeams.Count); 
        }

        turnOrderDisplay.UpdateUI(activeTeams);
    }

    [ContextMenu("Generate Path From 0,0 to 0, 6")]
    protected void DebugPathTest()
    {
        Vector3Int originVector = new Vector3Int(0, -6, 0);
        Vector3Int destinationVector = new Vector3Int(0, 6, 0);
        Cell origin;
        Cell destination;

        BattlefieldManager.World.TryGetValue(originVector, out origin);
        BattlefieldManager.World.TryGetValue(destinationVector, out destination);

        HashSet<Cell> newRoute = pathFinder.AvoidUnitsPath(origin, destination) as HashSet<Cell>;

        gridManager.HighlightGrid(newRoute);
    }

    [ContextMenu("Generate Unit")]
    protected void DebugGenerateUnit()
    {
        gridManager.DebugGenerateUnit();
    }
}
