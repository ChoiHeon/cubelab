using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectTool : MonoBehaviour
{
    enum State { IDLE, WORKING, }

    [SerializeField] [Range(0f, 10f)] private float distance;       // raycast 거리
    [SerializeField] private Camera gizmoCamera;                    // Gizmo 레이어만 렌더링하는 카메라
    [SerializeField] private State state;                           // 현재 컨넥트 툴의 상태
    [SerializeField] private Vector3[] positions;                   // 선택된 기즈모들의 좌표
    [SerializeField] private GameObject line;                       // 프리팹에서 인스턴스화한 라인 객체

    private static Dictionary<ObjectType, ObjectType> gizmoConnectableInfo;    // 오브젝트 타입에 따른 연결 가능한 오브젝트 타입들
    private LineRenderer lineRenderer;

    public GameObject originalLine;


    void Awake()
    {
        InitizlizeGIzmoConnectInfo();
    }

    // Start is called before the first frame update
    void Start()
    {
        state = State.IDLE;
        positions = new Vector3[2];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            int layerMask = 1 << LayerMask.NameToLayer("Gizmo");
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Physics.Raycast(mousePosition, GameObject.FindWithTag("MainCamera").transform.forward, out hit, 10.0f, layerMask))
            {
                GizmoClickEvent(hit.transform.gameObject);  
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            int layerMask = 1 << LayerMask.NameToLayer("Rotation Direction");
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, GameObject.FindWithTag("MainCamera").transform.forward, distance, layerMask);

            if (hit)
            {
                hit.transform.GetComponent<BearingRoationDirection>().ReverseRotationDirection();
            }
        }


        if (state == State.WORKING)
        {
            positions[1] = Camera.main.transform.position + Camera.main.transform.rotation * Vector3.forward;
            lineRenderer.SetPositions(positions);
        }
    }

    private void InitizlizeGIzmoConnectInfo()
    {
        gizmoConnectableInfo = new Dictionary<ObjectType, ObjectType>();

        ObjectType driverSeatInfo   = ObjectType.BEARING | ObjectType.ENGINE;
        ObjectType switchInfo       = ObjectType.ENGINE | ObjectType.LOGIC_GATE | ObjectType.CONTROLLER | ObjectType.TIMER | ObjectType.BEARING | ObjectType.PISTON;
        ObjectType controllerInfo   = ObjectType.SWITCH | ObjectType.BEARING | ObjectType.PISTON;
        ObjectType engineInfo       = ObjectType.SWITCH | ObjectType.BEARING | ObjectType.DRIVER_SEAT;
        ObjectType logicGateInfo    = ObjectType.LOGIC_GATE | ObjectType.SWITCH | ObjectType.BEARING | ObjectType.PISTON | ObjectType.TIMER;
        ObjectType timerInfo        = ObjectType.SWITCH | ObjectType.LOGIC_GATE | ObjectType.BEARING | ObjectType.BEARING | ObjectType.PISTON;
        ObjectType bearingInfo      = ObjectType.DRIVER_SEAT | ObjectType.SWITCH | ObjectType.CONTROLLER | ObjectType.ENGINE | ObjectType.LOGIC_GATE | ObjectType.TIMER;
        ObjectType pistonInfo       = ObjectType.SWITCH | ObjectType.LOGIC_GATE | ObjectType.TIMER | ObjectType.CONTROLLER;

        gizmoConnectableInfo.Add(ObjectType.BEARING, bearingInfo);
        gizmoConnectableInfo.Add(ObjectType.CONTROLLER, controllerInfo);
        gizmoConnectableInfo.Add(ObjectType.DRIVER_SEAT, driverSeatInfo);
        gizmoConnectableInfo.Add(ObjectType.ENGINE, engineInfo);
        gizmoConnectableInfo.Add(ObjectType.LOGIC_GATE, logicGateInfo);
        gizmoConnectableInfo.Add(ObjectType.PISTON, pistonInfo);
        gizmoConnectableInfo.Add(ObjectType.SWITCH, switchInfo);
        gizmoConnectableInfo.Add(ObjectType.TIMER, timerInfo);

    }

    private void GizmoClickEvent(GameObject clickedGizmo)
    {
        if (state == State.IDLE)
        {

            ActivateConnectableGizmo(clickedGizmo);

            // 라인 객체 인스턴스화
            line = (GameObject)Instantiate(Resources.Load("Prefabs/Line"));
            line.transform.parent = GameObject.Find("UserObject").transform;
            lineRenderer = line.GetComponent<LineRenderer>();

            // 라인 객체의 색을 변경
            Color lineColor = clickedGizmo.GetComponent<SpriteRenderer>().color;
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;

            // 라인 객체의 정점들을 설정
            positions[0] = clickedGizmo.transform.position;
            positions[1] = positions[0];
            lineRenderer.SetPositions(positions);

            // clickedGizmo가 가리키는 객체를 라인 객체에 저장
            line.GetComponent<Line>().vertices[0] = clickedGizmo.GetComponent<Gizmo>().target;
            line.GetComponent<Line>().vertexCnt++;


            state = State.WORKING;
        }
        else // state == State.WORKING
        {
            // 같은 기즈모를 클릭했을 경우
            if (clickedGizmo.transform.position == positions[0])
            {
                Debug.Log("Clicked duplicated gizmo.");
                Debug.Log("Destroy line object");

                Destroy(line);
            }
            else
            {
                GameObject prevClickedObject = line.GetComponent<Line>().vertices[0];
                
                int prevObjectLevel = prevClickedObject.GetComponent<MyObject>().objectLevel;
                int crntObjectLevel = clickedGizmo.GetComponent<Gizmo>().target.GetComponent<MyObject>().objectLevel;

                // 현재 클릭한 기즈모가 가리키는 오브젝트의 레벨이 더 높을 경우 라인의 방향 변경
                if (prevObjectLevel < crntObjectLevel)
                {
                    line.GetComponent<Line>().vertices[0] = clickedGizmo.GetComponent<Gizmo>().target;
                    positions[0] = clickedGizmo.transform.position;
                    clickedGizmo = prevClickedObject.GetComponent<MyObject>().gizmo;
                }
                // 오브젝트 레벨이 같을 경우, 이미 두 오브젝트 사이에 연결이 있는지 확인
                else if(prevObjectLevel == crntObjectLevel)
                {
                    GameObject tempLine;
                    tempLine = prevClickedObject.GetComponent<MyObject>().CheckIsConnected(clickedGizmo.GetComponent<Gizmo>().target);
                    if (tempLine == null)
                        tempLine = clickedGizmo.GetComponent<Gizmo>().target.GetComponent<MyObject>().CheckIsConnected(prevClickedObject);

                    if (tempLine != null)
                    {
                        Debug.Log("Remove Line");

                        // 각 오브젝트에서 서로를 삭제
                        clickedGizmo.GetComponent<Gizmo>().target.GetComponent<MyObject>().RemoveConnectedObject(prevClickedObject);
                        prevClickedObject.GetComponent<MyObject>().RemoveConnectedObject(clickedGizmo.GetComponent<Gizmo>().target);
                        
                        // 라인 삭제
                        Destroy(tempLine);
                        Destroy(line);

                        ActivateAllGizmo();

                        state = State.IDLE;
                        return;
                    }
                }

                // 라인 객체의 끝점을 클릭된 gizmo의 좌표로 설정
                positions[1] = clickedGizmo.transform.position;
                lineRenderer.SetPositions(positions);

                // 클릭된 기즈모가 가리키는 객체를 라인 객체에 저장
                line.GetComponent<Line>().vertices[1] = clickedGizmo.GetComponent<Gizmo>().target;
                line.GetComponent<Line>().vertexCnt++;

                // 클릭된 gizmo의 색을 변경
                //clickedGizmo.GetComponent<SpriteRenderer>().color = lineRenderer.endColor;

                // 각 객체에 서로를 저장
                GameObject vertex1 = line.GetComponent<Line>().vertices[0];
                GameObject vertex2 = line.GetComponent<Line>().vertices[1];

                if (!vertex1.GetComponent<MyObject>().CheckConnectable(vertex2, ConnectType.OUT) || 
                    !vertex2.GetComponent<MyObject>().CheckConnectable(vertex1, ConnectType.IN))
                {
                    Debug.Log("Clicked object is not connectable.");
                    Debug.Log("Destroy line object");

                    Destroy(line);
                }
                else
                {
                    vertex1.GetComponent<MyObject>().AddConnectableObject(vertex2, ConnectType.OUT);
                    vertex2.GetComponent<MyObject>().AddConnectableObject(vertex1, ConnectType.IN);

                    // 라인 객체의 색을 시작 기즈모의 색으로 변경
                    Color color = line.GetComponent<Line>().vertices[0].GetComponent<MyObject>().gizmo.GetComponent<SpriteRenderer>().color;
                    line.GetComponent<Line>().SetColor(color);
                    vertex1.GetComponent<MyObject>().gizmo.GetComponent<Gizmo>().lines.Add(line);
                    vertex2.GetComponent<MyObject>().SetGizmoColor(color);

                    // 끝점의 객체의 오브젝트 레벨을 시작점 객체의 레벨로 변경
                    vertex2.GetComponent<MyObject>().SetObjectLevel(vertex1.GetComponent<MyObject>().objectLevel);
                }
            }
            // 모든 기즈모들을 활성화
            ActivateAllGizmo();

            state = State.IDLE; 
        }
    }

    private void ActivateConnectableGizmo(GameObject clickedGizmo)
    {
        GameObject gizmoList = GameObject.FindWithTag("Gizmo List");
        int gizmoCnt = gizmoList.transform.childCount;
        ObjectType objectType1 = clickedGizmo.GetComponent<Gizmo>().target.GetComponent<MyObject>().objectType;

        for (int i = 0; i < gizmoCnt; i++)
        {
            GameObject gizmo = gizmoList.transform.GetChild(i).gameObject;

            if (!gizmo.Equals(clickedGizmo))
            {
                ObjectType objectType2 = gizmo.GetComponent<Gizmo>().target.GetComponent<MyObject>().objectType;

                if ((gizmoConnectableInfo[objectType1] & objectType2) == 0)
                    gizmo.SetActive(false);
            }
        }
    }

    private void ActivateAllGizmo()
    {
        GameObject gizmoList = GameObject.FindWithTag("Gizmo List");
        int gizmoCnt = gizmoList.transform.childCount;

        for (int i = 0; i < gizmoCnt; i++)
        {
            GameObject gizmo = gizmoList.transform.GetChild(i).gameObject;
            gizmo.SetActive(true);
        }
    }
}
