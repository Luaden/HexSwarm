using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class SelectedUnitPanel : CoreUIElement<IUnit>, IButtonIndexer
{
    [SerializeField] protected Image unitImage;
    [SerializeField] protected Image unitImageBackground;
    [SerializeField] protected TMP_Text unitName;
    [SerializeField] protected TMP_Text unitDescription;
    [SerializeField] protected GameManager gm;

    [SerializeField] protected List<AbilityDisplay> Abilites;
    [SerializeField] protected int lastSelected;
    public int LastSelected => lastSelected;
    public void Start()
    {
        for (int i = 0; i < Abilites.Count; i++)
            Abilites[i].Init(false, i, this);
    }

    public override void UpdateUI(IUnit unit)
    {
        if (ClearedIfEmpty(unit))
            return;

        UpdateSprite(unitImage, unit.Icon);
        unitImage.color = GameManager.ConfigManager.TeamColors[unit.Team.TeamNumber].PrimaryColor;
        UpdateText(unitName, unit.Name);
        UpdateText(unitDescription, unit.Description);
        foreach (AbilityDisplay ablitydisplay in Abilites)
            ablitydisplay.UpdateUI(unit.Abilites.ElementAtOrDefault(ablitydisplay.Index));
        Buttonclicked(-1);
    }

    protected override bool ClearedIfEmpty(IUnit unit)
    {
        if (unit != default)
            return false;

        UpdateSprite(unitImage, null);
        UpdateText(unitName, string.Empty);
        UpdateText(unitDescription, string.Empty);

        foreach (AbilityDisplay ablitydisplay in Abilites)
            ablitydisplay.UpdateUI(default);

        return true;
    }

    public void UpdateSelectedAbility(int x)
    {
        if (lastSelected > -1)
            Abilites[lastSelected].IsSelected = false;
        lastSelected = x;
        if (lastSelected > -1)
            Abilites[lastSelected].IsSelected = true;
        gm.AbilitySelected();

    }
    public void Buttonclicked(int index) => UpdateSelectedAbility(index);
}

