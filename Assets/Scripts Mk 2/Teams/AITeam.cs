using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AITeam : Team
{
    //Need to get references from Game Manager
    protected Pathfinder pathfinder;

    //Used for base data population
    protected Stack<Vector3Int> checkLocations;
    protected int defaultDetectionRange = 3;
    protected int detectionRange;
    protected int controlledArea;
    protected AttackPattern selectedAttack;
    protected DefendPattern selectedDefense;

    //Used for determining strategy
    protected HashSet<Vector3Int> enemiesSeen = new HashSet<Vector3Int>();
    protected HashSet<Vector3Int> unguardedCells = new HashSet<Vector3Int>();
    protected Dictionary<Vector3Int, HashSet<AttackPattern>> possibleAttacks =
                                                            new Dictionary<Vector3Int, HashSet<AttackPattern>>();
    protected Dictionary<Vector3Int, HashSet<DefendPattern>> possibleDefenses =
                                                            new Dictionary<Vector3Int, HashSet<DefendPattern>>();
    protected Stack<AttackPattern> bestHits = new Stack<AttackPattern>();
    protected Stack<DefendPattern> bestBlocks = new Stack<DefendPattern>();
    protected Stack<Vector3Int> teamRange = new Stack<Vector3Int>();

    //Temp comparables
    protected IAbility toUse;
    protected ICell checkCell;
    protected Vector3Int locationToGo;
    protected Vector3Int toTarget;

    public AITeam
        (GameManager gameManager,
        string name,
        string description,
        Sprite icon,
        Color primaryColor,
        Color secondaryColor,
        Teams teamNumber,
        Vector3Int origin,
        HashSet<IUnit> newUnits)
    {
        this.gameManager = gameManager;
        //battlefieldManager = gameManager.BattlefieldManager;
        Name = name;
        Description = description;
        Icon = icon;
        PrimaryColor = primaryColor;
        SecondaryColor = secondaryColor;
        TeamNumber = teamNumber;
        StartPosition = origin;
        units = newUnits;
    }

    protected override void TakeTurn()
    {
        TeamInit();

        CheckForEnemies();
        DetermineStrategy();

        EndTurn();
    }

    protected void TeamInit()
    {
        controlledArea = units.Count();
        //Need a reference for this.
        //detectionRange = Mathf.RoundToInt(defaultDetectionRange * GameManager.Difficulty);
        teamRange = battlefieldManager.GetNeighborCells(StartPosition, detectionRange * 2) as Stack<Vector3Int>;
        checkLocations.Clear();
        enemiesSeen.Clear();
        unitsUnmoved = units;
        unguardedCells.Clear();
        possibleAttacks.Clear();
        possibleDefenses.Clear();
        bestHits.Clear();
        bestBlocks.Clear();
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
        checkLocations = battlefieldManager.GetNeighborCells(unit.Location, detectionRange) as Stack<Vector3Int>;

        foreach (Vector3Int location in checkLocations)
        {
            battlefieldManager.World.TryGetValue(location, out checkCell);

            if (checkCell.Unit != null && checkCell.Unit.Team.TeamNumber == Teams.Player)
            {
                enemiesSeen.Add(checkCell.Position);
            }
        }
    }

    protected void DetermineStrategy()
    {
        GetEnemyLOS();
        GetPossibleMoves();        

        while (unitsUnmoved.Count > 0)
        {
            if (enemiesSeen.Count == 0)
            {
                MoveRandomly(unitsUnmoved.First());
                GetBestAttacks();
                continue;
            }

            while (bestHits.Count > 0 && bestBlocks.Count > 0 && enemiesSeen.Count > 0)
            {
                if (ResolvedAttackDifficult())
                {
                    UseOffensiveStrategy();
                    GetBestAttacks();
                    continue;
                }
                UseDefensiveStrategy();
                GetBestDefense();
            }

            while(bestBlocks.Count > 0 && enemiesSeen.Count > 0)
            {
                UseDefensiveStrategy();
                GetBestDefense();
            }

            if (bestHits.Count == 0)
                GetBestAttacks();
            if (bestBlocks.Count == 0)
                GetBestDefense();

            GetRoute(unitsUnmoved.First(), unguardedCells, false);
        }        
    }

    protected void GetEnemyLOS()
    {
        foreach (Vector3Int enemyLoc in enemiesSeen)
        {
            Queue<Vector3Int> path = pathfinder.FindPath(StartPosition, enemyLoc, false) as Queue<Vector3Int>;

            foreach (Vector3Int location in path)
            {
                battlefieldManager.World.TryGetValue(location, out checkCell);

                if (checkCell.Unit != null)
                {
                    path.Dequeue();
                    continue;
                }
                unguardedCells.Add(path.Dequeue());
            }
        }
    }

    protected void GetPossibleMoves()
    {
        foreach (IUnit unit in unitsUnmoved)
            foreach (IAbility ability in unit.Abilites)
            {
                if(CheckInTeamRange(unit))
                {
                    IEnumerable<Vector3Int> results = unit.CalcuateValidNewLocation(ability);
                    EvaluatePossibleAttacks(unit, ability, results);
                    EvaluatePossibleDefense(unit, ability, results);
                }
                else
                {
                    GetRoute(unit, teamRange);
                }
            }
    }

    private bool CheckInTeamRange(IUnit unit)
    {
        if (!teamRange.Contains(unit.Location))
            return false;
        return true;
    }

    protected void EvaluatePossibleAttacks(IUnit unit, IAbility ability, IEnumerable<Vector3Int> results)
    {    
        foreach (Vector3Int location in results)
        {
            IEnumerable<Vector3Int> hits = unit.DiscoverHits(location, ability);

            if (hits == default)
                continue;

            AttackPattern attack = new AttackPattern(unit, ability, location, hits);

            if (enemiesSeen.Intersect(hits).Count() > 0)
                EvaluateBestAttacks(attack);

            foreach (Vector3Int hitLocation in hits)
                SaveAttackPattern(possibleAttacks, hitLocation, attack);
        }

        GetBestAttacks();
    }

    private void EvaluatePossibleDefense(IUnit unit, IAbility ability, IEnumerable<Vector3Int> results)
    {
        foreach (Vector3Int location in results)
            foreach (Vector3Int cell in unguardedCells)
            {
                if (location != cell)
                    continue;

                DefendPattern defense = new DefendPattern(unit, ability, location);
                SaveDefensePattern(possibleDefenses, location, defense);
            }
        EvaluateBestDefenses();
    }

    protected void EvaluateBestAttacks(AttackPattern attackToCheck)
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

    protected void EvaluateBestDefenses()
    {
        foreach (KeyValuePair<Vector3Int, HashSet<DefendPattern>> entry in possibleDefenses)
        {
            if (entry.Value.Count == 1)
                bestBlocks.Push(entry.Value.ElementAt(0));
        }

        if (bestBlocks.Count == 0)
            bestBlocks.Push(possibleDefenses.First().Value.ElementAt(0));
    }

    protected void SaveAttackPattern(Dictionary<Vector3Int, HashSet<AttackPattern>> target, Vector3Int key, AttackPattern value)
    {
        HashSet<AttackPattern> targetPoint;

        if (!target.TryGetValue(key, out targetPoint))
        {
            targetPoint = new HashSet<AttackPattern>();
            target.Add(key, targetPoint);
        }

        targetPoint.Add(value);
    }

    protected void SaveDefensePattern(Dictionary<Vector3Int, HashSet<DefendPattern>> target, Vector3Int key, DefendPattern value)
    {
        HashSet<DefendPattern> targetPoint;

        if (!target.TryGetValue(key, out targetPoint))
        {
            targetPoint = new HashSet<DefendPattern>();
            target.Add(key, targetPoint);
        }

        targetPoint.Add(value);
    }

    protected bool ResolvedAttackDifficult()
    {
        int i = UnityEngine.Random.Range(1, 11); // * GameManager;
        //Need a reference here
        //i *= GameManager.Difficulty;

        if (i <= 5)
            return false;
        return true;
    }

    protected void MoveRandomly(IUnit unit)
    {
        CheckInTeamRange(unit);
        GetRoute(unit, teamRange);
        UnitHasMoved(unit);
    }

    protected void GetBestAttacks()
    {
        foreach (KeyValuePair<Vector3Int, HashSet<AttackPattern>> entry in possibleAttacks)
        {
            if (!enemiesSeen.Contains(entry.Key))
                continue;

            foreach (AttackPattern attack in entry.Value)
                EvaluateBestAttacks(attack);
        }

        if (bestHits.Count != 0)
            selectedAttack = bestHits.Pop();
    }

    protected void UseOffensiveStrategy()
    {
        gameManager.PerformMove(selectedAttack.Unit, selectedAttack.Ability, selectedAttack.TargetLocation);

        enemiesSeen.Remove(selectedAttack.TargetLocation);
        UnitHasMoved(selectedAttack.Unit);

        selectedAttack = null;
    }

    protected void GetBestDefense()
    {
        foreach (KeyValuePair<Vector3Int, HashSet<AttackPattern>> entry in possibleAttacks)
        {
            if (!enemiesSeen.Contains(entry.Key))
                continue;

            EvaluateBestDefenses();
        }

        if (bestBlocks.Count != 0)
            selectedDefense = bestBlocks.Pop();
    }

    protected void UseDefensiveStrategy()
    {
        gameManager.PerformMove(selectedDefense.Unit, selectedDefense.Ability, selectedDefense.TargetLocation);
        unguardedCells.Remove(selectedDefense.TargetLocation);

        UnitHasMoved(selectedDefense.Unit);

        selectedDefense = null;
    }

    protected void UnitHasMoved(IUnit unit)
    {
        CheckForEnemies(unit);
        unitsUnmoved.Remove(unit);

        foreach (KeyValuePair<Vector3Int, HashSet<AttackPattern>> key in possibleAttacks)
            foreach (AttackPattern attack in key.Value)
                if (attack.Unit == unit)
                {
                    possibleAttacks.Remove(key.Key);
                }

        foreach (KeyValuePair<Vector3Int, HashSet<DefendPattern>> key in possibleDefenses)
            foreach (DefendPattern defense in key.Value)
                if (defense.Unit == unit)
                {
                    possibleAttacks.Remove(key.Key);
                }
    }        

    protected void GetRoute(IUnit unit, IEnumerable<Vector3Int> locationsToCheck, bool shortestRoute = true)
    {
        locationToGo = locationsToCheck.First();

        foreach(Vector3Int location in locationsToCheck)
        {
            if (pathfinder.FindPath(unit.Location, locationToGo).Count() >
                pathfinder.FindPath(unit.Location, location).Count() && shortestRoute)
                locationToGo = location;
            else if (pathfinder.FindPath(unit.Location, locationToGo).Count() < 
                     pathfinder.FindPath(unit.Location, location).Count())
                 locationToGo = location;
        }

        checkLocations = pathfinder.FindPath(unit.Location, locationToGo) as Stack<Vector3Int>;

        toTarget = unit.Location;

        while(toTarget == unit.Location)
            foreach (IAbility ability in unit.Abilites)
                foreach (Vector3Int location in unit.CalcuateValidNewLocation(ability))
                {
                    if (location == checkLocations.Peek())
                    {
                        toUse = ability;
                        toTarget = location;                        
                    }

                    checkLocations.Pop();
                }

        gameManager.PerformMove(unit, toUse, toTarget);
        UnitHasMoved(unit);
    }

    public override void EndTurn() => base.EndTurn();
}
