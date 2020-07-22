using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerTeam : Team
{
    protected Dictionary<int, int> unitIdToLastSelectedMove;
    protected bool camControls = false;

    public PlayerTeam
        (GameManager gameManager,
        string name,
        string description,
        Sprite icon,
        Teams teamNumber,
        Vector3Int origin,
        HashSet<IUnit> newUnits)
    {
        this.gameManager = gameManager;
        Name = name;
        Description = description;
        Icon = icon;
        TeamNumber = teamNumber;
        StartPosition = origin;
        units = newUnits;
    }

    public override void StartTurn()
    {
        TeamInit();
        ToggleCameraControls();
        EndTurn();
    }

    public override void EndTurn()
    {
        ToggleCameraControls();
    }

    protected void TeamInit()
    {
        unitsUnmoved.Clear();
        unitsUnmoved.Union(units);
    }

    protected void ToggleCameraControls()
    {
        camControls = !camControls;
        ConfigManager.instance.ToggleCameraControls(camControls);
    }


    public void GetMouseInput()
    {
        if(Input.GetMouseButtonDown(0) && gameManager.SelectedAbility == null) 
        {
            gameManager.InspectUnitUnderMouse();
            return;
        }

        if (Input.GetMouseButtonDown(0) && gameManager.DisplayedUnit != null && gameManager.SelectedAbility != null)
        {
            Vector3Int mousePos = gameManager.GetMousePosition();
            IEnumerable<Vector3Int> path = GameManager.Pathing.FindPath(gameManager.DisplayedUnit.Location, mousePos); 
            gameManager.PerformMove(gameManager.DisplayedUnit, gameManager.SelectedAbility, mousePos, path);
        }            

        if (Input.GetMouseButtonDown(1))
        {
            if (gameManager.DisplayedUnit != null)
            {
                Vector3Int mousePos = gameManager.GetMousePosition();
                IEnumerable<Vector3Int> path = GameManager.Pathing.FindPath(gameManager.DisplayedUnit.Location, mousePos);

                gameManager.PerformMove(
                    gameManager.DisplayedUnit, 
                    gameManager.SelectedAbility,
                    mousePos, 
                    path);
            }
        }
    }
}
