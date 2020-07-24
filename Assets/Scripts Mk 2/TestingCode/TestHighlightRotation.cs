using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHighlightRotation : TestingGameManager
{
    [SerializeField, Header("TestHighlightRotation")] protected SOAbility ability;
    [SerializeField] protected Vector3Int location;
    [SerializeField] protected Vector3 delta;
    [SerializeField] protected Cell TargetCell;
    [SerializeField] protected Direction direction;
    [SerializeField] protected bool useUpdate = true;
    [SerializeField] protected float angle;

    public new void Update()
    {
        if (!useUpdate)
            return;

        ICell selectedcell;
        if (!Battlefield.World.TryGetValue(GetMousePosition(), out selectedcell))
            return;

        TargetCell = selectedcell as Cell;

        location.x = selectedcell.GridPosition.x;
        location.y = selectedcell.GridPosition.y;
        location.z = selectedcell.GridPosition.z;

        direction = SolveAngle();
        Battlefield.HighlightGrid(((IAbility)(Ability)ability).GetAttack(direction, TargetCell.GridPosition));
    }

    protected Direction SolveAngle()
    {
        ICell selectedcell;
        if (!Battlefield.World.TryGetValue(GetMousePosition(), out selectedcell))
            return Direction.Zero;

        var mouseLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var mydelta = mouseLocation - selectedcell.WorldPosition;
        delta.x = mydelta.x;
        delta.y = mydelta.y;
        delta.x = mydelta.x;
        angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg + 30;
        if (angle < 0)
            angle += 360;
        angle %= 360;
        return (Direction)Mathf.FloorToInt((angle / 60));
    }
}
