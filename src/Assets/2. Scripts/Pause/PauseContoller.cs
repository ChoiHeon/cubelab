using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseContoller : MonoBehaviour
{
    [SerializeField] private GameObject helpController;
    [SerializeField] private GameObject blurShader;
    [SerializeField] private float alphaSpeed;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            StartCoroutine("DecreaseAlpha");

    }

    public void RestartStage()
    {
        LoadingSceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainScene()
    {
        LoadingSceneManager.LoadScene("Main");
    }

    public void ChangeActivation()
    {
        if (!gameObject.activeSelf)
        {
            if (!helpController.activeSelf)
            {
                gameObject.SetActive(true);
                StartCoroutine("IncreaseAlpha");
            }
        }
        else
        {
            StartCoroutine("DecreaseAlpha");
        }
    }

    IEnumerator IncreaseAlpha()
    {
        CanvasGroup group = GetComponent<CanvasGroup>();

        while (group.alpha < 1)
        {
            group.alpha += alphaSpeed * Time.deltaTime;

            yield return null;
        }

        blurShader.SetActive(true);

        yield return null;
    }

    IEnumerator DecreaseAlpha()
    {
        CanvasGroup group = GetComponent<CanvasGroup>();

        while (0 < group.alpha)
        {
            group.alpha -= alphaSpeed * Time.deltaTime;

            yield return null;
        }

        gameObject.SetActive(false);
        blurShader.SetActive(false);

        yield return null;
    }
}
