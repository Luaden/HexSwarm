using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroller : MonoBehaviour
{
    [Range(.5f, 1.5f)] [SerializeField] protected float cameraSensitivity;
    [Range(.5f, 1.5f)] [SerializeField] protected float cameraSpeed;

    //Cached references
    protected Camera mainCamera;
    protected Transform cameraTransform;
    protected MenuSwitcher menuSwitcher;

    protected float xMax = 4;
    protected float yMax = 2;
    protected Vector3 mousePos;
    protected Vector3 cameraPosToBe;    

    public float SensitivityModifier { get => cameraSensitivity; set => cameraSensitivity = value; }
    public float SpeedModifier { get => cameraSpeed; set => cameraSpeed = value; }

    protected void Awake()
    {
        mainCamera = Camera.main;
        cameraTransform = mainCamera.transform;
        menuSwitcher = FindObjectOfType<MenuSwitcher>();
    }

    protected void OnLevelWasLoaded(int level)
    {
        mainCamera = Camera.main;
        cameraTransform = mainCamera.transform;
    }

    protected void Update()
    {
        if (!menuSwitcher.OptionsActive) 
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
            mousePos.y > mainCamera.transform.position.y + yMax  / SensitivityModifier ||
            mousePos.y < mainCamera.transform.position.y - yMax  / SensitivityModifier)
        {            
            mainCamera.transform.position = Vector3.Lerp(transform.position, cameraPosToBe, (cameraSpeed * SpeedModifier) * Time.deltaTime);
        }
    }
}
