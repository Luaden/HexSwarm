using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderDisplay : CoreUIElement<IGameManager>
{
    [SerializeField] protected Image[] turnOrderIcons;
    [SerializeField] protected TMP_Text turnCount;
    [SerializeField] protected TMP_Text levelCount;



    public override void UpdateUI(IGameManager gameManager)
    {

        Queue<ITeam> tempQueue = new Queue<ITeam>(gameManager.ActiveTeams);

        if (ClearedIfEmpty(gameManager))
            return;

        for (int i = 0; i < turnOrderIcons.Length; i++)
        {
            turnOrderIcons[i].sprite = tempQueue.Peek().Icon;
            tempQueue.Enqueue(tempQueue.Dequeue());
        }

        UpdateText(turnCount, "Turn Count: " + gameManager.TurnCounter.ToString());
        UpdateText(levelCount, "Win Count: " + gameManager.LevelCounter.ToString());
    }

    protected override bool ClearedIfEmpty(IGameManager gameManager)
    {
        if (teamQueue.Count > 0)
            return false;

        for (int i = 0; i < turnOrderIcons.Length; i++)
        {
            UpdateSprite(turnOrderIcons[i], default);
        }                

        return true;
    }
}
