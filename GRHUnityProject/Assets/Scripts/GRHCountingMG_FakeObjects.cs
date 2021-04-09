using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GRHCountingMG_FakeObjects : MonoBehaviour
{
    [SerializeField]
    float initialFadeInTime;

    float fadeStartTime, fadeInProgress;
    bool fadingIn;
    Animator animator;
    SpriteRenderer sprite;
    
    //No initialize method is really needed, these are just visual decoration and can start when they show up no problem.
    void Start()
    {
        //Get the sprite renderer, and get ready to fade in.
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color(1f, 1f, 1f, 0f);
        fadingIn = true;
        fadeStartTime = Time.time;
        
        //Get the animator. Set the playback time randomly for variation. Check if there's an animator, for the lilypad case.
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.Play(0, -1, Random.Range(0f, 1f));
        }
    }
    
    //Fade in if currently fading in, otherwise do nothing.
    void Update()
    {
        if (fadingIn)
        {
            //Determine the progress.
            fadeInProgress = (Time.time - fadeStartTime) / initialFadeInTime;

            //Cap to 1.
            if (fadeInProgress > 1f)
            {
                fadeInProgress = 1f;
            }

            //Set the alpha accordingly.
            sprite.color = new Color(1f, 1f, 1f, fadeInProgress);

            //If we're done fading in, stop fading in.
            if (fadeInProgress == 1f)
            {
                fadingIn = false;
            }
        }
    }
}
