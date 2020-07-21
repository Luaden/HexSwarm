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
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float dieSpeed;

    protected Dictionary<IUnit, GameObject> worldUnits;
    protected GameObject currentUnit;
    protected Queue<Vector3Int> currentPath = new Queue<Vector3Int>();
    protected Vector3Int currentPathIndex;
    protected ConfigManager configManager;



    public void PlaceNewUnit(IUnit unit)
    {
        GameObject worldunit = Instantiate(worldUnitPrefab, this.transform) as GameObject;
        //worldunit.GetComponent<SpriteRenderer>().sprite = unitSprites[unit.ID];
        worldunit.transform.position = unit.Location;
    }

    public void MoveUnit(IUnit unit, IEnumerable<Vector3Int> path, IEnumerable<IUnit> unitsToKill = null)
    {
        worldUnits.TryGetValue(unit, out currentUnit);
        currentPath = path as Queue<Vector3Int>;

        PlayMoveSFX(unit);
        StartCoroutine(MoveUnit(unit, unitsToKill));
    }

    protected void PlayMoveSFX(IUnit unit)
    {
        //configManager.PlaySound(movementSounds[unit.ID]);
    }

    protected void PlayAttackSFX(IUnit unit)
    {
        //configManager.PlaySound(attackSounds[unit.ID]);
    }

    protected void Awake()
    {
        if (configManager == null)
            configManager = FindObjectOfType<ConfigManager>();
    }

    protected IEnumerator MoveUnit(IUnit unit, IEnumerable<IUnit> unitsToKill = null)
    {
        Vector3Int destination = currentPath.Last();
        currentPathIndex = currentPath.Dequeue();

        while (currentUnit.transform.position != destination)
        {
            if (currentUnit.transform.position == currentPathIndex && currentPath.Count > 0)
                currentPathIndex = currentPath.Dequeue();

            currentUnit.transform.position = Vector3.Lerp(currentUnit.transform.position, currentPathIndex, moveSpeed * Time.deltaTime);

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
        float alpha = 1f;

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

