using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : MonoBehaviour, IConfigManager
{
    [SerializeField] protected IColorConfig[] teamColors;
    protected KeyCode scrollLeft = KeyCode.A;
    protected KeyCode scrollRight = KeyCode.D;
    protected KeyCode scrollUp = KeyCode.W;
    protected KeyCode scrollDown = KeyCode.S;
    protected KeyCode ability1 = KeyCode.Alpha1;
    protected KeyCode ability2 = KeyCode.Alpha2;
    protected KeyCode ability3 = KeyCode.Alpha3;
    protected KeyCode ability4 = KeyCode.Alpha4;
    
    public KeyCode ScrollLeft => scrollLeft;
    public KeyCode ScrollRight => scrollRight;
    public KeyCode ScrollUp => scrollUp;
    public KeyCode ScrollDown => scrollDown;
    public KeyCode Ability1 => ability1;
    public KeyCode Ability2 => ability2;
    public KeyCode Ability3 => ability3;
    public KeyCode Ability4 => ability4;
    public float SFXVolume { get; set; }
    public float BGMVolume { get; set; }
    public float SensitivityModifier { get; set; }
    public float SpeedModifier { get; set; }
    public float GameDifficulty { get; set; }
    public MapShape MapShape { get; set; }
    public IColorConfig[] TeamColors { get => teamColors; }
    public AudioController AudioController { get; set; }
    public CameraController CameraController { get; set; }

    public void ChangeTeamColor(int teamIndex, IColorConfig colors) => TeamColors[teamIndex] = colors;
    public void PlayMusic(AudioClip bgm) => AudioController.PlayMusic(bgm);
    public void PlaySound(AudioClip sfx) => AudioController.PlaySound(sfx);
    public void RepositionCamera(Vector3Int cameraPosition) => CameraController.RepositionCamera(cameraPosition);
    public void ToggleCameraControls(bool cameraControlOnOff) => CameraController.ToggleCameraControls(cameraControlOnOff);
}
