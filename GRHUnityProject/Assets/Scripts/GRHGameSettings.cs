using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GRHGameSettings : MonoBehaviour
{
    // EASY, MEDIUM, HARD
    public string gameDifficulty;

    // Tells if game shows pump count or not
    public bool showPumpCount;

    private void Start()
    {
        showPumpCount = true;
        DontDestroyOnLoad(this);
    }
}
