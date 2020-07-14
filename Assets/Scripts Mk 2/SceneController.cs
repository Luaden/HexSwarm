using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    protected int currentScene;

    protected void Awake()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(currentScene++);    
    }

    public void LoadTitleScene()
    {
        SceneManager.LoadScene(0);
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(currentScene);
    }
}
