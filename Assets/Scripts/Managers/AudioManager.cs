using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip bgmSong;
    protected AudioSource[] audioSources;
    protected AudioSource sfxAudioSource;
    protected AudioSource bgmAudioSource;

    public float SFXVolume { get => sfxAudioSource.volume; set => sfxAudioSource.volume = value; }
    public float BGMVolume { get => bgmAudioSource.volume; set => bgmAudioSource.volume = value; }

    public void PlaySoundEffect(AudioClip clipToPlay) => sfxAudioSource.PlayOneShot(clipToPlay);

    protected void Awake()
    {
        audioSources = GetComponents<AudioSource>();
        sfxAudioSource = audioSources[0];
        bgmAudioSource = audioSources[1];
    }

    protected void Start()
    {
        bgmAudioSource.clip = bgmSong;
        bgmAudioSource.Play();
    }
}
