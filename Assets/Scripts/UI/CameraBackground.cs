using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBackground : MonoBehaviour
{
    protected Transform cameraTransform;
    protected Vector3 positionToBe;

    protected void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        positionToBe.x = cameraTransform.position.x;
        positionToBe.y = cameraTransform.position.y;
        positionToBe.z = transform.position.z;

        transform.position = positionToBe; 
    }
}
