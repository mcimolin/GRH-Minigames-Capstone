using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GRHBalloonMG_GameManager : MonoBehaviour
{
    //Initial framework - Joseph
    enum BalloonPopGameStates { Introduction, PlayerTurn, AI1Turn, AI2Turn, AI3Turn, GameEnd };

    BalloonPopGameStates currentGameState;

    //The array for whether a player is still in the game or not. Locations are as follows:
    //0 - Player / 1 - AI 1 / 2 - AI 2 / 3 - AI 3
    bool[] activePlayers = new bool[4];

    //Game scene initialization.
    void Start()
    {
        //Object initialization can be called here to prevent object racing.
        //Set all players to active.
        for (int i = 0; i < 4; i++)
        {
            activePlayers[i] = true;
        }

        //After initialization, set the game state to the introduction.
        currentGameState = BalloonPopGameStates.Introduction;
    }

    //Game advancement method.
    void AdvanceGame()
    {
        switch (currentGameState)
        {
            //The introduction state will run from the game fading in, until the camera has finished setting the scene and we're ready to go.
            case BalloonPopGameStates.Introduction:


                //Introduction has finished. Enter player turn state.
                currentGameState = BalloonPopGameStates.PlayerTurn;
                break;

            //The player turn branch.
            case BalloonPopGameStates.PlayerTurn:
                

                //Check at the end of the turn to see if the game has ended.
                if (HasGameFinished())
                {
                    //Game has finished. Enter game end state.
                    currentGameState = BalloonPopGameStates.GameEnd;
                }
                else
                {
                    //Game has not finished. Determine the next state, and enter it.
                    currentGameState = DetermineNextActivePlayer(0);
                }
                break;

            //AI #1 turn branch.
            case BalloonPopGameStates.AI1Turn:


                //Check at the end of the turn to see if the game has ended.
                if (HasGameFinished())
                {
                    //Game has finished. Enter game end state.
                    currentGameState = BalloonPopGameStates.GameEnd;
                }
                else
                {
                    //Game has not finished. Determine the next state, and enter it.
                    currentGameState = DetermineNextActivePlayer(1);
                }
                break;

            //AI #2 turn branch.
            case BalloonPopGameStates.AI2Turn:


                //Check at the end of the turn to see if the game has ended.
                if (HasGameFinished())
                {
                    //Game has finished. Enter game end state.
                    currentGameState = BalloonPopGameStates.GameEnd;
                }
                else
                {
                    //Game has not finished. Determine the next state, and enter it.
                    currentGameState = DetermineNextActivePlayer(2);
                }
                break;

            //AI #3 turn branch.
            case BalloonPopGameStates.AI3Turn:


                //Check at the end of the turn to see if the game has ended.
                if (HasGameFinished())
                {
                    //Game has finished. Enter game end state.
                    currentGameState = BalloonPopGameStates.GameEnd;
                }
                else
                {
                    //Game has not finished. Determine the next state, and enter it.
                    currentGameState = DetermineNextActivePlayer(3);
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
        //Create the return value. Currently using a placeholder value to prevent compiler errors.
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
                break;

            case 1:
                nextActivePlayer = BalloonPopGameStates.AI1Turn;
                break;

            case 2:
                nextActivePlayer = BalloonPopGameStates.AI2Turn;
                break;

            case 3:
                nextActivePlayer = BalloonPopGameStates.AI3Turn;
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
                activePlayers[0] = false;
                break;

            case BalloonPopGameStates.AI1Turn:
                activePlayers[1] = false;
                break;

            case BalloonPopGameStates.AI2Turn:
                activePlayers[2] = false;
                break;

            case BalloonPopGameStates.AI3Turn:
                activePlayers[3] = false;
                break;

            default:
                Debug.Log("KnockOutCurrentPlayer attempting to knock out a non-existent player.");
                break;
        }
    }
}