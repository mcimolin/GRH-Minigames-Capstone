using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GRHBalloonMG_AIController : MonoBehaviour
{
    //Initial framework - Bryce

    int balloonGuess; //The AI's guess at how many pumps are left until the balloon pops
    int difficultyVariable; //The variability in the AI's ability to guess how many pumps are left in the balloon

    enum AIDifficulty
    {
        Easy,
        Medium,
    }

    AIDifficulty aiDifficulty; //Determines the AI's decisions when guessing how many pumps are left and how many pumps to do.

    void Start()
    {
        //Initialize AI's difficulty from selected Settings

    }

    //Generates a guess for the AI for how many pumps are left until the balloon pops prior to pumping the balloon.
    internal void GenerateBalloonGuess(int balloonPumpsLeft)
    {
        if (balloonPumpsLeft - difficultyVariable > 0)
        {
            balloonGuess = Random.Range(balloonPumpsLeft - difficultyVariable, balloonPumpsLeft + difficultyVariable);
        }
    }

    //Generates the AI's pump amount for the balloon.
    internal int GeneratePumpAmount()
    {
        int pumpAmount = 0; //The amount of pumps the AI will do for the balloon.

        switch (aiDifficulty)
        {

            //Easy AI Pump generation
            case AIDifficulty.Easy:

                //The AI will randomly choose an amount to pump
                pumpAmount = Random.Range(1, 3);
                break;

            //Medium AI Pump generation
            case AIDifficulty.Medium:

                //Check to see if the AI believes they can cause an opponent to lose on their turn
                if (balloonGuess > 4)
                {
                    pumpAmount = Random.Range(1, 3);
                }
                else
                {
                    //The AI believes they can cause an opponent to lose on their turn
                    pumpAmount = balloonGuess - 1;
                }
                break;

            //No AI Difficulty found when this method was called.
            default:
                Debug.LogError("Pump generation for AI was unsuccessful: Could not find AI difficulty.");
                break;
        }

        return pumpAmount;
    }

    public void SetAIDifficulty(int difficulty)
    {
        aiDifficulty = (AIDifficulty)difficulty;
    }
}
