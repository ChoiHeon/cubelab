using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Logic
{
    AND, OR, XOR, NAND, NOR, NXOR,
}


public class LogicGate : MyObject
{
    [SerializeField] private List<GameObject> connectedInputObjects;
    [SerializeField] private GameObject connectedOutputObject;

    public Logic logic;

    // Start is called before the first frame update
    void Awake()
    {
        initObjectLevel = objectLevel = 2;
        objectType = ObjectType.LOGIC_GATE;

        connectedInputObjects = new List<GameObject>();

        logic = Logic.AND;      // 디폴트
        isOn = false;
        
    }

    // Update is called once per frame
    void Start()
    {
        CreateGizmo(gameObject);
        //initizlizeGizmoColor();
        //ResetGizmoColor();
    }

    private void Update()
    {
        RotateGizmo();
    }

    public override void AddConnectableObject(GameObject obj, ConnectType connectType)
    {
        if (connectType == ConnectType.IN)
        {
            connectedInputObjects.Add(obj);
        }
        else
        {
            connectedOutputObject = obj;

            connectedOutputObject.GetComponent<MyObject>().Operation(gameObject);
        }
    }

    public override bool CheckConnectable(GameObject obj, ConnectType connectType)
    {
        // 출력 오브젝트가 있을 경우 flase 반환
        if (connectType == ConnectType.OUT && connectedOutputObject != null)
            return false;

        return true;
    }

    public override void Operation(GameObject sender)
    {
        if (connectedInputObjects.Count == 0)
            return;

        // 입력이 1개일 경우 입력의 isOn을 따름
        if (connectedInputObjects.Count == 1)
            isOn = sender.GetComponent<MyObject>().isOn;
        else
        {
            switch (logic)
            {
                case Logic.AND:
                    isOn = connectedInputObjects.Aggregate(true, (acc, x) => acc & x.GetComponent<MyObject>().isOn);
                    break;

                case Logic.NAND:
                    isOn = !connectedInputObjects.Aggregate(true, (acc, x) => acc & x.GetComponent<MyObject>().isOn);
                    break;

                case Logic.OR:
                    isOn = connectedInputObjects.Aggregate(false, (acc, x) => acc | x.GetComponent<MyObject>().isOn);
                    break;

                case Logic.NOR:
                    isOn = !connectedInputObjects.Aggregate(false, (acc, x) => acc | x.GetComponent<MyObject>().isOn);
                    break;

                case Logic.XOR:
                    isOn = connectedInputObjects.GetRange(1, connectedInputObjects.Count - 1).
                        Aggregate(connectedInputObjects[0].GetComponent<MyObject>().isOn, (acc, x) => acc ^ x.GetComponent<MyObject>().isOn);
                    break;

                case Logic.NXOR:
                    isOn = !connectedInputObjects.GetRange(1, connectedInputObjects.Count - 1).
                        Aggregate(connectedInputObjects[0].GetComponent<MyObject>().isOn, (acc, x) => acc ^ x.GetComponent<MyObject>().isOn);
                    break;
            }
        }

        if (connectedOutputObject != null)
            connectedOutputObject.GetComponent<MyObject>().Operation(gameObject);
    }

    public override void SetGizmoColor(Color color)
    {
        gizmo.GetComponent<SpriteRenderer>().color = color;

        if (connectedOutputObject != null)
        {
            gizmo.GetComponent<Gizmo>().lines[0].GetComponent<Line>().SetColor(color);
            connectedOutputObject.GetComponent<MyObject>().SetGizmoColor(color);
        }
    }

    public override void RemoveConnectedObject(GameObject obj)
    {
        // 인자로 받은 오브젝트 제거
        if (connectedOutputObject == obj)
        {
            connectedOutputObject = null;
        }
        else
        {
            connectedInputObjects.Remove(obj);
        }

        // 연결된 오브젝트가 없을 경우 기즈모 색과 오브젝트 레벨을 초기화
        if(connectedOutputObject == null && connectedInputObjects.Count == 0)
        {
            ResetGizmoColor();
            SetObjectLevel(initObjectLevel);
        }
    }

    public override void SetObjectLevel(int level)
    {
        objectLevel = level;
        if (connectedOutputObject != null)
            connectedOutputObject.GetComponent<MyObject>().SetObjectLevel(level);
    }

    public void SetLogicToAND(bool value)
    {
        logic = Logic.AND;
    }

    public void SetLogicToOR(bool value)
    {
        logic = Logic.OR;
    }

    public void SetLogicToXOR(bool value)
    {
        logic = Logic.XOR;
    }

    public void SetLogicToNAND(bool value)
    {
        logic = Logic.NAND;
    }

    public void SetLogicToNOR(bool value)
    {
        logic = Logic.NOR;
    }

    public void SetLogicToNXOR(bool value)
    {
        logic = Logic.NXOR;
    }
}
