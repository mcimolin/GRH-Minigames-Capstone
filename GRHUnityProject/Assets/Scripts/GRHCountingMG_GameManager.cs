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

    // AI scripts
    GameObject[] AIObjects = null;

    // The amount of objects to spawn into the game for the player to guess, and the player's current guess amount.
    int spawnablesAmount = 0, fakeSpawnablesAmount= 0, playerGuess = 0, AI1Guess = 0, AI2Guess = 0, AI3Guess = 0;

    // The text display of how much time is left for the minigame;
    [SerializeField] Text timeLeftText = null, startGameText = null;

    // The Game Ending panel
    [SerializeField] GameObject gameEndPanel = null;

    // The win condition display text
    [SerializeField] Text gameEndWinCondition = null;

    enum GameDifficulty { EASY, MEDIUM, HARD }

    // The difficulty of the game.
    GameDifficulty gameDifficulty = GameDifficulty.HARD;

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

        try
        {
            //gameLength = GRHGameSettings.gameSettings.gameLength;
        }
        catch
        {
            Debug.LogError($"Parse Error: failed to load game length.");
        }

        //Sets the amount of spawnables that the player must guess
        spawnablesAmount = UnityEngine.Random.Range(5, 16);

        StartCoroutine(DelayForGameStart());
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime >= gameLength)
        {
            gameEnd = true;
        }

        if (gameIsPlaying && !gameEnd)
        {
            currentTime += Time.deltaTime;
            timeLeftText.text = $"Time Left: {(int)(gameLength - currentTime)}";
        }
        else if (gameEnd)
        {
            StartCoroutine(EndGame());
        }
    }

    internal void SpawnEntities()
    {
        //TO ADD: make random values pull the boundaries from their respective scripts
        Vector3 randomPos;

        switch (gameDifficulty)
        {
            case GameDifficulty.EASY:
                // Spawn Butterflies
                for (int i = 0; i < spawnablesAmount; i++)
                {
                    randomPos = new Vector3(UnityEngine.Random.Range(-9.0f, 9.0f), UnityEngine.Random.Range(-1f, 3f), 0);
                    Entity(butterflySpawnArea, butterflyPrefab, randomPos);
                }

                //Spawn Flowers
                for (int i = 0; i < fakeSpawnablesAmount; i++)
                {
                    int catchNum = 0;
                    do
                    {
                        randomPos = new Vector3(UnityEngine.Random.Range(-9.0f, 9.0f), 0, UnityEngine.Random.Range(-12f, 8f));
                        catchNum += 1;
                    } while (randomPos.x < 10 && randomPos.x > -8 && randomPos.z < 5 && randomPos.z > -10 && catchNum < 20);

                    if (catchNum != 20)
                    {
                        Entity(flowerSpawnArea, flowerPrefab, randomPos);
                    }
                    else
                    {
                        Debug.LogError("Entity Spawn Error: Did not spawn object.");
                    }

                }
                break;


            case GameDifficulty.MEDIUM:
                //Spawn Frogs
                for (int i = 0; i < spawnablesAmount; i++)
                {
                    randomPos = new Vector3(UnityEngine.Random.Range(-4.0f, 4.0f), 0, UnityEngine.Random.Range(-2f, 2f));
                    Entity(frogSpawnArea, frogPrefab, randomPos);
                }

                //Spawn Lilypads
                for (int i = 0; i < fakeSpawnablesAmount; i++)
                {
                    randomPos = new Vector3(UnityEngine.Random.Range(-4.0f, 4.0f), 0, UnityEngine.Random.Range(-2f, 0.75f));
                    Entity(lilypadSpawnArea, lilypadPrefab, randomPos);
                }
                break;


            case GameDifficulty.HARD:
                //Spawn Fish
                for (int i = 0; i < spawnablesAmount; i++)
                {
                    randomPos = new Vector3(UnityEngine.Random.Range(-4.0f, 4.0f), 0, UnityEngine.Random.Range(-2f, 0.75f));
                    Entity(fishSpawnArea, fishPrefab, randomPos);

                }

                //Spawn Bubbles
                for (int i = 0; i < fakeSpawnablesAmount; i++)
                {
                    randomPos = new Vector3(UnityEngine.Random.Range(-4.0f, 4.0f), 0, UnityEngine.Random.Range(-2f, 0.75f));
                    Entity(bubbleSpawnArea, bubblePrefab, randomPos);
                }
                break;
            default:
                break;
        } 
    }

    internal void Entity(GameObject spawnLocation, GameObject entityPrefab, Vector3 location)
    {
        GameObject entityObj = Instantiate(entityPrefab);
        entityObj.transform.position = spawnLocation.transform.position + location;
        entityObj.transform.Rotate(Vector3.left * -90);
        entityObj.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
    }

    internal void AdvanceGame()
    {
        gameIsPlaying = true;
        currentTime = 0;
    }

    internal void PlayerGuess(int value)
    {
        if (playerGuess == 0) 
        { 
            if (value > 0)
            {
                playerGuess += value;
            }
            else
            {
                Debug.LogError("Current guess would be negative. Ignoring guess change.");
            }
        }
        else
        {
            playerGuess += value;
        }
    }

    IEnumerator DelayForGameStart()
    {
        startGameText.text = "3";
        yield return new WaitForSeconds(1);
        
        startGameText.text = "2";
        yield return new WaitForSeconds(1);

        startGameText.text = "1";
        yield return new WaitForSeconds(1);

        startGameText.text = "GO!";
        yield return new WaitForSeconds(1);

        startGameText.text = "";
        SpawnEntities();
        AdvanceGame();
    }

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
