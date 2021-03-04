using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class GRHHubWorld_SceneManager : MonoBehaviour
{
    // Initial framework - Adam

    [SerializeField] private GameObject difficultySettingPanel;
    private string gameSelected;

    GRHGameSettings gameSettings;

    private void Awake()
    {
        gameSettings = GameObject.FindObjectOfType<GRHGameSettings>();
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

    // Selects game difficulty (EASY, MEDIUM, HARD)
    public void SelectDifficultySetting(string gameDifficulty)
    {
        gameSettings.gameDifficulty = gameDifficulty;
        SceneManager.LoadScene(gameSelected);

        Debug.Log("Game Loaded: " + gameSelected);
        Debug.Log("Difficulty: " + gameDifficulty);
    }

    public void TogglePumpCount()
    {
        if (!gameSettings.showPumpCount)
        {
            gameSettings.showPumpCount = true;
        }
        else
        {
            gameSettings.showPumpCount = false;
        }
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
