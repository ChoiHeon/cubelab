using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStart : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] bool StartTest = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(StartTest)
            StartingTest();

    }

    public void StartingTest()
    {
        StartTest = false;        
        foreach(Transform child in transform)
        {
            child.GetComponent<CoreControl>().isStart = true;
        }     
    }
}
