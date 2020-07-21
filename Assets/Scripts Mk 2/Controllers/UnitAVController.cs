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
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float dieSpeed;

    protected Dictionary<IUnit, GameObject> worldUnits;
    protected GameObject currentUnit;
    protected Vector3Int currentPathNode;
    protected ConfigManager configManager;
    protected BattlefieldManager battlefieldManager;



    public void PlaceNewUnit(IUnit unit)
    {        
        GameObject worldUnit = Instantiate(worldUnitPrefab, this.transform);

        if (!worldUnits.TryGetValue(unit, out worldUnit))
            worldUnits.Add(unit, worldUnit);

        //worldunit.GetComponent<SpriteRenderer>().sprite = unitSprites[unit.ID];
        //worldUnit.transform.position = gameManager.BattlefieldManager.GetWorldLocation(unit.Location);
    }

    public void MoveUnit(IUnit unit, IEnumerable<Vector3Int> path, IEnumerable<IUnit> unitsToKill = null)
    {
        worldUnits.TryGetValue(unit, out currentUnit);

        Queue<Vector3Int> worldPath = new Queue<Vector3Int>();

        foreach(Vector3Int location in path)
        {
            //worldPath.Enqueue(gameManager.BattlefieldManager.GetWorldLocation(location));
        }

        PlayMoveSFX(unit);
        StartCoroutine(MoveUnit(unit, worldPath, unitsToKill));
    }

    public void ChangeTeamColors(IEnumerable<IColorConfig> colors)
    {
        //
    }

    protected void Awake()
    {
        if (configManager == null)
            configManager = FindObjectOfType<ConfigManager>();
    }

    protected void PlayMoveSFX(IUnit unit)
    {
        //configManager.PlaySound(movementSounds[unit.ID]);
    }

    protected void PlayAttackSFX(IUnit unit)
    {
        //configManager.PlaySound(attackSounds[unit.ID]);
    }

    protected IEnumerator MoveUnit(IUnit unit, Queue<Vector3Int> path, IEnumerable<IUnit> unitsToKill = null)
    {        
        Vector3Int destination = path.Last();
        Vector3Int currentPathNode = path.Dequeue();

        while (currentUnit.transform.position != destination)
        {
            if (currentUnit.transform.position == currentPathNode && path.Count > 0)
                currentPathNode = path.Dequeue();

            currentUnit.transform.position = Vector3.Lerp(currentUnit.transform.position, currentPathNode, moveSpeed * Time.deltaTime);

            yield return null;
        }

        if (unitsToKill != null)
        {
            //PlayAttackSFX(attackSounds[unit.ID]);
            foreach (IUnit deadUnit in unitsToKill)
            {
                StartCoroutine(KillUnit(deadUnit));
            }
        }
    }

    protected IEnumerator KillUnit(IUnit unit)
    {
        GameObject deadUnit;
        worldUnits.TryGetValue(unit, out deadUnit);
        SpriteRenderer sprite = deadUnit.GetComponent<SpriteRenderer>();
        float timer = 0f;

        while(timer < dieSpeed)
        {
            timer += Time.deltaTime;

            Color targetAlpha = new Color(1, 1, 1, Mathf.Lerp(1, 0, timer / dieSpeed));
            sprite.color = targetAlpha;
            yield return null;
        }

        worldUnits.Remove(unit);
    }

    #region Old Update Method Code

    //protected void Update()
    //{
    //    if (currentUnit != null && currentPath != null)
    //        DoMove();
    //    if (currentUnit != null && currentPath == null)
    //        DoKill();
    //}

    //protected void DoMove()
    //{
    //    currentPathIndex = currentPath.Peek();

    //    if (currentUnit.transform.position != currentPathIndex)
    //        currentUnit.transform.position = Vector3.Lerp(currentUnit.transform.position, currentPathIndex, moveSpeed * Time.deltaTime);

    //    if (currentUnit.transform.position == currentPathIndex && currentPath.Count > 0)
    //    {
    //        currentPath.Dequeue();
    //        currentPathIndex = currentPath.Peek();
    //    }

    //    if(currentUnit.transform.position == currentPathIndex && currentPath.Count == 0)
    //    {
    //        currentPath = null;
    //        currentUnit = null;
    //    }
    //}
    #endregion
}

