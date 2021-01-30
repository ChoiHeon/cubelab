using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class Gizmo : MonoBehaviour
{
    public GameObject target;
    public List<GameObject> lines;
    public Vector3 initNormal;

    private Rigidbody targetRigidbody;


    public void Start()
    {
        targetRigidbody = target.GetComponent<Rigidbody>();
        lines = new List<GameObject>();
        initNormal = new Vector3(0, 0, -1);
    }

    void Update()
    {
        // gizmo 위치를 큐브의 위치로 이동
        transform.position = target.transform.position;
    }
}
