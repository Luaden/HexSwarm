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
        activeTeams.Peek().StartTurn();
        activeTeams.Enqueue(activeTeams.Dequeue());
        turnOrderDisplay.UpdateUI(activeTeams);
    }

    public void InspectUnitAt(Vector3Int location)
    {
        throw new NotImplementedException();
    }

    public void NewGame()
    {
        throw new NotImplementedException();
    }

    public bool PerformMove(IUnit unit, IPosAbilityDefault ablity, Vector3Int target)
    {
        throw new NotImplementedException();
    }

    public void StartLevel()
    {
        throw new NotImplementedException();
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
