using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GRH_CameraFacingSprite : MonoBehaviour
{
    //Setup - Joseph
    //Simple script to put on any sprites that need to face the camera.
    //NOTE - Since the sprite is facing the camera, it's essentially 'backwards' the entire time. Therefore, the sprite's flipX variable must be treated as the opposite.
    Camera mainCam;

    void Start()
    {
        //Get the camera.
        mainCam = Camera.main;
    }
    
    void Update()
    {
        //Point the opposite direction the camera is pointing. This prevents sprites from 'turning' as they move to the sides.
        transform.forward = -mainCam.transform.forward;
    }
}