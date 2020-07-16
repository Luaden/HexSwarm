using UnityEngine;

public interface IAudioControls
{
    float MasterVolume { get; }
    float SFXVolume { get; }
    float BGMVolume { get; }
    void PlaySound(AudioClip sfx);
    void PlayMusic(AudioClip bgm);
}
