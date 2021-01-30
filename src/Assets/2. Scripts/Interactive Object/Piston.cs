using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piston : MyObject
{
    private IEnumerator coroutine;

    [SerializeField] private Transform top;
    [SerializeField] private Transform column;
    [SerializeField] private Transform bottom;

    private float initDistance;
    private float initColumnScale;
    private bool paused;

    public GameObject connectedObject;

    public Vector3 initDirection;
    public Vector3 direction;

    public float maxRange;
    public float range;

    public float maxSpeed;
    public float speed;

    private void Awake()
    {
        objectType = ObjectType.PISTON;
        initObjectLevel = objectLevel = 0;

        isOn = false;

        top = transform.Find("Mesh/Top");
        column = transform.Find("Mesh/Column");
        bottom = transform.Find("Mesh/Bottom");

        initDistance = Vector3.Distance(top.position, bottom.position);
        initColumnScale = column.localScale.y;

        //top = transform.Find("Top"); ;
        //column = transform.Find("Column");
        //bottom = transform;

        paused = false;
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

        direction = (top.position - bottom.position).normalized;
    }

    public override bool CheckConnectable(GameObject obj, ConnectType connectType)
    {
        return connectedObject == null;
    }

    public override void AddConnectableObject(GameObject obj, ConnectType connectType)
    {
        connectedObject = obj;
    }

    public override void RemoveConnectedObject(GameObject obj)
    {
        connectedObject = null;

        StopAllCoroutines();
        coroutine = null;

        ResetGizmoColor();
        SetObjectLevel(initObjectLevel);
    }

    public override void SetGizmoColor(Color color)
    {
        gizmo.GetComponent<SpriteRenderer>().color = color;
    }

    public override void SetObjectLevel(int level)
    {
        objectLevel = level;
    }

    public override void Operation(GameObject sender)
    {
        if (connectedObject == null)
            return;

        switch (sender.GetComponent<MyObject>().objectType)
        {
            case ObjectType.SWITCH:
            case ObjectType.LOGIC_GATE:
                isOn = sender.GetComponent<MyObject>().isOn;
                coroutine = MoveBySwitch();

                StopAllCoroutines();
                StartCoroutine(coroutine);
                break;

            case ObjectType.TIMER:
                isOn = sender.GetComponent<MyObject>().isOn;
                coroutine = MoveBySwitch(sender.GetComponent<Timer>().GetDelay());

                StopAllCoroutines();
                StartCoroutine(coroutine);
                break;

            case ObjectType.CONTROLLER:
                if (sender.GetComponent<MyObject>().isOn)
                {
                    paused = false;

                    if (coroutine == null)
                    {
                        Controller controller = sender.GetComponent<Controller>();
                        coroutine = MoveByController(controller.dictObjectInfo[gameObject].settingValues, controller.timeStep);
                        StopAllCoroutines();

                        Debug.Log("Start coroutine, RotateTargetByController()");
                        StartCoroutine(coroutine);
                    }
                }
                else
                {
                    paused = true;
                }

                break;

            default:
                break;
        }
    }

    private IEnumerator MoveBySwitch(float delay=0)
    {
        float distance;
        Vector3 targetPosition;
        Vector3 columnScale = column.localScale;

        if (isOn)
            targetPosition = bottom.position + (initDistance * range) * direction;
        else
            targetPosition = bottom.position + initDistance * direction;

        if (delay != 0)
            yield return new WaitForSeconds(delay);

        while (top.position != targetPosition)
        {
            // 탑 이동
            top.position = Vector3.MoveTowards(top.position, targetPosition, speed * Time.deltaTime);

            // 탑 이동거리에 맞춰서 기둥의 크기와 위치 조정
            distance = Vector3.Distance(top.position, bottom.position);
            column.localScale = new Vector3(columnScale.x, initColumnScale * (distance / initDistance), columnScale.z);

            // 기둥을 탑과 바닥의 사이에 위치
            column.position = (top.position + bottom.position) / 2;

            yield return null;
        }

        coroutine = null;
        yield return null;
    }

    private IEnumerator MoveByController(List<int> values, float timeStep)
    {
        Vector3 targetPosition;
        Vector3 columnScale = column.localScale;
        float currentScale = columnScale.y / initColumnScale;
        float distance;

        foreach(int value in values)
        {
            targetPosition = bottom.position + (initDistance * value) * direction;

            while (top.position != targetPosition)
            {
                while (paused) { }

                // 탑 이동
                top.position = Vector3.MoveTowards(top.position, targetPosition, speed * Time.deltaTime);

                // 탑 이동거리에 맞춰서 기둥의 크기와 위치 조정
                distance = Vector3.Distance(top.position, bottom.position);
                column.localScale = new Vector3(columnScale.x, initColumnScale * (distance / initDistance), columnScale.z);

                // 기둥을 탑과 바닥의 사이에 위치
                column.position = (top.position + bottom.position) / 2;

                yield return null;
            }
        }

        coroutine = null;
        yield return null;
    }

    public void SetColumnScale(int value)
    {
        top.position = bottom.position + (initDistance * value) * direction;
        column.localScale = new Vector3(column.localScale.x, initColumnScale * value, column.localScale.z);
        column.position = (top.position + bottom.position) / 2;
    }
}
