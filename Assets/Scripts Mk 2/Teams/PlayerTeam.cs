using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerTeam : Team
{
    protected Dictionary<int, int> unitIdToLastSelectedMove;
    protected bool camControls = false;
    protected Vector3Int mousePosHighlight;

    protected ICell pathEndPoint;
    protected HashSet<ICell> validMoves;
    protected IEnumerable<Vector3Int> travelPath;

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

    protected void HandleHighlighting()
    {
        pathEndPoint = GameManager.GetCellUnderMouse();

        if ((gameManager.SelectedAbility == default)||(gameManager.DisplayedUnit == default)||
            (gameManager.DisplayedUnit.Team != this))
        {
            pathEndPoint = default;
            return;
        }

        //my unit is selected and I have a selected ablity
    }


    public void GetMouseInput() { ResolveMouseInput(); }
    protected bool ResolveMouseInput()
    {
        if (Input.GetMouseButtonDown(1))
            gameManager.ClearActiveUnit();

        if (!EventSystem.current.IsPointerOverGameObject())
            return false;
        //must be over grid

        HandleHighlighting();

        if (!Input.GetMouseButtonDown(0))
            return true;
        
        if (pathEndPoint != default)
            return gameManager.PerformMove(gameManager.DisplayedUnit, gameManager.SelectedAbility, pathEndPoint.GridPosition, travelPath);

        gameManager.InspectUnitUnderMouse();
        return true;
    }
}
