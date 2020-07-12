using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class BattlefieldManager : IBattlefield
{
    //Cached references
    GridManager gridManager;

    //State variables
    protected Dictionary<Vector3Int, Cell> world;
    protected IUnit currentUnit;
    protected Cell fromCell;
    protected Cell toCell;
    protected Cell tempCell;

    //Properties
    public Dictionary<Vector3Int, Cell> World { get => world; }
    
    public BattlefieldManager(Dictionary<Vector3Int, Cell> world, GridManager gridManager)
    {
        this.world = world;
        this.gridManager = gridManager;
    }

    public void DestroyUnits(Vector3Int unitPosition)
    {
        throw new System.NotImplementedException();
    }

    public void MoveUnit(Vector3Int unitPosition, Vector3Int destination, Color teamColor)
    {
        world.TryGetValue(unitPosition, out fromCell);
        world.TryGetValue(destination, out toCell);

        if (teamColor != fromCell.Unit.Color)
            return;

        if (gridManager.CheckCanMove(toCell))
        {
            toCell.Unit = fromCell.Unit;
            (toCell.Unit as Unit).Location = destination;
            fromCell.Unit = default;

            gridManager.PaintUnitTile(fromCell.Position, default);
            gridManager.PaintUnitTile(toCell.Position, toCell.Unit.Tile);
        }
    }

    public void PlaceNewUnit(IUnit unit, Vector3Int position)
    {
        toCell = null;
        world.TryGetValue(position, out toCell);

        if (toCell.Unit != null)
            return;

        toCell.Unit = unit;
        (toCell.Unit as Unit).Location = position;

        gridManager.PaintUnitTile(toCell.Position, toCell.Unit.Tile);
    }
}
