using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour, ICameraControls
{
    //Cached references
    protected Camera mainCamera;
    protected Transform cameraTransform;

    //State variables
    protected float xMax = 4;
    protected float yMax = 2;
    protected float cameraSensitivity = 1f;
    protected float cameraSpeed = 1f;
    protected Vector3 mousePos;
    protected Vector3 cameraPosToBe;
    protected bool movementEnabled = false;

    public float SensitivityModifier { get => cameraSensitivity; set => cameraSensitivity = value; }
    public float SpeedModifier { get => cameraSpeed; set => cameraSpeed = value; }

    public void ToggleCameraControls(bool cameraControlOnOff) => movementEnabled = cameraControlOnOff;

    protected void Awake()
    {
        mainCamera = Camera.main;
        cameraTransform = mainCamera.transform;
    }

    protected void Update()
    {
        if (!movementEnabled)
            CameraMovement();
    }

    protected void CameraMovement()
    {
        mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        cameraPosToBe.x = mousePos.x;
        cameraPosToBe.y = mousePos.y;
        cameraPosToBe.z = mainCamera.transform.position.z;

        if (mousePos.x > mainCamera.transform.position.x + xMax / SensitivityModifier ||
            mousePos.x < mainCamera.transform.position.x - xMax / SensitivityModifier ||
            mousePos.y > mainCamera.transform.position.y + yMax / SensitivityModifier ||
            mousePos.y < mainCamera.transform.position.y - yMax / SensitivityModifier)
        {
            mainCamera.transform.position = Vector3.Lerp(transform.position, cameraPosToBe, (cameraSpeed * SpeedModifier) * Time.deltaTime);
        }
    }

    public void RepositionCamera(Vector3Int cameraPosition)
    {
        cameraPosToBe.x = cameraPosition.x;
        cameraPosToBe.y = cameraPosition.y;
        cameraPosToBe.z = mainCamera.transform.position.z;

        mainCamera.transform.position = cameraPosToBe; 
    }
}
