using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject settingsPanel;
    bool isSettingsPanelOpen;

    public Slider[] volumeSliders;
    public Toggle[] resolutionToggles;
    public int[] screenWidths;
    int activeScreenResIndex;

    AudioSource[] musicSources;
    AudioSource[] soundFxSources;
    AudioSource[] masterSoundSources;

    private void Start()
    {
        settingsPanel.SetActive(false);
        isSettingsPanelOpen = false;

        activeScreenResIndex = PlayerPrefs.GetInt("Screen res index");
        bool isFullScreen = (PlayerPrefs.GetInt("Fullscreen") == 1)?true:false;

        // Finds last used screen res size and loads it
        for (int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].isOn = 1 == activeScreenResIndex;
        }
        SetFullScreen(isFullScreen);
    }

    private void Update()
    {
        // Opens setting panel
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenSettings();
        }
    }

    // Loads ballon game scene
    public void StartBallonGame()
    {
        SceneManager.LoadScene("BallonGame");
        Debug.Log("BallonGame");
    }

    // Loads counting game scene
    public void StartCountingGame()
    {
        SceneManager.LoadScene("CountingGame");
        Debug.Log("CountingGame");
    }

    // Loads space game scene
    public void StartSpaceGame()
    {
        SceneManager.LoadScene("SpaceGame");
        Debug.Log("SpaceGame");
    }

    // Remove all but "Application.Quit();" before building as it will crash
    public void QuitApplication()
    {
        //    // Closes game if in editor mode
        //if (UnityEditor.EditorApplication.isPlaying == true)
        //{
        //    UnityEditor.EditorApplication.isPlaying = false;
        //}
        //else
        //{
        //    // Closes game if in build mode
            Application.Quit();
        //}
    }

    // Closes or Opens settings panel depending on bool
    public void OpenSettings()
    {
        if (isSettingsPanelOpen == false)
        {
            settingsPanel.SetActive(true);
            isSettingsPanelOpen = true;
            Debug.Log("SettingsPanelOn");
        }
        else
        {
            settingsPanel.SetActive(false);
            isSettingsPanelOpen = false;
            Debug.Log("SettingsPanelOff");
        }
    }

    public void SetScreenResolution(int i)
    {
        if (resolutionToggles[i].isOn)
        {
            activeScreenResIndex = i;
            float aspectRatio = 16 / 9f;
            Screen.SetResolution(screenWidths[i], (int)(screenWidths[i] / aspectRatio), false);
            PlayerPrefs.SetInt("Screen res index", activeScreenResIndex);
            PlayerPrefs.Save();
        }
    }

    public void SetFullScreen(bool isFullscreen)
    {
        for (int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].interactable = !isFullscreen;
        }

        if (isFullscreen)
        {
            Resolution[] allResolutions = Screen.resolutions;
            Resolution maxResolution = allResolutions[allResolutions.Length - 1];
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);
        }
        else
        {
            SetScreenResolution(activeScreenResIndex);
        }

        PlayerPrefs.SetInt("Fullscreen", ((isFullscreen) ? 1:0));
        PlayerPrefs.Save();
    }

    public void SetMasterVolume(float value)
    {
        
    }

    public void SetMusicVolume(float value)
    {

    }

    public void SetSoundEffectsVolume(float value)
    {

    }
}
