using Old;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class AITeam : Team
{
    //Need to get references from Game Manager
    protected BattlefieldManager bfm;
    protected Pathfinder pathfinder;

    //Used for base data population
    protected List<Vector3Int> checkedLocations = new List<Vector3Int>();
    protected List<Vector3Int> hits = new List<Vector3Int>();
    protected int detectionRange = 3;
    protected ICell spawnPoint;
    protected AttackPattern selectedAttack;
    protected DefendPattern selectedDefense;

    //Used for determining strategy
    protected HashSet<Vector3Int> enemiesSeen = new HashSet<Vector3Int>();
    protected List<Vector3Int> unguardedCells = new List<Vector3Int>();
    protected List<AttackPattern> attackPatterns = new List<AttackPattern>();
    protected List<DefendPattern> defendPatterns = new List<DefendPattern>();
    
    //Temp comparables
    protected ICell checkCell;
    protected ICell checkCell2;
    protected IAbility toUse;
    protected Vector3Int toTarget;
    

    protected override void TakeTurn()
    {
        TeamInit();

        CheckForEnemies();
        DetermineStrategy();

        EndTurn();
    }

    protected void TeamInit()
    {
        if (spawnPoint == null)
        {
            foreach (IUnit unit in units)
            {
                //Need a better way to determine spawner.
                if (unit.Name == "Spawner")
                    bfm.World.TryGetValue(unit.Location, out spawnPoint);
            }
        }

        checkedLocations.Clear();
        hits.Clear();
        enemiesSeen.Clear();
        unitsUnmoved = units;
        unitsMoved.Clear();
        unguardedCells.Clear();
    }

    protected void CheckForEnemies()
    {
        foreach (IUnit unit in units)
        {
            CheckForEnemies(unit);
        }
    }

    protected void CheckForEnemies(IUnit unit)
    {
        checkedLocations = bfm.GetNeighborCells(unit.Location, detectionRange) as List<Vector3Int>;

        foreach (Vector3Int location in checkedLocations)
        {
            bfm.World.TryGetValue(location, out checkCell);

            if (checkCell.Unit != null && checkCell.Unit.Team.TeamNumber == 0)
            {
                enemiesSeen.Add(checkCell.Position);
            }
        }
    }

    protected void DetermineStrategy()
    {
        GetPossibleAttacks();
        while (unitsUnmoved.Count > 0)
        {
            if (enemiesSeen.Count == 0)
            {
                MoveRandomly(units[0]);
                GetBestAttacks();
                continue;
            }  
            while (bestHits.Count > 0 && enemiesSeen.Count > 0)
            {
                if (ResolvedAttackDifficult())
                {
                    UseOffensiveStrategy();
                    continue;
                }

                //Defensive logic goes here
            }
            if (bestHits.Count == 0)
                GetBestAttacks();
        }        
    }
    /// <summary>
    /// This function returns if an attack was determied
    /// this will be based on difficulty logic and best possible moves
    /// </summary>
    /// <returns> false when there wan't an attack move selected</returns>
    protected bool ResolvedAttackDifficult()
    {
        if (true)
            return false;

        return true;
    }

    protected void MoveRandomly(IUnit unit)
    {
        //Move code here
        UnitHasMoved(unit);
    }

    protected void UnitHasMoved(IUnit unit)
    {
        CheckForEnemies(unit);
        unitsMoved.Add(unit);
        unitsUnmoved.Remove(unit);

        foreach(AttackPattern attack in attackPatterns)
        {
            if(attack.Unit == unit)
            {
                attackPatterns.Remove(attack);
            }
        }

        foreach (DefendPattern defense in defendPatterns)
        {
            if (defense.Unit == unit)
            {
                defendPatterns.Remove(defense);
            }
        }
    }
    /// <summary>
    /// The vector 3 int is the key hit point for any attack.
    /// </summary>
    protected Dictionary<Vector3Int, HashSet<AttackPattern>> possibleAttacks
         = new Dictionary<Vector3Int, HashSet<AttackPattern>>();    
    protected void GetPossibleAttacks()
    {
        foreach (IUnit unit in unitsUnmoved)
            foreach (IAbility ability in unit.Abilites)
                ReviewPossibleMoves(unit, ability);
    }

    protected void ReviewPossibleMoves(IUnit unit, IAbility ability)
    {
        foreach (Vector3Int location in unit.CalcuateValidNewLocation(ability))
        {
            IEnumerable<Vector3Int> results = unit.DiscoverHits(location, ability);
            if (results == default)
                continue;
            AttackPattern attack = new AttackPattern(unit, ability, location, results);

            if (enemiesSeen.Intersect(results).Count() > 0)
                HandleBestAttackCheck(attack);

            foreach (Vector3Int hitLocation in results)
                SmartAdd(possibleAttacks, hitLocation, attack);
        }
    }
    Stack<AttackPattern> bestHits = new Stack<AttackPattern>();



    protected void SmartAdd(Dictionary<Vector3Int, HashSet<AttackPattern>> target,
        Vector3Int key, AttackPattern value)
    {
        HashSet<AttackPattern> targetPoint;
        //if the dictionary doesn't have the HashSet<AttackPattern> add it
        if (!target.TryGetValue(key, out targetPoint))
        {
            targetPoint = new HashSet<AttackPattern>();
            target.Add(key,targetPoint);
        }
        //append value
        targetPoint.Add(value);
    }

    protected void HandleBestAttackCheck(AttackPattern attackToCheck )
    {
        if ((bestHits.Count == 0) || (attackToCheck.HitCount > bestHits.Peek().HitCount))
        {
            bestHits.Clear();
            bestHits.Push(attackToCheck);
        }
        else if (attackToCheck.HitCount == bestHits.Peek().HitCount)
        {
            bestHits.Push(attackToCheck);
        }
    }

    protected void GetBestAttacks()
    {
        foreach (KeyValuePair<Vector3Int, HashSet<AttackPattern>> entry in possibleAttacks)
        {
            if (!enemiesSeen.Contains(entry.Key))
                continue;

            foreach(AttackPattern attack in entry.Value)
                HandleBestAttackCheck(attack);
        }
    }

    /// <summary>
    /// Performs the attack
    /// clears possible moves from any attack that refrences now dead unts
    /// may need to clear best  moves and check again
    /// </summary>
    protected void UseOffensiveStrategy()
    {
        GameManager.PerformMove(selectedAttack.Unit, selectedAttack.Ability, selectedAttack.TargetLocation);

        enemiesSeen.Remove(selectedAttack.TargetLocation);
        UnitHasMoved(selectedAttack.Unit);

        selectedAttack = null;
    }

    protected void GetEnemyLOS()
    {
        foreach (Vector3Int enemyLoc in enemiesSeen)
        {
            bfm.World.TryGetValue(enemyLoc, out checkCell);

            foreach (Cell location in pathfinder.DirectPath(checkCell as Cell, spawnPoint as Cell))
            {
                if (!unguardedCells.Contains(location.Position))
                    unguardedCells.Add(location.Position);
            }
        }
    }
    
    protected void GetPossibleDefense()
    {
        foreach (IUnit unit in unitsUnmoved)
        {
            foreach (IAbility ability in unit.Abilites)
            {
                foreach (Vector3Int location in unit.CalcuateValidNewLocation(ability) as List<Vector3Int>)
                {
                    bfm.World.TryGetValue(location, out checkCell);

                    if(unguardedCells.Contains(location) && checkCell.Unit == null)
                    {
                        DefendPattern defense = new DefendPattern(unit, ability, location);
                        defendPatterns.Add(defense);
                    }                    
                }
            }            
        }
    }

    protected void GetDefensivePattern()
    {
        if(defendPatterns.Count > 0)
            selectedDefense = defendPatterns[0];
    }

    protected void UseDefensiveStrategy()
    {
        GameManager.PerformMove(selectedDefense.Unit, selectedDefense.Ability, selectedDefense.TargetLocation);

        foreach (DefendPattern defense in defendPatterns)
        {
            if (defense.Unit == selectedDefense.Unit)
                defendPatterns.Remove(defense);
        }

        UnitHasMoved(selectedDefense.Unit);

        selectedDefense = null;
    }

    protected void GetAlternateRoute(IUnit unit)
    {
        bfm.World.TryGetValue(unit.Location, out checkCell);
        bfm.World.TryGetValue(unguardedCells[0], out checkCell2);

        checkedLocations = pathfinder.DirectPath(checkCell as Cell, checkCell2 as Cell) as List<Vector3Int>;

        foreach(Vector3Int cell in unguardedCells)
        {
            bfm.World.TryGetValue(cell, out checkCell2);

            if ((pathfinder.AvoidUnitsPath(checkCell as Cell, checkCell2 as Cell) as List<Cell>).Count < checkedLocations.Count)
                checkedLocations = pathfinder.AvoidUnitsPath(checkCell as Cell, checkCell2 as Cell) as List<Vector3Int>;
        }

        toTarget = unit.Location;

        for (int i = checkedLocations.Count; i > 0; i--)
        {
            foreach (IAbility ability in unit.Abilites)
            {
                foreach (Vector3Int location in unit.CalcuateValidNewLocation(ability) as List<Vector3Int>)
                {
                    if (checkedLocations.Contains(location) &&
                        Vector3.Distance(location, checkedLocations[checkedLocations.Count - 1]) <
                        Vector3.Distance(toTarget, checkedLocations[checkedLocations.Count - 1]))
                    {
                        toUse = ability;
                        toTarget = location;
                    }
                }
            }
        }

        GameManager.PerformMove(unit, toUse, toTarget);
    }
}
