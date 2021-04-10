using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class GRHHubWorld_SceneManager : MonoBehaviour
{
    // Initial framework - Adam

    [SerializeField] private GameObject difficultySettingPanelBalloonGame, difficultySettingPanelCountingGame;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private Dropdown characterSelector;
    [SerializeField] private InputField pumpCount;
    [SerializeField] private Button easyButton;
    [SerializeField] private Button mediumButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Slider opponentLevelSlider, timeSlider, objectSpeedSlider, objectScaleSlider, objectCountSlider;
    [SerializeField] private GRHLoadingScreen loadingScreen;

    private string gameSelected;

    GRHGameSettings gameSettings;
    GRHBalloonMG_SoundManager soundManager;

    bool creditsToggle;

    private void Awake()
    {
        gameSettings = FindObjectOfType<GRHGameSettings>();
        soundManager = FindObjectOfType<GRHBalloonMG_SoundManager>();
        loadingScreen = GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<GRHLoadingScreen>();
        difficultySettingPanelBalloonGame.SetActive(false);
        difficultySettingPanelCountingGame.SetActive(false);
        creditsPanel.SetActive(false);

        easyButton.enabled = false;
        mediumButton.enabled = false;
        hardButton.enabled = false;

        creditsToggle = false;
    }

    private void Start()
    {
        //Sets the defaults for balloon minigame difficulty panel
        pumpCount.text = gameSettings.pumpCount.ToString();
        soundManager.HubWorldGameMusic();
    }

    // Button to start balloon game
    public void StartBalloonGame()
    {
        difficultySettingPanelBalloonGame.SetActive(true);
        gameSelected = "GRHBalloonMG_Scene";
    }

    // Button to start counting game
    public void StartCountingGame()
    {
        difficultySettingPanelCountingGame.SetActive(true);
        gameSelected = "GRHCountingMG_Scene";
    }

    // Button to start space game
    public void StartSpaceGame()
    {
        difficultySettingPanelBalloonGame.SetActive(true);
        gameSelected = "GRHSpaceMG_Scene";
    }

    // "X" button to close the difficulty panel
    public void CloseDifficultyPanel()
    {
        difficultySettingPanelBalloonGame.SetActive(false);
        difficultySettingPanelCountingGame.SetActive(false);
    }

    //Opens and closes the credits panel
    public void ToggleCreditsPanel()
    {
        creditsToggle = !creditsToggle;
        creditsPanel.SetActive(creditsToggle);
    }

    // Selects game difficulty (EASY, MEDIUM, HARD)
    public void SelectDifficultySetting(string gameDifficulty)
    {
        gameSettings.gameDifficulty = gameDifficulty;

        StartCoroutine(LoadMinigame(gameSelected, true));
    }

    IEnumerator LoadMinigame(string scene, bool stopMusic)
    {
        StartCoroutine(loadingScreen.LoadScene(scene));

        if (stopMusic)
        {
            yield return new WaitForSeconds(1);
            if (soundManager.balloonMG_Audio[0].isPlaying)
            {
                soundManager.balloonMG_Audio[0].Stop();
            }
        }
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

    //Sets weather the opponents counts are displayed above their heads (Counting Game)
    public void ToggleOpponentCounts()
    {
        if (!gameSettings.displayOpponentCount)
        {
            gameSettings.displayOpponentCount = true;
        }
        else
        {
            gameSettings.displayOpponentCount = false;
        }
    }

    //Sets tpump count amount for the Balloon Minigame
    public void SetPumpCount()
    {
        if (!string.IsNullOrEmpty(pumpCount.text) && !pumpCount.text.Contains("-"))
        {
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
        else
        {
            pumpCount.textComponent.color = Color.red;

            easyButton.enabled = false;
            mediumButton.enabled = false;
            hardButton.enabled = false;
        }
    }

    //Set opponent level (counting game)
    public void SetOpponentLevel()
    {
        gameSettings.opponentLevel = (int)opponentLevelSlider.value;
    }

    //Set time limit (counting game)
    public void SetTimeLimit()
    {
        gameSettings.timeLimit = (int)(timeSlider.value * 15);
    }

    // Sets the player's character preference [Added by Bryce]
    public void CharacterSelection()
    {
        gameSettings.selectedCharacter = characterSelector.value;
    }

    //Set Object Speed (counting game)
    public void SetObjectSpeed()
    {
        gameSettings.entityMovementSpeed = (objectSpeedSlider.value / 2);
    }

    //Set object size (counting game)
    public void SetObjectSize()
    {
        if (objectScaleSlider.value == 3)
        {
            gameSettings.entityScaling = 2f;
        }
        else if(objectScaleSlider.value == 2)
        {
            gameSettings.entityScaling = 1.5f;
        }
        else
        {
            gameSettings.entityScaling = 1;
        }
    }

    //Set object count (Counting game)
    public void SetObjectCount()
    {
        gameSettings.entityAmount = (int)objectCountSlider.value;
    }

    // Remove all but "Application.Quit();" before building as it will crash if left in during build
    public void QuitApplication()
    {
        // Closes game if in editor mode
        //if (UnityEditor.EditorApplication.isPlaying == true)
        //{
        //    UnityEditor.EditorApplication.isPlaying = false;
        //}
        //else
        //{
        // Closes game if in build mode
        Application.Quit();
        //}
    }
}
