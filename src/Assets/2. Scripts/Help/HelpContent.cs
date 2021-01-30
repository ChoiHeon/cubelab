using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpContent : MonoBehaviour
{
    [SerializeField] private int pageCount;
    [SerializeField] private int currentPage;
    [SerializeField] private GameObject pages;
    [SerializeField] private GameObject prevButton;
    [SerializeField] private GameObject nextButton;


    private void Start()
    {
        pages = transform.Find("Pages").gameObject;
        prevButton = transform.Find("Prev Button").gameObject;
        nextButton = transform.Find("Next Button").gameObject;

        currentPage = 0;
        pageCount = pages.transform.childCount;

        nextButton.GetComponent<Button>().onClick.AddListener(NextPage);
        prevButton.GetComponent<Button>().onClick.AddListener(PrevPage);

        ActivatePage();
    }

    private void ActivatePage()
    {
        prevButton.SetActive(0 < currentPage ? true : false);
        nextButton.SetActive(currentPage < pageCount-1 ? true : false);

        for (int i = 0; i < pageCount; i++)
            pages.transform.GetChild(i).gameObject.SetActive(false);

        pages.transform.GetChild(currentPage).gameObject.SetActive(true);
    }

    public void NextPage()
    {
        currentPage += 1;

        ActivatePage();
    }

    public void PrevPage()
    {
        currentPage -= 1;

        ActivatePage();
    }
}
