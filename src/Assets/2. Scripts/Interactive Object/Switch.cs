using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MyObject
{
    [SerializeField] private List<GameObject> connectedObjects = new List<GameObject>();

    private bool isOnObserver;
    private Animator animator;

    private void Awake()
    {
        objectType = ObjectType.SWITCH;
        initObjectLevel = objectLevel = 3;

        isOn = false;
        isOnObserver = isOn;
        
        animator = transform.GetChild(0).GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        CreateGizmo(gameObject);
        //initizlizeGizmoColor();
        //ResetGizmoColor();
    }

    private void Update()
    {
        RotateGizmo();

        if (isOnObserver != isOn)
        {
            isOnObserver = isOn;
            Operation(gameObject);
            animator.SetTrigger("On_Off");
        }
    }

    public override bool CheckConnectable(GameObject obj, ConnectType connectType)
    {
        return true;
    }

    public override void AddConnectableObject(GameObject obj, ConnectType connectType)
    {
        connectedObjects.Add(obj);
        obj.GetComponent<MyObject>().Operation(gameObject);
    }

    public override void RemoveConnectedObject(GameObject obj)
    {
        connectedObjects.Remove(obj);
    }

    public override void SetGizmoColor(Color color)
    {
        gizmo.GetComponent<SpriteRenderer>().color = color;

        foreach (GameObject line in gizmo.GetComponent<Gizmo>().lines)
            line.GetComponent<Line>().SetColor(color);

        foreach (GameObject connectedObj in connectedObjects)
            connectedObj.GetComponent<MyObject>().SetGizmoColor(color);
    }

    public override void SetObjectLevel(int level)
    {
        objectLevel = level;

        foreach (GameObject connectedObj in connectedObjects)
            connectedObj.GetComponent<MyObject>().SetObjectLevel(level);
    }

    public override void Operation(GameObject sender)
    {
        foreach (GameObject connectedObj in connectedObjects)
            connectedObj.GetComponent<MyObject>().Operation(sender);
    }

    public void ChangeIsOn()
    {
        isOn = !isOn;
    }
}
