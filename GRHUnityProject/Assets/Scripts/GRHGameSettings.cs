using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GRHGameSettings : MonoBehaviour
{
    // Singleton variable [Added by Bryce]
    public static GRHGameSettings gameSettings = null;

    // EASY, MEDIUM, HARD
    public string gameDifficulty;

    // Tells which character the player will play in game, default being "Popcorn" [Added by Adam]
    public int selectedCharacter;

    // Tells if game shows pump count or not (Balloon Minigame)
    public bool showPumpCount;

    // The amount of pumps the balloon will start with (Balloon Minigame)
    public int pumpCount;

    // The level of difficulty for the opponent (Counting Minigame)
    public int opponentLevel;

    //The length of time that the game will run for (Counting Minigame)
    public int timeLimit;

    //The option to display AI's guesses during the game (Counting Minigame)
    public bool displayOpponentCount;

    private void Awake()
    {
        // Checks to make sure there is only one of this script loaded [Added by Bryce]
        if (gameSettings == null)
        {
            gameSettings = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (gameSettings != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        showPumpCount = true;
        pumpCount = 15;
        selectedCharacter = 0;
        opponentLevel = 0;
        timeLimit = 20;
        displayOpponentCount = true;
    }
}