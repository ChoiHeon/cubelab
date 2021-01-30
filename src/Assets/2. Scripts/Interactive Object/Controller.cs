using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectedObjectInfo
{
    public int index;
    public int defaultValue;
    public List<int> settingValues;
}

public class Controller : MyObject
{
    [SerializeField] private int maxConnectedObjectCount;                           
    [SerializeField] private int currentConnectedObjectCount;
    [SerializeField] private GameObject connectedSwitch;
    [SerializeField] private bool isLoof;

    private bool isOnObserver;

    private float minTimeStep;
    public float maxTimeStep;

    public float timeStep;
    public List<int> defaultValues;
    public List<List<int>> objectOperationSetting;
    public int maxSettingValueCount;
    public List<GameObject> connectedObjects;
    public Dictionary<GameObject, ConnectedObjectInfo> dictObjectInfo;


    // Start is called before the first frame update
    void Awake()
    {
        initObjectLevel = objectLevel = 2;
        objectType = ObjectType.CONTROLLER;

        currentConnectedObjectCount = 0;
        connectedObjects = new List<GameObject>();
        objectOperationSetting = new List<List<int>>();
        defaultValues = new List<int>();

        dictObjectInfo = new Dictionary<GameObject, ConnectedObjectInfo>();

        maxConnectedObjectCount = 6;
        maxSettingValueCount = 6;

        isOn = false;
        isOnObserver = isOn;

        isLoof = false;

        minTimeStep = 0.1f;
        maxTimeStep = 1f;
    }

    private void Start()
    {
        CreateGizmo(gameObject);
    }

    private void Update()
    {
        RotateGizmo();
    }

    public override void AddConnectableObject(GameObject obj, ConnectType connectType)
    {
        if (connectType == ConnectType.OUT)
        {
            ConnectedObjectInfo info = new ConnectedObjectInfo();

            info.index = currentConnectedObjectCount;
            currentConnectedObjectCount++;
            info.defaultValue = 0;
            info.settingValues = InitializeSettingValueList(obj.GetComponent<MyObject>().objectType);

            connectedObjects.Add(obj);
            dictObjectInfo.Add(obj, info);
        }
        else
        {
            connectedSwitch = obj;
        }
    }

    public override bool CheckConnectable(GameObject obj, ConnectType connectType)
    {
        if (connectType == ConnectType.OUT)
            return currentConnectedObjectCount < maxConnectedObjectCount;
        else
            return connectedSwitch == null;
    }

    public override void Operation(GameObject sender)
    {
        isOn = sender.GetComponent<MyObject>().isOn;

        foreach (GameObject connectedObject in connectedObjects)
            connectedObject.GetComponent<MyObject>().Operation(gameObject);
    }

    public override void SetGizmoColor(Color color)
    {
        gizmo.GetComponent<SpriteRenderer>().color = color;

        foreach (GameObject line in gizmo.GetComponent<Gizmo>().lines)
            line.GetComponent<Line>().SetColor(color);

        foreach (GameObject connectedObj in connectedObjects)
            connectedObj.GetComponent<MyObject>().SetGizmoColor(color);
    }

    private List<int> InitializeSettingValueList(ObjectType type)
    {
        List<int> ret = new List<int>();
        int initValue = type == ObjectType.BEARING ? 0 : 1;

        for (int i = 0; i < maxSettingValueCount; i++)
            ret.Add(initValue);

        return ret;
    }

    public void UpdateSettingValue(GameObject target, int horizontalIndex, int value)
    {
        if (horizontalIndex == 0)
        {
            dictObjectInfo[target].defaultValue = value;

            // 디폴트 값만큼 연결된 오브젝트의 상태 변경
            if (target.GetComponent<MyObject>().objectType == ObjectType.BEARING)
            {
                target.GetComponent<Bearing>().SetRotationTargetAngle(value);
            }
            else
            {
                target.GetComponent<Piston>().SetColumnScale(value);
            }
        }
        else
        {
            dictObjectInfo[target].settingValues[horizontalIndex] = value;
        }
    }

    public void NextOperation(GameObject obj)
    {
        if (isLoof)
        {
            obj.GetComponent<MyObject>().Operation(gameObject);
        }
    }

    public override void RemoveConnectedObject(GameObject obj)
    {
        if (connectedObjects.Contains(obj))
        {
            ConnectedObjectInfo info = dictObjectInfo[obj];

            dictObjectInfo.Remove(obj);
            connectedObjects.Remove(obj);

            foreach (GameObject connectedObj in connectedObjects)
                if (dictObjectInfo[connectedObj].index > info.index)
                    dictObjectInfo[connectedObj].index -= 1;
        }
        else
        {
            connectedSwitch = null;
        }

        /*if(connectedObjects.Count == 0 && connectedSwitch == null )
        {
            SetGizmoColor(initGizmoColor);
            SetObjectLevel(initObjectLevel);
        }*/

        if (connectedSwitch == null)
        {
            SetGizmoColor(initGizmoColor);
            SetObjectLevel(initObjectLevel);
        }
    }

    public override void SetObjectLevel(int level)
    {
        objectLevel = level;

        foreach (GameObject connectedObj in connectedObjects)
            connectedObj.GetComponent<MyObject>().SetObjectLevel(level);
    }

    public void SetTimeStep(float value)
    {
        value = Mathf.Max(minTimeStep, value);
        timeStep = value * maxTimeStep;
    }
    
    public void SetIsOn(bool value)
    {
        isOn = value;
    }
}
