using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpController : MonoBehaviour
{
    [SerializeField] private GameObject pauseController;
    [SerializeField] private GameObject blurShader;
    [SerializeField] private GameObject contents;
    [SerializeField] private float alphaSpeed;


    private void Start()
    {
        InitializeContents();
    }

    private void InitializeContents()
    {
        contents = transform.Find("Contents").gameObject;

        for (int i = 0; i < contents.transform.childCount; i++)
            contents.transform.GetChild(i).gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            StartCoroutine("DecreaseAlpha");
    }

    public void ChangeActivation()
    {
        if (!gameObject.activeSelf)
        {
            if (!pauseController.activeSelf)
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

    public void ActivateContnent(GameObject content)
    {
        for (int i = 0; i < contents.transform.childCount; i++)
            contents.transform.GetChild(i).gameObject.SetActive(false);

        content.SetActive(true);
    }
}
