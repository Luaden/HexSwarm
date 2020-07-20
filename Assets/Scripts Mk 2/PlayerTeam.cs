using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : Team
{
    protected IAbility selectedAbility;
    protected IUnit selectedUnit;

    public Player
        (GameManager gameManager,
        string name,
        string description,
        Sprite icon,
        Color primaryColor,
        Color secondaryColor,
        Teams teamNumber,
        Vector3Int origin,
        HashSet<IUnit> newUnits)
    {
        this.gameManager = gameManager;
        //battlefieldManager = gameManager.BattlefieldManager;
        Name = name;
        Description = description;
        Icon = icon;
        PrimaryColor = primaryColor;
        SecondaryColor = secondaryColor;
        TeamNumber = teamNumber;
        StartPosition = origin;
        units = newUnits;
    }
    protected override void TakeTurn()
    {
        TeamInit();

        ToggleCameraControls();

        while (unitsUnmoved.Count > 0)
        {
            GetMouseInput();
        }

        EndTurn();
    }

    public override void EndTurn()
    {
        MyTurn = false;
        ToggleCameraControls();
    }

    protected void TeamInit()
    {
        unitsUnmoved = units;
    }

    protected void ToggleCameraControls()
    {
        throw new NotImplementedException(); //GameManager.ConfigManager.ToggleCameraControls(MyTurn);
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
