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
    protected Direction direction;

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
    }

    public override void EndTurn()
    {
        ToggleCameraControls();
        gameManager.EndTurn();
    }

    protected void TeamInit()
    {
        unitsUnmoved.Clear();
        unitsUnmoved.UnionWith(units);
        validMoves = new HashSet<ICell>();
    }

    protected void ToggleCameraControls()
    {
        camControls = !camControls;
        ConfigManager.instance.ToggleCameraControls(camControls);
    }

    protected bool HandleHighlighting()
    {
        pathEndPoint = GameManager.GetCellUnderMouse();

        if (validMoves.Count != 0)
            GameManager.Battlefield.HighlightPossibleAttacks(validMoves);

        if ((pathEndPoint == default)||(gameManager.SelectedAbility == default) || (gameManager.DisplayedUnit == default))
            return InvalidateHighLight();
        if (!unitsUnmoved.Contains(gameManager.DisplayedUnit))
            return InvalidateHighLight();
        //my unit is selected and I have a selected ablity

        if (!validMoves.Contains(pathEndPoint))
            return InvalidateHighLight();
        //I actually am over a valid point

        direction = DeterminMouseAngle(pathEndPoint);

        travelPath =
            ((gameManager.SelectedAbility.IsJump) || validMoves.Count == 1) ?
                new Vector3Int[] { pathEndPoint.GridPosition } :
                GameManager.Pathing.FindPath(gameManager.DisplayedUnit.Location, pathEndPoint.GridPosition,true,gameManager.SelectedAbility.MovementRange);

        GameManager.Battlefield.HighlightGrid(
            travelPath,
            gameManager.SelectedAbility.GetAttack(direction, pathEndPoint.GridPosition)
        );

        return true;
    }

    public static Direction DeterminMouseAngle(ICell cell)
    {
        var mouseLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var mydelta = mouseLocation - cell.WorldPosition;
        var angle = Mathf.Atan2(mydelta.y, mydelta.x) * Mathf.Rad2Deg + 30;
        if (angle < 0)
            angle += 360;
        angle %= 360;
        return (Direction)Mathf.FloorToInt((angle / 60));
    }

    protected bool InvalidateHighLight()
    {
        pathEndPoint = default;
        GameManager.Battlefield.ClearHighlights();
        travelPath = default;
        return true;
    }


    public void GetMouseInput() { ResolveMouseInput(); }
    protected bool ResolveMouseInput()
    {
        GameManager.Battlefield.HighlightUnmovedUnits(unitsUnmoved.Select(X=>X.Location));
        GameManager.Battlefield.HighlightPossibleAttacks(validMoves);

        if (Input.GetMouseButtonDown(1))
        {
            GameManager.Battlefield.HighlightPossibleAttacks(Array.Empty<Vector3Int>());
            gameManager.ClearActiveUnit();
        }
        


        if (EventSystem.current.IsPointerOverGameObject())
            return false;
        //must be over grid

        HandleHighlighting();

        if (!Input.GetMouseButton(0))
            return true;

        if (validMoves.Contains(pathEndPoint))
        {
            if (gameManager.PerformMove(gameManager.DisplayedUnit, gameManager.SelectedAbility, direction, pathEndPoint.GridPosition, travelPath))
                unitsUnmoved.Remove(gameManager.DisplayedUnit);
            if (unitsUnmoved.Count == 0)
                gameManager.EndPlayerTurn();
           
            GameManager.Battlefield.HighlightUnmovedUnits(unitsUnmoved.Select(X => X.Location));
        }

        gameManager.InspectUnitUnderMouse();
        validMoves.Clear();
        return true;
    }

    public override void NextMove(float elapsedTime)
    {
        //TODO: DO NOTHING
        //TODO: maybe setup a time elapsed for turn and for game duration
        // for speed runing lol
    }

    public override void AbilitySelected(IAbility ablity)
    {
        validMoves.Clear();
        if (ablity == default)
            return;

        validMoves.UnionWith(gameManager.DisplayedUnit.CalcuateValidNewLocation(gameManager.SelectedAbility));
    }
}
