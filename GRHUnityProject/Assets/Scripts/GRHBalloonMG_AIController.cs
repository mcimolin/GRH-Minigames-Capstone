using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GRHBalloonMG_AIController : MonoBehaviour
{
    // Initial framework - Bryce

    enum AIDifficulty
    {
        EASY,
        MEDIUM,
        HARD
    }

    AIDifficulty aiDifficulty; // Determines the AI's decisions when guessing how many pumps are left and how many pumps to do.

    void Start()
    {
        /* Initializes the AI difficulty */
        try
        {
            aiDifficulty = (AIDifficulty)Enum.Parse(typeof(AIDifficulty), GRHGameSettings.gameSettings.gameDifficulty);
            Debug.Log($"\"{aiDifficulty}\" difficulty loaded.");
        }
        catch (Exception)
        {
            Debug.LogError($"Parse Error: failed to load AI difficulty \"{GRHGameSettings.gameSettings.gameDifficulty}\"");
        }
    }

    // Generates the AI's pump amount for the balloon.
    internal int GeneratePumpAmount(int pumpsLeft)
    {
        int pumpAmount = 0; //The amount of pumps the AI will do for the balloon.

        switch (aiDifficulty)
        {
            /* Easy AI will always pump 1 */
            case AIDifficulty.EASY:

                pumpAmount = 1;
                break;

            /* Medium AI will choose a random pump between 1 and 3 */
            case AIDifficulty.MEDIUM:

                pumpAmount = UnityEngine.Random.Range(1, 3);
                break;

            /*Hard AI will strategize and try to get players or other AI's out */
            case AIDifficulty.HARD:

                if (pumpsLeft > 4) // The AI cannot cause an opponent to lose on their turn
                {
                    pumpAmount = UnityEngine.Random.Range(1, 3);
                }
                else // The AI can cause an opponent to lose on their turn
                {
                    pumpAmount = pumpsLeft - 1;
                }

                break;

            /* AI Difficulty was not found, no pump generation can be completed */
            default:

                Debug.LogError("Error: Unable to complete AI pump generation. AI Difficulty not found.");
                break;
        }

        return pumpAmount;
    }
}
