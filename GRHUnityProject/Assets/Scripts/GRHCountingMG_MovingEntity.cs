using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GRHCountingMG_MovingEntity : MonoBehaviour
{
    //Setup - Joseph
    //Base moving entity class to extend the other behaviours off of. This class handles destination determination and shared variables, as well as enabling and disabling movement.
    [SerializeField]
    protected float speed, leftBound, rightBound, upperBound, lowerBound, minimumMovementDistance, maximumMovementDistance, minimumWaitTime, maximumWaitTime, initialFadeInTime;
    protected bool movementEnabled, fadingIn;

    protected Vector3 destinationPoint;
    protected float waitTime, waitStartTime, fadeInProgress, fadeStartTime;
    protected Camera mainCam;
    protected SpriteRenderer sprite;
    protected Animator animator;

    //Initialize script to be called from game manager's start function.
    internal virtual void Initialize()
    {
        //Get the sprite renderer. Set variables for the fade in.
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color(1f, 1f, 1f, 0f);
        fadingIn = true;
        fadeStartTime = Time.time;

        //Set up the game object for initial use.
        DetermineDestination();

        //Get the animator. Set the playback time randomly for variation.
        animator = GetComponent<Animator>();
        animator.Play(0, -1, Random.Range(0f, 1f));
    }

    //Entity movement will be done in the update method in the extended classes, so no movement code is in here.
    //Rather, this update will handle fade in at the game start. The children will call base.Update() until they're done fading in, at which point they'll stop calling this.
    protected void Update()
    {
        if (fadingIn)
        {
            //Determine the fading in progress.
            fadeInProgress = (Time.time - fadeStartTime) / initialFadeInTime;

            //Cap it to 1.
            if (fadeInProgress > 1f)
            {
                fadeInProgress = 1f;
            }

            //Set the sprite's alpha value.
            sprite.color = new Color(1f, 1f, 1f, fadeInProgress);

            //Finally, if the alpha is 1, set fadingIn to false.
            if (fadeInProgress == 1f)
            {
                fadingIn = false;
            }
        }
    }

    //Determine the next destination to move towards.
    protected void DetermineDestination()
    {
        //We'll try to find positions until we find a valid one.
        bool destinationDetermined = false;

        //Create the variables we'll need.
        float movementAngle, movementDistance;
        float newXPosition = 0f, newZPosition = 0f;

        while (!destinationDetermined)
        {
            //First we randomly get an angle, and convert it to radians.
            movementAngle = Random.Range(0, 360);
            movementAngle = movementAngle * Mathf.PI / 180f;

            //Then, get a random distance between the minimum and maximum movement distance.
            movementDistance = Random.Range(minimumMovementDistance, maximumMovementDistance);

            //Determine the new x and z positions from that angle and distance.
            newXPosition = transform.position.x + Mathf.Sin(movementAngle) * movementDistance;
            newZPosition = transform.position.z + Mathf.Cos(movementAngle) * movementDistance;

            //Check if the new position is within the bounds.
            if (newXPosition >= leftBound && newXPosition <= rightBound && newZPosition >= lowerBound && newZPosition <= upperBound)
            {
                //It is. Set the destination point, and exit the loop.
                destinationPoint = new Vector3(newXPosition, transform.position.y, newZPosition);
                destinationDetermined = true;
            }
        }

        //Now, determine if the sprite needs to be flipped horizontally or not.
        //NOTE - Because the camera facing sprite script essentially faces the sprite backwards, it will look like it's flipped by default.
        //So, we need to flip contrary to what we would do normally.
        if (newXPosition >= transform.position.x && !sprite.flipX)
        {
            //New position is at the same position or to the right, and the sprite is flipped. Set the flip to true.
            sprite.flipX = true;
        } else if (newXPosition < transform.position.x && sprite.flipX)
        {
            //New position is to the left, and the sprite isn't flipped. Set the flip to false.
            sprite.flipX = false;
        }
    }

    //Enable movement for the entity.
    internal void EnableMovement()
    {
        movementEnabled = true;
    }

    //Disable movement for the entity. Method is virtual as some entities have a little more to do when disabling movement at the end of the game.
    internal virtual void DisableMovement()
    {
        movementEnabled = false;
    }

    //Set the movement speed of the entity.
    internal void SetMovementSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}