using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float lerpSpeed;

    private float distance;
    private bool isLooking;

    private Vector3 closePosition;
    private Vector3 showPosition;

    private GameObject prevButton;
    private GameObject nextButton;

    private Transform pages;
    private int pageCount;
    private int currentPage;


    private void Start()
    {
        distance = GetComponent<RectTransform>().rect.width;
        isLooking = false;

        closePosition = transform.position;
        showPosition = transform.position + Vector3.right * distance;

        prevButton = transform.Find("Prev Button").gameObject;
        nextButton = transform.Find("Next Button").gameObject;

        prevButton.GetComponent<Button>().onClick.AddListener(PrevPage);
        nextButton.GetComponent<Button>().onClick.AddListener(NextPage);

        pages = transform.Find("Pages");
        pageCount = pages.childCount;
        currentPage = 0;

        prevButton.SetActive(currentPage > 0);
        nextButton.SetActive(currentPage < pageCount - 1);
    }

    public void PrevPage()
    {
        for (int i = 0; i < pageCount; i++)
            pages.GetChild(i).gameObject.SetActive(false);

        currentPage -= 1;
        pages.GetChild(currentPage).gameObject.SetActive(false);

        prevButton.SetActive(currentPage > 0);
        nextButton.SetActive(currentPage < pageCount-1);
    }

    public void NextPage()
    {
        for (int i = 0; i < pageCount; i++)
            pages.GetChild(i).gameObject.SetActive(false);

        currentPage += 1;
        pages.GetChild(currentPage).gameObject.SetActive(false);

        prevButton.SetActive(currentPage > 0);
        nextButton.SetActive(currentPage < pageCount - 1);
    }

    public void MoveWindow()
    {
        StartCoroutine("ShowOrClose");
    }

    IEnumerator ShowOrClose()
    {
        Vector3 towardPosition;
        if (isLooking)
            towardPosition = closePosition;
        else
            towardPosition = showPosition;

        while (true)
        {
            float dist = Mathf.Abs(towardPosition.x - transform.position.x);

            if (dist > 30)
            {
                transform.position = Vector3.MoveTowards(transform.position, towardPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, towardPosition, lerpSpeed * Time.deltaTime);

                if (dist < 1)
                {
                    transform.position = towardPosition;
                    break;
                }
            }

            yield return null;
        }

        isLooking = !isLooking;
    }
}
