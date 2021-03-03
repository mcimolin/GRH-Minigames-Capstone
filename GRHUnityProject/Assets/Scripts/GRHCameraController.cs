using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simple struct used to allow designers to easily make a series of positions for the camera to move to.
[System.Serializable]
public struct CameraMovement
{
    public float xPosition, yPosition, secondsForMovement;
    [Tooltip("Camera speed will follow a Sin wave, slowest at the start and the end, and fastest in the middle.")]
    public bool sinMovementStyle;
}

public class GRHCameraController : MonoBehaviour
{
    //Initial setup - Joseph
    [SerializeField]
    CameraMovement[] startUpCameraMovements = new CameraMovement[0];

    bool performingInitialMovements;

    int initialMovementsArrayPosition;

    float currentMovementStartTime, currentMovementProgress;

    Vector3 initialPosition;

    internal void Initialize()
    {
        performingInitialMovements = false;
        initialMovementsArrayPosition = 0;
        initialPosition = transform.position;
    }
    
    void Update()
    {
        //Only do things if we're currently moving.
        if (performingInitialMovements)
        {
            //Get the progress through the current movement. This value will be between 0 and 1, with 0 being the start and 1 being the end.
            currentMovementProgress = (Time.time - currentMovementStartTime) / startUpCameraMovements[initialMovementsArrayPosition].secondsForMovement;

            //There's a possibility we're slightly over 1. Cap it if needed.
            if (currentMovementProgress > 1f)
            {
                currentMovementProgress = 1f;
            }

            float newXPosition = 0f, newYPosition = 0f;

            //Is our movement linear or sin?
            if (!startUpCameraMovements[initialMovementsArrayPosition].sinMovementStyle)
            {
                //The current movement is linear style. Linearly interpolate our position.
                //Are we on our first movement?
                if (initialMovementsArrayPosition == 0)
                {
                    //We are. Use the initial position as our start point.
                    newXPosition = ((1 - currentMovementProgress) * initialPosition.x) + (currentMovementProgress * startUpCameraMovements[initialMovementsArrayPosition].xPosition);
                    newYPosition = ((1 - currentMovementProgress) * initialPosition.y) + (currentMovementProgress * startUpCameraMovements[initialMovementsArrayPosition].yPosition);
                } else
                {
                    //We aren't. Use the previous movement's final position as our start point.
                    newXPosition = ((1 - currentMovementProgress) * startUpCameraMovements[initialMovementsArrayPosition - 1].xPosition) + (currentMovementProgress * startUpCameraMovements[initialMovementsArrayPosition].xPosition);
                    newYPosition = ((1 - currentMovementProgress) * startUpCameraMovements[initialMovementsArrayPosition - 1].yPosition) + (currentMovementProgress * startUpCameraMovements[initialMovementsArrayPosition].yPosition);
                }
            } else
            {
                //The current movement is sin style. Determine our sin percentage value to use for how much of the mid position is being used.
                //Normally, this results in faster movement at the start, and slower in the middle. Therefore, we need to start at pi/2 and move out.
                float nonMidPositionPercentage;

                //Are we in the first or second half?
                if (currentMovementProgress <= 0.5f)
                {
                    //We're in the first half.
                    nonMidPositionPercentage = Mathf.Sin(Mathf.PI / 2 * ((0.5f - currentMovementProgress) / 0.5f));
                } else
                {
                    //We're in the second half.
                    nonMidPositionPercentage = Mathf.Sin(Mathf.PI / 2 * ((currentMovementProgress - 0.5f) / 0.5f));
                }

                //Are we on our first movement?
                if (initialMovementsArrayPosition == 0)
                {
                    //We are. Use the initial position as our start point. Determine our midpoint.
                    float midXPosition = (initialPosition.x + startUpCameraMovements[0].xPosition) / 2;
                    float midYPosition = (initialPosition.y + startUpCameraMovements[0].yPosition) / 2;

                    //Where are we in terms of progress?
                    if (currentMovementProgress < 0.5f)
                    {
                        //We're less than halfway.
                        //Use the mid position percentage with the initial and mid positions.
                        newXPosition = (nonMidPositionPercentage * initialPosition.x) + ((1 - nonMidPositionPercentage) * midXPosition);
                        newYPosition = (nonMidPositionPercentage * initialPosition.y) + ((1 - nonMidPositionPercentage) * midYPosition);
                    } else if (currentMovementProgress == 0.5f)
                    {
                        //We're exactly halfway. Our new position is the midpoint.
                        newXPosition = midXPosition;
                        newYPosition = midYPosition;
                    } else
                    {
                        //We're more than halfway.
                        //Use the mid position percentage with the mid and final positions.
                        newXPosition = ((1 - nonMidPositionPercentage) * midXPosition) + (nonMidPositionPercentage * startUpCameraMovements[0].xPosition);
                        newYPosition = ((1 - nonMidPositionPercentage) * midYPosition) + (nonMidPositionPercentage * startUpCameraMovements[0].yPosition);
                    }
                } else
                {
                    //We aren't. Use the previous movement's final position as our start point. Determine our midpoint.
                    float midXPosition = (startUpCameraMovements[initialMovementsArrayPosition - 1].xPosition + startUpCameraMovements[initialMovementsArrayPosition].xPosition) / 2;
                    float midYPosition = (startUpCameraMovements[initialMovementsArrayPosition - 1].yPosition + startUpCameraMovements[initialMovementsArrayPosition].yPosition) / 2;

                    //Where are we in terms of progress?
                    if (currentMovementProgress < 0.5f)
                    {
                        //We're less than halfway.
                        //Use the mid position percentage with the initial and mid positions.
                        newXPosition = (nonMidPositionPercentage * startUpCameraMovements[initialMovementsArrayPosition - 1].xPosition) + ((1 - nonMidPositionPercentage) * midXPosition);
                        newYPosition = (nonMidPositionPercentage * startUpCameraMovements[initialMovementsArrayPosition - 1].yPosition) + ((1 - nonMidPositionPercentage) * midYPosition);
                    }
                    else if (currentMovementProgress == 0.5f)
                    {
                        //We're exactly halfway. Our new position is the midpoint.
                        newXPosition = midXPosition;
                        newYPosition = midYPosition;
                    }
                    else
                    {
                        //We're more than halfway.
                        //Use the mid position percentage with the mid and final positions.
                        newXPosition = ((1 - nonMidPositionPercentage) * midXPosition) + (nonMidPositionPercentage * startUpCameraMovements[initialMovementsArrayPosition].xPosition);
                        newYPosition = ((1 - nonMidPositionPercentage) * midYPosition) + (nonMidPositionPercentage * startUpCameraMovements[initialMovementsArrayPosition].yPosition);
                    }
                }
            }

            //Set our position to the calculated position.
            transform.position = new Vector3(newXPosition, newYPosition, initialPosition.z);

            //Have we reached 100% progress through the current movement?
            if (currentMovementProgress == 1f)
            {
                //We have. Do we have any more movements to make?
                if (initialMovementsArrayPosition + 1 == startUpCameraMovements.Length)
                {
                    //We don't. Stop moving, and tell the game manager to advance the game.
                    performingInitialMovements = false;
                    FindObjectOfType<GRH_GameManager>().BroadcastMessage("AdvanceGame");
                } else
                {
                    //We do.
                    initialMovementsArrayPosition++;
                    currentMovementStartTime = Time.time;
                }
            }
        }
    }

    internal void BeginInitialMovements()
    {
        //Do we have initial movements to perform?
        if (startUpCameraMovements.Length > 0)
        {
            //We do. Set moving to true, and set start time for the first movement.
            performingInitialMovements = true;
            currentMovementStartTime = Time.time;
        } else
        {
            //We have no movements to perform. Tell the game manager to advance the game.
            FindObjectOfType<GRH_GameManager>().BroadcastMessage("AdvanceGame");
        }
    }
}