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
    [SerializeField] protected Image ability1Background;
    [SerializeField] protected Button ability1Button;
    [SerializeField] protected TMP_Text ability1Name;
    [SerializeField] protected TMP_Text ability1Description;

    [SerializeField] protected Image ability2Image;
    [SerializeField] protected Image ability2Background;
    [SerializeField] protected Button ability2Button;
    [SerializeField] protected TMP_Text ability2Name;    
    [SerializeField] protected TMP_Text ability2Description;

    protected GameManager gameManager;
    protected Color playerColor;

    protected bool firstIsSelected;
    public bool FirstIsSelected => firstIsSelected;

    public override void UpdateUI(IUnit unit)
    {
        if (ClearedIfEmpty(unit))
            return;

        UpdateSprite(unitImage, unit.Icon);
        UpdateText(unitName, unit.Name);
        UpdateText(unitDescription, unit.Description);

        //This needs to be a single sprite representing the action, or we need a way to determine if it's damage or movement.
        UpdateSprite(ability1Image, unit.Abilites[0].DamageGrid);
        UpdateText(ability1Name, unit.Abilites[0].Name);
        UpdateText(ability1Description, unit.Abilites[0].Description);

        UpdateSprite(ability2Image, unit.Abilites[1].DamageGrid);
        UpdateText(ability2Name, unit.Abilites[1].Name);
        UpdateText(ability2Description, unit.Abilites[1].Description);
    }

    protected override bool ClearedIfEmpty(IUnit unit)
    {
        if(unit == null)
        {
            UpdateSprite(unitImage, null);
            UpdateText(unitName, string.Empty);
            UpdateText(unitDescription, string.Empty);

            //This needs to be a single sprite representing the action, or we need a way to determine if it's damage or movement.
            UpdateSprite(ability1Image, null);
            UpdateText(ability1Name, string.Empty);
            UpdateText(ability1Description, string.Empty);

            UpdateSprite(ability2Image, null);
            UpdateText(ability2Name, string.Empty);
            UpdateText(ability2Description, string.Empty);

            return true;
        }

        return false;
    }

    public void SelectAbility(bool ability)
    {
        firstIsSelected = ability;

        if(firstIsSelected)
        {
            ability1Background.color = Color.green;
        }

        ability1Background.color = firstIsSelected ? Color.white : Color.grey;
        ability2Background.color = firstIsSelected ? Color.grey : Color.white;
    }
}
