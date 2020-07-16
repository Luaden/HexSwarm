using UnityEngine;

public interface ICameraControls
{
    float SensitivityModifier { get; }
    float SpeedModifier { get; }
    void ToggleCameraControls(bool cameraControlOnOff);
    void RepositionCamera(Vector3Int cameraPosition);
}
