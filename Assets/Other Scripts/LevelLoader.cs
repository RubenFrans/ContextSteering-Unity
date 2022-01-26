using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{


    public void LoadLevel(int levelId)
    {
        SceneManager.LoadScene(levelId);
    }

    public void QuitGame()
    {
        Application.Quit();
    }


}
