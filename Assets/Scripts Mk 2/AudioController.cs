using UnityEngine;

public class AudioController : MonoBehaviour, IAudioControls
{
    protected AudioSource[] audioSources;
    protected AudioSource sfxAudioSource;
    protected AudioSource bgmAudioSource;

    public float SFXVolume { get => sfxAudioSource.volume; set => sfxAudioSource.volume = value; }
    public float BGMVolume { get => bgmAudioSource.volume; set => bgmAudioSource.volume = value; }

    public void PlaySound(AudioClip clipToPlay) => sfxAudioSource.PlayOneShot(clipToPlay);
    public void PlayMusic(AudioClip clipToPlay)
    {
        bgmAudioSource.clip = clipToPlay;
        bgmAudioSource.Play();
    }

    protected void Awake()
    {
        audioSources = GetComponents<AudioSource>();
        sfxAudioSource = audioSources[0];
        bgmAudioSource = audioSources[1];
    }
}
