using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bearing : MyObject
{

    [SerializeField] private bool gizmoConnectable;         // 기즈모 연결 가능 여부
    [SerializeField] private GameObject connectedObject;    // 기즈모로 연결된 객체

    [SerializeField] private float rotationSpeed;           // 기본 회전속도

    [SerializeField] private Vector3 initAxis = new Vector3(0, 0, 1);              // 초기 회전축
    [SerializeField] private Vector3 currentAxis;           // 현재 회전축

    [SerializeField] private GameObject rotationTarget;     // 회전시킬 오브젝트(타겟)
    [SerializeField] private float targetAngle = 0;             // 타겟의 회전상태 (초기상태에서 얼만큼 회전했는지)

    [SerializeField] private float rotationDirection = 1;       // 회전 방향


    private IEnumerator rotationCoroutine;
    private bool paused = false;

    private void Awake()
    {
        initObjectLevel = objectLevel = 0;
        objectType = ObjectType.BEARING;
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateGizmo(gameObject);

        gizmo.transform.GetChild(0).gameObject.SetActive(true);
        rotationTarget = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 v = Matrix4x4.Rotate(transform.rotation) * initAxis;
        currentAxis = new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));

        RotateGizmo();
    }

    /// <summary>
    /// 연결할 기즈모를 추가
    /// </summary>
    /// <param name="gizmo"></param>
    /// <returns></returns>
    public override bool CheckConnectable(GameObject obj, ConnectType connectType)
    {
        return connectedObject == null;
    }

    public override void AddConnectableObject(GameObject obj, ConnectType connectType)
    {
        connectedObject = obj;

        // 기즈모 위에 회전방향 표시
        gizmo.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void ChangeRotationDirection()
    {
        rotationDirection *= -1;
    }


    public void SetRotationTarget(GameObject target)
    {
        rotationTarget = target;

        SetRotationTargetAngle(targetAngle);
    }

    public void SetRotationTargetAngle(float towardAngle)
    {
        if (rotationTarget != null)
        {
            float angle = towardAngle - targetAngle;

            rotationTarget.transform.Rotate(currentAxis, angle * rotationDirection);
            targetAngle = towardAngle;
        }
    }

    public override void Operation(GameObject sender)
    {
        if (rotationTarget == null)
            return;

        ObjectType objectType = sender.GetComponent<MyObject>().objectType;
        switch (objectType)
        {
            case ObjectType.ENGINE:
                if (sender.GetComponent<MyObject>().isOn)
                {
                    rotationCoroutine = RotateTargetByEngine(sender.GetComponent<Engine>().engineSpeed, 
                                                             sender.GetComponent<Engine>().reverse);
                    StopAllCoroutines();

                    StartCoroutine(rotationCoroutine);
                }
                else
                {
                    StopAllCoroutines();
                    rotationCoroutine = null;
                }
                break;

            case ObjectType.DRIVER_SEAT:
                if (Input.GetKey(KeyCode.A))
                    RotateTargetByDriverSeat(false);
                else if (Input.GetKey(KeyCode.D))
                    RotateTargetByDriverSeat(true);
                
                break;

            case ObjectType.CONTROLLER:
                if (sender.GetComponent<MyObject>().isOn)
                {
                    paused = false;

                    if (rotationCoroutine == null)
                    {
                        Controller controller = sender.GetComponent<Controller>();
                        rotationCoroutine = RotateTargetByController(controller.dictObjectInfo[gameObject].settingValues, controller.timeStep);
                        StopAllCoroutines();

                        StartCoroutine(rotationCoroutine);
                    }
                }
                else
                {
                    paused = true;
                }
                break;

            case ObjectType.TIMER:
                if (sender.GetComponent<MyObject>().isOn)
                {
                    rotationCoroutine = RotateTargetByOn(sender.GetComponent<Timer>().GetDelay());
                    StopAllCoroutines();

                    StartCoroutine(rotationCoroutine);
                }
                else
                {
                    StopAllCoroutines();
                    rotationCoroutine = null;
                }
                break;

            case ObjectType.SWITCH:
            case ObjectType.LOGIC_GATE:
                if (sender.GetComponent<MyObject>().isOn)
                {
                    rotationCoroutine = RotateTargetByOn();
                    StopAllCoroutines();

                    StartCoroutine(rotationCoroutine);
                }
                else
                {
                    StopAllCoroutines();
                    rotationCoroutine = null;
                }
                break;

            default:
                break;
        }
    }

    private void RotateTargetByDriverSeat(bool reverse = false)
    {
        float angle;        
        JointLimits limits = transform.GetComponent<HingeJoint>().limits;
        limits.min = -30;        
        limits.max = 30;
        transform.GetComponent<HingeJoint>().limits = limits;
        transform.GetComponent<HingeJoint>().useLimits = true;


        float dir = reverse ? -1 : 1;
        rotationTarget.GetComponent<Rigidbody>().AddTorque(currentAxis * 200 * Time.deltaTime * rotationDirection * dir);
        
        if (!reverse)
        {
            if (30 > targetAngle)
            {
                angle = Mathf.Min(rotationSpeed * Time.deltaTime, 30 - targetAngle);

                rotationTarget.transform.Rotate(currentAxis, angle * rotationDirection);
                targetAngle += angle;
            }
        }
        else
        {
            if (-30 < targetAngle)
            {
                angle = Mathf.Min(rotationSpeed * Time.deltaTime, targetAngle + 30);

                rotationTarget.transform.Rotate(currentAxis, -angle * rotationDirection);
                targetAngle -= angle;
            }
        }
        
    }

    private IEnumerator RotateTargetByEngine(float engineSpeed, bool reverse)
    {
        float dir = reverse ? -1 : 1;
        rotationTarget.GetComponent<Rigidbody>().maxAngularVelocity = engineSpeed;

        while (true)
        {
            rotationTarget.GetComponent<Rigidbody>().AddTorque(currentAxis * engineSpeed * Time.deltaTime * rotationDirection * dir);

            yield return false;
        }
    }

    private IEnumerator RotateTargetByOn(float delay=0)
    {
        float angle;

        // 타이머에 의해 실행되었을 경우 딜레이를 발생시킴
        if (delay != 0)
            yield return new WaitForSeconds(delay);

        while (true)
        {
            angle = rotationSpeed * Time.deltaTime;
            rotationTarget.transform.Rotate(currentAxis, angle * rotationDirection);

            yield return false;
        }
    }

    private IEnumerator RotateTargetByController(List<int> values, float timeStep)
    {
        float angle;
        float totalAngle;
        bool reverse;

        // 첫 번쨰 값 제외
        values = values.GetRange(1, values.Count - 1);

        foreach(float towardAngle in values)
        {
            totalAngle = 0;
            reverse = towardAngle < 0 ? true : false;

            if (!reverse)
            {
                while (totalAngle < towardAngle)
                {
                    while (paused) { }

                    angle = Mathf.Min(rotationSpeed * Time.deltaTime, towardAngle - totalAngle);

                    rotationTarget.transform.Rotate(currentAxis, angle * rotationDirection * timeStep);
                    totalAngle += angle;

                    yield return false;
                }
            }
            else
            {
                while (towardAngle < totalAngle)
                {
                    while (paused) { }

                    angle = Mathf.Min(rotationSpeed * Time.deltaTime, totalAngle - towardAngle);

                    rotationTarget.transform.Rotate(currentAxis, -angle * rotationDirection * timeStep);
                    totalAngle -= angle;

                    yield return false;
                }
            }
        }

        rotationCoroutine = null;
        connectedObject.GetComponent<Controller>().NextOperation(gameObject);

        yield return true;
    }


    public override void SetGizmoColor(Color color)
    {
        gizmo.GetComponent<SpriteRenderer>().color = color;
    }

    public override void RemoveConnectedObject(GameObject obj)
    {
        connectedObject = null;

        StopAllCoroutines();
        rotationCoroutine = null;

        ResetGizmoColor();
        SetObjectLevel(initObjectLevel);
    }

    public override void SetObjectLevel(int level)
    {
        objectLevel = level;
    }

    protected new void RotateGizmo()
    {
        gizmo.transform.rotation = Quaternion.FromToRotation(gizmo.GetComponent<Gizmo>().initNormal, currentAxis);
    }

    private void OnDestroy()
    {
        Destroy(gizmo);
    }

}

