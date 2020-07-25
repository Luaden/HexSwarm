using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestAITeam : Team
{
    //Used for base data population
    protected int defaultDetectionRange = 6;
    protected int detectionRange;
    protected int controlledArea;
    protected AttackPattern selectedAttack;
    protected DefendPattern selectedDefense;

    //Used for determining strategy
    protected HashSet<Vector3Int> enemiesSeen = new HashSet<Vector3Int>();
    protected HashSet<ICell> unguardedCells = new HashSet<ICell>();
    protected Dictionary<Vector3Int, HashSet<AttackPattern>> possibleAttacks =
                                                            new Dictionary<Vector3Int, HashSet<AttackPattern>>();
    protected Dictionary<Vector3Int, HashSet<DefendPattern>> possibleDefenses =
                                                            new Dictionary<Vector3Int, HashSet<DefendPattern>>();
    protected Stack<AttackPattern> bestHits = new Stack<AttackPattern>();
    protected Queue<DefendPattern> bestBlocks = new Queue<DefendPattern>();
    protected HashSet<ICell> teamRange = new HashSet<ICell>();

    public TestAITeam
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
        ICell cell;
        battlefieldManager.World.TryGetValue(StartPosition, out cell);
        controlledArea = units.Count();
        detectionRange = Mathf.RoundToInt(defaultDetectionRange * ConfigManager.instance.GameDifficulty);
        teamRange.UnionWith(battlefieldManager.GetNeighborCells(StartPosition, detectionRange * 2));
        teamRange.Add(cell);
        enemiesSeen.Clear();
        unitsUnmoved.UnionWith(units);
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
        IEnumerable<ICell> neighbors = battlefieldManager.GetNeighborCells(unit.Location, detectionRange);
        ICell checkcell;

        foreach (ICell location in neighbors)
        {
            if (battlefieldManager.World.TryGetValue(location.GridPosition, out checkcell))
                if (checkcell.Unit != null && checkcell.Unit.Team.TeamNumber == Teams.Player)
                { 
                    if(!enemiesSeen.Contains(checkcell.Unit.Location))
                        enemiesSeen.Add(checkcell.GridPosition);
                }
        }
    }

    protected void DetermineStrategy()
    {
        GetEnemyLOS();
        GetPossibleMoves();

        while(unitsUnmoved.Count > 0)
        {
            if (enemiesSeen.Count == 0)
            {
                GetNeutralRoute(unitsUnmoved.First());
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

            while (bestBlocks.Count > 0 && enemiesSeen.Count > 0)
            {
                UseDefensiveStrategy();
                GetBestDefense();
            }

            GetNeutralRoute(unitsUnmoved.First(), false);
        }        
    }

    protected void GetEnemyLOS()
    {
        foreach (Vector3Int enemyLoc in enemiesSeen)
        {
            IEnumerable<Vector3Int> path = GameManager.Pathing.FindPath(enemyLoc, StartPosition, false);
            ICell checkCell;

            foreach (Vector3Int location in path)
            {
                battlefieldManager.World.TryGetValue(location, out checkCell);

                if (checkCell.Unit != null && checkCell.Unit.Team.TeamNumber != Teams.Player)
                    continue;

                unguardedCells.Add(checkCell);
            }
        }
    }

    protected void GetPossibleMoves()
    {
        foreach (IUnit unit in units)
        {
            if (CheckInTeamRange(unit))
            {
                foreach (IAbility ability in unit.Abilites)
                {
                    IEnumerable<Vector3Int> results = unit.CalcuateValidNewLocation(ability).Select(X => X.GridPosition);
                    EvaluatePossibleAttacks(unit, ability, results);
                    EvaluatePossibleDefense(unit, ability, results);
                }
            }
            else
            {
                GetNeutralRoute(unit, true);
            }
        }
    }

    private bool CheckInTeamRange(IUnit unit)
    {
        ICell unitCell;
        battlefieldManager.World.TryGetValue(unit.Location, out unitCell);

        if (!teamRange.Contains(unitCell))
            return false;
        return true;
    }

    protected void EvaluatePossibleAttacks(IUnit unit, IAbility ability, IEnumerable<Vector3Int> results)
    {
        foreach (Vector3Int location in results)
        {
            foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
            {
                IEnumerable<Vector3Int> path = GameManager.Pathing.FindPath(unit.Location, location);
                IEnumerable<Vector3Int> hits = unit.DiscoverHits(location, ability, direction).Select(X => X.GridPosition);

                if (hits == default)
                    continue;

                AttackPattern attack = new AttackPattern(unit, ability, direction, location, path, hits);

                foreach (Vector3Int hitLocation in hits)
                    SaveAttackPattern(possibleAttacks, hitLocation, attack);
            }
        }

        GetBestAttacks();
    }

    private void EvaluatePossibleDefense(IUnit unit, IAbility ability, IEnumerable<Vector3Int> results)
    {
        foreach (Vector3Int location in results)
            foreach (Vector3Int cell in unguardedCells.Select(x => x.GridPosition))
            {
                if (location != cell)
                    continue;

                IEnumerable<Vector3Int> path = GameManager.Pathing.FindPath(unit.Location, location);

                DefendPattern defense = new DefendPattern(unit, ability, path, location);
                SaveDefensePattern(possibleDefenses, location, defense);
            }

        GetBestDefense();
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
                bestBlocks.Enqueue(entry.Value.ElementAt(0));
        }

        if (bestBlocks.Count == 0 && possibleDefenses.Count > 0)
            bestBlocks.Enqueue(possibleDefenses.First().Value.First());
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
        int i = Random.Range(1, 11);
        float j = i * ConfigManager.instance.GameDifficulty;

        if (j <= 5)
            return false;
        return true;
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
        if (gameManager.PerformMove(selectedAttack.Unit, selectedAttack.Ability, selectedAttack.Direction, selectedAttack.TargetLocation, selectedAttack.Path))
        {
            enemiesSeen.ExceptWith(selectedAttack.LocationsHit);
            UnitHasMoved(selectedAttack.Unit);

            selectedAttack = null;
            Debug.Log("Using best offense.");
        }
    }

    protected void GetBestDefense()
    {
        EvaluateBestDefenses();

        if (bestBlocks.Count > 0)
            selectedDefense = bestBlocks.Dequeue();
    }

    protected void UseDefensiveStrategy()
    {
        Debug.Log("Using best defense.");

        Queue<ICell> pathCells = new Queue<ICell>();

        foreach (Vector3Int location in selectedDefense.Path)
        {
            ICell cell;
            battlefieldManager.World.TryGetValue(location, out cell);
            pathCells.Enqueue(cell);
        }

        if (gameManager.PerformMove(selectedDefense.Unit, selectedDefense.Ability, Direction.Zero, selectedDefense.TargetLocation, selectedDefense.Path))
        {
            unguardedCells.Remove(battlefieldManager.World[selectedDefense.TargetLocation]);

            UnitHasMoved(selectedDefense.Unit);

            selectedDefense = null;
        }
    }

    protected void UnitHasMoved(IUnit unit)
    {
        CheckForEnemies(unit);
        unitsUnmoved.Remove(unit);

        possibleAttacks.Remove(unit.Location);
        possibleDefenses.Remove(unit.Location);

        foreach (KeyValuePair<Vector3Int, HashSet<AttackPattern>> key in possibleAttacks)
            foreach (AttackPattern attack in key.Value.ToList())
                if (attack.Unit == unit)
                    key.Value.Remove(attack);

        foreach (KeyValuePair<Vector3Int, HashSet<DefendPattern>> key in possibleDefenses)
            foreach (DefendPattern defense in key.Value.ToList())
                if (defense.Unit == unit)
                    key.Value.Remove(defense);
    }


    protected Vector3Int toTarget;
    protected Vector3Int destination;
    protected IAbility toUse;
    protected IEnumerable<Vector3Int> currentPath;
    protected IEnumerable<Vector3Int> checkPath;


    protected void GetNeutralRoute(IUnit unit, bool shortestRoute = true)
    {
        int i = Random.Range(0, teamRange.Count);
        int j = 0;

        foreach (ICell location in teamRange)
        {
            if (location.Unit != null || location == null)
                continue;

            j++;
            if (j == i)
                destination = location.GridPosition;
        }

        currentPath = GameManager.Pathing.FindPath(unit.Location, destination);

        if (shortestRoute)
        {
            foreach (ICell location in teamRange)
            {
                checkPath = GameManager.Pathing.FindPath(unit.Location, location.GridPosition);

                if (checkPath == null || location.Unit != null)
                    continue;

                if (currentPath.Count() > checkPath.Count())
                {
                    destination = location.GridPosition;
                    currentPath = checkPath;
                }
            }

            TakePath(unit, currentPath);
        }
        else if (!shortestRoute)
        {
            currentPath = GameManager.Pathing.FindPath(unit.Location, destination);
            TakePath(unit, currentPath);
        }
    }

    private void TakePath(IUnit unit, IEnumerable<Vector3Int> currentPath)
    {
        foreach (IAbility ability in unit.Abilites)
            foreach (Vector3Int location in currentPath)
                foreach (Vector3Int target in unit.CalcuateValidNewLocation(ability).Select(x => x.GridPosition))
                    if (target == location && target != unit.Location)
                    {
                        toTarget = target;
                        toUse = ability;
                        break;
                    }


        IEnumerable<Vector3Int> abilityPath = GameManager.Pathing.FindPath(unit.Location, toTarget);

        gameManager.PerformMove(unit, toUse, Direction.Zero, toTarget, abilityPath);
        UnitHasMoved(unit);
    }

    public override void EndTurn() => gameManager.EndTurn();

}
