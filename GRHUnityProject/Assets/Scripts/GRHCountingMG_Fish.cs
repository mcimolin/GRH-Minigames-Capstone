using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class extends off of base moving entity class.
public class GRHCountingMG_Fish : GRHCountingMG_MovingEntity
{
    //Setup - Joseph
    [SerializeField]
    float minimumVisibilityTransitionTime, maximumVisibilityTransitionTime, minimumVisibleTime, maximumVisibleTime, minimumInvisibleTime, maximumInvisibleTime;

    enum FishState { Moving, Waiting };
    enum VisibilityState { Visible, Invisible, TransitioningToVisible, TransitioningToInvisible };

    FishState currentFishState;
    VisibilityState currentVisualState;
    float transitionProgress;

    //Instead of using multiple variables for each type of wait time, we'll use generic visibility timers.
    float visibilityStateStartTime, visibilityStateWaitTime;

    internal override void Initialize()
    {
        //Start by calling the base initialize method, which will determine a destination and enable movement.
        base.Initialize();

        //Set the fish to the moving state.
        currentFishState = FishState.Moving;

        //The fish will start in the visible state. Determine the time, and set the state.
        visibilityStateStartTime = Time.time;
        visibilityStateWaitTime = Random.Range(minimumVisibleTime, maximumVisibleTime);
        currentVisualState = VisibilityState.Visible;
    }

    //Basically, the fish will move from their current position to the current destination, at their speed, if movement is enabled.
    //Once they reach that point, they'll wait a bit, and then they'll determine a new destination.
    //The fish will also transition from visible to invisible, waiting a certain amount of time between state changes.
    void Update()
    {
        //Is movement enabled?
        if (movementEnabled)
        {
            //It is. Let's handle the movement state first.
            switch (currentFishState)
            {
                case FishState.Moving:
                    //We're moving. Determine if the destination point is within this frame's position change.
                    float distanceToPoint = (transform.position - destinationPoint).magnitude;

                    if (distanceToPoint <= speed * Time.deltaTime)
                    {
                        //We're close enough to reach the destination this update. Set our position to that point, and start waiting.
                        transform.position = destinationPoint;

                        waitTime = Random.Range(minimumWaitTime, maximumWaitTime);
                        waitStartTime = Time.time;
                        currentFishState = FishState.Waiting;
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

                case FishState.Waiting:
                    //Check if we're done waiting.
                    if (Time.time >= waitStartTime + waitTime)
                    {
                        //We are. Determine a new destination, and start moving.
                        DetermineDestination();
                        currentFishState = FishState.Moving;
                    }
                    break;

                default:
                    Debug.Log("Fish is in an unrecognized movement state.");
                    break;
            }

            //Now, let's handle the visibility states. States are handled in the order they should happen.
            switch (currentVisualState)
            {
                case VisibilityState.Visible:
                    //Check if it's time to go to the next state.
                    if (Time.time >= visibilityStateStartTime + visibilityStateWaitTime)
                    {
                        //It is. Determine the transition time, set the start time, and move to the transitioning to invisible state.
                        visibilityStateStartTime = Time.time;
                        visibilityStateWaitTime = Random.Range(minimumVisibilityTransitionTime, maximumVisibilityTransitionTime);
                        currentVisualState = VisibilityState.TransitioningToInvisible;
                    }
                    break;

                case VisibilityState.TransitioningToInvisible:
                    //Determine progress through the state, depending on the transition time.
                    transitionProgress = (Time.time - visibilityStateStartTime) / visibilityStateWaitTime;

                    //Cap progress at 1.
                    if (transitionProgress > 1f)
                    {
                        transitionProgress = 1f;
                    }

                    //Set the alpha. Note that though the Unity editor uses values from 0 - 255, in code it uses 0 - 1.
                    sprite.color = new Color(1f, 1f, 1f, 1f - transitionProgress);

                    //Check if it's time to go to the next state.
                    if (Time.time >= visibilityStateStartTime + visibilityStateWaitTime)
                    {
                        //It is. Determine the invisible time, set the start time, and move to the invisible state.
                        visibilityStateStartTime = Time.time;
                        visibilityStateWaitTime = Random.Range(minimumInvisibleTime, maximumInvisibleTime);
                        currentVisualState = VisibilityState.Invisible;
                    }
                    break;

                case VisibilityState.Invisible:
                    //Check if it's time to go to the next state.
                    if (Time.time >= visibilityStateStartTime + visibilityStateWaitTime)
                    {
                        //It is. Determine the transition time, set the start time, and move to the transitioning to visible state.
                        visibilityStateStartTime = Time.time;
                        visibilityStateWaitTime = Random.Range(minimumVisibilityTransitionTime, maximumVisibilityTransitionTime);
                        currentVisualState = VisibilityState.TransitioningToVisible;
                    }
                    break;

                case VisibilityState.TransitioningToVisible:
                    //Determine progress through the state, depending on the transition time.
                    transitionProgress = (Time.time - visibilityStateStartTime) / visibilityStateWaitTime;

                    //Cap progress at 1.
                    if (transitionProgress > 1f)
                    {
                        transitionProgress = 1f;
                    }                    

                    //Set the alpha.
                    sprite.color = new Color(1f, 1f, 1f, transitionProgress);

                    //Check if it's time to go to the next state.
                    if (Time.time >= visibilityStateStartTime + visibilityStateWaitTime)
                    {
                        //It is. Determine the visible time, set the start time, and move to the visible state.
                        visibilityStateStartTime = Time.time;
                        visibilityStateWaitTime = Random.Range(minimumVisibleTime, maximumVisibleTime);
                        currentVisualState = VisibilityState.Visible;
                    }
                    break;

                default:
                    Debug.Log("Fish is in an unrecognized visibility state.");
                    break;
            }
        }
    }

    //We'll need to make the fish fully visible when disabling movement.
    internal override void DisableMovement()
    {
        //Set alpha to fully visible.
        sprite.color = new Color(255f, 255f, 255f, 255f);

        //Call the base function.
        base.DisableMovement();
    }
}