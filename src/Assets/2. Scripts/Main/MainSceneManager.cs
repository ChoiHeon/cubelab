using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneManager : MonoBehaviour
{

    [SerializeField] private Transform main;
    [SerializeField] private Transform select;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float lerpSpeed;
    [SerializeField] private float boundary;

    [SerializeField] List<string> stageNames;


    private float distance;
    private bool isMove;

    private Vector3 initMainPosition;
    private Vector3 initSelectPosition;

    private Vector3 towardMainPosition;
    private Vector3 towardSelectPosition;


    private void Start()
    {
        isMove = false;

        initMainPosition = main.position;
        initSelectPosition = select.position;

        distance = Vector3.Distance(initMainPosition, initSelectPosition);

        InitStageNames();
    }

    private void Update()
    {
        if (isMove)
        {
            if (Mathf.Abs(towardMainPosition.x - main.position.x) > boundary)
            {
                main.position = Vector3.Lerp(main.position, towardMainPosition, moveSpeed* Time.deltaTime);
                select.position = Vector3.Lerp(select.position, towardSelectPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                main.position = Vector3.MoveTowards(main.position, towardMainPosition, lerpSpeed * Time.deltaTime);
                select.position = Vector3.MoveTowards(select.position, towardSelectPosition, lerpSpeed * Time.deltaTime);

                if (Mathf.Abs(towardMainPosition.x - main.position.x) < 0.1)
                {
                    main.position = towardMainPosition;
                    select.position = towardSelectPosition;

                    isMove = false;
                }
            }
        }
    }

    private void InitStageNames()
    {
        stageNames = new List<string>();

        // stageNames.Add("_스테이지 이름_");
    }

    public void LoadStage(int num)
    {
        SceneManager.LoadScene(stageNames[num]);
    }

    public void ShowMain()
    {
        if (!isMove)
        {
            towardMainPosition = main.position + Vector3.right * distance;
            towardSelectPosition = select.position + Vector3.right * distance;

            isMove = true;
        }
    }

    public void ShowSelect()
    {
        if (!isMove)
        {
            towardMainPosition = main.position + Vector3.left * distance;
            towardSelectPosition = select.position + Vector3.left * distance;

            isMove = true;
        }
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
