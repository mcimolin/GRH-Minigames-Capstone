using UnityEngine;

public class GRHCountingMG_SoundManager : MonoBehaviour
{
    // Initial framework - Adam

    public AudioSource[] countingMG_Audio;
    bool soundManagerExists;

    /*
    * 0  - Counting Game Music
    * 1  - Guess Button Sound Increase/Decrease
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

    public void CountingGameMusic()
    {
        countingMG_Audio[0].Play();
    }

    public void GuessButtonSound()
    {
        countingMG_Audio[1].Play();
    }
}
