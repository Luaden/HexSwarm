using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : Team
{
    //Cached references
    protected BattlefieldManager battlefieldManager;
    protected GridManager gridManager;

    //State variable
    Vector3Int currentMouseLocation;
    Cell selectedCell;
    IUnit selectedUnit;


    public Player(GameManager game, string name, string description, Sprite icon, TileBase tile)
        : base(game, name, description, icon, tile) { }

    public override void StartTurn() => HasMove = true;
    
    public override void EndTurn() => gameManager.EndTurn();

    protected void Awake()
    {
        if (battlefieldManager == null)
            battlefieldManager = gameManager.BattlefieldManager;
        if (gridManager == null)
            gridManager = gameManager.GridManager;
    }

    protected void Update()
    {
        if (HasMove)
            ListenForMouseInput();
        else
            EndTurn();
    }

    protected void SelectCell()
    {
        gridManager.ClearHighlightedTiles();

        if (!battlefieldManager.World.TryGetValue(GetMousePosition(), out selectedCell))
            return;

        gridManager.HighlightGrid(selectedCell);

        if (selectedCell.Unit == default)
            return;

        selectedUnit = selectedCell.Unit;
        gameManager.UpdateUnitUI(selectedUnit);
    }

    private void ListenForMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
            SelectCell();

        if (Input.GetMouseButtonDown(1))
            MoveUnit();
    }

    protected void MoveUnit()
    {
        if (selectedCell.Unit == null)
            return;

        if (!battlefieldManager.World.TryGetValue(GetMousePosition(), out selectedCell))
            return;

        if (battlefieldManager.MoveUnit(selectedUnit.Location, selectedCell.Position, this))
            HasMove = false;
    }

    protected Vector3Int GetMousePosition() => gridManager.GetCellByClick(Input.mousePosition);
}
