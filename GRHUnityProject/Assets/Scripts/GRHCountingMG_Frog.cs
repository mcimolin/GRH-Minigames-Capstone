using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class extends off of base moving entity class.
public class GRHCountingMG_Frog : GRHCountingMG_MovingEntity
{
    //Setup - Joseph
    [SerializeField]
    float minimumJumpDistance, maximumJumpDistance, jumpHeight;

    enum FrogState { Jumping, Waiting };

    FrogState currentState;

    float jumpDistance, remainingDistance, jumpStartTime, movementProgress, newXposition, newYPosition, newZposition;

    Vector3 jumpEndPoint, jumpStartPoint;

    Vector2 groundedPosition;

    //Initalize the frog to the waiting state, determine it's waiting time, and then call the base initialize method.
    internal override void Initialize()
    {
        //Start by calling the base initialize method, which will determine a destination and enable movement.
        base.Initialize();

        //Get the animator for toggling jump animations.
        animator = GetComponent<Animator>();

        currentState = FrogState.Waiting;

        waitStartTime = Time.time;
        waitTime = Random.Range(minimumWaitTime, maximumWaitTime);

        //We don't want all the frogs to jump at about the same time, so reduce the initial waiting time by between 0 and the wait time.
        waitTime -= Random.Range(0f, waitTime);
    }
    
    void Update()
    {
        //Are we still fading in?
        if (fadingIn)
        {
            base.Update();
        }
        //Is movement enabled?
        if (movementEnabled)
        {
            //Perform behaviour based on the current state.
            switch (currentState)
            {
                case FrogState.Waiting:
                    //Have we finished waiting for the next jump?
                    if (Time.time >= waitStartTime + waitTime)
                    {
                        //We're done waiting. First, determine the jump distance.
                        jumpDistance = Random.Range(minimumJumpDistance, maximumJumpDistance);

                        //Is this distance enough to reach the current destination?
                        remainingDistance = (transform.position - destinationPoint).magnitude;

                        if (jumpDistance >= remainingDistance)
                        {
                            //We can reach the destination with this jump. Therefore, the jump end point will be the destination point.
                            //Update the jump distance for time calculations.
                            jumpEndPoint = destinationPoint;
                            jumpDistance = remainingDistance;
                        }
                        else
                        {
                            //We won't reach the destination with this jump. Create a normalized vector for direction, and get the jump end point.
                            Vector3 direction = destinationPoint - transform.position;
                            direction.Normalize();

                            jumpEndPoint = transform.position + direction * jumpDistance;
                        }

                        //Finally, enter the jumping state and set the jump start point.
                        jumpStartPoint = transform.position;
                        jumpStartTime = Time.time;
                        currentState = FrogState.Jumping;
                        animator.SetTrigger("JumpToggle");
                    }
                    break;

                case FrogState.Jumping:
                    //First, determine if the jump will reach the end point this frame or not. We need to use the X and Z values, ignoring Y, as it will be in the air.
                    groundedPosition = new Vector2(transform.position.x, transform.position.z);
                    Vector2 targetPosition = new Vector2(jumpEndPoint.x, jumpEndPoint.z);

                    remainingDistance = (groundedPosition - targetPosition).magnitude;

                    if (remainingDistance <= speed * Time.deltaTime)
                    {
                        //We will reach it. We can simply set the frog's position to the jump's end point.
                        transform.position = jumpEndPoint;

                        //Now, are we at our final destination or not?
                        if (transform.position == destinationPoint)
                        {
                            //We are. Determine a new destination for the frog to go to.
                            DetermineDestination();
                        }

                        //Whether we're there or not, we now start waiting.
                        waitStartTime = Time.time;
                        waitTime = Random.Range(minimumWaitTime, maximumWaitTime);
                        currentState = FrogState.Waiting;
                        animator.SetTrigger("JumpToggle");
                    } else
                    {
                        //We won't reach it. Calculate our progress through the jump. The animation for the jump is 1 second, so the calculation is simple.
                        movementProgress = Time.time - jumpStartTime;

                        //Cap the progress to 1.
                        if (movementProgress > 1)
                        {
                            movementProgress = 1;
                        }

                        //Determine the new X and Z positions through linear interpolation.
                        newXposition = (1 - movementProgress) * jumpStartPoint.x + movementProgress * jumpEndPoint.x;
                        newZposition = (1 - movementProgress) * jumpStartPoint.z + movementProgress * jumpEndPoint.z;

                        //Determine the new Y position through a Sin function. Add this value to the start point's Y, to prevent issues if the object isn't sitting on 0 normally.
                        newYPosition = jumpStartPoint.y + Mathf.Sin(movementProgress * Mathf.PI) * jumpHeight;

                        //Finally, set the position.
                        transform.position = new Vector3(newXposition, newYPosition, newZposition);
                    }
                    break;

                default:
                    Debug.Log("Frog entity is in an unrecognized state.");
                    break;
            }
        }
    }

    //If the frog is jumping, toggle the animation back to stationary.
    internal override void DisableMovement()
    {
        //If we're in the jumping state, we're in the jumping animation. Toggle it, and switch to the waiting state to avoid multiple calls.
        if (currentState == FrogState.Jumping)
        {
            currentState = FrogState.Waiting;
            animator.SetTrigger("JumpToggle");
        }

        //Call the base function.
        base.DisableMovement();
    }
}