
using System.Collections.Generic;
using UnityEngine;

public class TestHighlighter : MonoBehaviour
{
    [SerializeField] protected List<ICell> highlightedMoveCells = new List<ICell>();
    [SerializeField] protected List<ICell> highlightedAttackCells = new List<ICell>();
    protected GameManager gameManager;

    private void Awake()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
    }

    protected void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            if (Input.GetMouseButtonDown(0))
            {
                ICell cell;
                if (!GameManager.Battlefield.World.TryGetValue(gameManager.GetMousePosition(), out cell))
                    return;

                Debug.Log("Adding move cell at " + cell.GridPosition);
                highlightedMoveCells.Add(cell);
                GameManager.Battlefield.HighlightGrid(highlightedMoveCells, highlightedAttackCells);
            }

            if (Input.GetMouseButtonDown(1))
            {
                ICell cell;
                if (!GameManager.Battlefield.World.TryGetValue(gameManager.GetMousePosition(), out cell))
                    return;
                Debug.Log("Removing move cell at " + cell.GridPosition);
                highlightedMoveCells.Remove(cell);
                GameManager.Battlefield.HighlightGrid(highlightedMoveCells, highlightedAttackCells);
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            if (Input.GetMouseButtonDown(0))
            {
                ICell cell;
                if (!GameManager.Battlefield.World.TryGetValue(gameManager.GetMousePosition(), out cell))
                    return;
                Debug.Log("Adding attack cell at " + cell.GridPosition);
                highlightedAttackCells.Add(cell);
                GameManager.Battlefield.HighlightGrid(highlightedMoveCells, highlightedAttackCells);
            }

            if (Input.GetMouseButtonDown(1))
            {
                ICell cell;
                if (!GameManager.Battlefield.World.TryGetValue(gameManager.GetMousePosition(), out cell))
                    return;
                Debug.Log("Removing attack cell at " + cell.GridPosition);
                highlightedAttackCells.Remove(cell);
                GameManager.Battlefield.HighlightGrid(highlightedMoveCells, highlightedAttackCells);
            }
        }
    }

}
