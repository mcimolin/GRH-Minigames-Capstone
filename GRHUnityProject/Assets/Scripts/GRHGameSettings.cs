using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GRHGameSettings : MonoBehaviour
{
    // Singleton variable [Added by Bryce]
    public static GRHGameSettings gameSettings = null;

    // EASY, MEDIUM, HARD
    public string gameDifficulty;

    // Tells if game shows pump count or not
    // Tells if game shows pump count or not (Balloon Minigame)
    public bool showPumpCount;

    // Tells which character the player will play in game, default being "Popcorn" [Added by Adam]
    public int selectedCharacter;

    private void Awake()
    {
        selectedCharacter = 1;

        // Checks to make sure there is only one of this script loaded [Added by Bryce]
        if (gameSettings == null)
        {
            gameSettings = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (gameSettings != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        showPumpCount = true;
    }
}
