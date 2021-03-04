using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class GRHHubWorld_SceneManager : MonoBehaviour
{
    // Initial framework - Adam

    [SerializeField] private GameObject difficultySettingPanel;

    public GRHBalloonMG_GameDifficulty gameDifficulty;

    private string gameSelected;

    private void Awake()
    {
        difficultySettingPanel.SetActive(false);
    }

    // Button to start balloon game
    public void StartBallonGame()
    {
        difficultySettingPanel.SetActive(true);
        gameSelected = "GRHBalloonMG_Scene";
    }

    // Button to start counting game
    public void StartCountingGame()
    {
        difficultySettingPanel.SetActive(true);
        gameSelected = "GRHCountingMG_Scene";
    }

    // Button to start space game
    public void StartSpaceGame()
    {
        difficultySettingPanel.SetActive(true);
        gameSelected = "GRHSpaceMG_Scene";
    }

    // "X" button to close the difficulty panel
    public void CloseDifficultyPanel()
    {
        difficultySettingPanel.SetActive(false);
    }

    // Selects game difficulty parses and makes it ToUpper so no mistakes can be made (EASY, MEDIUM, HARD)
    public void SelectDifficultySetting(string gameDifficulty)
    {
        try
        {
            this.gameDifficulty = (GRHBalloonMG_GameDifficulty)Enum.Parse(typeof(GRHBalloonMG_GameDifficulty), gameDifficulty.ToUpper());
        }
        catch (Exception)
        {
            Debug.LogErrorFormat("Parse: Can't convert {0} to enum, please check spell.", gameDifficulty);
        }
        SceneManager.LoadScene(gameSelected);

        // Debugs the name of the scene loading and the difficulty (Easy, Medium or Hard)
        Debug.Log("Scene: " + SceneManager.GetSceneByName(gameSelected).name + "\n" + "Difficulty: " + gameDifficulty);
    }

    // Remove all but "Application.Quit();" before building as it will crash if left in during build
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
