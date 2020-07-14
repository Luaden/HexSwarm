using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConfigManager : IAudioControls, ICameraControls, IControlsConfig, IGameOptions
{
    AudioController AudioController { get; }
    CameraController CameraController { get; }
}
