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
        passedTime = 0;
    }

    public override void StartTurn()
    {
        TeamInit();

        CheckForEnemies();

        DetermineStrategy();
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
        foreach (IUnit unit in unitsUnmoved)
            Debug.Log(unit.Name);
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
        CheckForEnemies();
        GetEnemyLOS();
        GetPossibleMoves();

        if (enemiesSeen.Count == 0 && unitsUnmoved.Count != 0)
        {
            GetNeutralRoute(unitsUnmoved.First(), false);
            GetBestAttacks();
            return;
        }

        while (bestHits.Count > 0 && bestBlocks.Count > 0 && enemiesSeen.Count > 0)
        {
            if (ResolvedAttackDifficult())
            {
                UseOffensiveStrategy();
                GetBestAttacks();
                return;
            }

            UseDefensiveStrategy();
            GetBestDefense();
        }

        while (bestBlocks.Count > 0 && enemiesSeen.Count > 0)
        {
            UseDefensiveStrategy();
            GetBestDefense();
        }

        if(unitsUnmoved.Count > 0)
            GetNeutralRoute(unitsUnmoved.First());
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
        foreach (IUnit unit in unitsUnmoved)
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
                GetNeutralRoute(unit);
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
                IEnumerable<Vector3Int> path = GameManager.Pathing.FindPath(unit.Location, location, !ability.IsJump, ability.MovementRange);
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

                IEnumerable<Vector3Int> path = GameManager.Pathing.FindPath(unit.Location, location, !ability.IsJump, ability.MovementRange);
                    
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

        if (bestBlocks.Count == 0 && possibleDefenses.Count > 0 && possibleDefenses.First().Value.FirstOrDefault() != default)
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

        if (j <= 3)
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
        gameManager.PerformMove(selectedAttack.Unit, selectedAttack.Ability, selectedAttack.Direction, selectedAttack.TargetLocation, selectedAttack.Path);

        enemiesSeen.ExceptWith(selectedAttack.LocationsHit);
        UnitHasMoved(selectedAttack.Unit, selectedAttack.TargetLocation);

        Debug.Log(selectedAttack.Unit.Name + " is attacking. There are " + possibleAttacks + " remaining.");


        selectedAttack = null;
    }

    protected void GetBestDefense()
    {
        EvaluateBestDefenses();

        if (bestBlocks.Count > 0)
            selectedDefense = bestBlocks.Dequeue();
    }

    protected void UseDefensiveStrategy()
    {
        gameManager.PerformMove(selectedDefense.Unit, selectedDefense.Ability, Direction.Zero, selectedDefense.TargetLocation, selectedDefense.Path);

        unguardedCells.Remove(battlefieldManager.World[selectedDefense.TargetLocation]);

        UnitHasMoved(selectedDefense.Unit, selectedDefense.TargetLocation);

        Debug.Log(selectedDefense.Unit.Name + " is defending. There are " + possibleDefenses.Count + " possible defenses remaining.");

        selectedDefense = null;
    }

    protected void UnitHasMoved(IUnit unit, Vector3Int stopLocation)
    {
        CheckForEnemies(unit);
        unitsUnmoved.Remove(unit);

        possibleAttacks.Remove(stopLocation);
        possibleDefenses.Remove(stopLocation);

        foreach (KeyValuePair<Vector3Int, HashSet<AttackPattern>> key in possibleAttacks)
            foreach (AttackPattern attack in key.Value.ToList())
                if (attack.Unit == unit)
                    key.Value.Remove(attack);

        foreach (KeyValuePair<Vector3Int, HashSet<DefendPattern>> key in possibleDefenses)
            foreach (DefendPattern defense in key.Value.ToList())
                if (defense.Unit == unit)
                    key.Value.Remove(defense);

        bestBlocks.Clear();
        GetBestDefense();
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

        if (shortestRoute && enemiesSeen.Count == 0)
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

        if (shortestRoute && enemiesSeen.Count != 0)
        {
            foreach (ICell location in unguardedCells)
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
            TakePath(unit, currentPath);
        }
    }

    private void TakePath(IUnit unit, IEnumerable<Vector3Int> currentPath)
    {
        if(GetAbilityAndTarget(unit, currentPath))
        {
            checkPath = GameManager.Pathing.FindPath(unit.Location, toTarget, true, toUse.MovementRange);

            if (checkPath != default)
            {
                gameManager.PerformMove(unit, toUse, Direction.Zero, toTarget, checkPath);
                Debug.Log(unit.Name + " is taking a path.");
            }
        }

        UnitHasMoved(unit, unit.Location);
    }

    protected bool GetAbilityAndTarget(IUnit unit, IEnumerable<Vector3Int> currentPath)
    {
        toTarget = default;
        toUse = default;

        foreach (IAbility ability in unit.Abilites)
            foreach (Vector3Int location in currentPath)
                foreach (Vector3Int target in unit.CalcuateValidNewLocation(ability).Select(x => x.GridPosition))
                    if (target == location && target != unit.Location)
                    {
                        toTarget = target;
                        toUse = ability;
                        return true;
                    }
        return false;
    }

    public override void EndTurn() => gameManager.EndTurn();


    protected float passedTime;
    public override void NextMove(float elapsedTime)
    {
        passedTime += elapsedTime;
        if(unitsUnmoved.Count > 0)
            DetermineStrategy();

        if (unitsUnmoved.Count == 0 && GameManager.UnitAVController.MovementComplete)
            EndTurn();
    }

    public override void AbilitySelected(IAbility ablity)
    {
        //doNothing
    }
}
