
using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderUpdater : MonoBehaviour
{
    [SerializeField] protected Slider bgmVolumeSlider;
    [SerializeField] protected Slider sfxVolumeSlider;



    protected AudioManager audioManager;


    public void UpdateSFXVolume() => audioManager.SFXVolume = sfxVolumeSlider.value;
    public void UpdateBGMVolume() => audioManager.BGMVolume = bgmVolumeSlider.value;
    


    protected void Awake() => audioManager = FindObjectOfType<AudioManager>();
    protected void Start() => InitSliders();

    protected void InitSliders()
    {
        sfxVolumeSlider.value = audioManager.SFXVolume;
        bgmVolumeSlider.value = audioManager.BGMVolume;
    }
}
