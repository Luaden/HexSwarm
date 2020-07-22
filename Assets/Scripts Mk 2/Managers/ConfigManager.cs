using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : MonoBehaviour, IConfigManager
{
    protected ColorConfig[] teamColors;
    protected KeyCode scrollLeft = KeyCode.A;
    protected KeyCode scrollRight = KeyCode.D;
    protected KeyCode scrollUp = KeyCode.W;
    protected KeyCode scrollDown = KeyCode.S;
    protected KeyCode ability1 = KeyCode.Alpha1;
    protected KeyCode ability2 = KeyCode.Alpha2;
    protected KeyCode ability3 = KeyCode.Alpha3;
    protected KeyCode ability4 = KeyCode.Alpha4;
    protected float gameDifficulty = 1f;
    protected float masterVolume = 1f;
    protected float sfxVolume = 1f;
    protected float bgmVolume = 1f;
    protected float sensitivityModifier = 1f;
    protected float speedModifier = 1f;
    protected MapShape mapShape = MapShape.Hexagon;

    public static ConfigManager instance;
    public KeyCode ScrollLeft => scrollLeft;
    public KeyCode ScrollRight => scrollRight;
    public KeyCode ScrollUp => scrollUp;
    public KeyCode ScrollDown => scrollDown;
    public KeyCode Ability1 => ability1;
    public KeyCode Ability2 => ability2;
    public KeyCode Ability3 => ability3;
    public KeyCode Ability4 => ability4;
    public float GameDifficulty { get => gameDifficulty; set => gameDifficulty = value; }
    public IColorConfig[] TeamColors { get => teamColors; }
    public AudioController AudioController { get; set; }
    public CameraController CameraController { get; set; }
    public MapShape MapShape { get; set; }

    public float MasterVolume
    {
        get => masterVolume;
        set => UpdateMasterVolume(value);
    }
    public float SFXVolume
    {
        get => sfxVolume;
        set => UpdateSFXVolume(value);
    }
    public float BGMVolume
    {
        get => bgmVolume;
        set => UpdateBGMVolume(value);
    }
    public float SensitivityModifier
    {
        get => sensitivityModifier;
        set => UpdateSensitivityModifier(value);
    }

    public float SpeedModifier
    {
        get => speedModifier;
        set => UpdateSpeedModifier(value);
    }

    

    //This needs to be expanded to call to the team through the game manager and update colors.
    public void ChangeTeamColor(int teamIndex, IColorConfig colors) => TeamColors[teamIndex] = colors;
    public void PlayMusic(AudioClip bgm) => AudioController.PlayMusic(bgm);
    public void PlaySound(AudioClip sfx) => AudioController.PlaySound(sfx);
    public void RepositionCamera(Vector3Int cameraPosition) => CameraController.RepositionCamera(cameraPosition);
    public void ToggleCameraControls(bool cameraControlOnOff) => CameraController.ToggleCameraControls(cameraControlOnOff);

    protected void Awake()
    {
        #region Singleton
        if (instance != null)
            Destroy(this.gameObject);

        instance = this;
        DontDestroyOnLoad(this);
        #endregion
        
        InitTeamColors();
    }

    protected void UpdateMasterVolume(float value)
    {
        masterVolume = value;
        AudioController.MasterVolume = value;
    }

    protected void UpdateSFXVolume(float value)
    {
        sfxVolume = value;
        AudioController.SFXVolume = value;
    }

    protected void UpdateBGMVolume(float value)
    {
        bgmVolume = value;
        AudioController.BGMVolume = value;
    }
    protected void UpdateSensitivityModifier(float value)
    {
        sensitivityModifier = value;
        CameraController.SensitivityModifier = value;
    }
    protected void UpdateSpeedModifier(float value)
    {
        speedModifier = value;
        CameraController.SpeedModifier = value;
    }

    protected void InitTeamColors()
    {
        teamColors = new ColorConfig[9];

        for(int i = 0; i < teamColors.Length; i++)
        {
            ColorConfig config = new ColorConfig();
            config.TeamNumber = (Teams)i;
            config.PrimaryColor = config.GetColor((Colors)i);
            config.PrimaryColorCategory = (Colors)i;
            config.SecondaryColor = config.GetColor((Colors)i);
            config.SecondaryColorCategory = (Colors)i;

            teamColors[i] = config;
        }
    }

    protected void OnLevelWasLoaded(int level)
    {
        CameraController = FindObjectOfType<CameraController>();
        AudioController = FindObjectOfType<AudioController>();
    }
}
