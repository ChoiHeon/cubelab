using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SuccessController : MonoBehaviour
{
    private Button restart;
    private Button quit;

    // Start is called before the first frame update
    void Start()
    {
        restart = transform.Find("Restart").GetComponent<Button>();
        quit = transform.Find("Quit").GetComponent<Button>();

        restart.onClick.AddListener(RestartStage);
        quit.onClick.AddListener(LoadMainScene);
    }

    public void RestartStage()
    {
        LoadingSceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainScene()
    {
        LoadingSceneManager.LoadScene("Main");
    }
}
