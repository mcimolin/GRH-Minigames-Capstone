using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GRHHubWorld_SceneManager : MonoBehaviour
{
    // Initial framework - Adam

    [SerializeField] private GameObject difficultySettingPanel;

    [SerializeField] private Button easyButton;
    [SerializeField] private Button mediumButton;
    [SerializeField] private Button hardButton;

    private string gameSelected;

    GRHGameSettings gameSettings;

    private void Awake()
    {
        gameSettings = GameObject.FindObjectOfType<GRHGameSettings>();
        difficultySettingPanel.SetActive(false);

        easyButton.enabled = false;
        mediumButton.enabled = false;
        hardButton.enabled = false;
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
        Debug.Log("Player Character: " + gameSettings.selectedCharacter);
        Debug.Log("Show pump count: " + gameSettings.showPumpCount);
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

    public void ToggleCharacterSelect(int selectedCharacter)
    {
        switch (selectedCharacter)
        {
            // Popcorn #1
            case 1:
                gameSettings.selectedCharacter = 1;
                break;
            // Corndog #2
            case 2:
                gameSettings.selectedCharacter = 2;
                break;
            // Hotdog #3
            case 3:
                gameSettings.selectedCharacter = 3;
                break;
            // Donut #4
            case 4:
                gameSettings.selectedCharacter = 4;
                break;
            // Cotton Candy #5
            case 5:
                gameSettings.selectedCharacter = 5;
                break;
            // Use Popcorn by default
            default:
                gameSettings.selectedCharacter = 1;
                break;
        }

        easyButton.enabled = true;
        mediumButton.enabled = true;
        hardButton.enabled = true;
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
