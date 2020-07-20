using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

namespace Old
{ 
public class GameManager : MonoBehaviour, IGameManager
{
    //Editor variables
    [SerializeField] protected GridManager gridManager;
    [SerializeField] protected TurnOrderDisplay turnOrderDisplay;
    [SerializeField] protected SelectedUnitPanel selectedUnitPanel;    
    [SerializeField] protected int totalTeamCount;
    [SerializeField] protected List<Sprite> teamSprites = new List<Sprite>();
    [SerializeField] protected Player Player1;
    [SerializeField] protected List<Unit> playerOneUnitTemplates = new List<Unit>();
    [SerializeField] protected Player Player2;
    [SerializeField] protected List<Unit> playerTwoTemplates = new List<Unit>();
    [SerializeField] protected Enemy tempEnemy;
    [SerializeField] protected Queue<ITeam> activeTeams = new Queue<ITeam>();


    protected BattlefieldManager battlefieldManager;
    protected Pathfinder pathFinder;
    protected int turnCounter;
    protected int levelCounter;

    public BattlefieldManager BattlefieldManager => battlefieldManager;
    public Pathfinder Pathfinder => pathFinder;
    public GridManager GridManager => gridManager;
    public int TurnCounter => turnCounter;
    public int LevelCounter => levelCounter;
    public Queue<ITeam> ActiveTeams => activeTeams;

    //[SerializeField] protected IUnit displayedUnit;
    //public IUnit DisplayedUnit => displayedUnit;

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
        turnOrderDisplay.UpdateUI(this);

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

    //public Vector3Int GetMousePosition() => gridManager.GetCellByClick(Input.mousePosition);

    //public void InspectUnitUnderMouse()
    //{
    //    Cell selectedcell;
    //    if (!battlefieldManager.World.TryGetValue(GetMousePosition(), out selectedcell))
    //        return;

    //    if (selectedcell.Unit == default)
    //        return;

    //    if (displayedUnit != selectedcell.Unit)
    //        selectedUnitPanel.UpdateUI(selectedcell.Unit);

    //    displayedUnit = selectedcell.Unit;
    //}

    public void NewGame()
    {
        levelCounter = 0;
        turnCounter = 0;

        StartLevel();
    }

    public void UpdateUnitUI(IUnit unit)
    {
        selectedUnitPanel.UpdateUI(unit);
    }

    public bool PerformMove(IUnit unit, IPosAbilityDefault ablity, Vector3Int target)
    {
    //    if (unit.Team != activeTeams.Peek())
    //        return false;

    //    BattlefieldManager.MoveUnit(unit.Location, target);

    //    IEnumerable<Cell> neighbors = gridManager.GetNeighborCells(unit.Location);

    //    List<IUnit> deaths = neighbors.Select(X => X.Unit).Where(X=>X!=default).Where(X=>X.Member !=unit.Member) .ToList();

    //    ResolveDeaths(deaths, unit);

        return true;
    }

    public void StartLevel()
    {
            int Gridrange = levelCounter + 10;

            gridManager.GenerateGrid(Gridrange, gridManager.BasicTile);
            activeTeams.Clear();


            Player2 = new Player(this, "Player2", "Gooier", teamSprites[1], default);
            activeTeams.Enqueue(Player2);

            GenerateTeam(Player2,
                 playerTwoTemplates[0],
                 new Vector3Int(UnityEngine.Random.Range(-Gridrange / 4, Gridrange / 4), Gridrange / 2, 0),
                 Gridrange / 4);
            Debug.Log("second player made.");

            Player1 = new Player(this, "Player1", "First Goo", teamSprites[0], default);
            activeTeams.Enqueue(Player1);

            GenerateTeam(Player1,
                playerOneUnitTemplates[0],
                new Vector3Int(UnityEngine.Random.Range(-Gridrange / 4, Gridrange / 4), -Gridrange / 2, 0),
                Gridrange / 4);
            Debug.Log("First player made.");

            EndTurn();
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
        battlefieldManager = gridManager.BattlefieldManager;
        pathFinder = new Pathfinder(BattlefieldManager, gridManager);
        NewGame();
    }

    protected void Update()
    {
        //if (activeTeams.Count > 0)
        //    activeTeams.Peek().Update();
    }

    protected void ResolveDeaths(IEnumerable<IUnit> deaths, IUnit unit)
    {
        //ITeam currentTurn = activeTeams.Dequeue();
        //activeTeams.Enqueue(currentTurn);

        //foreach (IUnit corspe in deaths)
        //    BattlefieldManager.DestroyUnits(corspe.Location);

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

    protected void GenerateUnitForTeam(ITeam team, Unit template, Vector3Int location)
    {
        Debug.Log("Made a unit");
        Unit newUnit = new Unit(template);
        BattlefieldManager.PlaceNewUnit(newUnit, location);
        team.GetUnit(newUnit);
    }

    protected void BuildTeams(int totalTeams)
    {

        for (int i = 0; i < totalTeams; i++)
        {
            ITeam newEnemy = new Enemy(tempEnemy);
            activeTeams.Enqueue(newEnemy);
            print(activeTeams.Count); 
        }

        turnOrderDisplay.UpdateUI(this);
    }

    protected void GenerateTeam(Player team, Unit template, Vector3Int centerPoint, int radius = 0)
    {
        foreach (Cell cell in gridManager.GetNeighborCells(centerPoint, radius))
            GenerateUnitForTeam(team,
            template,
            cell.Position);
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


    #region Debug
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

    [ContextMenu("Generate Unit")]
    protected void DebugGenerateUnit()
    {
        gridManager.DebugGenerateUnit();
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

        List<Cell> newRoute = pathFinder.AvoidUnitsPath(origin, destination) as List<Cell>;

        gridManager.HighlightGrid(newRoute);
    }
    #endregion
}
}
