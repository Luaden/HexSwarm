using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SliderUpdater : MonoBehaviour
{
    [SerializeField] protected Slider masterVolumeSlider;
    [SerializeField] protected Slider bgmVolumeSlider;
    [SerializeField] protected Slider sfxVolumeSlider;
    [SerializeField] protected Slider cameraSensitivitySlider;
    [SerializeField] protected Slider cameraSpeedSlider;
    [SerializeField] protected Slider gameDifficultySlider;
    [SerializeField] protected TMP_Dropdown mapShape;
    [SerializeField] protected TMP_Dropdown teamNumber;
    [SerializeField] protected TMP_Dropdown primaryColor;
    [SerializeField] protected TMP_Dropdown secondaryColor;
    

    protected ConfigManager configManager;

    public void UpdateTeamColors() => configManager.ChangeTeamColor((Teams)teamNumber.value, GetColors());
    public void UpdateDifficulty() => configManager.GameDifficulty = masterVolumeSlider.value;
    public void UpdateMapShape() => configManager.MapShape = (MapShape)mapShape.value;
    public void UpdateMasterVolume() => configManager.MasterVolume = masterVolumeSlider.value;
    public void UpdateSFXVolume() => configManager.SFXVolume = sfxVolumeSlider.value;
    public void UpdateBGMVolume() => configManager.BGMVolume = bgmVolumeSlider.value;
    public void UpdateSensitivity() => configManager.SensitivityModifier = cameraSensitivitySlider.value;
    public void UpdateSpeed() => configManager.SpeedModifier = cameraSpeedSlider.value;

    protected void Awake() => configManager = FindObjectOfType<ConfigManager>();

    protected void Start()
    {
        gameDifficultySlider.value = configManager.GameDifficulty;
        masterVolumeSlider.value = configManager.MasterVolume;
        sfxVolumeSlider.value = configManager.SFXVolume;
        bgmVolumeSlider.value = configManager.BGMVolume;
        cameraSensitivitySlider.value = configManager.SensitivityModifier;
        cameraSpeedSlider.value = configManager.SpeedModifier;
    }

    protected ColorConfig GetColors()
    {
        ColorConfig colorConfig = new ColorConfig();
        colorConfig.PrimaryColor = colorConfig.GetColor((Colors)primaryColor.value);

        colorConfig.PrimaryColorCategory = (Colors)primaryColor.value;

        return colorConfig; 
    }

    public void GetTeamColors()
    {
        ColorConfig config = configManager.TeamColors[(Teams)teamNumber.value];
        
        primaryColor.value = (int)config.PrimaryColorCategory;

        primaryColor.RefreshShownValue();
        secondaryColor.RefreshShownValue();
    }
}
