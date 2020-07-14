using UnityEngine;

public interface ICameraControls
{
    float SensitivityModifier { get; set; }
    float SpeedModifier { get; set; }
    void ToggleCameraControls(bool cameraControlOnOff);
    void RepositionCamera(Vector3Int cameraPosition);
}
