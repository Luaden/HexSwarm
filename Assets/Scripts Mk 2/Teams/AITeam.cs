using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AITeam : Team
{
    //Need to get references from Game Manager
    protected Pathfinder pathfinder;

    //Used for base data population
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

    public AITeam
        (GameManager gameManager,
        string name,
        string description,
        Sprite icon,
        Teams teamNumber,
        Vector3Int origin,
        HashSet<IUnit> units)
    {
        this.gameManager = gameManager;
        battlefieldManager = GameManager.Battlefield as BattlefieldManager;
        Name = name;
        Description = description;
        Icon = icon;
        TeamNumber = teamNumber;
        StartPosition = origin;
        this.units = units;
    }

    public override void StartTurn()
    {
        TeamInit();

        CheckForEnemies();
        DetermineStrategy();

        EndTurn();
    }

    protected void TeamInit()
    {
        controlledArea = units.Count();        
        detectionRange = Mathf.RoundToInt(defaultDetectionRange * ConfigManager.instance.GameDifficulty);
        teamRange = battlefieldManager.GetNeighborCells(StartPosition, detectionRange * 2) as Stack<Vector3Int>;
        enemiesSeen.Clear();
        unitsUnmoved.Clear();
        unitsUnmoved.UnionWith(units);
        unguardedCells.Clear();
        possibleAttacks.Clear();
        possibleDefenses.Clear();
        bestHits.Clear();
        bestBlocks.Clear();

        Debug.Log("Initialization complete.");
    }

    protected void CheckForEnemies()
    {
        Debug.Log("Checking for enemies.");
        foreach (IUnit unit in units)
        {
            CheckForEnemies(unit);
        }
    }

    protected void CheckForEnemies(IUnit unit)
    {
        IEnumerable<ICell> neighbors = battlefieldManager.GetNeighborCells(unit.Location, detectionRange);
        ICell checkcell;

        foreach (ICell location in neighbors)
        {
            battlefieldManager.World.TryGetValue(location.GridPosition, out checkcell);

            if (checkcell.Unit != null && checkcell.Unit.Team.TeamNumber == Teams.Player)
            {
                enemiesSeen.Add(checkcell.GridPosition);
            }
        }
    }

    protected void DetermineStrategy()
    {
        Debug.Log("Determining strategy.");
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
        Debug.Log("Getting line of sight.");
        foreach (Vector3Int enemyLoc in enemiesSeen)
        {
            Queue<Vector3Int> path = pathfinder.FindPath(StartPosition, enemyLoc, false) as Queue<Vector3Int>;
            ICell checkCell;

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
        Debug.Log("Getting possible moves.");
        Debug.Log("I have " + unitsUnmoved.Count + " unmoved units out of " + units.Count + " total units.");
        foreach (IUnit unit in unitsUnmoved)
            foreach (IAbility ability in unit.Abilites)
            {
                if(CheckInTeamRange(unit))
                {
                    Debug.Log("This unit is in team range!");
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
        Debug.Log("Evaluating possible attacks.");
        foreach (Vector3Int location in results)
        {
            IEnumerable<Vector3Int> path = pathfinder.FindPath(unit.Location, location);
            IEnumerable<Vector3Int> hits = unit.DiscoverHits(location, ability).Select(X=>X.GridPosition);

            if (hits == default)
                continue;

            AttackPattern attack = new AttackPattern(unit, ability, location, path, hits);

            if (enemiesSeen.Intersect(hits).Count() > 0)
                EvaluateBestAttacks(attack);

            foreach (Vector3Int hitLocation in hits)
                SaveAttackPattern(possibleAttacks, hitLocation, attack);
        }

        GetBestAttacks();
    }

    private void EvaluatePossibleDefense(IUnit unit, IAbility ability, IEnumerable<Vector3Int> results)
    {
        Debug.Log("Evaluating possible defenses.");
        foreach (Vector3Int location in results)
            foreach (Vector3Int cell in unguardedCells)
            {
                if (location != cell)
                    continue;

                IEnumerable<Vector3Int> path = pathfinder.FindPath(unit.Location, location);

                DefendPattern defense = new DefendPattern(unit, ability, path, location);
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
        Debug.Log("I have " + possibleAttacks.Count + " attack patterns.");
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
        Debug.Log("I have " + possibleDefenses.Count + " defense patterns.");
    }

    protected bool ResolvedAttackDifficult()
    {
        int i = Random.Range(1, 11);
        float j = i * ConfigManager.instance.GameDifficulty;

        if (j <= 5)
            return false;
        return true;
    }

    protected void MoveRandomly(IUnit unit)
    {
        Debug.Log("Decided to move randomly.");
        CheckInTeamRange(unit);
        GetRoute(unit, teamRange);
    }

    protected void GetBestAttacks()
    {
        Debug.Log("Decided to use get best attack.");
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
        Debug.Log("Using best attack.");
        if (gameManager.PerformMove(selectedAttack.Unit, selectedAttack.Ability, selectedAttack.TargetLocation, selectedAttack.Path))
        {
            enemiesSeen.Remove(selectedAttack.TargetLocation);
            UnitHasMoved(selectedAttack.Unit);

            selectedAttack = null;
        }        
    }

    protected void GetBestDefense()
    {
        Debug.Log("Decided to use best defense.");
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
        Debug.Log("Using best defense.");

        if (gameManager.PerformMove(selectedDefense.Unit, selectedDefense.Ability, selectedDefense.TargetLocation, selectedDefense.Path))
        {
            unguardedCells.Remove(selectedDefense.TargetLocation);

            UnitHasMoved(selectedDefense.Unit);

            selectedDefense = null;
        }        
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


    protected Vector3Int toTarget;
    protected IAbility toUse;
    protected void GetRoute(IUnit unit, IEnumerable<Vector3Int> locationsToCheck, bool shortestRoute = true)
    {
        Debug.Log("Getting a new route, nonattack, nondefense.");

        Vector3Int locationToGo = locationsToCheck.First();

        foreach(Vector3Int location in locationsToCheck)
        {
            if (pathfinder.FindPath(unit.Location, locationToGo).Count() >
                pathfinder.FindPath(unit.Location, location).Count() && shortestRoute)
                locationToGo = location;
            else if (pathfinder.FindPath(unit.Location, locationToGo).Count() < 
                     pathfinder.FindPath(unit.Location, location).Count())
                 locationToGo = location;
        }

        IEnumerable<Vector3Int> checkLocations = pathfinder.FindPath(unit.Location, locationToGo);

        Vector3Int toTarget = unit.Location;
        
        while(toTarget == unit.Location)
            foreach (IAbility ability in unit.Abilites)
                foreach (Vector3Int location in unit.CalcuateValidNewLocation(ability))
                    foreach(Vector3Int pathLocation in checkLocations)
                        if (location == pathLocation)
                        {
                            toUse = ability;
                            toTarget = location;
                        }

        IEnumerable<Vector3Int> path = pathfinder.FindPath(unit.Location, toTarget);
        if (gameManager.PerformMove(unit, toUse, toTarget, path))
            UnitHasMoved(unit);
    }

    public override void EndTurn() => base.EndTurn();
}
