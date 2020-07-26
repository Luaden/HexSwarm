using UnityEngine;

public class AITesting2 : GameManager
{
    protected new void FixedUpdate()
    {

    }
    [ContextMenu("NextTurn")]
    protected void TriggerNextTurn()
    {
        activeTeams.Peek().NextMove(0);
    }

    protected new void Start()
    {
        StartLevel();
    }

    [ContextMenu("lets see it")]
    protected new void StartLevel()
    {
        int gridRange = levelCounter + 4;

        Battlefield.GenerateGrid(gridRange, MapShape.Hexagon);
        activeTeams.Clear();
        DisplayedUnit = default;
        SelectedUnitPanel.UpdateUI(DisplayedUnit);

        SpawnAITeam(1);
        SpawnPlayerTeam();

        EndTurn();
    }

}
