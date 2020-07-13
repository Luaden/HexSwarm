using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Enemy after having StartTurnCalled will for each unit
/// randomly pick to call the AIUnit's best or random move
/// </summary>
/// 
[System.Serializable]
public class Enemy : Team
{
    [SerializeField] GridManager gridManager;

    public Enemy(Enemy enemy)
    {
        this.gameManager = enemy.gameManager;
        this.gridManager = enemy.gridManager;
        teamName = enemy.Name;
        description = enemy.Description;
        icon = enemy.Icon;
        tile = enemy.tile;
    }

    public override void StartTurn()
    {
        IUnit tempUnit = gridManager.DebugGenerateUnit();
        units.Add(tempUnit);

        foreach (IUnit unit in units)
        {
            GetRandomMove(unit);
        }

        HasMove = false;
        
        EndTurn();
    } 

    public override void ResolveHighlight(IUnit unit, IPosAbilityDefault ablity, Vector3Int target)
    {

    }

    protected void Start()
    {
    }
    
    protected void GetRandomMove(IUnit unit)
    {
        //Get game manager difficulty here;
        //subtract GameManager.Difficulty from Max;
        int moveToUse = Random.Range(0, 10);

        if(moveToUse > 0)
        {
            unit.CalcuateValidNewLocation(default);
            Debug.Log("Used dumb ability");
        }
        else
        {
            unit.CalcuateValidNewLocation(default);
            Debug.Log("Used smart ability");
        }
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }
}
