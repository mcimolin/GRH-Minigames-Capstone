using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [SerializeField] GameObject[] charactersInScene, playerLabel; //The Characters in the scene & their set labels

    [SerializeField] Sprite playerLabelImage, cpuLabelImage;

    int selectedCharacter; //The character the player selected to play as

    int maxBalloonPumps, currentBalloonPumps;

    GRHBalloonMG_AIController aiController; //Controls the AI's decisions [Added by Bryce]

    [SerializeField] Text balloonPumpsLeftText; // The Display on the balloon of how many pumps are left [Added by Bryce]

    [SerializeField] GameObject endScreen, endScreenText; // Panel and text field that displays the end game message to the user [Added by Zane]

    [SerializeField] GameObject pumpButton1, pumpButton2, pumpButton3, howManyPumpsPanel; //players pump buttons and the question panel asking how many pumps [Added by Zane]

    string endMessage; //The end message displayed to the user ie. (you win!/you lose) [Added by Zane]

    GRHBalloonMG_AnimationController animationController;
    GRHBalloonMG_SoundManager soundManager;

    //Game scene initialization.
    void Start()
    {
        //Get the main camera / animation controller.
        mainCamera = Camera.main;
        animationController = FindObjectOfType<GRHBalloonMG_AnimationController>();
        
        //Set the sound manager.
        soundManager = FindObjectOfType<GRHBalloonMG_SoundManager>();

        //Set the AI Controller. [Added by Bryce]
        aiController = GetComponent<GRHBalloonMG_AIController>();

        //Object initialization is called here to prevent object racing. We check for objects that aren't necessarily required, such as the camera controller.
        if (mainCamera.GetComponent<GRHCameraController>())
        {
            mainCamera.GetComponent<GRHCameraController>().Initialize();
        }

        //Sets current player character selection
        selectedCharacter = GRHGameSettings.gameSettings.selectedCharacter;

        //Sets the Player's selected character & position
        animationController.SetCharacterAnimationPosition(GRHBalloonMG_AnimationController.AnimationObject.Player, charactersInScene[selectedCharacter].GetComponent<Animator>(), charactersInScene[selectedCharacter]);
        SetLabel(playerLabel[selectedCharacter], playerLabelImage, "P1", Color.red);

        // Set the rest of the AI's active states and positions
        SetAIActiveState();

        //Initializes Settings in the Animation Controller.
        animationController.Initialize();

        //Set all players to active.
        for (int i = 0; i < 4; i++)
        {
            activePlayers[i] = true;
        }

        //Get the max balloon pumps from the difficulty settings script.
        maxBalloonPumps = FindObjectOfType<GRHGameSettings>().pumpCount;

        currentBalloonPumps = 0;

        endMessage = "";

        //Update balloon pumps text.
        balloonPumpsLeftText.text = $"{maxBalloonPumps}";

        if (!FindObjectOfType<GRHGameSettings>().showPumpCount)
        {
           balloonPumpsLeftText.enabled = false;
        }

        //Start the music for the game.
        soundManager.BalloonMGGameMusic();

        //After initialization, set the game state to the introduction, and start the main camera movements, if we have a main camera controller.
        currentGameState = BalloonPopGameStates.Introduction;
        if (mainCamera.GetComponent<GRHCameraController>())
        {
            mainCamera.GetComponent<GRHCameraController>().BeginInitialMovements();
        }

        //Hide the player UI.
        HidePlayerUI();
    }

    //Sets the CPU/Player labels
    void SetLabel(GameObject label, Sprite img, string msg, Color color)
    {
        label.GetComponentInChildren<Image>().sprite = img;
        label.GetComponentInChildren<Text>().text = msg;
        label.GetComponentInChildren<Text>().color = color;
    }

    //Game advancement method.
    protected override void AdvanceGame()
    {
        Debug.Log("Advancing game.");
        switch (currentGameState)
        {
            //The introduction state will run from the game fading in, until the camera has finished setting the scene and we're ready to go.
            case BalloonPopGameStates.Introduction:
                //Introduction has finished. Begin the first turn.
                StartCoroutine(BeginFirstTurn());
                break;

            //The player turn branch.
            case BalloonPopGameStates.PlayerTurn:
                //We're advancing the game from the player turn, so no matter what, we're hiding the player UI.
                HidePlayerUI();
                break;

            //AI #1 turn branch.
            case BalloonPopGameStates.AI1Turn:
                PumpBalloon(aiController.GeneratePumpAmount(maxBalloonPumps - currentBalloonPumps));
                break;

            //AI #2 turn branch.
            case BalloonPopGameStates.AI2Turn:
                PumpBalloon(aiController.GeneratePumpAmount(maxBalloonPumps - currentBalloonPumps));
                break;

            //AI #3 turn branch.
            case BalloonPopGameStates.AI3Turn:
                PumpBalloon(aiController.GeneratePumpAmount(maxBalloonPumps - currentBalloonPumps));
                break;

            //The game end state will show the end of game visuals (ie. You Lost! or You Won!), and will run until the hub is loaded afterwards.
            case BalloonPopGameStates.GameEnd:
                Debug.Log("Displaying End Screen");
                ShowPlayerUI();
                endScreen.SetActive(true);
                pumpButton1.SetActive(false);
                pumpButton2.SetActive(false);
                pumpButton3.SetActive(false);
                howManyPumpsPanel.SetActive(false);
                endScreenText.GetComponent<Text>().text = endMessage;
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
            balloonPumpsLeftText.text = "";

            //Plays final crunch sound (does not play on knockout)
            StartCoroutine(PlayCrunchSoundEffect());
        }

        //If there's still an AI left, check if the player's still active.
        if (!gameFinished && activePlayers[0] == false)
        {
            StartCoroutine(PlayCrunchSoundEffect());
            gameFinished = true;
            balloonPumpsLeftText.text = "";
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
        switch (currentGameState)
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
        //If this is the player pumping the balloon, hide the UI to keep them from mashing the button.
        if (currentGameState == BalloonPopGameStates.PlayerTurn)
        {
            HidePlayerUI();
        }

        //Add the pumps.
        currentBalloonPumps += numberOfPumps;

        Debug.Log(currentBalloonPumps);

        //Are we at or above the maximum number of pumps?
        if (currentBalloonPumps >= maxBalloonPumps)
        {
            //We are. Knocks out the current player
            KnockOutCurrentPlayer();
            
            //Pops the balloon and resets current pumps.
            StartCoroutine(DoBalloonPopAnimation(numberOfPumps));
            currentBalloonPumps = 0;
        }

        //balloonPumpsLeftText.text = $"{maxBalloonPumps}";

        //Plays the balloon pump sound effect
        StartCoroutine(PlayPumpSoundEffect(numberOfPumps));

        //When done pumping, the turn ends. We'll call EndTurn, and that will handle the next steps.
        StartCoroutine(DoPumpAnimation(numberOfPumps));
    }

    //Method to display end of game visuals/animations.
    void EndGame()
    {
        Debug.Log("Game is ending.");
        currentGameState = BalloonPopGameStates.GameEnd;
        AdvanceGame();
    }

    IEnumerator PlayCrunchSoundEffect()
    {
        yield return new WaitForSeconds(2);
        soundManager.CrunchSound();
    }

    IEnumerator PlayPumpSoundEffect(int pumps)
    {
        if (pumps != 0)
        {
            pumps -= 1;
            soundManager.PumpSound();
            yield return new WaitForSeconds(0.9f);
            
            if (pumps != 0)
            {
                pumps -= 1;
                soundManager.PumpSound();
                yield return new WaitForSeconds(0.9f);
                
                if (pumps != 0)
                {
                    soundManager.PumpSound();
                    yield return new WaitForSeconds(0.9f);
                }
            }
        }
    }

    //Pump Animation function. Controls the current characters animation synced with the animation of the pump handle.
    IEnumerator DoPumpAnimation(int time)
    {
        //Initializes target for animation to avoid null values being past through. Defaults to the Player.
        GRHBalloonMG_AnimationController.AnimationObject target = GRHBalloonMG_AnimationController.AnimationObject.Player;

        //Sets the target to the current character who will be pumping the balloon.
        switch (currentGameState)
        {
            case BalloonPopGameStates.PlayerTurn:
                target = GRHBalloonMG_AnimationController.AnimationObject.Player;
                break;
            case BalloonPopGameStates.AI1Turn:
                target = GRHBalloonMG_AnimationController.AnimationObject.AI1;
                break;
            case BalloonPopGameStates.AI2Turn:
                target = GRHBalloonMG_AnimationController.AnimationObject.AI2;
                break;
            case BalloonPopGameStates.AI3Turn:
                target = GRHBalloonMG_AnimationController.AnimationObject.AI3;
                break;
            default:
                break;
        }

        //Begins the pump animation sequence for the target character and their state, and the pumps animation state.
        animationController.ControlPumpAnimations(target, true, 2);

        //Starts the balloon inflation animation
        StartCoroutine(animationController.IncreaseSizeOfBalloon(maxBalloonPumps, time));

        yield return new WaitForSeconds(time - 0.17f);

        //Stops the pump animation sequence for the target character and their state, and the pumps animation state.
        animationController.ControlPumpAnimations(target, false, 0);

        StartCoroutine(EndTurn());

    }

    //Balloon pop animation function. Controls the timing of the balloon popping sequence.
    IEnumerator DoBalloonPopAnimation(int time)
    {
        //Waits for the pump animation sequence to complete before popping
        yield return new WaitForSeconds(time);

        //Starts the balloon pop animation sequence
        animationController.SetBalloonState(true);

        yield return new WaitForSeconds(animationController.GetTimeForBalloonPop() - 0.25f);

        //Plays the sound effect (wait times above and below to offset pop animation delay)
        soundManager.PopSound();

        yield return new WaitForSeconds(0.25f);

        //Ends the balloon pop animation sequence
        animationController.SetBalloonState(false);
    }

    //End of turn function. Determines whether the game has ended, the animation to play, and who the next player is.
    IEnumerator EndTurn()
    {
        //Set up the variables for animations. We default these to prevent compiler errors from the switch statements not liking us handling every case.
        GRHBalloonMG_AnimationController.AnimationObject target1 = GRHBalloonMG_AnimationController.AnimationObject.Player, target2 = GRHBalloonMG_AnimationController.AnimationObject.Player;
        GRHBalloonMG_AnimationController.AnimationLocation destination1 = GRHBalloonMG_AnimationController.AnimationLocation.Pump, destination2 = GRHBalloonMG_AnimationController.AnimationLocation.Pump;

        //First off, has the game ended?
        if (HasGameFinished())
        {
            //It has. That means the character currently at the pump needs to be taken away as a single movement. Determine the character to be moved.
            //We can find this by whose turn it currently is.
            switch(currentGameState)
            {
                case BalloonPopGameStates.PlayerTurn:
                    target1 = GRHBalloonMG_AnimationController.AnimationObject.Player;
                    endMessage = "You Lose";
                    break;

                case BalloonPopGameStates.AI1Turn:
                    target1 = GRHBalloonMG_AnimationController.AnimationObject.AI1;
                    endMessage = "You Win";
                    break;

                case BalloonPopGameStates.AI2Turn:
                    target1 = GRHBalloonMG_AnimationController.AnimationObject.AI2;
                    endMessage = "You Win";
                    break;

                case BalloonPopGameStates.AI3Turn:
                    target1 = GRHBalloonMG_AnimationController.AnimationObject.AI3;
                    endMessage = "You Win";
                    break;

                default:
                    Debug.Log("Game manager's end turn is calling for a non-existent player.");
                    break;
            }

            //The destination will be the same regardless of who it is.
            destination1 = GRHBalloonMG_AnimationController.AnimationLocation.StartingLocation;

            //Now, start the animation, and wait for the right amount of time.
            animationController.MoveSingleCharacterToLocation(target1, destination1);
            yield return new WaitForSeconds(animationController.GetTimeForSingleMovement());

            //Now, we call EndGame.
            EndGame();
        } 
        else
        {
            //Determine the first target and their destination.
            switch(currentGameState)
            {
                case BalloonPopGameStates.PlayerTurn:
                    //If we're in the player state, and the player was knocked out, it would be picked up in the HasGameEnded check earlier.
                    //Therefore, if it's the player's turn, we're returning them to their position.
                    target1 = GRHBalloonMG_AnimationController.AnimationObject.Player;
                    destination1 = GRHBalloonMG_AnimationController.AnimationLocation.PlayerLocation;

                    //Determine the next game state.
                    currentGameState = DetermineNextActivePlayer(0);
                    break;

                case BalloonPopGameStates.AI1Turn:
                    //First target is AI 1.
                    target1 = GRHBalloonMG_AnimationController.AnimationObject.AI1;

                    if (activePlayers[1])
                    {
                        //AI 1 is still active. Return them to their normal location.
                        destination1 = GRHBalloonMG_AnimationController.AnimationLocation.AI1Location;
                    } else
                    {
                        //AI 1 was knocked out. Put them off the screen.
                        destination1 = GRHBalloonMG_AnimationController.AnimationLocation.StartingLocation;
                        StartCoroutine(PlayCrunchSoundEffect());
                    }

                    //Determine the next game state.
                    currentGameState = DetermineNextActivePlayer(1);
                    break;

                case BalloonPopGameStates.AI2Turn:
                    //First target is AI 2.
                    target1 = GRHBalloonMG_AnimationController.AnimationObject.AI2;

                    if (activePlayers[2])
                    {
                        //AI 2 is still active. Return them to their normal location.
                        destination1 = GRHBalloonMG_AnimationController.AnimationLocation.AI2Location;
                    }
                    else
                    {
                        //AI 2 was knocked out. Put them off the screen.
                        destination1 = GRHBalloonMG_AnimationController.AnimationLocation.StartingLocation;
                        StartCoroutine(PlayCrunchSoundEffect());
                    }

                    //Determine the next game state.
                    currentGameState = DetermineNextActivePlayer(2);
                    break;

                case BalloonPopGameStates.AI3Turn:
                    //First target is AI 3.
                    target1 = GRHBalloonMG_AnimationController.AnimationObject.AI3;

                    if (activePlayers[3])
                    {
                        //AI 3 is still active. Return them to their normal location.
                        destination1 = GRHBalloonMG_AnimationController.AnimationLocation.AI3Location;
                    }
                    else
                    {
                        //AI 3 was knocked out. Put them off the screen.
                        destination1 = GRHBalloonMG_AnimationController.AnimationLocation.StartingLocation;
                        StartCoroutine(PlayCrunchSoundEffect());
                    }

                    //Determine the next game state.
                    currentGameState = DetermineNextActivePlayer(3);
                    break;

                default:
                    Debug.Log("Game manager's end turn is checking a non-existent player.");
                    break;
            }

            //Now, using our new game state, determine the second target.
            switch(currentGameState)
            {
                case BalloonPopGameStates.PlayerTurn:
                    target2 = GRHBalloonMG_AnimationController.AnimationObject.Player;
                    break;

                case BalloonPopGameStates.AI1Turn:
                    target2 = GRHBalloonMG_AnimationController.AnimationObject.AI1;
                    break;

                case BalloonPopGameStates.AI2Turn:
                    target2 = GRHBalloonMG_AnimationController.AnimationObject.AI2;
                    break;

                case BalloonPopGameStates.AI3Turn:
                    target2 = GRHBalloonMG_AnimationController.AnimationObject.AI3;
                    break;

                default:
                    Debug.Log("Game manager's end turn is checking a non-existent player.");
                    break;
            }

            //Update the balloon pump count display if difficulty is set to easy
            if (GRHGameSettings.gameSettings.gameDifficulty == "EASY")
            {
                balloonPumpsLeftText.text = $"{maxBalloonPumps - currentBalloonPumps}";
            }

            //The second destination is always the same.
            destination2 = GRHBalloonMG_AnimationController.AnimationLocation.Pump;

            //Now, start the animation, and wait the proper amount of time.
            animationController.MoveDoubleCharacterToLocations(target1, destination1, target2, destination2);
            yield return new WaitForSeconds(animationController.GetTimeForDoubleMovement());

            //Animation has played. If this is the player turn, show the UI. Otherwise, advance the game.
            if (currentGameState == BalloonPopGameStates.PlayerTurn)
            {
                ShowPlayerUI();
            } else
            {
                AdvanceGame();
            }
        }
    }

    //Handle initial turn.
    IEnumerator BeginFirstTurn()
    {
        //Move the player to the pump.
        animationController.MoveSingleCharacterToLocation(GRHBalloonMG_AnimationController.AnimationObject.Player, GRHBalloonMG_AnimationController.AnimationLocation.Pump);
        yield return new WaitForSeconds(animationController.GetTimeForSingleMovement());

        //Set the proper game state, and show the player UI.
        currentGameState = BalloonPopGameStates.PlayerTurn;
        ShowPlayerUI();
    }

    //Sets the active AI's in the scene (Max 3 out of the 4 characters)
    private void SetAIActiveState()
    {
        //The character that will be ignored
        int ignoreCharacter = Random.Range(0, 5);

        //Select random character that isn't the player's character
        while (ignoreCharacter == selectedCharacter)
        {
            ignoreCharacter = Random.Range(0, 5);
        }

        //Disable the character & location that is to be ignored
        charactersInScene[ignoreCharacter].SetActive(false);

        //Set All characters that are enabled in the scene to their respective positions
        int AIToSet = 1;
        for (int i = 0; i < charactersInScene.Length; i++)
        {
            if (i != ignoreCharacter && i != selectedCharacter)
            {
                SetLabel(playerLabel[i], cpuLabelImage, "CPU", Color.blue);
                animationController.SetCharacterAnimationPosition((GRHBalloonMG_AnimationController.AnimationObject)AIToSet, charactersInScene[i].GetComponent<Animator>(), charactersInScene[i]);
                AIToSet++;
            }
        }
    }

    //Loads the scene again if the player wants to play again
    public void PlayAgain()
    {
        SceneManager.LoadScene("GRHBalloonMG_Scene");
    }

    //Sends the user back to the hub world when the game ends
    public void QuitGame()
    {
        soundManager.balloonMG_Audio[1].Stop();
        SceneManager.LoadScene("GRHHubWorld_SceneManager");
    }
}