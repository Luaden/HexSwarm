using UnityEngine;

public class AudioController : MonoBehaviour
{
    protected AudioSource[] audioSources;
    protected AudioSource sfxAudioSource;
    protected AudioSource bgmAudioSource;
    protected ConfigManager configManager;
    protected float masterVolume = 1;
    protected float sfxAudioVolume = 1f;
    protected float bgmAudioVolume = 1f;

    public float MasterVolume { set => UpdateMasterVolume(value); }
    public float SFXVolume { set => UpdateSFXVolume(value); }
    public float BGMVolume { set => UpdateBGMVolume(value); }

    public void PlaySound(AudioClip clipToPlay) => sfxAudioSource.PlayOneShot(clipToPlay);
    public void PlayMusic(AudioClip clipToPlay)
    {
        bgmAudioSource.clip = clipToPlay;
        bgmAudioSource.Play();
    }

    protected void Awake()
    {
        configManager = FindObjectOfType<ConfigManager>();
        audioSources = GetComponents<AudioSource>();
        sfxAudioSource = audioSources[0];
        bgmAudioSource = audioSources[1];

        configManager.AudioController = this;
    }

    protected void Start()
    {
        MasterVolume = configManager.MasterVolume;
        SFXVolume = configManager.SFXVolume;
        BGMVolume = configManager.BGMVolume;
    }
    
    protected void UpdateMasterVolume(float value)
    {
        masterVolume = value;
        sfxAudioSource.volume = sfxAudioVolume * masterVolume;
        bgmAudioSource.volume = bgmAudioVolume * masterVolume;
    }

    protected void UpdateSFXVolume(float value)
    {
        sfxAudioVolume = value;
        sfxAudioSource.volume = sfxAudioVolume * masterVolume;
    }

    protected void UpdateBGMVolume(float value)
    {
        bgmAudioVolume = value;
        bgmAudioSource.volume = bgmAudioVolume * masterVolume;
    }
}
