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

    //Cached references
    protected BattlefieldManager battlefieldManager;

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
        activeTeams.Enqueue(activeTeams.Dequeue());
        activeTeams.Peek().StartTurn();
        turnOrderDisplay.UpdateUI(activeTeams);
    }

    public void InspectUnitAt(Vector3Int location)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    [SerializeField] protected List<Sprite> teamSprites = new List<Sprite>();
    [SerializeField] protected Player Player1;
    [SerializeField] protected List<Unit> playerOneUnitTemplates = new List<Unit>();
    [SerializeField] protected Player Player2;
    [SerializeField] protected List<Unit> playerTwoTemplates = new List<Unit>();

    public void StartLevel()
    {
        int Gridrange = (int)Mathf.Log(levelCounter + 5, 3) + 1;

        gridManager.GenerateGrid(Gridrange, gridManager.BasicTile);

        Player1 = new Player(this, "Player1", "First Goo", teamSprites[0], default);

        battlefieldManager.PlaceNewUnit(new Unit(playerOneUnitTemplates[0]), new Vector3Int(0, -Gridrange / 2, 0));

        activeTeams.Enqueue(Player1);

        Player2 = new Player(this, "Player2", "Gooier", teamSprites[1], default);

        battlefieldManager.PlaceNewUnit(new Unit(playerTwoTemplates[0]), new Vector3Int(0, Gridrange / 2, 0));

        activeTeams.Enqueue(Player2);
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
        pathFinder = new Pathfinder(battlefieldManager, gridManager);
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

        battlefieldManager.World.TryGetValue(originVector, out origin);
        battlefieldManager.World.TryGetValue(destinationVector, out destination);

        HashSet<Cell> newRoute = pathFinder.AvoidUnitsPath(origin, destination) as HashSet<Cell>;

        gridManager.HighlightGrid(newRoute);
    }

    [ContextMenu("Generate Unit")]
    protected void DebugGenerateUnit()
    {
        gridManager.DebugGenerateUnit();
    }
}
