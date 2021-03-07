using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GRHBalloonMG_AnimationController : MonoBehaviour
{
    //Initial setup - Joseph
    //The various objects we'll interact with. Objects are defaulted to null to prevent displaying ten million warnings when compiling.
    [SerializeField]
    GameObject deathHandObject = null, playerObject = null, AI1Object = null, AI2Object = null, AI3Object = null;

    //The locations for character movement. Objects are also defaulted to null.
    [SerializeField]
    GameObject pumpLocation = null, playerLocation = null, AI1Location = null, AI2Location = null, AI3Location = null, startingLocation = null;

    //The animator for each of the characters. Defaults all to null;
    [SerializeField]
    Animator playerAnimator = null, AI1Animator = null, AI2Animator = null, AI3Animator = null;

    //The animator on the balloon, for idle / pop toggling. Default to null.
    [SerializeField]
    Animator balloonAnimator = null;

    //The animation clips for balloon inflation and popping. Default to null
    [SerializeField]
    AnimationClip balloonInflationClip = null, balloonPopClip = null;

    //The animator on the pump, for idle / pump toggling. Default to null.
    [SerializeField]
    Animator pumpAnimator;

    //The animator on the hand, for open / closed toggling. Again, defaulted to null.
    [SerializeField]
    Animator handAnimator = null;

    //The time it takes to complete a movement.
    [SerializeField]
    internal float movementTime;

    //Used to store where the hand will be moving.
    GameObject objectTarget1, objectTarget2, destination1, destination2;

    //Enums used to let other scripts communicate to this one which objects and locations we're using.
    internal enum AnimationObject { Player, AI1, AI2, AI3 };
    internal enum AnimationLocation { Pump, PlayerLocation, AI1Location, AI2Location, AI3Location, StartingLocation };

    //Animation states for the controller.
    /*States work as follows:
     * SingleMovement1 - Moving from start position to character to move.
     * SingleMovement2 - Moving character to target location.
     * DuoMovement1 - Moving from start position to first character to move.
     * DuoMovement2 - Moving first character to first location.
     * DuoMovement3 - Moving from first location to second character.
     * DuoMovement4 - Moving second character to second location.
     * Returning - Moving from current position back to starting location in the sky.
     * Idle - Doing nothing.
     */
    enum AnimationState { SingleMovement1, SingleMovement2, DuoMovement1, DuoMovement2, DuoMovement3, DuoMovement4, Returning, Idle };

    AnimationState currentState;

    //Reference starting position for movements.
    Vector3 movementStartPosition;

    //Reference time for movements.
    float movementStartTime;

    //Variables used for calculations of new positions.
    float newXPosition, newYPosition, movementProgress;

    //Initialize the object.
    internal void Initialize()
    {
        //If we don't have a movement time set, set it to 2 seconds.
        if (movementTime == 0f)
        {
            movementTime = 2f;
        }

        //Default to the idle state.
        currentState = AnimationState.Idle;

        //Set the balloons clip speed to zero to pause animation on startup
        balloonAnimator.speed = 0;
    }

    void Update()
    {
        //If we're not idle, calculate the progress through the current motion.
        if (currentState != AnimationState.Idle)
        {
            movementProgress = (Time.time - movementStartTime) / movementTime;

            //Cap at 1 if needed.
            if (movementProgress > 1f)
            {
                movementProgress = 1f;
            }
        }

        //Perform location calculations based on the current state.
        switch (currentState)
        {
            case AnimationState.Idle:
                //Do nothing if idle.
                break;

            case AnimationState.SingleMovement1:
                //We're moving towards the character.
                newXPosition = ((1 - movementProgress) * movementStartPosition.x) + (movementProgress * objectTarget1.transform.position.x);
                newYPosition = ((1 - movementProgress) * movementStartPosition.y) + (movementProgress * objectTarget1.transform.position.y);

                //Update hand position.
                deathHandObject.transform.position = new Vector3(newXPosition, newYPosition, startingLocation.transform.position.z);

                //Have we finished the movement?
                if (movementProgress == 1f)
                {
                    //Move to the next stage, SingleMovement2.
                    currentState = AnimationState.SingleMovement2;
                    movementStartPosition = deathHandObject.transform.position;
                    movementStartTime = Time.time;

                    //Toggle hand to closed.
                    handAnimator.SetTrigger("ToggleHandState");
                }
                break;

            case AnimationState.SingleMovement2:
                //We're moving towards the destination.
                newXPosition = ((1 - movementProgress) * movementStartPosition.x) + (movementProgress * destination1.transform.position.x);
                newYPosition = ((1 - movementProgress) * movementStartPosition.y) + (movementProgress * destination1.transform.position.y);

                //Update hand and character position.
                deathHandObject.transform.position = new Vector3(newXPosition, newYPosition, startingLocation.transform.position.z);
                objectTarget1.transform.position = new Vector3(newXPosition, newYPosition, startingLocation.transform.position.z);

                //Have we finished the movement?
                if (movementProgress == 1f)
                {
                    //In the case of eliminating the last remaining character, we'll already be at the starting location. In that case, we can just enter idle.
                    if (deathHandObject.transform.position == startingLocation.transform.position)
                    {
                        //We are at the start. Enter the idle state.
                        currentState = AnimationState.Idle;
                    } else
                    {
                        //We aren't at the start, so move to the next stage, Returning.
                        currentState = AnimationState.Returning;
                        movementStartPosition = deathHandObject.transform.position;
                        movementStartTime = Time.time;
                    }

                    //Toggle hand to open.
                    handAnimator.SetTrigger("ToggleHandState");
                }
                break;

            case AnimationState.DuoMovement1:
                //We're moving towards the first character.
                newXPosition = ((1 - movementProgress) * movementStartPosition.x) + (movementProgress * objectTarget1.transform.position.x);
                newYPosition = ((1 - movementProgress) * movementStartPosition.y) + (movementProgress * objectTarget1.transform.position.y);

                //Update hand position.
                deathHandObject.transform.position = new Vector3(newXPosition, newYPosition, startingLocation.transform.position.z);

                //Have we finished the movement?
                if (movementProgress == 1f)
                {
                    //Move to the next stage, DuoMovement2.
                    currentState = AnimationState.DuoMovement2;
                    movementStartPosition = deathHandObject.transform.position;
                    movementStartTime = Time.time;

                    //Toggle hand to closed.
                    handAnimator.SetTrigger("ToggleHandState");
                }
                break;

            case AnimationState.DuoMovement2:
                //We're moving towards the first destination.
                newXPosition = ((1 - movementProgress) * movementStartPosition.x) + (movementProgress * destination1.transform.position.x);
                newYPosition = ((1 - movementProgress) * movementStartPosition.y) + (movementProgress * destination1.transform.position.y);

                //Update hand and character position.
                deathHandObject.transform.position = new Vector3(newXPosition, newYPosition, startingLocation.transform.position.z);
                objectTarget1.transform.position = new Vector3(newXPosition, newYPosition, startingLocation.transform.position.z);

                //Have we finished the movement?
                if (movementProgress == 1f)
                {
                    //Move to the next stage, DuoMovement3.
                    currentState = AnimationState.DuoMovement3;
                    movementStartPosition = deathHandObject.transform.position;
                    movementStartTime = Time.time;

                    //Toggle hand to open.
                    handAnimator.SetTrigger("ToggleHandState");
                }
                break;

            case AnimationState.DuoMovement3:
                //We're moving towards the second character.
                newXPosition = ((1 - movementProgress) * movementStartPosition.x) + (movementProgress * objectTarget2.transform.position.x);
                newYPosition = ((1 - movementProgress) * movementStartPosition.y) + (movementProgress * objectTarget2.transform.position.y);

                //Update hand position.
                deathHandObject.transform.position = new Vector3(newXPosition, newYPosition, startingLocation.transform.position.z);

                //Have we finished the movement?
                if (movementProgress == 1f)
                {
                    //Move to the next stage, DuoMovement4.
                    currentState = AnimationState.DuoMovement4;
                    movementStartPosition = deathHandObject.transform.position;
                    movementStartTime = Time.time;

                    //Toggle hand to closed.
                    handAnimator.SetTrigger("ToggleHandState");
                }
                break;

            case AnimationState.DuoMovement4:
                //We're moving towards the second destination.
                newXPosition = ((1 - movementProgress) * movementStartPosition.x) + (movementProgress * destination2.transform.position.x);
                newYPosition = ((1 - movementProgress) * movementStartPosition.y) + (movementProgress * destination2.transform.position.y);

                //Update hand and character position.
                deathHandObject.transform.position = new Vector3(newXPosition, newYPosition, startingLocation.transform.position.z);
                objectTarget2.transform.position = new Vector3(newXPosition, newYPosition, startingLocation.transform.position.z);

                //Have we finished the movement?
                if (movementProgress == 1f)
                {
                    //Move to the next stage, Returning.
                    currentState = AnimationState.Returning;
                    movementStartPosition = deathHandObject.transform.position;
                    movementStartTime = Time.time;

                    //Toggle hand to open.
                    handAnimator.SetTrigger("ToggleHandState");
                }
                break;

            case AnimationState.Returning:
                //We're returning to the start position.
                newXPosition = ((1 - movementProgress) * movementStartPosition.x) + (movementProgress * startingLocation.transform.position.x);
                newYPosition = ((1 - movementProgress) * movementStartPosition.y) + (movementProgress * startingLocation.transform.position.y);

                //Update hand position.
                deathHandObject.transform.position = new Vector3(newXPosition, newYPosition, startingLocation.transform.position.z);

                //Have we finished the movement?
                if (movementProgress == 1f)
                {
                    //Return to the idle state.
                    currentState = AnimationState.Idle;
                }
                break;

            default:
                Debug.Log("Animation controller in an unrecognized state.");
                break;
        }
    }

    //Sets the location of the character's position, their animator and the character's object.
    internal void SetCharacterAnimationPosition(AnimationObject animCharacter, Animator animator, GameObject character, GameObject location)
    {
        switch (animCharacter)
        {
            case AnimationObject.Player:
                playerObject = character;
                playerAnimator = animator;
                playerLocation = location;
                break;
            case AnimationObject.AI1:
                AI1Object = character;
                AI1Animator = animator;
                AI1Location = location;
                break;
            case AnimationObject.AI2:
                AI2Object = character;
                AI2Animator = animator;
                AI2Location = location;
                break;
            case AnimationObject.AI3:
                AI3Object = character;
                AI3Animator = animator;
                AI3Location = location;
                break;
            default:
                Debug.LogError("Error: No character was selected to be saved.");
                break;
        }
    }

    //Controls the current character's pump animation state and the pump handles animation state
    internal void ControlPumpAnimations(AnimationObject character, bool pumpState, int characterState)
    {
        //Starts the animations for pump movement and characters pump animation
        pumpAnimator.SetBool("Pump", pumpState);
        switch (character)
        {
            case AnimationObject.Player:
                playerAnimator.SetInteger("Character State", characterState);
                break;
            case AnimationObject.AI1:
                AI1Animator.SetInteger("Character State", characterState);
                break;
            case AnimationObject.AI2:
                AI2Animator.SetInteger("Character State", characterState);
                break;
            case AnimationObject.AI3:
                AI3Animator.SetInteger("Character State", characterState);
                break;
            default:
                break;
        }
    }

    //Balloon's State: true = Popping / false = Inflating (or idle)
    internal void SetBalloonState(bool state, float speed)
    {
        balloonAnimator.speed = speed;
        Debug.Log(balloonAnimator.speed);

        if (state)
        {
            //balloonSprite.sprite = balloonPopSprite;
            balloonAnimator.Play("Balloon_Pop");
        }
        else
        {
            //balloonSprite.sprite = balloonInflationSprite;
            balloonAnimator.Play("GRHBalloonMG_BigBalloon");
        }

    }

    //Returns how long the pop animation plays
    internal float GetTimeForBalloonPop()
    {
        return balloonPopClip.length;
    }

    //Increases the size of the balloon
    internal IEnumerator IncreaseSizeOfBalloon(int maxPumps, int time)
    {
        //Set the speed of the balloon animator to match the amount of pumps required.
        balloonAnimator.speed = balloonInflationClip.length / maxPumps;

        yield return new WaitForSeconds(time);

        //Sets the speed to zero to pause animation.
        balloonAnimator.speed = 0;
    }

    internal void MoveSingleCharacterToLocation(AnimationObject objectTarget, AnimationLocation targetLocation)
    {
        //Set our values used for actual movement.
        //Set reference start position.
        movementStartPosition = startingLocation.transform.position;

        //Set the target.
        switch (objectTarget)
        {
            case AnimationObject.Player:
                objectTarget1 = playerObject;
                break;

            case AnimationObject.AI1:
                objectTarget1 = AI1Object;
                break;

            case AnimationObject.AI2:
                objectTarget1 = AI2Object;
                break;

            case AnimationObject.AI3:
                objectTarget1 = AI3Object;
                break;

            default:
                Debug.Log("Animation controller attempting to move a non-existent object.");
                break;
        }

        //Set the destination.
        switch (targetLocation)
        {
            case AnimationLocation.Pump:
                destination1 = pumpLocation;
                break;

            case AnimationLocation.PlayerLocation:
                destination1 = playerLocation;
                break;

            case AnimationLocation.AI1Location:
                destination1 = AI1Location;
                break;

            case AnimationLocation.AI2Location:
                destination1 = AI2Location;
                break;

            case AnimationLocation.AI3Location:
                destination1 = AI3Location;
                break;

            case AnimationLocation.StartingLocation:
                destination1 = startingLocation;
                break;

            default:
                Debug.Log("Animation controller attempting to move to a non-existent location.");
                break;
        }

        //Set the controller to do a single movement.
        movementStartTime = Time.time;
        currentState = AnimationState.SingleMovement1;
    }

    internal void MoveDoubleCharacterToLocations(AnimationObject firstTarget, AnimationLocation firstDestination, AnimationObject secondTarget, AnimationLocation secondDestination)
    {
        //Set our values used for actual movement.
        //Set reference start position.
        movementStartPosition = startingLocation.transform.position;

        //Set the first target.
        switch (firstTarget)
        {
            case AnimationObject.Player:
                objectTarget1 = playerObject;
                break;

            case AnimationObject.AI1:
                objectTarget1 = AI1Object;
                break;

            case AnimationObject.AI2:
                objectTarget1 = AI2Object;
                break;

            case AnimationObject.AI3:
                objectTarget1 = AI3Object;
                break;

            default:
                Debug.Log("Animation controller attempting to move a non-existent object.");
                break;
        }

        //Set the first destination.
        switch (firstDestination)
        {
            case AnimationLocation.Pump:
                destination1 = pumpLocation;
                break;

            case AnimationLocation.PlayerLocation:
                destination1 = playerLocation;
                break;

            case AnimationLocation.AI1Location:
                destination1 = AI1Location;
                break;

            case AnimationLocation.AI2Location:
                destination1 = AI2Location;
                break;

            case AnimationLocation.AI3Location:
                destination1 = AI3Location;
                break;

            case AnimationLocation.StartingLocation:
                destination1 = startingLocation;
                break;

            default:
                Debug.Log("Animation controller attempting to move to a non-existent location.");
                break;
        }

        //Set the second target.
        switch (secondTarget)
        {
            case AnimationObject.Player:
                objectTarget2 = playerObject;
                break;

            case AnimationObject.AI1:
                objectTarget2 = AI1Object;
                break;

            case AnimationObject.AI2:
                objectTarget2 = AI2Object;
                break;

            case AnimationObject.AI3:
                objectTarget2 = AI3Object;
                break;

            default:
                Debug.Log("Animation controller attempting to move a non-existent object.");
                break;
        }

        //Set the second destination.
        switch (secondDestination)
        {
            case AnimationLocation.Pump:
                destination2 = pumpLocation;
                break;

            case AnimationLocation.PlayerLocation:
                destination2 = playerLocation;
                break;

            case AnimationLocation.AI1Location:
                destination2 = AI1Location;
                break;

            case AnimationLocation.AI2Location:
                destination2 = AI2Location;
                break;

            case AnimationLocation.AI3Location:
                destination2 = AI3Location;
                break;

            case AnimationLocation.StartingLocation:
                destination2 = startingLocation;
                break;

            default:
                Debug.Log("Animation controller attempting to move to a non-existent location.");
                break;
        }

        //Set the controller to do a duo movement.
        movementStartTime = Time.time;
        currentState = AnimationState.DuoMovement1;
    }

    //Return the time it takes for a single character movement, for game pace purposes.
    internal float GetTimeForSingleMovement()
    {
        return movementTime * 3f;
    }

    //Return the time it takes for a double character movement, for game pace purposes.
    internal float GetTimeForDoubleMovement()
    {
        return movementTime * 5f;
    }
}