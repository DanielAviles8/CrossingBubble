using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    int levelIndex = 1;

    public void LoadLevel1(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
