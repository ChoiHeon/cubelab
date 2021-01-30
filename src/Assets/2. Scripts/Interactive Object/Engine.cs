using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Engine : MyObject
{
    [SerializeField] private List<GameObject> connectedOutputObjects = new List<GameObject>();
    [SerializeField] private GameObject connectedInputObject;

    private bool isOnObserver;
    private float minEngineSpeed=0.1f;


    public float maxEngineSpeed=200.0f;
    public float engineSpeed=50.0f;
    public bool reverse = false;

    private void Awake()
    {
        isOn=false;
        initObjectLevel = objectLevel = 2;
        objectType = ObjectType.ENGINE;
        isOnObserver = isOn;
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateGizmo(gameObject);      
        //initizlizeGizmoColor();
        //ResetGizmoColor();
    }

    // Update is called once per frame
    void Update()
    {
        RotateGizmo();
    }

    public override bool CheckConnectable(GameObject obj, ConnectType connectType)
    {
        if (connectType == ConnectType.IN)
            return connectedInputObject == null;
        return true;
    }

    public override void AddConnectableObject(GameObject obj, ConnectType connectType)
    {
        if (connectType == ConnectType.IN)
        {
            connectedInputObject = obj;

            if (obj.GetComponent<MyObject>().objectType == ObjectType.SWITCH)
                Operation(obj);
        }
        else
        {
            connectedOutputObjects.Add(obj);

            if (connectedInputObject != null)
                obj.GetComponent<MyObject>().Operation(connectedInputObject);
        }
    }

    public override void Operation(GameObject sender)
    {
        switch (sender.GetComponent<MyObject>().objectType)
        {
            case ObjectType.SWITCH:
                isOn = sender.GetComponent<MyObject>().isOn;
                foreach (GameObject connectedObject in connectedOutputObjects)
                    if (!connectedObject.Equals(sender))
                        connectedObject.GetComponent<MyObject>().Operation(gameObject);
                break;

            case ObjectType.DRIVER_SEAT:
                isOn = sender.GetComponent<MyObject>().isOn;
                reverse = sender.GetComponent<DriverSeat>().reverse;
                foreach (GameObject connectedObject in connectedOutputObjects)
                    if (!connectedObject.Equals(sender))
                        connectedObject.GetComponent<MyObject>().Operation(gameObject);

                break;

            default:
                Debug.Log("Sender's type is wrong.");
                break;
        }
    }

    public override void SetGizmoColor(Color color)
    {
        gizmo.GetComponent<SpriteRenderer>().color = color;

        foreach (GameObject line in gizmo.GetComponent<Gizmo>().lines)
            line.GetComponent<Line>().SetColor(color);

        foreach (GameObject obj in connectedOutputObjects)
        {
            obj.GetComponent<MyObject>().SetGizmoColor(color);
        }
    }

    public void SetEngineSpeed(float value)
    {
        value = Mathf.Max(value, minEngineSpeed);
        engineSpeed = value * maxEngineSpeed;
    }

    public override void RemoveConnectedObject(GameObject obj)
    {
        if (connectedInputObject == obj)
            connectedInputObject = null;
        else
            connectedOutputObjects.Remove(obj);
        
        if(connectedInputObject == null)
        {
            ResetGizmoColor();
            SetObjectLevel(initObjectLevel);
        }
    }

    public override void SetObjectLevel(int level)
    {
        objectLevel = level;

        foreach (GameObject connectedObj in connectedOutputObjects)
            connectedObj.GetComponent<MyObject>().SetObjectLevel(level);
    }
}
