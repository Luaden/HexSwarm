using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitAVController : MonoBehaviour
{
    [SerializeField] protected GameObject worldUnitPrefab;
    [SerializeField] protected Sprite[] unitSprites;
    [SerializeField] protected AudioClip[] movementSounds;
    [SerializeField] protected AudioClip[] attackSounds;
    [SerializeField] protected float moveSpeed = 1f;
    [SerializeField] protected float dieSpeed;

    protected ConfigManager configManager;
    protected Dictionary<IUnit, GameObject> worldUnits = new Dictionary<IUnit, GameObject>();
    protected Dictionary<Queue<Vector3>, GameObject> worldUnitPath = new Dictionary<Queue<Vector3>, GameObject>();
    protected List<GameObject> totalUnitsToDie = new List<GameObject>();
    protected List<GameObject> currentUnitsToDie = new List<GameObject>();

    public void PlaceNewUnit(IUnit unit)
    {
        GameObject worldUnit = Instantiate(worldUnitPrefab, this.transform);

        if (!worldUnits.ContainsKey(unit))
            worldUnits.Add(unit, worldUnit);

        worldUnit.GetComponent<SpriteRenderer>().sprite = unitSprites[(int)unit.ID];
        worldUnit.transform.position = GameManager.Battlefield.GetWorldLocation(unit.Location);
    }

    public void MoveUnit(IUnit unit, IEnumerable<Vector3Int> path)
    {
        GameObject worldUnit;
        worldUnits.TryGetValue(unit, out worldUnit);

        Queue<Vector3> worldPath = new Queue<Vector3>();

        foreach (Vector3Int location in path)
        {
            ICell cell;
            GameManager.Battlefield.World.TryGetValue(location, out cell);
            worldPath.Enqueue(cell.WorldPosition);
        }
        
        worldUnitPath.Add(worldPath, worldUnit);

    }

    public void DestroyUnit(IUnit unit)
    {
        GameObject deadUnit;
        worldUnits.TryGetValue(unit, out deadUnit);
        totalUnitsToDie.Add(deadUnit);

        worldUnits.Remove(unit);
    }

    public void ChangeTeamColors(IEnumerable<ColorConfig> colors)
    {
        foreach (ColorConfig color in colors)
            foreach (KeyValuePair<IUnit, GameObject> worldUnit in worldUnits)
                if (color.TeamNumber == worldUnit.Key.Team.TeamNumber)
                    worldUnit.Value.GetComponent<SpriteRenderer>().color = color.PrimaryColor;
    }

    protected void Awake()
    {
        if (configManager == null)
            configManager = FindObjectOfType<ConfigManager>();
    }

    protected void Update()
    {
        if (worldUnitPath.Count > 0)
        {
            GameObject worldUnit = worldUnitPath.First().Value;
            Vector3 nextPosition = worldUnitPath.First().Key.Peek();
            worldUnit.transform.position = Vector3.MoveTowards(
                    worldUnit.transform.position,
                    nextPosition,
                    moveSpeed * Time.deltaTime);

            KillCurrentUnits(worldUnit);

            if (worldUnit.transform.position == nextPosition)
                nextPosition = worldUnitPath.First().Key.Dequeue();
            

            if (worldUnitPath.First().Key.Count == 0 && worldUnit.transform.position == nextPosition)
                worldUnitPath.Remove(worldUnitPath.First().Key);
        }

        if (worldUnitPath.Count == 0)
            KillAllUnits();
    }

    protected void KillAllUnits()
    {
        if(totalUnitsToDie.Count > 0)
            foreach(GameObject deadUnit in totalUnitsToDie)
            {
                SpriteRenderer sprite = deadUnit.GetComponent<SpriteRenderer>();

                Color targetAlpha = new Color(1, 1, 1, sprite.color.a);
                targetAlpha.a = Mathf.Clamp01(targetAlpha.a - (dieSpeed * Time.deltaTime));
                sprite.color = targetAlpha; 

                if(sprite.color.a == 0)
                {
                    Debug.Log("Killing " + deadUnit);
                    currentUnitsToDie.Remove(deadUnit);
                    Destroy(deadUnit);
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

                Color targetAlpha = new Color(1, 1, 1, Mathf.Lerp(1, 0, dieSpeed * Time.deltaTime));
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
        configManager.PlaySound(movementSounds[(int)unit.ID]);
    }

    protected void PlayAttackSFX(IUnit unit)
    {
        configManager.PlaySound(attackSounds[(int)unit.ID]);
    }
}