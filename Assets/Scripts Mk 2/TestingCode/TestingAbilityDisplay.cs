using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingAbilityDisplay : MonoBehaviour
{
    [SerializeField] protected Ability TestingAblity = new Ability();
    [SerializeField] protected Vector3Int worldOrigin = new Vector3Int();
    [SerializeField] protected Direction direction;


    [SerializeField] protected Vector3Int worldDestination = new Vector3Int();
    [SerializeField] protected Quaternion worldAngle;
    [SerializeField] protected Vector3Int worldDelta;


    [SerializeField] protected Vector3Int gridOrigin = new Vector3Int();
    [SerializeField] protected Vector3Int gridDestination = new Vector3Int();
    [SerializeField] protected Quaternion gridAngle;
    [SerializeField] protected Vector3Int gridDelta;

    [ContextMenu("BlurtOptions")]
    public void Blurt()
    {
        foreach(var quartPair in Ability.Transforms)
        {
            Debug.Log(quartPair.Value.eulerAngles);
            float angle;
            Vector3 axis;
            quartPair.Value.ToAngleAxis(out angle, out axis);
            Debug.Log(string.Format("angle :{0}, axis {1}", angle, axis));

        }
    }


    [ContextMenu("CheckAngles")]
    public void CheckAngles()
    {
        ICell thing = GameManager.Battlefield.GetValidCell(worldOrigin);
        if (thing == default)
            return;
        ICell thing2 = GameManager.Battlefield.GetValidCell(worldDestination);
        if (thing2 == default)
            return;
         

        worldDelta.x = worldOrigin.x - worldDestination.x;
        worldDelta.y = worldOrigin.y - worldDestination.y;
        worldDelta.z = worldOrigin.z - worldDestination.z;


        Quaternion resultAngle = Quaternion.FromToRotation(worldOrigin, worldDestination);
        worldAngle.x = resultAngle.x;
        worldAngle.y = resultAngle.y;
        worldAngle.z = resultAngle.z;
        worldAngle.w = resultAngle.w;
        gridOrigin = thing.GridPosition;
        gridDestination = thing2.GridPosition;
        gridDelta = gridOrigin - gridDestination;
        gridAngle = Quaternion.FromToRotation(worldOrigin, worldDestination);

    }

    [ContextMenu("TestAttack")]
    public void TestAttack()
    {
        ICell thing = GameManager.Battlefield.GetValidCell(worldOrigin);

        if (thing == default)
            return;
        gridOrigin = thing.GridPosition;

        GameManager.Battlefield.HighlightGrid
            (
                TestingAblity.GetAttack(direction,gridOrigin)
            );

    }
}
