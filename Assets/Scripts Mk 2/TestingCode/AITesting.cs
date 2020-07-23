using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.XR.WSA.Input;
using System.Linq;

public class AITesting : GameManager
{
    [SerializeField] protected Sprite Player1Icon;
    [SerializeField] protected Sprite AIIcon;
    [SerializeField] protected Unit queuedUnit = null;
    protected bool gameIsStarted = false;
    protected PlayerTeam player1;

    public PlayerTeam Player { get => player1;}

    protected new void Start()
    {
        return;
    }

    protected new void Update()
    {
        if (activeTeams.Count == 0)
            return;
        if (activeTeams.Peek() == player1)
            player1.GetMouseInput();
    }

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

        GenerateUnitForTeam(AI1, UnitManager[Units.Spawner], AI1.StartPosition);
        IEnumerable<ICell> unitStartNeighbors = Battlefield.GetNeighborCells(AI1.StartPosition);
        Vector3Int unitStartPos = unitStartNeighbors.First().GridPosition;
        GenerateUnitForTeam(AI1, UnitManager[Units.Infantry], unitStartPos);


        player1 = new PlayerTeam(this, "Player1", "Grey Goo", Player1Icon, Teams.Player,
        new Vector3Int(UnityEngine.Random.Range(-GridSize / 4, GridSize / 4), -GridSize / 2, 0),
        new HashSet<IUnit>());
        activeTeams.Enqueue(player1);

        GenerateUnitForTeam(player1, UnitManager[Units.Nanos], player1.StartPosition);
    }

    protected void ClickToDropUnit()
    {
        if(Input.GetMouseButtonDown(0))
        {
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

    protected new void GenerateTeam(Team team, Unit template, Vector3Int centerPoint, int radius = 0)
    {
        GenerateUnitForTeam(team, template, centerPoint);
        foreach (ICell cell in Battlefield.GetNeighborCells(centerPoint, radius))
            GenerateUnitForTeam(team,
            template,
            cell.GridPosition);
    }
}

