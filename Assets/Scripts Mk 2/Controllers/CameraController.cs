using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Cached references
    protected Camera mainCamera;
    protected Transform cameraTransform;
    protected GameManager gameManager;

    //State variables
    protected float xMax = 8.5f;
    protected float yMax = 4.5f;
    protected float xBoundary;
    protected float yBoundary;
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
        mainCamera = Camera.main;
        gameManager = FindObjectOfType<GameManager>();
        cameraTransform = mainCamera.transform;               
    }

    protected void Start()
    {
        ConfigManager.instance.CameraController = this;
        SensitivityModifier = ConfigManager.instance.SensitivityModifier;
        SpeedModifier = ConfigManager.instance.SpeedModifier;
    }

    protected void Update()
    {
        if (!movementEnabled)
            CameraMovement();
    }

    protected void CameraMovement()
    {
        mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if(gameManager != null)
        {
            cameraPosToBe.x = Mathf.Clamp(mousePos.x, -gameManager.GridSize, gameManager.GridSize);
            cameraPosToBe.y = Mathf.Clamp(mousePos.y, -gameManager.GridSize, gameManager.GridSize);
            cameraPosToBe.z = mainCamera.transform.position.z;
        }
        else
        {
            cameraPosToBe.x = mousePos.x;
            cameraPosToBe.y = mousePos.y;
            cameraPosToBe.z = mainCamera.transform.position.z;
        }        

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
