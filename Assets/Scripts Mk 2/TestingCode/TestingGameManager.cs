using UnityEngine;
using System.Collections.Generic;


public class TestingGameManager : GameManager
{
    [SerializeField] protected Sprite Team1Icon;
    [SerializeField] protected Unit Team1Unit;
    [SerializeField] protected Sprite Team2Icon;
    [SerializeField] protected Unit Team2Unit;


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
}

