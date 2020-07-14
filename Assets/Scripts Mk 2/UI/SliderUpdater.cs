using UnityEngine;
using UnityEngine.UI;


public class SliderUpdater : MonoBehaviour
{
    [SerializeField] protected Slider masterVolumeSlider;
    [SerializeField] protected Slider bgmVolumeSlider;
    [SerializeField] protected Slider sfxVolumeSlider;
    [SerializeField] protected Slider cameraSensitivitySlider;
    [SerializeField] protected Slider cameraSpeedSlider;

    protected ConfigManager configManager;

    public void UpdateMasterVolume() => configManager.MasterVolume = masterVolumeSlider.value;
    public void UpdateSFXVolume() => configManager.SFXVolume = sfxVolumeSlider.value;
    public void UpdateBGMVolume() => configManager.BGMVolume = bgmVolumeSlider.value;
    public void UpdateSensitivity() => configManager.SensitivityModifier = cameraSensitivitySlider.value;
    public void UpdateSpeed() => configManager.SpeedModifier = cameraSpeedSlider.value;

    protected void Awake() => configManager = FindObjectOfType<ConfigManager>();

    protected void Start()
    {
        sfxVolumeSlider.value = configManager.SFXVolume;
        bgmVolumeSlider.value = configManager.BGMVolume;
        cameraSensitivitySlider.value = configManager.SensitivityModifier;
        cameraSpeedSlider.value = configManager.SpeedModifier;
    }
}
