using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Editor variables
    [SerializeField] protected GridManager gridManager;

    //Editor debug variables, remove for live build.
    [SerializeField] IUnit tempUnit;

    //Cached references
    protected BattlefieldManager battlefieldManager;

    protected void Awake()
    {
        if (gridManager == null)
            gridManager = FindObjectOfType<GridManager>();
    }

    protected void Update()
    {

    }
}
