using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Initial Framework: Bryce
public class GRHCountingMG_GameManager : MonoBehaviour
{
    //Counting Game sound manager
    [SerializeField] internal GRHCountingMG_SoundManager soundManager;

    enum GameDifficulty { EASY, MEDIUM, HARD }

    // The difficulty of the game.
    [SerializeField] GameDifficulty gameDifficulty = GameDifficulty.HARD;

    // These objects will hold the Prefabs of what will be spawned into the scene when the game starts.
    [SerializeField] internal GameObject butterflyPrefab = null, flowerPrefab = null, frogPrefab = null, lilypadPrefab = null, fishPrefab = null, bubblePrefab = null;

    // These objects will hold the Spawn locations for the spawnable objects.
    [SerializeField] internal GameObject butterflySpawnArea = null, flowerSpawnArea = null, frogSpawnArea = null, lilypadSpawnArea = null, fishSpawnArea = null, bubbleSpawnArea = null;

    // The text display of how much time is left for the minigame;
    [SerializeField] Text timeLeftText = null, startGameText = null, playerGuessText;

    // The Game Ending panel
    [SerializeField] GameObject gameStartPanel = null, gameEndPanel = null;

    // The win condition display text
    [SerializeField] Text gameEndWinCondition = null;

    // The Add and Subtract buttons for the player's guess input
    [SerializeField] Button addButton = null, subtractButton = null;

    // The Fade panel for when scene is loaded and unloaded
    [SerializeField] internal GRHLoadingScreen loadingScreen;

    // The AI's guess textboxes to display their current guess
    [SerializeField] internal Text[] AIGuessTexts;

    // AI scripts
    [SerializeField] internal GRHCountingMG_AIController[] AIObjects = null;

    // This will hold the amount of objects that exist within the scene.
    internal GameObject[] objectsToGuess;

    // The game's length and the current time spent into the game.
    internal float gameDuration = 30, currentTime = 0;

    // The total amount of entities that will be spawned into the scene (split between the real and fake)
    internal int amountOfEntitiesToSpawn = 10;

    // The modification of the entity scaling from the settings
    internal float entityScaling = 1.25f;

    // The modification of the entity movement speed from the settings
    internal float entityMovementSpeed = 1;

    //Checks to see if the game is currently playing.
    internal bool gameIsPlaying = false, gameEnd = false;

    // Toggleable option to display the AI guesses while the game is playing.
    internal bool displayAIGuesses = true;

    // The amount of objects to spawn into the game for the player to guess, and the player's current guess amount.
    int entityAmount = 0, fakeEntityAmount = 0, playerGuess = 0;

    //The array that holds all entities for enabling/disabling their movement
    internal List<GameObject> entityObjects = null;


    private void Awake()
    {
        AIGuessTexts = new Text[3];
        entityObjects = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        loadingScreen = GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<GRHLoadingScreen>();

        LoadSettings();

        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<GRHCountingMG_SoundManager>();

        // Sets the amount of spawnables that the player must guess
        entityAmount = (int)UnityEngine.Random.Range((amountOfEntitiesToSpawn/2) / 1.75f, (amountOfEntitiesToSpawn/2) * 1.75f);
        fakeEntityAmount = amountOfEntitiesToSpawn - entityAmount;

        //Set the Time Left text to display how much time will be on the clock.
        timeLeftText.text = $"Time Left: {gameDuration}";

        for (int i = 0; i < AIObjects.Length; i++)
        {
            AIObjects[i].GetComponent<GRHCountingMG_AIController>().totalCount = entityAmount;
        }

        addButton.interactable = false;
        subtractButton.interactable = false;

        if (!soundManager.IsCountingGameMusicPlaying())
        {
            soundManager.CountingGameMusic();
        }

        //Starts the Countdown timer for starting the game.
        StartCoroutine(DelayForGameStart());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals) && gameIsPlaying)
        {
            PlayerGuess(1);
        }

        if (Input.GetKeyDown(KeyCode.Minus) && gameIsPlaying)
        {
            PlayerGuess(-1);
        }

        // Checks to see if the game has been running longer than the set length for the game. Sets the game as ended if so.
        if (currentTime >= gameDuration)
        {
            gameEnd = true;
        }

        
        if (gameIsPlaying && !gameEnd) // If the game is currently being played and the game has not been set as ended, continue to progress the time of the game.
        {
            currentTime += Time.deltaTime;
            timeLeftText.text = $"Time Left: {(int)(gameDuration - currentTime)}";
        }
        else if (gameEnd) // The game has been set as ended
        {
            StartCoroutine(EndGame());
        }

        //Updates the AI's guess displays if the option to display them is enabled.
        if (displayAIGuesses)
        {
            for (int i = 0; i < AIObjects.Length; i++)
            {
                AIGuessTexts[i].text = $"{AIObjects[i].ai_Guess}";
            }
        }

        playerGuessText.text = $"{playerGuess}";
    }

    /// <summary>
    /// Load all required settings from the GRHGameSettings script
    /// </summary>
    internal void LoadSettings()
    {
        // Gets the difficulty that was set for the minigame and stores it for use in this script.
        try
        {
            gameDifficulty = (GameDifficulty)Enum.Parse(typeof(GameDifficulty), GRHGameSettings.gameSettings.gameDifficulty);
        }
        catch (Exception)
        {
            Debug.LogError($"Parse Error: failed to load game difficulty.");
        }

        // Gets the difficulty that was set for the minigame and stores it for use in this script.
        try
        {
            displayAIGuesses = GRHGameSettings.gameSettings.displayOpponentCount;
        }
        catch (Exception)
        {
            Debug.LogError($"Parse Error: failed to load display of AI guesses.");
        }

        // Gets the game's duration for the minigame to store it for use in this script
        try
        {
            gameDuration = GRHGameSettings.gameSettings.timeLimit;
        }
        catch
        {
            Debug.LogError($"Parse Error: failed to load game length.");
        }

        // Gets the amount of entities to spawn for the minigame to store it for use in this script
        try
        {
            amountOfEntitiesToSpawn = GRHGameSettings.gameSettings.entityAmount;
        }
        catch
        {
            Debug.LogError($"Parse Error: failed to load entity amount.");
        }

        // Gets the entity scaling setting for the minigame to store it for use in this script
        try
        {
            entityScaling = GRHGameSettings.gameSettings.entityScaling;
        }
        catch
        {
            Debug.LogError($"Parse Error: failed to load entity scaling.");
        }

        // Gets the entity movement speed setting for the minigame to store it for use in this script
        try
        {
            entityMovementSpeed = GRHGameSettings.gameSettings.entityMovementSpeed;
        }
        catch
        {
            Debug.LogError($"Parse Error: failed to load entity movement speed.");
        }
    }

    /// <summary>
    /// Handles the selection of which entities will be spawned into the scene.
    /// Entities that will be spawned will be passed to the Entity() method to handle the instantiation of the entity and the modifications done to the entity object.
    /// Entities that can be spawned will be handled by the game's difficulty that is selected. One real entity type and one fake entity type will be set.
    /// </summary>
    internal void SpawnEntities()
    {
        Vector3 randomPos;

        switch (gameDifficulty)
        {
            /* EASY difficulty selected: Butterflies & Flowers */
            case GameDifficulty.EASY:
                
                // Spawn Butterflies
                for (int i = 0; i < entityAmount; i++)
                {
                    randomPos = new Vector3(UnityEngine.Random.Range(-11.0f, 11.0f), UnityEngine.Random.Range(-1f, 3f), 0);
                    Entity(butterflySpawnArea, butterflyPrefab, randomPos, new Vector3(0.75f, 0.75f, 0.75f) * entityScaling);
                }

                List<Vector3> entityPositionsList = new List<Vector3>(); // Holds a list of the entity positions. Used to stop entities spawning on one another

                // Spawn Flowers
                for (int i = 0; i < fakeEntityAmount; i++)
                {
                    int catchNum = 0; // Catch number for do-while loop. Used to break out if infinite loop occurs or too long of loading occurs

                    bool similarPositioning = false;
                    do
                    {
                        randomPos = new Vector3(UnityEngine.Random.Range(-11.0f, 11.0f), 0.5f, UnityEngine.Random.Range(-2f, 5f));
                        catchNum += 1;

                        if (entityPositionsList.Count != 0)
                        {
                            for (int x = 0; x < entityPositionsList.Count; x++)
                            {
                                if (Vector3.Distance(randomPos, entityPositionsList[x]) < 1)
                                {
                                    similarPositioning = true;
                                }
                            }
                        }
                    } while (((randomPos.x <= 9.5f && randomPos.x >= -7.75f && randomPos.z <= 3.5f && randomPos.z >= -2) || similarPositioning) && catchNum < 15);

                    // Spawns entity if viable position was found
                    if (catchNum != 20)
                    {
                        Entity(flowerSpawnArea, flowerPrefab, randomPos, new Vector3(0.8f, 0.8f, 0.8f) * entityScaling);
                    }
                    else
                    {
                        Debug.LogError("Entity Spawn Error: Did not spawn object.");
                    }

                }
                break;


            /* MEDIUM difficulty selected: Fish and Bubbles */
            case GameDifficulty.MEDIUM:
                
                // Spawn Fish
                for (int i = 0; i < entityAmount; i++)
                {
                    randomPos = new Vector3(UnityEngine.Random.Range(-4.75f, 6.0f), 0.25f, UnityEngine.Random.Range(-4f, 0.75f));
                    Entity(fishSpawnArea, fishPrefab, randomPos, new Vector3(1, 1, 1) * entityScaling);

                }

                //Spawn Bubbles
                for (int i = 0; i < fakeEntityAmount; i++)
                {
                    randomPos = new Vector3(UnityEngine.Random.Range(-4.75f, 6.0f), 0, UnityEngine.Random.Range(-4f, 0.75f));
                    Entity(bubbleSpawnArea, bubblePrefab, randomPos, new Vector3(0.5f, 0.5f, 0.5f) * entityScaling);
                }
                break;


            /* HARD difficulty selected: Frogs & Lilypads */
            case GameDifficulty.HARD:

                // Spawn Frogs
                for (int i = 0; i < entityAmount; i++)
                {
                    int catchNum = 0; // Catch number for do-while loop. Used to break out if infinite loop occurs or too long of loading occurs

                    do
                    {
                        randomPos = new Vector3(UnityEngine.Random.Range(-9.0f, 9.0f), 0.5f, UnityEngine.Random.Range(-8f, 5f));
                        catchNum += 1;
                    } while (randomPos.x <= 9.5f && randomPos.x >= -7.75f && randomPos.z <= 3.5f && randomPos.z >= -4 && catchNum < 15);

                    // Spawns entity if viable position was found
                    if (catchNum != 20)
                    {
                        Entity(frogSpawnArea, frogPrefab, randomPos, new Vector3(1.25f, 1.25f, 1.25f) * entityScaling);
                    }
                    else
                    {
                        Debug.LogError("Entity Spawn Error: Did not spawn object.");
                    }
                }

                // Spawn Lilypads
                for (int i = 0; i < fakeEntityAmount; i++)
                {
                    randomPos = new Vector3(UnityEngine.Random.Range(-4.75f, 6.0f), 0.25f, UnityEngine.Random.Range(-4f, 0.75f));
                    Entity(lilypadSpawnArea, lilypadPrefab, randomPos, new Vector3(0.15f, 0.15f, 0.15f) * entityScaling);
                }
                break;
            default:
                break;
        } 
    }

    /// <summary>
    /// Spawns the specified entity type into the scene. 
    /// Sets the desired spawning location, the rotation and scaling of the entity.
    /// </summary>
    internal void Entity(GameObject spawnLocation, GameObject entityPrefab, Vector3 location, Vector3 scale)
    {
        // Spawn entities prefab and store to a modifiable object
        GameObject entityObj = Instantiate(entityPrefab);

        // Modify the entities positioning, rotation and scaling
        entityObj.transform.position = spawnLocation.transform.position + location;
        entityObj.transform.localScale = scale;

        //Initializes the Entity information from it's root script
        if (entityObj.GetComponent<GRHCountingMG_MovingEntity>())
        {
            entityObjects.Add(entityObj);
            entityObj.GetComponent<GRHCountingMG_MovingEntity>().SetMovementSpeed(entityMovementSpeed);
            entityObj.GetComponent<GRHCountingMG_MovingEntity>().Initialize();
        }
    }

    /// <summary>
    /// Starts the game.
    /// Set's all variables required to begin the playing of the game.
    /// </summary>
    internal void StartGame()
    {
        StartCoroutine(DelayForGameStart());
    }

    /// <summary>
    /// Modifies the player's current guess count.
    /// This will be handled by button calls in the scene.
    /// value: the value that is passed by the button (-1 or 1) that will be used to modify the player's guess.
    /// </summary>
    public void PlayerGuess(int value)
    {
        if (playerGuess == 0) // Player's current guess is zero. Confirms that player will not have a guess lower than zero.
        { 
            if (value > 0) // Player inputs a positive guess
            {
                playerGuess += value;
                soundManager.GuessButtonSound();
            }
            else // Player inputs a negative guess that puts their current guess lower than zero
            {
                Debug.LogError("Current guess would be negative. Ignoring guess change.");
            }
        }
        else // Player's input will not result in a negative guess
        {
            if (playerGuess < 30) //Player's input does not go over the max limit of 30.
            {
                playerGuess += value;
                soundManager.GuessButtonSound();
            }
            else //Player's input would be over the max limit of 30.
            {
                if (value < 0)
                {
                    playerGuess += value;
                    soundManager.GuessButtonSound();
                }
                else
                {
                    Debug.LogError("Max guess limit reached. Ignoring guess change.");
                }
            }
        }
    }

    /// <summary>
    /// Delay's the starting of the game, counting down.
    /// </summary>
    IEnumerator DelayForGameStart()
    {
        gameStartPanel.SetActive(true);
        startGameText.text = "3";
        yield return new WaitForSeconds(1);
        
        startGameText.text = "2";
        yield return new WaitForSeconds(1);

        startGameText.text = "1";
        yield return new WaitForSeconds(1);

        startGameText.text = "GO!";
        yield return new WaitForSeconds(1);

        gameStartPanel.SetActive(false);
        startGameText.text = "";

        gameIsPlaying = true;
        currentTime = 0;
        addButton.interactable = true;
        subtractButton.interactable = true;

        SpawnEntities();

        for (int i = 0; i < entityObjects.Count; i++)
        {
            if (entityObjects[i].GetComponent<GRHCountingMG_MovingEntity>())
            {
                entityObjects[i].GetComponent<GRHCountingMG_MovingEntity>().EnableMovement();
            }
        }

        for (int i = 0; i < AIObjects.Length; i++)
        {
            AIObjects[i].GetComponent<GRHCountingMG_AIController>().canGuess = true;
        }
    }

    /// <summary>
    /// Handles all end game functionality.
    /// Displays the win/lose status of the player.
    /// Displays the play again or return to menu panel.
    /// </summary>
    IEnumerator EndGame()
    {
        addButton.interactable = false;
        subtractButton.interactable = false;

        for (int i = 0; i < entityObjects.Count; i++)
        {
            if (entityObjects[i].GetComponent<GRHCountingMG_MovingEntity>())
            {
                entityObjects[i].GetComponent<GRHCountingMG_MovingEntity>().DisableMovement();
            }
        }

        for (int i = 0; i < AIObjects.Length; i++)
        {
            AIObjects[i].GetComponent<GRHCountingMG_AIController>().canGuess = false;
        }

        if (playerGuess == entityAmount)
        {
            // Set the player's win condition as 'Win'
            gameEndWinCondition.text = "You Win";
        }
        else
        {
            // Set the player's win status as 'Lose'
            gameEndWinCondition.text = "You Lose";
        }

        //Wait for a short amount of time after game ends before displaying panel.
        yield return new WaitForSeconds(1.25f);

        gameEndPanel.SetActive(true);
    }

    IEnumerator LoadScene(string scene, bool stopMusic)
    {
        StartCoroutine(loadingScreen.LoadScene(scene));

        if (stopMusic)
        {
            yield return new WaitForSeconds(1);
            if (soundManager.countingMG_Audio[0].isPlaying)
            {
                soundManager.StopCountingGameMusic();
            }
        }
    }

    public void PlayAgain()
    {
        StartCoroutine(LoadScene("GRHCountingMG_Scene", false));
    }

    public void ExitMinigame()
    {
        StartCoroutine(LoadScene("GRHHubWorld_SceneManager", true));
    }
}
