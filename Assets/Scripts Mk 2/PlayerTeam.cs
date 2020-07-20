using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : Team
{
    protected IAbility selectedAbility;
    protected IUnit selectedUnit;

    public Player
        (GameManager gameManager,
        string name,
        string description,
        Sprite icon,
        Teams teamNumber,
        Vector3Int origin,
        HashSet<IUnit> newUnits)
    {
        this.gameManager = gameManager;
        //battlefieldManager = gameManager.BattlefieldManager;
        Name = name;
        Description = description;
        Icon = icon;
        TeamNumber = teamNumber;
        StartPosition = origin;
        units = newUnits;
    }

    protected override void TakeTurn()
    {
        TeamInit();

        ToggleCameraControls();
        EndTurn();
    }

    public override void EndTurn()
    {
        MyTurn = false;
        ToggleCameraControls();
    }

    protected void TeamInit()
    {
        unitsUnmoved.Clear();
        unitsUnmoved.Union(units);
    }

    protected void ToggleCameraControls()
    {
        gameManager.ConfigManager.ToggleCameraControls(MyTurn);
    }

    protected void GetMouseInput()
    {
        if(Input.GetMouseButtonDown(0) && selectedAbility == null) 
        {
            gameManager.InspectUnitUnderMouse();
            return;
        }

        if (Input.GetMouseButtonDown(0) && selectedUnit != null && selectedAbility != null)
            gameManager.PerformMove(selectedUnit, selectedAbility, battlefieldManager.GetVectorByClick(Input.mousePosition));

        if (Input.GetMouseButtonDown(1))
        {
            if (selectedAbility != null)
            {
                selectedAbility = null;
                return;
            }
            if (selectedUnit != null)
                selectedUnit = null;
        }
    }
}
