using UnityEngine;
using System.Collections.Generic;
using System;

public class AITesting : GameManager
{
    [SerializeField] protected Sprite Player1Icon;
    [SerializeField] protected Unit Player1Unit;
    [SerializeField] protected Sprite AIIcon;
    [SerializeField] protected Unit AIUnit;
    [SerializeField] protected Unit queuedUnit = null;
    protected bool gameIsStarted = false;
    protected PlayerTeam player1;


    [ContextMenu("Spawn Teams")]
    protected new void StartLevel()
    {
        GridSize = Mathf.RoundToInt((10 + levelCounter) * ConfigManager.GameDifficulty);
        Battlefield.GenerateGrid(GridSize, ConfigManager.MapShape);

        activeTeams.Clear();        

        AITeam AI1 = new AITeam(this, "AI1", "Tank wielding maniac.", AIIcon, Teams.AI1,
            new Vector3Int(UnityEngine.Random.Range(-GridSize / 4, GridSize / 4), GridSize / 2, 0),
            new HashSet<IUnit>());
        activeTeams.Enqueue(AI1);

        GenerateUnitForTeam(AI1, AIUnit, AI1.StartPosition);

        player1 = new PlayerTeam(this, "Player1", "Grey Goo", Player1Icon, Teams.Player,
        new Vector3Int(UnityEngine.Random.Range(-GridSize / 4, GridSize / 4), -GridSize / 2, 0),
        new HashSet<IUnit>());
        activeTeams.Enqueue(player1);

        GenerateUnitForTeam(player1, Player1Unit, player1.StartPosition);
    }

    protected new void Update()
    {
        if (!gameIsStarted && queuedUnit != null)
            ClickToDropUnit();
        if (gameIsStarted)
            if (activeTeams.Peek() == player1)
                player1.GetMouseInput();
    }

    protected void ClickToDropUnit()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("Generating unit.");
            GenerateUnitForTeam(queuedUnit.Team, queuedUnit, GetMousePosition());
            queuedUnit = null;
        }
    }

    [ContextMenu("Begin Turn")]
    protected void EndPCurrentTurn()
    {
        queuedUnit = null;
        gameIsStarted = true;
        EndTurn();
        activeTeams.Peek().StartTurn();
    }

    [ContextMenu("Place AI Unit")]
    protected void PlaceAIUnit()
    {
        Unit newUnit = new Unit(AIUnit);
        foreach(Team team in activeTeams)
            if(team.TeamNumber == Teams.AI1)
            {
                team.AddNewUnit(newUnit);
                queuedUnit = newUnit;
            }            
    }
    

    [ContextMenu("Place Player Unit")]
    protected void PlacePlayerUnit()
    {
        Unit newUnit = new Unit(Player1Unit);
        foreach (Team team in activeTeams)
            if (team.TeamNumber == Teams.Player)
            {
                team.AddNewUnit(newUnit);
                queuedUnit = newUnit;
            }
    }
    

    [ContextMenu("Highlight")]
    protected void Highlight()
    {
        Vector3Int vec = new Vector3Int(0, 10, 0);
        Vector3Int vec2 = new Vector3Int(0, -10, 0);
        IEnumerable<Vector3Int> newPath = GameManager.Pathing.FindPath(vec, vec2);
        ICell cell;
        Queue<ICell> cellPath = new Queue<ICell>();

        foreach (Vector3Int location in newPath)
        {
            GameManager.Battlefield.World.TryGetValue(location, out cell);
            cellPath.Enqueue(cell);
        }

        Battlefield.HighlightGrid(cellPath);
    }
}

