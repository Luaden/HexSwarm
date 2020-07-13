
using UnityEngine;
using UnityEngine.UI;

public class SliderUpdater : MonoBehaviour
{
    [SerializeField] protected Slider bgmVolumeSlider;
    [SerializeField] protected Slider sfxVolumeSlider;
    [SerializeField] protected Slider cameraSensitivitySlider;
    [SerializeField] protected Slider cameraSpeedSlider;

    protected AudioManager audioManager;
    protected CameraScroller cameraScroller;

    public void UpdateSFXVolume() => audioManager.SFXVolume = sfxVolumeSlider.value;
    public void UpdateBGMVolume() => audioManager.BGMVolume = bgmVolumeSlider.value;
    public void UpdateSensitivity() => cameraScroller.SensitivityModifier = cameraSensitivitySlider.value;
    public void UpdateSpeed() => cameraScroller.SpeedModifier = cameraSpeedSlider.value;

    protected void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        cameraScroller = FindObjectOfType<CameraScroller>();
    }

    protected void Start() => InitSliders();

    protected void InitSliders()
    {
        sfxVolumeSlider.value = audioManager.SFXVolume;
        bgmVolumeSlider.value = audioManager.BGMVolume;
        cameraSensitivitySlider.value = cameraScroller.SensitivityModifier;
        cameraSpeedSlider.value = cameraScroller.SpeedModifier;        
    }
}
