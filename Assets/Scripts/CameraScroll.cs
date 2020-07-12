using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroll : MonoBehaviour
{
    [Range(.5f, 1.5f)] [SerializeField] protected float cameraSensitivity;
    [Range(.1f, 3f)] [SerializeField] protected float cameraSpeed;


    protected float xMax = 4;
    protected float modifiedXMax;
    protected float yMax = 2;
    protected float modifiedYMax;
    protected Camera mainCamera;
    protected Vector3 mousePos;
    protected Vector3 cameraPosToBe;
    protected Vector2 maxDistanceFromMouse;

    public float sensitivityModifier { get; set; }
    public float speedModifier { get; set; }

    protected void Awake()
    {
        mainCamera = Camera.main;
        maxDistanceFromMouse.x = xMax;
        maxDistanceFromMouse.y = yMax;
    }

    protected void Update() => CameraMovement();

    void CameraMovement()
    {
        modifiedXMax = xMax * sensitivityModifier;
        modifiedYMax = yMax * sensitivityModifier;

        mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);            

        cameraPosToBe.x = mousePos.x;
        cameraPosToBe.y = mousePos.y;
        cameraPosToBe.z = transform.position.z; 

        if (mousePos.x > transform.position.x + modifiedXMax ||
            mousePos.x < transform.position.x - modifiedXMax ||
            mousePos.y > transform.position.y + modifiedYMax ||
            mousePos.y < transform.position.y - modifiedYMax)
        {            
            transform.position = Vector3.Lerp(transform.position, cameraPosToBe, cameraSpeed * speedModifier * Time.deltaTime);
        }
    }
}
