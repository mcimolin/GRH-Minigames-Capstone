using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Initial Framework: Bryce
public class GRHCountingMG_GameManager : MonoBehaviour
{
    // These objects will hold the Prefabs of what will be spawned into the scene when the game starts.
    [SerializeField] internal GameObject butterflyPrefab = null, flowerPrefab = null, frogPrefab = null, lilypadPrefab = null, fishPrefab = null, somethingPrefab = null;
    
    // This will hold the amount of objects that exist within the scene.
    internal GameObject[] objectsToGuess;

    // The game's length and the current time spent into the game.
    internal float gameLength = 0, currentTime = 0;

    //Checks to see if the game is currently playing.
    internal bool gameIsPlaying = false, gameEnd = false;

    // The amount of objects to spawn into the game for the player to guess, and the player's current guess amount.
    int guessAmount = 0, playerGuess = 0, AI1Guess = 0, AI2Guess = 0, AI3Guess = 0;

    // The text display of how much time is left for the minigame;
    [SerializeField] Text timeLeftText = null;

    enum GameDifficulty { EASY, MEDIUM, HARD }

    // The difficulty of the game.
    GameDifficulty gameDifficulty = GameDifficulty.EASY;

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

        //gameLength = GRHGameSettings.gameSettings.gameLength;

        SpawnEntities();
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
            timeLeftText.text = $"Time Left: {gameLength - currentTime}";
        }
        else if (gameEnd)
        {
            EndGame();
        }
    }

    internal void SpawnEntities()
    {
        switch (gameDifficulty)
        {
            case GameDifficulty.EASY:
                break;
            case GameDifficulty.MEDIUM:
                break;
            case GameDifficulty.HARD:
                break;
            default:
                break;
        }
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

    internal void EndGame()
    {
        if (playerGuess == guessAmount && AI1Guess != guessAmount && AI2Guess != guessAmount && AI3Guess != guessAmount)
        {
            // Set the player's win condition as 'Win'
        }
        else if (playerGuess == guessAmount && (AI1Guess == guessAmount || AI2Guess == guessAmount || AI3Guess == guessAmount))
        {
            // Set the player's win condition as 'Tie' (will we have multiple rounds that will wait for the player to win?)
        }
        else
        {
            // Sert the player's win status as 'Lose'
        }
    }
}
