using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartLevelByName(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
