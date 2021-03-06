using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GRHHubWorld_SceneManager : MonoBehaviour
{
    // Initial framework - Adam

    [SerializeField] private GameObject difficultySettingPanel;
    [SerializeField] private Dropdown characterSelector;
    [SerializeField] private InputField pumpCount;
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

    private void Start()
    {
        //Sets the defaults for balloon minigame difficulty panel
        pumpCount.text = gameSettings.pumpCount.ToString();
    }

    // Button to start balloon game
    public void StartBalloonGame()
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

    //Sets whether the amount of pumps left are displayed on the balloon (Balloon Game)
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

    //Sets tpump count amount for the Balloon Minigame
    public void SetPumpCount()
    {
        if (string.IsNullOrEmpty(pumpCount.text))
        {
            pumpCount.text = "0";
        }

        try
        {
            if (int.Parse(pumpCount.text) >= 15) // The minimum amount of pumps entered is met
            {
                pumpCount.textComponent.color = Color.black;
                gameSettings.pumpCount = int.Parse(pumpCount.text);
                easyButton.enabled = true;
                mediumButton.enabled = true;
                hardButton.enabled = true;
            }
            else // The minimum amount is not met
            {
                pumpCount.textComponent.color = Color.red;

                easyButton.enabled = false;
                mediumButton.enabled = false;
                hardButton.enabled = false;
            }
        }
        catch (Exception)
        {
            Debug.LogError("Error in parsing integer: No value to parse.");
        }
        
        
    }

    // Sets the player's character preference [Added by Bryce]
    public void CharacterSelection()
    {
        gameSettings.selectedCharacter = characterSelector.value;
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
