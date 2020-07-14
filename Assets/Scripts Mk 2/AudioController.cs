using UnityEngine;

public class AudioController : MonoBehaviour
{
    protected AudioSource[] audioSources;
    protected AudioSource sfxAudioSource;
    protected AudioSource bgmAudioSource;
    protected ConfigManager configManager;
    protected float masterVolume;

    public float MasterVolume { set => masterVolume = value; }
    public float SFXVolume { set => sfxAudioSource.volume = value * masterVolume; }
    public float BGMVolume { set => bgmAudioSource.volume = value * masterVolume; }

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
    }

    protected void Start()
    {
        MasterVolume = configManager.MasterVolume;
        SFXVolume = configManager.SFXVolume;
        BGMVolume = configManager.BGMVolume;
    }
}
