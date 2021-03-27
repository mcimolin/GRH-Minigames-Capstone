using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GRHCountingMG_AIController : MonoBehaviour
{
    // Initial framework - Adam

    public int totalCount;
    [Space(12)]
    public int ai1_Guess;
    [Space(12)]
    public int percentChance; // Difficulty slider

    public float guessRate;
    private float timeStamp;

    private void Start()
    {
        timeStamp = -1.0f;
    }

    private void Update()
    {

        // remove after testing
        if (Input.GetKeyDown(KeyCode.P))
        {
            totalCount += 1;
        }
        // remove after testing
        if (Input.GetKeyDown(KeyCode.O))
        {
            totalCount -= 1;
        }
        // remove after testing
        if (Input.GetKeyDown(KeyCode.G))
        {
            Guess();
        }

        // Set the guess rate, when the game starts the AI will wait (guessRate) seconds before guessing 
        // and continue doing so for the set time throughout the game
        if (Time.time > timeStamp + guessRate)
        {
            Guess();
            timeStamp = Time.time;
        }
    }

    void Guess()
    {
        // If AI's count is greater than the total count on screen, will add 1 if its within the % chance
        if (ai1_Guess < totalCount)
        {
            int ran = Random.Range(1, 101);
            Debug.Log(ran);
            if (ran >= 1 && ran <= percentChance)
            {
                Debug.Log("increase count by 1");
                ai1_Guess += 1;
            }
        }

        // If AI's count is greater than the total count on screen, will subtract 1 if its within the % chance
        if (ai1_Guess > totalCount)
        {
            int ran = Random.Range(1, 101);
            Debug.Log(ran);
            if (ran >= 1 && ran <= percentChance)
            {
                Debug.Log("decrease count by 1");
                ai1_Guess -= 1;
            }
        }
    }
}

