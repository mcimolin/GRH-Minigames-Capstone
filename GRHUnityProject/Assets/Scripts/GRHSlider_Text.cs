using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GRHSlider_Text : MonoBehaviour
{
    [SerializeField]
    GameObject targetSlider; //The target slider to which you the text will update according to

    public bool isTimeSlider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimeSlider)
        {
            float time;
            time = targetSlider.GetComponent<Slider>().value * 15;
            this.gameObject.GetComponent<Text>().text = string.Format("{0}:{1:00}", (int)time / 60, (int)time % 60);
            //this.gameObject.GetComponent<Text>().text = time.ToString();
        }
        else
        {
            this.gameObject.GetComponent<Text>().text = targetSlider.GetComponent<Slider>().value.ToString(); //Getting the value of the slider and turning it into a string to display
        }
        
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60000;
        int seconds = (int)time / 1000 - 60 * minutes;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
