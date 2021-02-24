using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class GRHHubWorld_SceneManager : MonoBehaviour
{
    // Loads ballon game scene
    public void StartBallonGame()
    {
        SceneManager.LoadScene("BallonGame");
        Debug.Log("BallonGame");
    }

    // Loads counting game scene
    public void StartCountingGame()
    {
        SceneManager.LoadScene("CountingGame");
        Debug.Log("CountingGame");
    }

    // Loads space game scene
    public void StartSpaceGame()
    {
        SceneManager.LoadScene("SpaceGame");
        Debug.Log("SpaceGame");
    }

    // Remove all but "Application.Quit();" before building as it will crash
    public void QuitApplication()
    {
        // Closes game if in editor mode
        if (UnityEditor.EditorApplication.isPlaying == true)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            // Closes game if in build mode
            Application.Quit();
        }
    }
}
