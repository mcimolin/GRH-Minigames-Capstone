using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Initial Framework: Bryce
public class GRHCountingMG_GameManager : MonoBehaviour
{
    // These objects will hold the Prefabs of what will be spawned into the scene when the game starts.
    [SerializeField] internal GameObject butterflyPrefab = null, flowerPrefab = null, frogPrefab = null, lilypadPrefab = null, fishPrefab = null, bubblePrefab = null;

    // These objects will hold the Spawn locations for the spawnable objects.
    [SerializeField] internal GameObject butterflySpawnArea = null, flowerSpawnArea = null, frogSpawnArea = null, lilypadSpawnArea = null, fishSpawnArea = null, bubbleSpawnArea = null;

    // This will hold the amount of objects that exist within the scene.
    internal GameObject[] objectsToGuess;

    // The game's length and the current time spent into the game.
    internal float gameLength = 20, currentTime = 0;

    //Checks to see if the game is currently playing.
    internal bool gameIsPlaying = false, gameEnd = false;

    // Toggleable option to display the AI guesses while the game is playing.
    internal bool displayAIGuesses = true;

    // AI scripts
    internal GameObject[] AIObjects = null;

    // The amount of objects to spawn into the game for the player to guess, and the player's current guess amount.
    int spawnablesAmount = 0, fakeSpawnablesAmount = 0, playerGuess = 0, AI1Guess = 0, AI2Guess = 0, AI3Guess = 0;

    // The text display of how much time is left for the minigame;
    [SerializeField] Text timeLeftText = null, startGameText = null;

    // The Game Ending panel
    [SerializeField] GameObject gameEndPanel = null;

    // The win condition display text
    [SerializeField] Text gameEndWinCondition = null;

    // The AI's guess textboxes to display their current guess
    [SerializeField] internal Text[] AIGuessTexts;

    enum GameDifficulty { EASY, MEDIUM, HARD }

    // The difficulty of the game.
    [SerializeField] GameDifficulty gameDifficulty = GameDifficulty.HARD;

    private void Awake()
    {
        AIGuessTexts = new Text[3];
    }

    // Start is called before the first frame update
    void Start()
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
            gameLength = GRHGameSettings.gameSettings.timeLimit;
        }
        catch
        {
            Debug.LogError($"Parse Error: failed to load game length.");
        }

        // Sets the amount of spawnables that the player must guess
        spawnablesAmount = UnityEngine.Random.Range(5, 16);
        fakeSpawnablesAmount = 5;

        StartCoroutine(DelayForGameStart());
    }

    // Update is called once per frame
    void Update()
    {
        // Checks to see if the game has been running longer than the set length for the game. Sets the game as ended if so.
        if (currentTime >= gameLength)
        {
            gameEnd = true;
        }

        
        if (gameIsPlaying && !gameEnd) // If the game is currently being played and the game has not been set as ended, continue to progress the time of the game.
        {
            currentTime += Time.deltaTime;
            timeLeftText.text = $"Time Left: {(int)(gameLength - currentTime)}";
        }
        else if (gameEnd) // The game has been set as ended
        {
            StartCoroutine(EndGame());
        }

        //Updates the AI's guess displays if the option to display them is enabled.
        if (displayAIGuesses)
        {
            AIGuessTexts[0].text = $"{AI1Guess}";
            AIGuessTexts[1].text = $"{AI2Guess}";
            AIGuessTexts[2].text = $"{AI3Guess}";
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
                for (int i = 0; i < spawnablesAmount; i++)
                {
                    randomPos = new Vector3(UnityEngine.Random.Range(-9.0f, 9.0f), UnityEngine.Random.Range(-1f, 3f), 0);
                    Entity(butterflySpawnArea, butterflyPrefab, randomPos, new Vector3(0, 0, 0));
                }

                // Spawn Flowers
                for (int i = 0; i < fakeSpawnablesAmount; i++)
                {
                    int catchNum = 0; // Catch number for do-while loop. Used to break out if infinite loop occurs or too long of loading occurs

                    do
                    {
                        randomPos = new Vector3(UnityEngine.Random.Range(-9.0f, 9.0f), 0, UnityEngine.Random.Range(-12f, 8f));
                        catchNum += 1;
                    } while (randomPos.x < 10 && randomPos.x > -8 && randomPos.z < 5 && randomPos.z > -10 && catchNum < 15);

                    // Spawns entity if viable position was found
                    if (catchNum != 20)
                    {
                        Entity(flowerSpawnArea, flowerPrefab, randomPos, new Vector3(0, 0, 0));
                    }
                    else
                    {
                        Debug.LogError("Entity Spawn Error: Did not spawn object.");
                    }

                }
                break;

            /* MEDIUM difficulty selected: Frogs & Lilypads */
            case GameDifficulty.MEDIUM:
                
                // Spawn Frogs
                for (int i = 0; i < spawnablesAmount; i++)
                {
                    int catchNum = 0; // Catch number for do-while loop. Used to break out if infinite loop occurs or too long of loading occurs

                    do
                    {
                        randomPos = new Vector3(UnityEngine.Random.Range(-9.0f, 9.0f), 0, UnityEngine.Random.Range(-12f, 8f));
                        catchNum += 1;
                    } while (randomPos.x < 10 && randomPos.x > -8 && randomPos.z < 5 && randomPos.z > -10 && catchNum < 15);
                    
                    // Spawns entity if viable position was found
                    if (catchNum != 20)
                    {
                        Entity(frogSpawnArea, frogPrefab, randomPos, new Vector3(0, 0, 0));
                    }
                    else
                    {
                        Debug.LogError("Entity Spawn Error: Did not spawn object.");
                    }
                }

                // Spawn Lilypads
                for (int i = 0; i < fakeSpawnablesAmount; i++)
                {
                    randomPos = new Vector3(UnityEngine.Random.Range(-4.0f, 4.0f), 0, UnityEngine.Random.Range(-2f, 0.75f));
                    Entity(lilypadSpawnArea, lilypadPrefab, randomPos, new Vector3(0, 0, 0));
                }
                break;

            /* HARD difficulty selected: Fish and Bubbles */
            case GameDifficulty.HARD:
                
                // Spawn Fish
                for (int i = 0; i < spawnablesAmount; i++)
                {
                    randomPos = new Vector3(UnityEngine.Random.Range(-4.0f, 4.0f), 0, UnityEngine.Random.Range(-2f, 0.75f));
                    Entity(fishSpawnArea, fishPrefab, randomPos, new Vector3(1, 1, 1));

                }

                //Spawn Bubbles
                for (int i = 0; i < fakeSpawnablesAmount; i++)
                {
                    randomPos = new Vector3(UnityEngine.Random.Range(-4.0f, 4.0f), 0, UnityEngine.Random.Range(-2f, 0.75f));
                    Entity(bubbleSpawnArea, bubblePrefab, randomPos, new Vector3(1, 1, 1));
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

        // Sets the alpha value of the sprite to 0 (invisible)
        entityObj.GetComponent<SpriteRenderer>().material.color = new Color(1, 1, 1, 0);

        // Modify the entities positioning, rotation and scaling
        entityObj.transform.position = spawnLocation.transform.position + location;
        entityObj.transform.localScale = scale;

        // Slowly fades the entity into view
        StartCoroutine(FadeEntityIn(entityObj));

    }

    /// <summary>
    /// Starts the game.
    /// Set's all variables required to begin the playing of the game.
    /// </summary>
    internal void AdvanceGame()
    {
        gameIsPlaying = true;
        currentTime = 0;
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
            }
            else // Player inputs a negative guess that puts their current guess lower than zero
            {
                Debug.LogError("Current guess would be negative. Ignoring guess change.");
            }
        }
        else // Player's input will not result in a negative guess
        {
            playerGuess += value;
        }
    }

    /// <summary>
    /// Fades the specified entity into the scene.
    /// </summary>
    IEnumerator FadeEntityIn(GameObject entity)
    {
        float alphaValue = 1.0f; //The value the entity will fade to
        float alphaTime = 1.5f; //The time it will take to complete the fade transition
        float alpha = entity.GetComponent<SpriteRenderer>().material.color.a; //The current alpha value of the sprite

        // Fades in the entity sprite over time
        for (float a = 0f; a < 1.0f; a += Time.deltaTime / alphaTime)
        {
            Color color = new Color(1, 1, 1, Mathf.Lerp(alpha, alphaValue, a));
            entity.GetComponent<SpriteRenderer>().material.color = color;
            yield return null;
        }
    }

    /// <summary>
    /// Delay's the starting of the game, counting down.
    /// </summary>
    IEnumerator DelayForGameStart()
    {
        yield return new WaitForSeconds(0.75f);

        startGameText.text = "3";
        yield return new WaitForSeconds(1);
        
        startGameText.text = "2";
        yield return new WaitForSeconds(1);

        startGameText.text = "1";
        yield return new WaitForSeconds(1);

        startGameText.text = "GO!";
        yield return new WaitForSeconds(1);

        startGameText.text = "";

        // Begins the game
        SpawnEntities();
        AdvanceGame();
    }

    /// <summary>
    /// Handles all end game functionality.
    /// Displays the win/lose status of the player.
    /// Displays the play again or return to menu panel.
    /// </summary>
    IEnumerator EndGame()
    {

        if (playerGuess == spawnablesAmount)
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
}
