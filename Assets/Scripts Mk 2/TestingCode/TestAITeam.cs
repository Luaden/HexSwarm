using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestAITeam : Team
{
    //Used for base data population
    protected int defaultDetectionRange = 3;
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
        Debug.Log("Determining strategy.");
        GetEnemyLOS();
        Debug.Log("Team range is: " + detectionRange * 2);
        Debug.Log("Team range count: " + teamRange.Count);
        GetPossibleMoves();
        //if (bestHits.Count() > 0)
        //    UseOffensiveStrategy();
        //if (selectedDefense != null)
        //    UseDefensiveStrategy();
        
            
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

        battlefieldManager.HighlightGrid(unguardedCells);
    }

    protected void GetPossibleMoves()
    {
        foreach (IUnit unit in units)
        {
            if (!unitsUnmoved.Contains(unit))
                continue;

            if (CheckInTeamRange(unit))
                foreach (IAbility ability in unit.Abilites)
                {
                    IEnumerable<Vector3Int> results = unit.CalcuateValidNewLocation(ability);
                    //EvaluatePossibleAttacks(unit, ability, results);
                    //EvaluatePossibleDefense(unit, ability, results);
                    GetRoute(unit, false);
                }
            else
                {
                    Debug.Log("Unit out of team range.");
                    GetRoute(unit, true);
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

    //protected void EvaluatePossibleAttacks(IUnit unit, IAbility ability, IEnumerable<Vector3Int> results)
    //{
    //    foreach (Vector3Int location in results)
    //    {
    //        foreach(Direction direction in System.Enum.GetValues(typeof(Direction)))
    //        {
    //            IEnumerable<Vector3Int> path = GameManager.Pathing.FindPath(unit.Location, location);
    //            IEnumerable<Vector3Int> hits = unit.DiscoverHits(location, ability, direction).Select(X => X.GridPosition);

    //            if (hits == default)
    //                continue;

    //            AttackPattern attack = new AttackPattern(unit, ability, direction, location, path, hits);

    //            foreach (Vector3Int hitLocation in hits)
    //                SaveAttackPattern(possibleAttacks, hitLocation, attack);
    //        }            
    //    }

    //    GetBestAttacks();
    //}

    //private void EvaluatePossibleDefense(IUnit unit, IAbility ability, IEnumerable<Vector3Int> results)
    //{
    //    Debug.Log("Evaluating possible defenses.");
    //    foreach (Vector3Int location in results)
    //        foreach (Vector3Int cell in unguardedCells.Select(x => x.GridPosition))
    //        {
    //            if (location != cell)
    //                continue;

    //            IEnumerable<Vector3Int> path = GameManager.Pathing.FindPath(unit.Location, location);

    //            DefendPattern defense = new DefendPattern(unit, ability, path, location);
    //            SaveDefensePattern(possibleDefenses, location, defense);
    //        }

    //    GetBestDefense();
    //}

    //protected void EvaluateBestAttacks(AttackPattern attackToCheck)
    //{
    //    Debug.Log("Adding / amending best hits : " + attackToCheck.HitCount);

    //    if ((bestHits.Count == 0) || (attackToCheck.HitCount > bestHits.Peek().HitCount))
    //    {
    //        bestHits.Clear();
    //        bestHits.Push(attackToCheck);
    //    }

    //    else if (attackToCheck.HitCount == bestHits.Peek().HitCount)
    //    {
    //        bestHits.Push(attackToCheck);
    //    }
    //}

    //protected void EvaluateBestDefenses()
    //{
    //    foreach (KeyValuePair<Vector3Int, HashSet<DefendPattern>> entry in possibleDefenses)
    //    {
    //        if (entry.Value.Count == 1)
    //        {
    //            bestBlocks.Enqueue(entry.Value.ElementAt(0));
    //            Debug.Log("Added defense to best blocks.");
    //        }                
    //    }

    //    if (bestBlocks.Count == 0 && possibleDefenses.Count > 0)
    //    {
    //        bestBlocks.Enqueue(possibleDefenses.First().Value.First());
    //        Debug.Log("Added defense to best blocks.");
    //    }            
    //}

    //protected void SaveAttackPattern(Dictionary<Vector3Int, HashSet<AttackPattern>> target, Vector3Int key, AttackPattern value)
    //{
    //    HashSet<AttackPattern> targetPoint;

    //    if (!target.TryGetValue(key, out targetPoint))
    //    {
    //        targetPoint = new HashSet<AttackPattern>();
    //        target.Add(key, targetPoint);
    //    }

    //    targetPoint.Add(value);
    //}

    //protected void SaveDefensePattern(Dictionary<Vector3Int, HashSet<DefendPattern>> target, Vector3Int key, DefendPattern value)
    //{
    //    HashSet<DefendPattern> targetPoint;

    //    if (!target.TryGetValue(key, out targetPoint))
    //    {
    //        targetPoint = new HashSet<DefendPattern>();
    //        target.Add(key, targetPoint);
    //    }

    //    targetPoint.Add(value);
    //    Debug.Log("I have " + possibleDefenses.Count + " defense patterns.");
    //}

    //protected bool ResolvedAttackDifficult()
    //{
    //    int i = UnityEngine.Random.Range(1, 11);
    //    float j = i * ConfigManager.instance.GameDifficulty;

    //    if (j <= 5)
    //        return false;
    //    return true;
    //}

    //protected void MoveRandomly(IUnit unit)
    //{
    //    Debug.Log("Decided to move randomly.");
    //    CheckInTeamRange(unit);
    //    GetRoute(unit, teamRange);
    //}

    //protected void GetBestAttacks()
    //{
    //    foreach (KeyValuePair<Vector3Int, HashSet<AttackPattern>> entry in possibleAttacks)
    //    {
    //        if (!enemiesSeen.Contains(entry.Key))
    //            continue;

    //        foreach (AttackPattern attack in entry.Value)
    //            EvaluateBestAttacks(attack);
    //    }

    //    if (bestHits.Count != 0)
    //        selectedAttack = bestHits.Pop();
    //}

    //protected void UseOffensiveStrategy()
    //{
    //    Queue<ICell> pathCells = new Queue<ICell>();
    //    Queue<ICell> attackLocs = new Queue<ICell>();

    //    foreach(Vector3Int location in selectedAttack.Path)
    //    {
    //        ICell cell;
    //        battlefieldManager.World.TryGetValue(location, out cell);
    //        pathCells.Enqueue(cell);
    //    }

    //    foreach(Vector3Int location in selectedAttack.LocationsHit)
    //    {
    //        ICell cell;
    //        battlefieldManager.World.TryGetValue(location, out cell);
    //        attackLocs.Enqueue(cell);
    //    }

    //    GameManager.Battlefield.HighlightGrid(pathCells, attackLocs);

    //    if (gameManager.PerformMove(selectedAttack.Unit, selectedAttack.Ability, selectedAttack.Direction, selectedAttack.TargetLocation, selectedAttack.Path))
    //    {
    //        enemiesSeen.ExceptWith(selectedAttack.LocationsHit);
    //        UnitHasMoved(selectedAttack.Unit);

    //        selectedAttack = null;
    //    }
    //}

    //protected void GetBestDefense()
    //{
    //    EvaluateBestDefenses();

    //    if (bestBlocks.Count > 0)
    //    {
    //        Debug.Log("Assigning selected defense.");
    //        selectedDefense = bestBlocks.Dequeue();
    //    }            
    //}

    //protected void UseDefensiveStrategy()
    //{
    //    Debug.Log("Using best defense.");

    //    if (gameManager.PerformMove(selectedDefense.Unit, selectedDefense.Ability, Direction.Zero, selectedDefense.TargetLocation, selectedDefense.Path))
    //    {
    //        unguardedCells.Remove(battlefieldManager.World[selectedDefense.TargetLocation]);

    //        UnitHasMoved(selectedDefense.Unit);

    //        selectedDefense = null;
    //    }
    //}

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
    protected IAbility toUse;
    protected void GetRoute(IUnit unit, bool shortestRoute = true)
    {
        if (!shortestRoute)
        {
            IEnumerable<ICell> avoidRange = battlefieldManager.GetNeighborCells(StartPosition, detectionRange);
            HashSet<ICell> tempRange = new HashSet<ICell>();
            tempRange.UnionWith(teamRange);
            tempRange.ExceptWith(avoidRange);

            TakeRoute(unit, tempRange, shortestRoute);            
        }
        else
        {
            TakeRoute(unit, teamRange, shortestRoute);
        }            
    }

    protected void TakeRoute(IUnit unit, HashSet<ICell> cellRange, bool shortestRoute)
    {
        Vector3Int destination = cellRange.First().GridPosition;
        IEnumerable<Vector3Int> currentRoute = GameManager.Pathing.FindPath(unit.Location, destination);


        foreach (ICell location in cellRange)
        {
            if (location.Unit != null)
                continue;

            IEnumerable<Vector3Int> checkPath = GameManager.Pathing.FindPath(unit.Location, location.GridPosition);

            if (!shortestRoute && currentRoute.Count() < checkPath.Count())
            {
                destination = location.GridPosition;
                currentRoute = checkPath;
            }            

            if (shortestRoute && currentRoute.Count() > checkPath.Count())
            {
                destination = location.GridPosition;
                currentRoute = checkPath;
            }
        }

        toTarget = unit.Location;

        while (toTarget == unit.Location)
            foreach (IAbility ability in unit.Abilites)
                foreach (Vector3Int location in unit.CalcuateValidNewLocation(ability))
                    foreach (Vector3Int pathLocation in currentRoute)
                        if (location == pathLocation)
                        {
                            toUse = ability;
                            toTarget = location;
                        }

        IEnumerable<Vector3Int> path = GameManager.Pathing.FindPath(unit.Location, toTarget);
        if (gameManager.PerformMove(unit, toUse, Direction.Zero, toTarget, path))
            UnitHasMoved(unit);
    }

    public override void EndTurn() => base.EndTurn();
}
