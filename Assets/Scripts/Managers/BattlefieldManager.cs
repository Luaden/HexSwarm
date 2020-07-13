using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class BattlefieldManager : IBattlefield
{
    //Cached references
    protected GridManager gridManager;

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
        Cell killbox;
        if (world.TryGetValue(unitPosition, out killbox))
        {
            killbox.Unit = default;
            gridManager.PaintUnitTile(unitPosition, default);
        }
    }

    public bool MoveUnit(Vector3Int unitPosition, Vector3Int destination, ITeam team)
    {
        world.TryGetValue(unitPosition, out fromCell);
        world.TryGetValue(destination, out toCell);

        if (team != fromCell.Unit.Team)
            return false;

        if (!gridManager.CheckCanMove(toCell))
            return false;
        
        toCell.Unit = fromCell.Unit;
        (toCell.Unit as Unit).Location = destination;
        fromCell.Unit = default;

        gridManager.PaintUnitTile(fromCell.Position, default);
        gridManager.PaintUnitTile(toCell.Position, toCell.Unit.Tile);

        return true;
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
