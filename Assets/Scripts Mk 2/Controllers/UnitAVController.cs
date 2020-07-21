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

    protected Dictionary<IUnit, GameObject> worldUnits = new Dictionary<IUnit, GameObject>();
    protected GameObject currentUnit;
    protected List<GameObject> unitsToDie = new List<GameObject>();
    protected Vector3Int currentPathNode;

    public void PlaceNewUnit(IUnit unit)
    {        
        GameObject worldUnit = Instantiate(worldUnitPrefab, this.transform);

        if (!worldUnits.ContainsKey(unit))
            worldUnits.Add(unit, worldUnit);

        worldUnit.GetComponent<SpriteRenderer>().sprite = unitSprites[unit.ID];
        worldUnit.transform.position = GameManager.Battlefield.GetWorldLocation(unit.Location);
    }

    public void MoveUnit(IUnit unit, IEnumerable<Vector3Int> path)
    {
        worldUnits.TryGetValue(unit, out currentUnit);

        Queue<Vector3> worldPath = new Queue<Vector3>();

        foreach(Vector3Int location in path)
        {
           worldPath.Enqueue(GameManager.Battlefield.GetWorldLocation(location));
        }

        PlayMoveSFX(unit);
        StartCoroutine(CoroutineMoveUnit(currentUnit, worldPath));
    }

    public void KillUnit(IUnit unit)
    {
        GameObject deadUnit;
        worldUnits.TryGetValue(unit, out deadUnit);
        unitsToDie.Add(deadUnit);

        worldUnits.Remove(unit);
    }

    public void ChangeTeamColors(IEnumerable<ColorConfig> colors)
    {
        foreach (ColorConfig color in colors)
            foreach (KeyValuePair<IUnit, GameObject> worldUnit in worldUnits)
                if (color.TeamNumber == worldUnit.Key.Team.TeamNumber)
                    worldUnit.Value.GetComponent<SpriteRenderer>().color = color.PrimaryColor;
    }

    protected void PlayMoveSFX(IUnit unit)
    {
        //configManager.PlaySound(movementSounds[unit.ID]);
    }

    protected void PlayAttackSFX(IUnit unit)
    {
        //configManager.PlaySound(attackSounds[unit.ID]);
    }

    protected IEnumerator CoroutineMoveUnit(GameObject worldUnit, Queue<Vector3> path)
    {        
        Vector3 destination = path.Last();
        Vector3 currentPathNode = path.Dequeue();

        while (worldUnit.transform.position != destination)
        {
            if (worldUnit.transform.position == currentPathNode && path.Count > 0)
                currentPathNode = path.Dequeue();

            worldUnit.transform.position = Vector3.Lerp(worldUnit.transform.position, currentPathNode, moveSpeed * Time.deltaTime);

            yield return null;
        }

        if (unitsToDie.Count > 0)
        {
            foreach(GameObject unit in unitsToDie)
            {
                StartCoroutine(CoroutineKillUnit(unit));
            }
        }
    }

    protected IEnumerator CoroutineKillUnit(GameObject deadUnit)
    {
        SpriteRenderer sprite = deadUnit.GetComponent<SpriteRenderer>();
        float timer = 0f;

        while(timer < dieSpeed)
        {
            timer += Time.deltaTime;
            Color targetAlpha = new Color(1, 1, 1, Mathf.Lerp(1, 0, timer / dieSpeed));

            sprite.color = targetAlpha;
            yield return null;
        }

        unitsToDie.Remove(deadUnit);
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

