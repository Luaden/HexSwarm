using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSliderUpdater : MonoBehaviour
{
    [SerializeField] protected Slider cameraSensitivity;
    [SerializeField] protected Slider cameraSpeed;

    protected CameraScroll cameraScroller;


    public void UpdateSensitivity() => cameraScroller.sensitivityModifier = cameraSensitivity.value;
    public void UpdateSpeed() => cameraScroller.speedModifier = cameraSpeed.value;



    protected void Awake() => cameraScroller = FindObjectOfType<CameraScroll>();
    protected void Start() => InitSliders();

    protected void InitSliders()
    {
        cameraSensitivity.value = cameraScroller.sensitivityModifier;
        cameraSpeed.value = cameraScroller.speedModifier;
    }
}
