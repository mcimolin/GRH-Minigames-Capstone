using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class extends off of base moving entity class.
public class GRHCountingMG_Butterfly : GRHCountingMG_MovingEntity
{
    //Setup - Joseph
    enum ButterflyState { Moving, Waiting};

    ButterflyState currentState;

    internal override void Initialize()
    {
        //Start by calling the base initialize method, which will determine a destination and enable movement.
        base.Initialize();

        //Set the butterfly to the moving state.
        currentState = ButterflyState.Moving;

        //Set the animation to a random time for variety.
        
    }

    //Basically, the butterfly will move from their current position to the current destination, at their speed, if movement is enabled.
    //Once they reach that point, they'll wait a bit, and then they'll determine a new destination.
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
            //It is. What state are we in?
            switch (currentState)
            {
                case ButterflyState.Moving:
                    //We're moving. Determine if the destination point is within this frame's position change.
                    float distanceToPoint = (transform.position - destinationPoint).magnitude;

                    if (distanceToPoint <= speed * Time.deltaTime)
                    {
                        //We're close enough to reach the destination this update. Set our position to that point, and start waiting.
                        transform.position = destinationPoint;

                        waitTime = Random.Range(minimumWaitTime, maximumWaitTime);
                        waitStartTime = Time.time;
                        currentState = ButterflyState.Waiting;
                    }
                    else
                    {
                        //We're not close enough. Get a normalized vector for the direction we're traveling.
                        Vector3 direction = destinationPoint - transform.position;
                        direction.Normalize();

                        //Move in that direction by the proper amount.
                        transform.position += direction * speed * Time.deltaTime;
                    }
                    break;

                case ButterflyState.Waiting:
                    //Check if we're done waiting.
                    if (Time.time >= waitStartTime + waitTime)
                    {
                        //We are. Determine a new destination, and start moving.
                        DetermineDestination();
                        currentState = ButterflyState.Moving;
                    }
                    break;

                default:
                    Debug.Log("Butterfly is in an unrecognized state.");
                    break;
            }
        }
    }
}