using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GRHLoadingScreen : MonoBehaviour
{
    // The Loading screen objects
    [SerializeField] GameObject loadingBackground = null, loadingIcon = null, loadingText = null;

    internal float alphaTime;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] loadingScreens = GameObject.FindGameObjectsWithTag("LoadingScreen");

        if (loadingScreens.Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

        Color color = new Color(1, 1, 1, 0);
        Color textColor = new Color(loadingText.GetComponent<Text>().color.r, loadingText.GetComponent<Text>().color.g, 0);

        loadingBackground.GetComponent<Image>().color = color;
        loadingIcon.GetComponent<Image>().color = color;
        loadingText.GetComponent<Text>().color = textColor;

        alphaTime = 0.5f;

        loadingBackground.SetActive(false);
    }

    public IEnumerator LoadScene(string scene)
    {
        loadingBackground.SetActive(true);
        Color newColor;

        for (float a = 0f; a <= 1.0f; a += Time.deltaTime / alphaTime)
        {
            newColor = new Color(1, 1, 1, Mathf.Lerp(0, 1, a)); 
            Color textColor = new Color(loadingText.GetComponent<Text>().color.r, loadingText.GetComponent<Text>().color.g, loadingText.GetComponent<Text>().color.b, Mathf.Lerp(0, 1, a));

            loadingBackground.GetComponent<Image>().color = newColor;
            loadingIcon.GetComponent<Image>().color = newColor;
            loadingText.GetComponent<Text>().color = textColor;

            yield return null;
        }

        yield return new WaitForSeconds(0.75f);

        SceneManager.LoadScene(scene);

        StartCoroutine(FadeOut(scene));
    }

    public IEnumerator FadeOut(string scene)
    {
        yield return new WaitForSeconds(0.75f);
        for (float a = 0f; a <= 1.0f; a += Time.deltaTime / alphaTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(1, 0, a));
            Color textColor = new Color(loadingText.GetComponent<Text>().color.r, loadingText.GetComponent<Text>().color.g, loadingText.GetComponent<Text>().color.b, Mathf.Lerp(1, 0, a));

            loadingBackground.GetComponent<Image>().color = newColor;
            loadingIcon.GetComponent<Image>().color = newColor;
            loadingText.GetComponent<Text>().color = textColor;

            yield return null;
        }

        loadingBackground.SetActive(false);

        if (scene == "GRHCountingMG_Scene")
        {
            GameObject.Find("GameManager").GetComponent<GRHCountingMG_GameManager>().StartGame();
        }
        else if (scene == "GRHBalloonMG_Scene")
        {
            GameObject.Find("Game/AI Manager").GetComponent<GRHBalloonMG_GameManager>().StartGame();
        }
    }
}
