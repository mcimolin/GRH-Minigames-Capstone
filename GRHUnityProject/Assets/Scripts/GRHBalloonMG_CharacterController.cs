using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Initial Framework: Bryce
public class GRHBalloonMG_CharacterController : MonoBehaviour
{
    //The Counting Minigame's GameManager script
    [SerializeField] GRHCountingMG_GameManager gm;

    //These are the positions in which the characters will be standing.
    [SerializeField] GameObject[] positions;

    //These arrays will hold the characters gameobjects to be transferred over to the positions and their labels
    [SerializeField] internal GameObject[] characters, labels;

    //Holds the sprite images of the CPU icon and Player Icon.
    [SerializeField] Sprite cpuLabelImage, playerLabelImage;

    //Set's the toggleable option of random spawns or set spawns.
    [SerializeField] bool setRandomPositions = false;

    //The player's selected character (defaults to 0: Cotton Candy)
    int playerCharacter = 0;

    //The character to ignore.
    int ignoreCharacter;

    // Start is called before the first frame update
    void Start()
    {
        //Gets the player's selected character model and stores it for use in this script.
        try
        {
            playerCharacter = GRHGameSettings.gameSettings.selectedCharacter;
        }
        catch (Exception)
        {
            Debug.LogError("Character Select Error: Unable to fetch player's selected character from game settings.");
        }

        //Selects a random character model to ignore (loops until selected model to ignore is not the player's model).
        do
        {
            ignoreCharacter = UnityEngine.Random.Range(0, 5);
        } while (ignoreCharacter == playerCharacter);

        
        if (setRandomPositions) //Random positions selected, spawn all character's in random positions.
        {
            SetRandomPositions();
        }
        else // Spawn character's in set positions.
        {
            SetBasePositions();
        }
    }

    //Sets the positions of the characters from left to right in order.
    void SetBasePositions()
    {
        //Sets the player's character to the far left side.
        characters[playerCharacter].transform.parent = positions[0].transform;
        characters[playerCharacter].transform.position = positions[0].transform.position;
        characters[playerCharacter].GetComponentInChildren<Image>();
        SetLabel(labels[playerCharacter], playerLabelImage, "P1", Color.red);

        SetRandomPositions(1);
    }

    //Sets the positions of the characters to be randomized.
    void SetRandomPositions()
    {
        int randomPosition;
        int aiIndex = 0;

        for (int i = 0; i < characters.Length; i++)
        {
            if (i != ignoreCharacter)
            {
                int catchNum = 0; //Catch number to hop out of the do-while loop if an error occurs

                do
                {
                    catchNum++;
                    randomPosition = UnityEngine.Random.Range(0, 4);
                } while (positions[randomPosition].transform.childCount != 0 && catchNum < 25);

                if (catchNum == 10)
                {
                    Debug.LogError("Error setting a random position: Could not find any free positions available.");
                }
                else
                {
                    if (i == playerCharacter)
                    {
                        characters[i].GetComponentInChildren<Image>();
                        SetLabel(labels[i], playerLabelImage, "P1", Color.red);
                    }
                    else
                    {
                        SetLabel(labels[i], cpuLabelImage, "?", Color.blue);
                        gm.AIGuessTexts[aiIndex] = labels[i].GetComponentInChildren<Text>();
                        aiIndex += 1;
                    }

                    characters[i].transform.parent = positions[randomPosition].transform;
                    characters[i].transform.position = positions[randomPosition].transform.position;
                }
            }
        }
    }

    //Sets the positions of the characters to be randomized. Overloaded method to ignore the player's character
    void SetRandomPositions(int startPosition)
    {
        int randomPosition;
        int aiIndex = 0; //The index for setting which AI is where.

        for (int i = 0; i < characters.Length; i++)
        {
            if (i != ignoreCharacter && i != playerCharacter)
            {
                int catchNum = 0; //Catch number to hop out of the do-while loop if an error occurs

                do
                {
                    catchNum++;
                    randomPosition = UnityEngine.Random.Range(startPosition, 4);
                } while (positions[randomPosition].transform.childCount != 0 && catchNum < 25);

                if (catchNum == 10)
                {
                    Debug.LogError("Error setting a random position: Could not find any free positions available.");
                }
                else
                {
                    SetLabel(labels[i], cpuLabelImage, "?", Color.blue);
                    gm.AIGuessTexts[aiIndex] = labels[i].GetComponentInChildren<Text>();
                    aiIndex += 1;

                    characters[i].transform.parent = positions[randomPosition].transform;
                    characters[i].transform.position = positions[randomPosition].transform.position;
                }
                    

            }
        }
    }

    //Sets the CPU/Player labels
    void SetLabel(GameObject label, Sprite img, string msg, Color color)
    {
        label.GetComponentInChildren<Image>().sprite = img;
        label.GetComponentInChildren<Text>().text = msg;
        label.GetComponentInChildren<Text>().color = color;
    }
}
