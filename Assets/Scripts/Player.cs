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
    Cell selectedCell;
    Cell unitCell;
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

    protected void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetUnitAtMouse(GetMousePosition());
        }

        if (Input.GetMouseButtonDown(1))
        {
            MoveUnit();
        }
    }

    private void MoveUnit()
    {
        if (unitCell == null)
            return;

        battlefield.MoveUnit(unitCell.Position, GetMousePosition(), Color);
        unitCell = null;
    }

    protected Vector3Int GetMousePosition() => gridManager.GetCellByClick(Input.mousePosition);

    protected void GetUnitAtMouse(Vector3Int mousePosition)
    {
        battlefield.World.TryGetValue(mousePosition, out selectedCell);

        if (selectedCell.Unit == null)
        {
            unitCell = null;
            return;
        }            

        unitCell = selectedCell;
    }
}
