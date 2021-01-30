using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Globalization;


public enum ConnectType
{
    IN, OUT,
}

public abstract class MyObject : MonoBehaviour
{
    public bool isOn;
    public int initObjectLevel;
    public int objectLevel;
    public ObjectType objectType;
    public GameObject gizmo;
    public Color initGizmoColor;

    public abstract bool CheckConnectable(GameObject obj, ConnectType connectType);
    public abstract void AddConnectableObject(GameObject obj, ConnectType connectType);
    public abstract void Operation(GameObject sender);
    public abstract void SetGizmoColor(Color color);
    public abstract void RemoveConnectedObject(GameObject obj);
    public abstract void SetObjectLevel(int level);


    // 기즈모의 색을 초기화
    protected void initizlizeGizmoColor()
    {
        Color color;

        switch (objectType)
        {
            case ObjectType.BEARING:
                color = new Color(1, 1, 0);                     break;
            case ObjectType.CONTROLLER:
                color = new Color(0, 1, 0);                     break;
            case ObjectType.DRIVER_SEAT:
                color = new Color(1, 204f / 255, 0);            break;
            case ObjectType.ENGINE:
                color = new Color(51f / 255, 1, 204f / 255);    break;
            case ObjectType.LOGIC_GATE:
                color = new Color(1, 51f / 255, 51f / 255);     break;
            case ObjectType.PISTON:
                color = new Color(1, 0, 1);                     break;
            case ObjectType.SWITCH:
                color = new Color(153f / 255, 51f / 255, 1);    break;
            case ObjectType.TIMER:
                color = new Color(0, 153f / 255, 1);            break;
            default:
                color = new Color(0, 0, 0);                     break;
        }

        initGizmoColor = color;
    }

    /// <summary>
    /// 기즈모의 색을 초기의 색으로 리셋
    /// </summary>
    public void ResetGizmoColor()
    {
        SetGizmoColor(initGizmoColor);
    }

    public GameObject CheckIsConnected(GameObject obj)
    {
        GameObject ret = null;
        foreach (GameObject line in gizmo.GetComponent<Gizmo>().lines)
            if (obj == line.GetComponent<Line>().vertices[1])
            {
                ret = line;
                break;
            }

        gizmo.GetComponent<Gizmo>().lines.Remove(ret);

        return ret;
    }

    protected void RotateGizmo()
    {
        // gizmo의 방향이 항상 카메라를 향하도록 변경
        gizmo.transform.rotation = Camera.main.transform.rotation;
    }

    protected void CreateGizmo(GameObject obj)
    {
        GameObject gizmoList = GameObject.FindWithTag("Gizmo List");
        TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

        // Gizmo List 오브젝트가 없을 경우 생성 및 초기화
        if (gizmoList == null)
            gizmoList = new GameObject("Giamo List");

        // 프리팹에서 GIzmo를 인스턴스화
        GameObject gizmo = (GameObject)Object.Instantiate(Resources.Load("Prefabs/Gizmo"));

        // Gizmo를 Gizmo List 오브젝트의 자식으로 설정
        gizmo.transform.SetParent(gizmoList.transform);

        // 프리팹에서 objectType에 맞는 오브젝트를 인스턴스화
        // Gizmo와 Object의 public 변수에 서로를 초기화
        gizmo.GetComponent<Gizmo>().target = obj;
        obj.GetComponent<MyObject>().gizmo = gizmo;

        // 하이라키에 오브젝트 UI에 대한 캔버스가 있는지 확인, 없을 경우 프리팹에서 인스턴스화
        if (GameObject.FindWithTag("Object UI") == null)
            Object.Instantiate(Resources.Load("Prefabs/Object UI"));

        initizlizeGizmoColor();
        ResetGizmoColor();
    }
}
