using UnityEngine;

public class GRHBalloonMG_SoundManager : MonoBehaviour
{
    // Initial framework - Adam

    public AudioSource[] balloonMG_Audio;
    GRHHubWorld_SceneManager sceneManager;
    bool soundManagerExists;

    /*
    * 0  - Hub World Game Music
    * 1  - BalloonMG Game Music
    * 2  - Air Pump 
    * 3  - Balloon Pop
    * 4  - Crunch Sound
    * 5  - 
    * 6  - 
    * 7  - 
    * 8  - 
    * 9  - 
    * 10 -
    */

    private void Start()
    {
        if (!soundManagerExists)
        {
            soundManagerExists = true;
            DontDestroyOnLoad(transform.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void HubWorldGameMusic()
    {
        // Play "Hub world game music" sound clip
        balloonMG_Audio[0].Play();
    }

    public void BalloonMGGameMusic()
    {
        balloonMG_Audio[1].Play();
    }

    public void PumpSound()
    {
        // Play "pump" sound clip
        balloonMG_Audio[2].Play();
    }

    public void PopSound()
    {
        // Play "pop" sound clip
        balloonMG_Audio[3].Play();
    }

    public void CrunchSound()
    {
        // Play "cromnch" sound clip
        balloonMG_Audio[4].Play();
    }
}
