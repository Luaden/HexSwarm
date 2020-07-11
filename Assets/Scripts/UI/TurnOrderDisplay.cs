using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderDisplay : CoreUIElement<Queue<ITeam>>
{
    [SerializeField] protected Image[] turnOrderIcons;



    public override void UpdateUI(Queue<ITeam> teamQueue)
    {

        Queue<ITeam> tempQueue = new Queue<ITeam>(teamQueue);

        if(ClearedIfEmpty(teamQueue))
            for (int i = 0; i < turnOrderIcons.Length; i++)
            {
                turnOrderIcons[i].sprite = tempQueue.Dequeue().Icon;
                tempQueue.Enqueue(tempQueue.Dequeue());
            }
    }

    protected override bool ClearedIfEmpty(Queue<ITeam> teamQueue)
    {
        if(teamQueue.Count <= 0)
            return false;

        return true;
    }
}
