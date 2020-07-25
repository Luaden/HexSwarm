using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class UnitValidator: TestingGameManager
{
    [SerializeField, Header("UnitValidator")] protected SOAbility validateAblity;
    [SerializeField] protected SOUnit valdateUnit;
    [SerializeField] protected bool ValidateMovement;

    public new void Update()
    {
        ICell targetPoint = GameManager.GetCellUnderMouse();
        if (targetPoint == default)
            return;
        Unit vUnit = (Unit)valdateUnit;
        vUnit.Location = targetPoint.GridPosition;

        Battlefield.HighlightGrid(
            (ValidateMovement) ? GameManager.Pathing.FindTraversableArea(targetPoint, vUnit, (Ability)validateAblity).AsEnumerable() : Array.Empty<ICell>(),
            CalcuateAttack(targetPoint, vUnit)
        );
    }

    protected IEnumerable<ICell> CalcuateAttack(ICell target, IUnit vUnit)
    {
        if ((target == default) || (vUnit == default) || ValidateMovement)
            return Array.Empty<ICell>();

        Direction direction = PlayerTeam.DeterminMouseAngle(target);
        IAbility move = (Ability)validateAblity;

        return move.GetAttack(direction, vUnit.Location);
    }
}

