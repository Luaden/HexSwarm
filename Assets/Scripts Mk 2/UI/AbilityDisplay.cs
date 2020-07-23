using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityDisplay : CoreUIElement<IAbility>
{
    [SerializeField, Header("coreVisual")] protected Image forground;
    [SerializeField] protected TMP_Text displayName;
    [SerializeField] protected TMP_Text description;

    [SerializeField, Header("Selected")]  protected Image background;
    [SerializeField] protected int index;
    public int Index => index;
    [SerializeField] bool isSelected;
    IButtonIndexer buttonIndexer;

    public bool IsSelected
    {
        get => IsSelected;
        set => UpdateIsSelected(value);
    }

    public void WasClicked() { buttonIndexer?.Buttonclicked(index); }

    protected void UpdateIsSelected( bool selection)
    {
        if (isSelected == selection)
            return;
        isSelected = selection;
        forground.color = isSelected ? Color.white : Color.grey;
        background.color = isSelected ? Color.grey : Color.white;
    }

    public void Init(bool initialState, int index, IButtonIndexer indexer)
    {
        isSelected = true;
        IsSelected = !isSelected;
        this.index = index;
        buttonIndexer = indexer;
    }

    public override void UpdateUI(IAbility primaryData)
    {
        if (ClearedIfEmpty(primaryData))
            return;

        UpdateSprite(forground, primaryData.Icon);
        UpdateText(displayName, primaryData.Name);
        UpdateText(description, primaryData.Description);

        gameObject.SetActive(true);
    }

    protected override bool ClearedIfEmpty(IAbility newData)
    {
        if (newData != default)
            return false;

        gameObject.SetActive(false);
        return true;
    }
}
