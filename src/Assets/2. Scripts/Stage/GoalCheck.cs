using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalCheck : MonoBehaviour
{
    [SerializeField] private string tagName;
    [SerializeField] private GameObject successUI;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals(tagName))
        {
            successUI.SetActive(true);
        }
    }
}
