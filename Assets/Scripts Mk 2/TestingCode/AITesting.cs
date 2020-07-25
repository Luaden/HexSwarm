using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AITesting : GameManager
{
    [SerializeField] protected Sprite Player1Icon;
    [SerializeField] protected Sprite AIIcon;
    [SerializeField] protected int teamsToSpawn;
    [SerializeField] protected int rangeFromNeighbors;
    protected bool gameIsStarted = false;
    protected PlayerTeam player1;
    protected TestAITeam ai1;
    protected bool canAddUnits = true;
    protected Vector3Int spawnLocation;

    public PlayerTeam Player { get => player1; }


    protected new void Update()
    {
        if (canAddUnits)
        {
            if (Input.GetMouseButtonDown(4))
                GenerateAITeam(GetMousePosition());
            if (Input.GetMouseButtonDown(3))
                GeneratePlayerTeam(GetMousePosition());
            if (Input.GetMouseButtonDown(0))
                GeneratePlayerUnit(GetMousePosition());
            if (Input.GetMouseButtonDown(1))
                GenerateAIUnit(GetMousePosition());
        }

        if (activeTeams.Count == 0)
            return;
        if (activeTeams.Peek() == player1)
            player1.GetMouseInput();
    }

    protected new void Start()
    {
        GridSize = Mathf.RoundToInt((25 + levelCounter) * ConfigManager.GameDifficulty);
        Battlefield.GenerateGrid(GridSize, ConfigManager.MapShape);

        activeTeams.Clear();
    }

    protected void GenerateAITeam(Vector3Int target)
    {
        ai1 = new TestAITeam(this, "AI1", "Tank wielding maniac.", AIIcon, Teams.AI1,
            target,
            new HashSet<IUnit>());
        activeTeams.Enqueue(ai1);

        GenerateUnitForTeam(ai1, UnitManager[Units.Infantry], ai1.StartPosition);
    }

    protected void GeneratePlayerTeam(Vector3Int target)
    {
        player1 = new PlayerTeam(this, "Player1", "Grey Goo", Player1Icon, Teams.Player,
        target,
        new HashSet<IUnit>());
        activeTeams.Enqueue(player1);

        GenerateUnitForTeam(player1, UnitManager[Units.Nanos], player1.StartPosition);
    }

    protected void GeneratePlayerUnit(Vector3Int target)
    {
        GenerateUnitForTeam(player1, UnitManager[Units.Nanos], target);
    }
    protected void GenerateAIUnit(Vector3Int target)
    {
        GenerateUnitForTeam(ai1, UnitManager[Units.Infantry], target);
    }

    [ContextMenu("Begin Turn")]
    protected void EndPCurrentTurn()
    {
        canAddUnits = false;
        EndTurn();
    }

    [ContextMenu("Spawn Teams")]
    protected void SpawnTeams()
    {
        for(int i = 0; i < teamsToSpawn; i++)
        {
            if (i == 0)
                SpawnPlayerTeam();

            SpawnAITeam(i);
        }
    }

    protected Vector3Int GetSpawnLocation()
    {
        int i = Random.Range(0, Battlefield.World.Count + 1);
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


        if (neighbors.Count() < Battlefield.GetNeighborCells(new Vector3Int(0,0,0), rangeFromNeighbors).Count())
            return false;

        foreach (ICell neighbor in neighbors)            
            if (neighbor.Unit != null)
                return false;

        Debug.Log(neighbors.Count());
        return true;
    }

    protected void SpawnPlayerTeam()
    {
        while (true)
        {
            if (ValidateSpawnLocation(GetSpawnLocation()))
            {
                Debug.Log("Spawning player team");
                player1 = new PlayerTeam(this, "Player1", "Grey Goo", Player1Icon, Teams.Player,
                spawnLocation,
                new HashSet<IUnit>());
                activeTeams.Enqueue(player1);

                GenerateUnitForTeam(player1, UnitManager[Units.Nanos], player1.StartPosition);
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
                ai1 = new TestAITeam(this, "AI"+i, "Tank wielding maniac.", AIIcon, (Teams)i,
                spawnLocation,
                new HashSet<IUnit>());
                activeTeams.Enqueue(ai1);

                GenerateUnitForTeam(ai1, UnitManager[Units.Infantry], ai1.StartPosition);
                break;
            }
        }
    }
}

