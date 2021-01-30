using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    private static string nextScene;

    public float minWaitSecond;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;

        SceneManager.LoadScene("Loading");
    }

    IEnumerator LoadScene()
    {
        yield return null;

        float startTime = Time.time;

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        while (!op.isDone && Time.time - startTime < minWaitSecond)
        {
            yield return null;
        }

        op.allowSceneActivation = true;
    }
}
