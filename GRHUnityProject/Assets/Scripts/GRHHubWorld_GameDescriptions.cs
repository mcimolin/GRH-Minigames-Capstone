using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GRHHubWorld_GameDescriptions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Initial work - Adam

    bool mouseOver = false;
    [SerializeField] private GameObject gameDescription;


    private void Update()
    {
        if (mouseOver)
        {
            gameDescription.SetActive(true);
        }
        else
        {
            gameDescription.SetActive(false);
        }
    }

    // If mouse is hovered over the button will display description of game
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true; 
    }

    // Will turn off description once mouse leaves buttons area
    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
    }
}
