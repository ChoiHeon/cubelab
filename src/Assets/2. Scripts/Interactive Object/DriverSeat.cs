using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 */


public class DriverSeat : MyObject
{
    [SerializeField] private List<GameObject> connectedBearings = new List<GameObject>();
    [SerializeField] private List<GameObject> connectedEngines = new List<GameObject>();
    public bool isPlayerOn = false;

    public bool reverse = false;


    void Awake()
    {
        initObjectLevel = objectLevel = 3;
        objectType = ObjectType.DRIVER_SEAT;
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
        
        if (isPlayerOn)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                foreach (GameObject bearing in connectedBearings)
                    bearing.GetComponent<Bearing>().Operation(gameObject);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                isOn = true;
                reverse = false;

                foreach (GameObject engine in connectedEngines)
                    engine.GetComponent<Engine>().Operation(gameObject);
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                isOn = false;

                foreach (GameObject engine in connectedEngines)
                    engine.GetComponent<Engine>().Operation(gameObject);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                isOn = true;
                reverse = true;

                foreach (GameObject engine in connectedEngines)
                    engine.GetComponent<Engine>().Operation(gameObject);
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                isOn = false;

                foreach (GameObject engine in connectedEngines)
                    engine.GetComponent<Engine>().Operation(gameObject);
            }
        }
    }

    

    public override bool CheckConnectable(GameObject obj, ConnectType connectType)
    {
        bool ret = false;

        switch (obj.GetComponent<MyObject>().objectType)
        {
            case ObjectType.BEARING:
                ret = true;
                break;

            case ObjectType.ENGINE:
                ret = true;
                break;

            default:
                break;
        }

        return ret;
    }

    public override void AddConnectableObject(GameObject obj, ConnectType connectType)
    {
        switch (obj.GetComponent<MyObject>().objectType)
        {
            case ObjectType.BEARING:
                connectedBearings.Add(obj);
                break;

            case ObjectType.ENGINE:
                connectedEngines.Add(obj);
                break;

            default:
                break;
        }
    }

    public override void Operation(GameObject sender)
    {
        // Empty;
    }

    public override void SetGizmoColor(Color color)
    {
        gizmo.GetComponent<SpriteRenderer>().color = color;

        foreach (GameObject line in gizmo.GetComponent<Gizmo>().lines)
            line.GetComponent<Line>().SetColor(color);
    }

    public override void RemoveConnectedObject(GameObject obj)
    {
        switch (obj.GetComponent<MyObject>().objectType)
        {
            case ObjectType.BEARING:
                connectedBearings.Remove(obj);
                break;

            case ObjectType.ENGINE:
                connectedEngines.Remove(obj);
                break;

            default:
                break;
        }
    }

    public override void SetObjectLevel(int level)
    {
        objectLevel = level;

        foreach (GameObject bearing in connectedBearings)
            bearing.GetComponent<MyObject>().SetObjectLevel(level);

        foreach (GameObject engine in connectedEngines)
            engine.GetComponent<MyObject>().SetObjectLevel(level);
    }
}
