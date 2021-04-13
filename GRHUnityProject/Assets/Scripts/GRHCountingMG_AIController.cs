using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GRHCountingMG_AIController : MonoBehaviour
{
    // Initial framework - Adam

    public int totalCount;
    [Space(8)]
    public int ai_Guess;
    [Space(8)]
    public int percentChance; // Difficulty slider

    public float guessRate;
    private float timeStamp;

    public bool canGuess = false; // AI is able to guess if game is still playing

    private void Start()
    {
        percentChance = GRHGameSettings.gameSettings.opponentLevel * 10;
        timeStamp = -1.0f;
    }

    private void Update()
    {
        // Set the guess rate, when the game starts the AI will wait (guessRate) seconds before guessing 
        // and continue doing so for the set time throughout the game
        if (Time.time > timeStamp + guessRate && canGuess)
        {
            Guess();
            timeStamp = Time.time;
        }
    }

    void Guess()
    {
        // If AI's count is greater than the total count on screen, will add 1 if its within the % chance
        if (ai_Guess < totalCount)
        {
            int ran = Random.Range(1, 101);
            if (ran >= 1 && ran <= percentChance)
            {
                ai_Guess += 1;
            }
        }

        // If AI's count is greater than the total count on screen, will subtract 1 if its within the % chance
        if (ai_Guess > totalCount)
        {
            int ran = Random.Range(1, 101);
            if (ran >= 1 && ran <= percentChance)
            {
                ai_Guess -= 1;
            }
        }

        // If AI's count is equal to the total count on screen, 90% chance to go up or down by 1 or a 10% chance to "forget" count and restart at 0
        if (ai_Guess == totalCount)
        {
            int ran = Random.Range(1, 101);
            if (ran >= 1 && ran <= 45)
            {
                ai_Guess += 1;
            }
            else if (ran > 45 && ran <= 90)
            {
                ai_Guess -= 1;
            }
            else
            {
                ai_Guess = 0;
            }
        }
    }
}

