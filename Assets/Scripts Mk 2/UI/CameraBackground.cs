using UnityEngine;

public class CameraBackground : MonoBehaviour
{
    [SerializeField] private float scrollSpeed;

    //Cached references
    protected Material cloudyMaterial;

    //State variables
    protected Vector2 offset;

    protected void Start()
    {
        cloudyMaterial = GetComponent<Renderer>().material;
        offset = new Vector2(scrollSpeed, 0f);
    }

    protected void Update() => ScrollBackground();

    protected void ScrollBackground() => cloudyMaterial.mainTextureOffset += offset * Time.deltaTime;
}
