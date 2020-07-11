using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SelectedUnitPanel : CoreUIElement<IUnit>
{
    [SerializeField] protected Image unitImage;
    [SerializeField] protected TMP_Text unitName;
    [SerializeField] protected TMP_Text unitDescription;

    [SerializeField] protected Image ability1Image;
    [SerializeField] protected TMP_Text ability1Name;
    [SerializeField] protected TMP_Text ability1Description;

    [SerializeField] protected Image ability2Image;   
    [SerializeField] protected TMP_Text ability2Name;    
    [SerializeField] protected TMP_Text ability2Description;

    public override void UpdateUI(IUnit primaryData)
    {
        UpdateSprite(unitImage, IUnit.Icon);
        UpdateText(unitName, IUnit.Name);
    }

    protected override bool ClearedIfEmpty(IUnit newData)
    {
        throw new System.NotImplementedException();
    }
}
