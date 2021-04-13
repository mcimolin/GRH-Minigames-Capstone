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

    // The amount of objects to spawn (Counting Minigame)
    public int entityAmount;

    // The scale size of the entities that will be spawned (Counting Minigame)
    public float entityScaling;

    // The speed in which the entities move around on the scene
    public float entityMovementSpeed;

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
        opponentLevel = 1;
        timeLimit = 30;
        displayOpponentCount = true;
        entityAmount = 10;
        entityScaling = 1f;
        entityMovementSpeed = 0.5f;
    }
}