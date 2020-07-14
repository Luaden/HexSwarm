using UnityEngine;

public interface IAudioControls
{
    float SFXVolume { get; set; }
    float BGMVolume { get; set; }
    void PlaySound(AudioClip sfx);
    void PlayMusic(AudioClip bgm);
}
