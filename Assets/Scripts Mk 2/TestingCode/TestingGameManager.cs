using UnityEngine;
using System.Collections.Generic;


public class TestingGameManager : GameManager
{
    [SerializeField] protected Sprite Team1Icon;
    [SerializeField] protected Unit Team1Unit;
    [SerializeField] protected Sprite Team2Icon;
    [SerializeField] protected Unit Team2Unit;

    protected new void Start()
    {
        base.Start();
    }

    [ContextMenu("lets see it")]
    protected new void StartLevel()
    {
        int gridRange = levelCounter + 10;

        Battlefield.GenerateGrid(gridRange, MapShape.Hexagon);
        activeTeams.Clear();


        Player player2 = new Player(this, "Player2", "Gooier", Team2Icon, Teams.Player,
            new Vector3Int(UnityEngine.Random.Range(-gridRange / 4, gridRange / 4), gridRange / 2, 0),
            new HashSet<IUnit>());
        activeTeams.Enqueue(player2);

        GenerateTeam(player2,
             Team2Unit,
             player2.StartPosition,
             gridRange / 4);
        Debug.Log("second player made.");

        player1 = new Player(this, "Player1", "First Goo", Team1Icon, Teams.Player,
        new Vector3Int(UnityEngine.Random.Range(-gridRange / 4, gridRange / 4), -gridRange / 2, 0),
        new HashSet<IUnit>());
       activeTeams.Enqueue(player1);

        GenerateTeam(player1,
            Team1Unit,
            new Vector3Int(UnityEngine.Random.Range(-gridRange / 4, gridRange / 4), -gridRange / 2, 0),
            gridRange / 4);
        Debug.Log("First player made.");

        EndTurn();
    }

    private new void Update()
    {
        base.Update();
        if(Input.GetMouseButtonDown(0))
        {
            ICell cell;
            Vector3Int mouseCell = Battlefield.GetVectorByClick(Input.mousePosition);

            Battlefield.World.TryGetValue(mouseCell, out cell);
            Debug.Log("Mouse cell is " + cell.GridPosition + "." + "World position is " + cell.WorldPosition + ".");
        }
    }

    [ContextMenu("EndPlayer1")]
    protected void EndPlayer1Turn()
    {
        if (Player1 == activeTeams.Peek())
            EndTurn();
    }

    [ContextMenu("EndOtherTurn")]
    protected void EndPlayer2Turn()
    {
        if (Player1 != activeTeams.Peek())
            EndTurn();
    }

    [ContextMenu("Highlight")]
    protected void Highlight()
    {
        Vector3Int vec = new Vector3Int(0, 10, 0);
        Vector3Int vec2 = new Vector3Int(0, -10, 0);
        IEnumerable<Vector3Int> newPath = Pathing.FindPath(vec, vec2);
        ICell cell;
        Queue<ICell> cellPath = new Queue<ICell>();

        foreach(Vector3Int location in newPath)
        {
            Battlefield.World.TryGetValue(location, out cell);
            cellPath.Enqueue(cell);
        }

        Battlefield.HighlightGrid(cellPath);
    }
}

