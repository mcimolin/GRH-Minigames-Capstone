using UnityEngine;

public class GRHCountingMG_SoundManager : MonoBehaviour
{
    // Initial framework - Adam

    public AudioSource[] countingMG_Audio;
    GameObject[] soundManagers;
    bool soundManagerExists;

    /*
    * 0  - Counting Game Music
    * 1  - Guess Button Sound Increase/Decrease
    */

    private void Start()
    {
        soundManagers = GameObject.FindGameObjectsWithTag("SoundManager");
        if (soundManagers.Length == 1)
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

    public bool IsCountingGameMusicPlaying()
    {
        if (countingMG_Audio[0].isPlaying)
        {
            return true;
        }

        return false;
    }

    public void StopCountingGameMusic()
    {
        countingMG_Audio[0].Stop();
    }

    public void GuessButtonSound()
    {
        countingMG_Audio[1].Play();
    }
}
