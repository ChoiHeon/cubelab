using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class StageButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string targetStageName;
    private Toggle toggle;

    private void Start()
    {
        toggle = GetComponent<Toggle>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            try
            {
                LoadingSceneManager.LoadScene(targetStageName);
            }
            catch
            {
                Debug.LogError("StageButton(), 씬 로딩에 실패했습니다.");
            }
        }
    }
}
