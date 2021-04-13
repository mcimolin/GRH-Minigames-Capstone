using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GRH_GameManager : MonoBehaviour
{
    //Parent game manager class to extend the minigames from, to allow for broader game manager event broadcasting and handling basics that'll be in every minigame. - Joseph
    protected Camera mainCamera;

    [SerializeField]
    protected Canvas playerUI;

    //Inherited advance game method to broadcast to.
    protected virtual void AdvanceGame()
    {

    }
    
    //Show the player UI, if there is one.
    protected void ShowPlayerUI()
    {
        if (playerUI != null)
        {
            playerUI.enabled = true;
        }
    }

    //Hide the player UI, if there is one.
    protected void HidePlayerUI()
    {
        if (playerUI != null)
        {
            playerUI.enabled = false;
        }
    }
}