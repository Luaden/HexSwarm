using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectedUnitPanel : CoreUIElement<IUnit>
{
    [SerializeField] protected Image unitImage;
    [SerializeField] protected Image unitImageBackground;
    [SerializeField] protected TMP_Text unitName;
    [SerializeField] protected TMP_Text unitDescription;

    [SerializeField] protected Image ability1Image;
    [SerializeField] protected Image ability1ImageBackground;
    [SerializeField] protected Button ability1Button;
    [SerializeField] protected TMP_Text ability1Name;
    [SerializeField] protected TMP_Text ability1Description;

    [SerializeField] protected Image ability2Image;
    [SerializeField] protected Image ability2ImageBackground;
    [SerializeField] protected Button ability2Button;
    [SerializeField] protected TMP_Text ability2Name;
    [SerializeField] protected TMP_Text ability2Description;

    [SerializeField] protected Image ability3Image;
    [SerializeField] protected Image ability3ImageBackground;
    [SerializeField] protected Button ability3Button;
    [SerializeField] protected TMP_Text ability3Name;
    [SerializeField] protected TMP_Text ability3Description;

    [SerializeField] protected Image ability4Image;
    [SerializeField] protected Image ability4ImageBackground;
    [SerializeField] protected Button ability4Button;
    [SerializeField] protected TMP_Text ability4Name;
    [SerializeField] protected TMP_Text ability4Description;

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

        UpdateSprite(ability1Image, unit.Abilites[0].Icon);
        UpdateText(ability1Name, unit.Abilites[0].Name);
        UpdateText(ability1Description, unit.Abilites[0].Description);

        UpdateSprite(ability2Image, unit.Abilites[1].Icon);
        UpdateText(ability2Name, unit.Abilites[1].Name);
        UpdateText(ability2Description, unit.Abilites[1].Description);

        if (unit.Abilites.Count < 3)
        {
            ability3Button.gameObject.SetActive(false);
            ability4Button.gameObject.SetActive(false);
        }

        if (unit.Abilites.Count >= 3)
        {
            ability3Button.gameObject.SetActive(true);

            UpdateSprite(ability3Image, unit.Abilites[1].Icon);
            UpdateText(ability3Name, unit.Abilites[1].Name);
            UpdateText(ability3Description, unit.Abilites[1].Description);
        }

        if (unit.Abilites.Count == 4)
        {
            ability4Button.gameObject.SetActive(true);

            UpdateSprite(ability4Image, unit.Abilites[1].Icon);
            UpdateText(ability4Name, unit.Abilites[1].Name);
            UpdateText(ability4Description, unit.Abilites[1].Description);
        }

    }

    protected override bool ClearedIfEmpty(IUnit unit)
    {
        if (unit == null)
        {
            UpdateSprite(unitImage, null);
            UpdateText(unitName, string.Empty);
            UpdateText(unitDescription, string.Empty);

            UpdateSprite(ability1Image, null);
            UpdateText(ability1Name, string.Empty);
            UpdateText(ability1Description, string.Empty);

            UpdateSprite(ability2Image, null);
            UpdateText(ability2Name, string.Empty);
            UpdateText(ability2Description, string.Empty);

            UpdateSprite(ability3Image, null);
            UpdateText(ability3Name, string.Empty);
            UpdateText(ability3Description, string.Empty);

            UpdateSprite(ability3Image, null);
            UpdateText(ability3Name, string.Empty);
            UpdateText(ability3Description, string.Empty);

            return true;
        }

        return false;
    }

    public void SelectAbility(bool ability)
    {
        firstIsSelected = ability;

        if (firstIsSelected)
        {
            ability1ImageBackground.color = Color.green;
        }

        ability1ImageBackground.color = firstIsSelected ? Color.white : Color.grey;
        ability2ImageBackground.color = firstIsSelected ? Color.grey : Color.white;
    }

    public void UpdateSelectedAbility(int x)
    {
        gameManager.SelectedAbility = gameManager.DisplayedUnit.Abilites[x];
    }
}

