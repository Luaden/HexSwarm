using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitAVController : MonoBehaviour
{
    [SerializeField] protected GameObject worldUnitPrefab;
    [SerializeField] protected AudioClip[] initMoveSFX;
    [SerializeField] protected AudioClip[] initAttackSFX;
    [SerializeField] protected float moveSpeed = 1f;
    [SerializeField] protected float dieSpeed = 1f;
    
    protected GameManager gameManager;
    protected ConfigManager configManager;
    protected Dictionary<Unit, GameObject> worldObjects = new Dictionary<Unit, GameObject>();
    protected Dictionary<GameObject, Unit> worldUnits = new Dictionary<GameObject, Unit>();
    protected Dictionary<Queue<Vector3>, GameObject> worldUnitPath = new Dictionary<Queue<Vector3>, GameObject>();
    protected List<GameObject> totalUnitsToDie = new List<GameObject>();
    protected List<GameObject> currentUnitsToDie = new List<GameObject>();
    protected bool movementComplete = true;
    protected bool playNewSound = true;

    protected Dictionary<Units, AudioClip> movementSFX = new Dictionary<Units, AudioClip>();
    protected Dictionary<Units, AudioClip> attackSFX = new Dictionary<Units, AudioClip>();

    public bool MovementComplete { get => movementComplete; private set => movementComplete = value; }
    

    public void PlaceNewUnit(Unit unit)
    {
        GameObject worldUnit = Instantiate(worldUnitPrefab, this.transform);
        SpriteRenderer renderer = worldUnit.GetComponent<SpriteRenderer>();

        if (!worldObjects.ContainsKey(unit))
            worldObjects.Add(unit, worldUnit);
        if (!worldUnits.ContainsKey(worldUnit))
            worldUnits.Add(worldUnit, unit);


        renderer.color = configManager.TeamColors[unit.Team.TeamNumber].PrimaryColor;
        renderer.sprite = unit.Icon;
        
        worldUnit.transform.position = GameManager.Battlefield.GetWorldLocation(unit.Location);
    }

    public void MoveUnit(Unit unit, IEnumerable<Vector3Int> path)
    {
        if (unit == null)
            return;

        GameObject worldUnit;
        worldObjects.TryGetValue(unit, out worldUnit);

        Queue<Vector3> worldPath = new Queue<Vector3>();

        foreach (Vector3Int location in path)
        {
            ICell cell;
            GameManager.Battlefield.World.TryGetValue(location, out cell);
            worldPath.Enqueue(cell.WorldPosition);
        }

        worldUnitPath.Add(worldPath, worldUnit);
        
    }

    public void DestroyUnit(Unit unit)
    {
        GameObject deadUnit;
        Unit deadIUnit;

        worldObjects.TryGetValue(unit, out deadUnit);
        worldUnits.TryGetValue(deadUnit, out deadIUnit);
        totalUnitsToDie.Add(deadUnit);

        worldObjects.Remove(unit);
        worldUnits.Remove(deadUnit);
    }

    public void ChangeTeamColors(Dictionary<Teams, ColorConfig> colors)
    {
        foreach(KeyValuePair<Unit, GameObject> worldUnit in worldObjects)
            worldUnit.Value.GetComponent<SpriteRenderer>().color = colors[worldUnit.Key.Team.TeamNumber].PrimaryColor;
    }

    public void Nuke()
    {
        MovementComplete = false;
        foreach(KeyValuePair<Unit, GameObject> entry in worldObjects)
        {
            Destroy(entry.Value);
        }       

        worldObjects.Clear();
        worldUnits.Clear();
        worldUnitPath.Clear();
        totalUnitsToDie.Clear();
        currentUnitsToDie.Clear();
        MovementComplete = true;
    }

    protected void Awake()
    {
        if (configManager == null)
            configManager = FindObjectOfType<ConfigManager>();
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
    }

    protected void Start()
    {
        movementSFX.Add(Units.Infantry, initMoveSFX[0]);
        movementSFX.Add(Units.Tank, initMoveSFX[1]);
        movementSFX.Add(Units.Heli, initMoveSFX[2]);
        movementSFX.Add(Units.Nanos, null);
        movementSFX.Add(Units.Spawner, null);
        attackSFX.Add(Units.Infantry, initAttackSFX[0]);
        attackSFX.Add(Units.Tank, initAttackSFX[1]);
        attackSFX.Add(Units.Heli, initAttackSFX[2]);
        attackSFX.Add(Units.Nanos, null);
        attackSFX.Add(Units.Spawner, null);

    }

    protected void FixedUpdate()
    {
        if (worldUnitPath.Count > 0)
        {
            MovementComplete = false;

            if (worldUnitPath.First().Key.Count == 0)
            {
                if(totalUnitsToDie.Count != 0)
                    PlayAttackSFX(worldUnits[worldUnitPath.First().Value]);
                worldUnitPath.Remove(worldUnitPath.First().Key);
                playNewSound = true;

                return;
            }

            Unit temp;
            if (playNewSound && worldUnits.TryGetValue(worldUnitPath.First().Value, out temp))
                PlayMoveSFX(worldUnits[worldUnitPath.First().Value]);

            playNewSound = false;

            GameObject worldUnit = worldUnitPath.First().Value;
            Vector3 nextPosition = worldUnitPath.First().Key.Peek();

            worldUnit.transform.position = Vector3.MoveTowards(
                    worldUnit.transform.position,
                    nextPosition,
                    moveSpeed * Time.deltaTime);

            ConfigManager.instance.RepositionCamera(worldUnit.transform.position);

            if (worldUnit.transform.position == nextPosition && worldUnitPath.First().Key.Count != 0)
                nextPosition = worldUnitPath.First().Key.Dequeue();
                
        }
            
        if (worldUnitPath.Count == 0)
        {
            KillAllUnits();
            MovementComplete = true;
        }
    }

    protected void KillAllUnits()
    {
        if(totalUnitsToDie.Count > 0)
            foreach(GameObject deadUnit in totalUnitsToDie)
            {
                SpriteRenderer sprite = deadUnit.GetComponent<SpriteRenderer>();

                Color targetAlpha = new Color(sprite.color.r, sprite.color.g, sprite.color.r, sprite.color.a);
                targetAlpha.a = Mathf.Clamp01(targetAlpha.a - (dieSpeed * Time.deltaTime));
                sprite.color = targetAlpha; 

                if(sprite.color.a == 0)
                {
                    totalUnitsToDie.Remove(deadUnit);
                    Destroy(deadUnit);
                    break;
                }
            }
    }

    protected void KillCurrentUnits(GameObject worldUnit)
    {
        foreach (GameObject deadUnit in totalUnitsToDie)
        {
            if (worldUnit.transform.position == deadUnit.transform.position)
            {
                currentUnitsToDie.Add(deadUnit);
                totalUnitsToDie.Remove(deadUnit);
                break;
            }
        }

        if (currentUnitsToDie.Count > 0)
            foreach (GameObject deadUnit in currentUnitsToDie)
            {
                SpriteRenderer sprite = deadUnit.GetComponent<SpriteRenderer>();

                Color targetAlpha = new Color(sprite.color.r, sprite.color.g, sprite.color.r, sprite.color.a);
                targetAlpha.a = Mathf.Clamp01(targetAlpha.a - (dieSpeed * Time.deltaTime));
                sprite.color = targetAlpha;

                if (sprite.color.a == 0)
                {
                    currentUnitsToDie.Remove(deadUnit);
                    Destroy(deadUnit);
                }
            }
    }

    protected void PlayMoveSFX(IUnit unit)
    {
        AudioClip clip; 
        if(movementSFX.TryGetValue(unit.ID, out clip))
            ConfigManager.instance.PlaySound(clip);
    }

    protected void PlayAttackSFX(IUnit unit)
    {
        AudioClip clip;
        if (attackSFX.TryGetValue(unit.ID, out clip))
            ConfigManager.instance.PlaySound(clip);
    }
}