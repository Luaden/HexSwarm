using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AITesting : GameManager
{
    [SerializeField] protected Sprite Player1Icon;
    [SerializeField] protected Sprite AIIcon;
    protected bool gameIsStarted = false;
    protected PlayerTeam player1;
    protected TestAITeam ai1;
    protected bool canAddUnits = true;
   
    public PlayerTeam Player { get => player1;}


    protected new void Update()
    {
        if(canAddUnits)
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
        GridSize = Mathf.RoundToInt((10 + levelCounter) * ConfigManager.GameDifficulty);
        Battlefield.GenerateGrid(GridSize, ConfigManager.MapShape);

        activeTeams.Clear();        
    }

    protected void GenerateAITeam(Vector3Int target)
    {
        ai1 = new TestAITeam(this, "AI1", "Tank wielding maniac.", AIIcon, Teams.AI1,
            target,
            new HashSet<IUnit>());
        activeTeams.Enqueue(ai1);

        GenerateUnitForTeam(ai1, UnitManager[Units.Spawner], ai1.StartPosition);
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
}

