using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GRHLoadingScreen : MonoBehaviour
{
    // The Loading screen objects
    [SerializeField] GameObject loadingBackground = null, loadingIcon = null, loadingText = null;

    [SerializeField] float alphaTime = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        loadingBackground.SetActive(false);
    }

    public IEnumerator LoadScene(string scene)
    {
        loadingBackground.SetActive(true);

        float alphaValue = loadingBackground.GetComponent<Image>().color.a;

        for (float a = 0f; a < 1.0f; a += Time.deltaTime / alphaTime)
        {
            Color color = new Color(1, 1, 1, Mathf.Lerp(1, alphaValue, a));

            loadingBackground.GetComponent<Image>().color = color;
            loadingIcon.GetComponent<Image>().color = color;
            loadingText.GetComponent<Text>().color = color;

            yield return null;
        }

        yield return new WaitForSeconds(0.75f);

        SceneManager.LoadScene(scene);

        yield return new WaitForSeconds(0.75f);
        for (float a = 0f; a < 1.0f; a += Time.deltaTime / alphaTime)
        {
            Color color = new Color(1, 1, 1, Mathf.Lerp(1, alphaValue, a));

            loadingBackground.GetComponent<Image>().color = color;
            loadingIcon.GetComponent<Image>().color = color;
            loadingText.GetComponent<Text>().color = color;

            yield return null;
        }

        loadingBackground.SetActive(false);
    }
}
