using UnityEngine;

public class AudioController : MonoBehaviour
{
    protected AudioSource[] audioSources;
    protected AudioSource sfxAudioSource;
    protected AudioSource bgmAudioSource;
    protected ConfigManager configManager;

    public float SFXVolume { set => sfxAudioSource.volume = value; }
    public float BGMVolume { set => bgmAudioSource.volume = value; }

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
        SFXVolume = configManager.SFXVolume;
        BGMVolume = configManager.BGMVolume;
    }
}
