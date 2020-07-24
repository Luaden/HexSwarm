using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AITesting : GameManager
{
    [SerializeField] protected Sprite Player1Icon;
    [SerializeField] protected Sprite AIIcon;
    protected bool gameIsStarted = false;
    protected PlayerTeam player1;
    protected AITeam ai1;
    protected bool canAddAI = false;
    protected bool canAddPlayer = false;
    public PlayerTeam Player { get => player1;}

    protected new void Start()
    {
        return;
    }

    protected new void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAddAI)
            GenerateUnitForTeam(ai1, UnitManager[Units.Infantry], GetMousePosition());
        if (Input.GetMouseButtonDown(0) && canAddPlayer)
            GenerateUnitForTeam(player1, UnitManager[Units.Nanos], GetMousePosition());
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

        ai1 = new AITeam(this, "AI1", "Tank wielding maniac.", AIIcon, Teams.AI1,
            new Vector3Int(UnityEngine.Random.Range(-GridSize / 4, GridSize / 4), GridSize / 2, 0),
            new HashSet<IUnit>());
        activeTeams.Enqueue(ai1);

        GenerateUnitForTeam(ai1, UnitManager[Units.Spawner], ai1.StartPosition);


        player1 = new PlayerTeam(this, "Player1", "Grey Goo", Player1Icon, Teams.Player,
        new Vector3Int(UnityEngine.Random.Range(-GridSize / 4, GridSize / 4), -GridSize / 2, 0),
        new HashSet<IUnit>());
        activeTeams.Enqueue(player1);

        GenerateUnitForTeam(player1, UnitManager[Units.Nanos], player1.StartPosition);
    }

    [ContextMenu("Click to Add AI Unit")]
    protected void ClickToAddAIUnit()
    {
        canAddAI = true;
        canAddPlayer = false;
    }
    [ContextMenu("Click to Add Player Unit")]
    protected void ClickToAddPlayerUnit()
    {
        canAddAI = false;
        canAddPlayer = true;
    }

    [ContextMenu("Begin Turn")]
    protected void EndPCurrentTurn()
    {
        canAddAI = false;
        canAddPlayer = false;

        EndTurn();
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

