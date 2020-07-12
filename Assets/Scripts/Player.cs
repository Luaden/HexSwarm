using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : Team
{
    //Cached references
    protected BattlefieldManager battlefield;
    protected GridManager gridManager;

    //State variable
    Vector3Int currentMouseLocation;

    public Player(GameManager game, string name, string description, Sprite icon, TileBase tile)
        : base(game, name, description, icon, tile) { }

    public override void StartTurn()
    {

    }

    protected void Awake()
    {
        if (battlefield == null)
            battlefield = gameManager.BattlefieldManager;
        if (gridManager == null)
            gridManager = gameManager.GridManager;
    }

    private void MoveUnit()
    {
        GameManager.PerformMove(
            GameManager.DisplayedUnit,
            default,//GameManager.DisplayedUnit.Abilites[0],
            GameManager.GetMousePosition());
    }

    public override void Update()
    {
        if (Input.GetMouseButtonDown(0))
            GameManager.InspectUnitUnderMouse();

        if ((GameManager.DisplayedUnit == null)||(GameManager.DisplayedUnit.Member != this))
                return;

        if (Input.GetMouseButtonDown(1))
            MoveUnit();
    }
}
