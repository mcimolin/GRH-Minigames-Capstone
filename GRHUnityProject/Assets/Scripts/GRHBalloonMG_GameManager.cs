using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//The balloon pop game manager extends from the base game manager class.
public class GRHBalloonMG_GameManager : GRH_GameManager
{
    //Initial setup - Joseph
    enum BalloonPopGameStates { Introduction, PlayerTurn, AI1Turn, AI2Turn, AI3Turn, GameEnd };

    BalloonPopGameStates currentGameState;

    //The array for whether a player is still in the game or not. Locations are as follows:
    //0 - Player / 1 - AI 1 / 2 - AI 2 / 3 - AI 3
    bool[] activePlayers = new bool[4];

    int maxBalloonPumps, currentBalloonPumps;

    GRHBalloonMG_AIController aiController; //Controls th AI's decisions [Added by Bryce]

    [SerializeField] Text balloonPumpsLeftText; // The Display on the balloon of how many pumps are left [Added by Bryce]

    GRHBalloonMG_AnimationController animationController;

    //Game scene initialization.
    void Start()
    {
        //Get the main camera / animation controller.
        mainCamera = Camera.main;
        animationController = FindObjectOfType<GRHBalloonMG_AnimationController>();

        //Set the AI Controller [Added by Bryce]
        aiController = GetComponent<GRHBalloonMG_AIController>();

        //Object initialization is called here to prevent object racing. We check for objects that aren't necessarily required, such as the camera controller.
        if (mainCamera.GetComponent<GRHCameraController>())
        {
            mainCamera.GetComponent<GRHCameraController>().Initialize();
        }

        animationController.Initialize();

        //Set all players to active.
        for (int i = 0; i < 4; i++)
        {
            activePlayers[i] = true;
        }

        //Get the max balloon pumps from the difficulty selection script here. Using a temp value for now.
        maxBalloonPumps = 10;

        currentBalloonPumps = 0;

        //After initialization, set the game state to the introduction, and start the main camera movements, if we have a main camera controller.
        currentGameState = BalloonPopGameStates.Introduction;
        if (mainCamera.GetComponent<GRHCameraController>())
        {
            mainCamera.GetComponent<GRHCameraController>().BeginInitialMovements();
        }

        //Hide the player UI.
        HidePlayerUI();
    }

    //Game advancement method.
    protected override void AdvanceGame()
    {
        Debug.Log("Advancing game.");
        switch (currentGameState)
        {
            //The introduction state will run from the game fading in, until the camera has finished setting the scene and we're ready to go.
            case BalloonPopGameStates.Introduction:


                //Introduction has finished. Enter player turn state.
                currentGameState = BalloonPopGameStates.PlayerTurn;
                break;

            //The player turn branch.
            case BalloonPopGameStates.PlayerTurn:
                //We're advancing the game from the player turn, so no matter what, we're hiding the player UI.
                HidePlayerUI();

                //Check at the end of the turn to see if the game has ended.
                if (HasGameFinished())
                {
                    //Game has finished. End the game.
                    EndGame();
                }
                else
                {
                    //Game has not finished. Determine the next state, and enter it.
                    currentGameState = DetermineNextActivePlayer(0);
                }
                break;

            //AI #1 turn branch.
            case BalloonPopGameStates.AI1Turn:

                //Access AI's pump amount and set it [Added by Bryce]
                PumpBalloon(aiController.GeneratePumpAmount(maxBalloonPumps - currentBalloonPumps));

                //Check at the end of the turn to see if the game has ended.
                if (HasGameFinished())
                {
                    //Game has finished. End the game.
                    EndGame();
                }
                else
                {
                    //Game has not finished. Determine the next state, and enter it.
                    currentGameState = DetermineNextActivePlayer(1);

                    //If the game moves to the player's turn, we need to show the player UI.
                    if (currentGameState == BalloonPopGameStates.PlayerTurn)
                    {
                        ShowPlayerUI();
                    }
                }
                break;

            //AI #2 turn branch.
            case BalloonPopGameStates.AI2Turn:

                //Access AI's pump amount and set it [Added by Bryce]
                PumpBalloon(aiController.GeneratePumpAmount(maxBalloonPumps - currentBalloonPumps));

                //Check at the end of the turn to see if the game has ended.
                if (HasGameFinished())
                {
                    //Game has finished. End the game.
                    EndGame();
                }
                else
                {
                    //Game has not finished. Determine the next state, and enter it.
                    currentGameState = DetermineNextActivePlayer(2);

                    //If the game moves to the player's turn, we need to show the player UI.
                    if (currentGameState == BalloonPopGameStates.PlayerTurn)
                    {
                        ShowPlayerUI();
                    }
                }
                break;

            //AI #3 turn branch.
            case BalloonPopGameStates.AI3Turn:

                //Access AI's pump amount and set it [Added by Bryce]
                PumpBalloon(aiController.GeneratePumpAmount(maxBalloonPumps - currentBalloonPumps));

                //Check at the end of the turn to see if the game has ended.
                if (HasGameFinished())
                {
                    //Game has finished. End the game.
                    EndGame();
                }
                else
                {
                    //Game has not finished. Determine the next state, and enter it.
                    currentGameState = DetermineNextActivePlayer(3);

                    //If the game moves to the player's turn, we need to show the player UI.
                    if (currentGameState == BalloonPopGameStates.PlayerTurn)
                    {
                        ShowPlayerUI();
                    }
                }
                break;

            //The game end state will show the end of game visuals (ie. You Lost! or You Won!), and will run until the hub is loaded afterwards.
            case BalloonPopGameStates.GameEnd:

                break;

            //Default branch where we should never end up.
            default:
                Debug.Log("Main game state not recognized.");
                break;
        }
    }

    //Ask if the game has finished or not. Returns true if it has, false if it hasn't.
    bool HasGameFinished()
    {
        bool gameFinished = false;

        //Game will determine if all the AIs are knocked out, or if the player is.
        //Start with checking AI status.
        if (activePlayers[1] == false && activePlayers[2] == false && activePlayers[3] == false)
        {
            //All AI are knocked out. Game has finished.
            gameFinished = true;
        }

        //If there's still an AI left, check if the player's still active.
        if (!gameFinished && activePlayers[0] == false)
        {
            gameFinished = true;
        }

        //Return the final value.
        return gameFinished;
    }

    //Determine who the next active player is, and return the associated state.
    BalloonPopGameStates DetermineNextActivePlayer(int currentPlayerPosition)
    {
        //Create the return value. Assigns a value to prevent compiler errors.
        BalloonPopGameStates nextActivePlayer = BalloonPopGameStates.PlayerTurn;

        //To handle looping around, we'll use a bool to determine if we need to loop around at all.
        bool hasDeterminedNextPlayer = false;

        //Create the int to hold the next player's position.
        int nextPlayerPosition = 0;

        //If this is the last player, there's no one ahead to look at.
        if (currentPlayerPosition != 3)
        {
            //Check if the players ahead of the current player are active.
            for (int i = currentPlayerPosition + 1; i < 4; i++)
            {
                if (activePlayers[i] == true)
                {
                    //This player is active. Set the player position, set the determination bool to true, and break out of the loop.
                    nextPlayerPosition = i;
                    hasDeterminedNextPlayer = true;
                    break;
                }
            }
        }

        //Next, handle the players behind the current player, if none ahead were active.
        if (!hasDeterminedNextPlayer)
        {
            for (int i = 0; i < currentPlayerPosition; i++)
            {
                if (activePlayers[i] == true)
                {
                    //This player is active. Set the player position, and break out of the loop.
                    nextPlayerPosition = i;
                    break;
                }
            }
        }

        //By this point, we know the next active player by the position we have.
        switch (nextPlayerPosition)
        {
            case 0:
                nextActivePlayer = BalloonPopGameStates.PlayerTurn;
                Debug.Log("Player turn.");
                break;

            case 1:
                nextActivePlayer = BalloonPopGameStates.AI1Turn;
                Debug.Log("AI 1 turn.");
                break;

            case 2:
                nextActivePlayer = BalloonPopGameStates.AI2Turn;
                Debug.Log("AI 2 turn.");
                break;

            case 3:
                nextActivePlayer = BalloonPopGameStates.AI3Turn;
                Debug.Log("AI 3 turn.");
                break;

            default:
                Debug.Log("DetermineNextActivePlayer returned a non-existent player.");
                break;
        }

        //Return the proper game state.
        return nextActivePlayer;
    }

    //Knock out the current player.
    internal void KnockOutCurrentPlayer()
    {
        //Set active to false depending on the current state.
        switch(currentGameState)
        {
            case BalloonPopGameStates.PlayerTurn:
                Debug.Log("Knocking out Player.");
                activePlayers[0] = false;
                break;

            case BalloonPopGameStates.AI1Turn:
                Debug.Log("Knocking out AI 1.");
                activePlayers[1] = false;
                break;

            case BalloonPopGameStates.AI2Turn:
                Debug.Log("Knocking out AI 2.");
                activePlayers[2] = false;
                break;

            case BalloonPopGameStates.AI3Turn:
                Debug.Log("Knocking out AI 3.");
                activePlayers[3] = false;
                break;

            default:
                Debug.Log("KnockOutCurrentPlayer attempting to knock out a non-existent player.");
                break;
        }
    }

    //Add a number of pumps to the balloon. Method is public to allow UI buttons to use it.
    public void PumpBalloon(int numberOfPumps)
    {
        //Add the pumps.
        currentBalloonPumps += numberOfPumps;

        //Are we at or above the maximum number of pumps?
        if (currentBalloonPumps >= maxBalloonPumps)
        {
            //We are. Knock out the current player.
            KnockOutCurrentPlayer();
            currentBalloonPumps = 0;
        }

        //The animation controller should handle when to advance the game, so we can have animations play out before moving to the next player.
        //Therefore, advance game won't be called from here.
        Debug.Log(currentBalloonPumps);
        AdvanceGame();
    }

    //Method to display end of game visuals/animations.
    void EndGame()
    {
        Debug.Log("Game is ending.");
        currentGameState = BalloonPopGameStates.GameEnd;
        AdvanceGame();
    }
}