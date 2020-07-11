using UnityEngine;


public class MenuSwitcher : MonoBehaviour
{
    [SerializeField] protected GameObject mainCanvas;
    [SerializeField] protected GameObject optionsCanvas;

    protected bool optionsActive = false;

    public void ToggleMenus()
    {
        optionsActive = !optionsActive;
        
        optionsCanvas.SetActive(optionsActive);
        mainCanvas.SetActive(!optionsActive);
    }
}
