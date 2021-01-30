using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MyObject
{
    [SerializeField] private GameObject connectedInputObject;
    [SerializeField] private GameObject connectedOutputObject;

    public int second;
    public int millisecond;
    public int ticks;


    public TextMeshProUGUI secondText;
    public TextMeshProUGUI millisecondText;
    public TextMeshProUGUI ticksText;


    // Start is called before the first frame update
    void Awake()
    {
        initObjectLevel = objectLevel = 1;
        objectType = ObjectType.TIMER;

        connectedInputObject = null;
        connectedOutputObject = null;

        second = 0;
        millisecond = 0;
        ticks = 0;
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
    }

    public override bool CheckConnectable(GameObject obj, ConnectType connectType)
    {
        if (connectType == ConnectType.IN)
            return connectedInputObject == null;
        else
            return connectedOutputObject == null;
    }

    public override void AddConnectableObject(GameObject obj, ConnectType connectType)
    {
        if (connectType == ConnectType.IN)
            connectedInputObject = obj;
        else
            connectedOutputObject = obj;
    }

    public override void Operation(GameObject sender)
    {
        isOn = sender.GetComponent<MyObject>().isOn;

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

    public override void SetObjectLevel(int level)
    {
        objectLevel = level;

        if (connectedOutputObject != null)
            connectedOutputObject.GetComponent<MyObject>().SetObjectLevel(level);
    }

    public override void RemoveConnectedObject(GameObject obj)
    {
        // 인자로 받은 오브젝트 제거
        if (connectedOutputObject == obj)
            connectedOutputObject = null;
        else
            connectedInputObject = null;


        // 연결된 오브젝트가 없을 경우 기즈모 색과 오브젝트 레벨을 초기화
        if (connectedOutputObject == null && connectedInputObject == null)
        {
            ResetGizmoColor();
            SetObjectLevel(initObjectLevel);
        }
    }

    public void SetSecond(float value)
    {
        // value 값은 [0, 1]

        value *= 60f;
        second = Mathf.FloorToInt(value);

        SetSecondText();
    }

    public void SetTicks(float value)
    {
        value *= 40f;

        ticks = Mathf.FloorToInt(value);
        millisecond = GetMillisecond();

        SetTicksText();
        SetMillisecondText();
    }

    public void SetSecondText()
    {
        secondText.text = second.ToString("00");
    }

    public void SetMillisecondText()
    {
        millisecondText.text = millisecond.ToString("000");
    }

    public void SetTicksText()
    {
        ticksText.text = ticks.ToString("00");
    }

    public int GetMillisecond()
    {
        return Math.Min(ticks * 25, 999);
    }

    public float GetDelay()
    {
        return second + ((float)millisecond / 1000);
    }
}
