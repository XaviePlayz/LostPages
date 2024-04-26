using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    public GameObject homeScreen, optionsScreen;
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Options()
    {
        homeScreen.SetActive(false);
        optionsScreen.SetActive(true);

    }

    public void Return()
    {
        homeScreen.SetActive(true);
        optionsScreen.SetActive(false);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
