using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Cached references
    protected Camera mainCamera;
    protected Transform cameraTransform;
    protected ConfigManager configManager;

    //State variables
    protected float xMax = 4;
    protected float yMax = 2;
    protected float sensitivityModifier = 1f;
    protected float speedModifier = 1f;
    protected Vector3 mousePos;
    protected Vector3 cameraPosToBe;
    protected bool movementEnabled = true;

    public float SensitivityModifier { set => sensitivityModifier = value; }
    public float SpeedModifier { set => speedModifier = value; }

    public void ToggleCameraControls(bool cameraControlOnOff) => movementEnabled = cameraControlOnOff;

    protected void Awake()
    {
        configManager = FindObjectOfType<ConfigManager>();        
        mainCamera = Camera.main;
        cameraTransform = mainCamera.transform;

        configManager.CameraController = this;
        SensitivityModifier = configManager.SensitivityModifier; ;
        SpeedModifier = configManager.SpeedModifier;
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

        if (mousePos.x > mainCamera.transform.position.x + xMax / sensitivityModifier ||
            mousePos.x < mainCamera.transform.position.x - xMax / sensitivityModifier ||
            mousePos.y > mainCamera.transform.position.y + yMax / sensitivityModifier ||
            mousePos.y < mainCamera.transform.position.y - yMax / sensitivityModifier)
        {
            mainCamera.transform.position = Vector3.Lerp(transform.position, cameraPosToBe, speedModifier * Time.deltaTime);
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
