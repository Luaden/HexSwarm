using UnityEngine;

public interface IAudioManager
{
    float SFXVolume { get; set; }
    float BGMVolume { get; set; }
    void PlaySound(AudioClip sfx);
    void PlayMusic(AudioClip sfx);
}
