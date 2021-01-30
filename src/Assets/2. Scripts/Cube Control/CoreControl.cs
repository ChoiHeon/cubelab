using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreControl : MonoBehaviour
{
    public bool isStart = false;
    private Rigidbody m_rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = transform.GetComponent<Rigidbody>();
        m_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    // Update is called once per frame
    void Update()
    {
        if(isStart)
        {
            m_rigidbody.constraints = RigidbodyConstraints.None;
        }
        else
        {
            m_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void StartingTest()
    {
        m_rigidbody.constraints = RigidbodyConstraints.None;
    }
}
