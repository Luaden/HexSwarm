using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip[] bgmSongs;
    protected AudioSource[] audioSources;
    protected AudioSource sfxAudioSource;
    protected AudioSource bgmAudioSource;

    public static AudioManager instance;

    public float SFXVolume { get => sfxAudioSource.volume; set => sfxAudioSource.volume = value; }
    public float BGMVolume { get => bgmAudioSource.volume; set => bgmAudioSource.volume = value; }

    public void PlaySoundEffect(AudioClip clipToPlay) => sfxAudioSource.PlayOneShot(clipToPlay);

    protected void Awake()
    {
        #region Singleton
        if (instance != null)
            Destroy(this);

        instance = this;
        DontDestroyOnLoad(this);
        #endregion

        audioSources = GetComponents<AudioSource>();
        sfxAudioSource = audioSources[0];
        bgmAudioSource = audioSources[1];
    }

    protected void OnLevelWasLoaded(int level)
    {
        bgmAudioSource.clip = bgmSongs[level];
        bgmAudioSource.Play();
    }

    private void Start()
    {
        bgmAudioSource.clip = bgmSongs[0];
        bgmAudioSource.Play();
    }
}
