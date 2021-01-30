using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // PRIVATE MEMBER
    private Hotbar hotbar;    
    private ObjectType currentEditMode;    
    private Dictionary<int, ObjectType> ToolDictionary;
    private GameObject Core;
    [SerializeField]
    private GameObject CopyUserObject;
    private bool isStartTesting = false;
    [SerializeField]
    private GameObject GizmoList;
    private CameraControl cameraControl;

    // PUBLIC MEMBER
    public GameObject hotbarObject;
    public GameObject Brush;
    public ObjectType CurrentEditMode;
    public GameObject UserObject;

    [Header("Prefabs")]
 
    public GameObject MainCamera;
    public GameObject CorePrefab;
    public GameObject QuadPrefab;
    public GameObject[] hotbarList;    
    public Dictionary<Vector3, GameObject> CubeDictionary;
    public Stack<GameObject> MakingLog;

    [SerializeField] private GameObject objectUI;

    public GameObject tool;
    public GameObject player;

    private bool isLookingObjectUI;

    void Start()
    {
        hotbar = hotbarObject.GetComponent<Hotbar>();
        CurrentEditMode = ObjectType.CUBE;
        
        ToolDictionary = new Dictionary<int, ObjectType>{};
        for(int i =0; i< ObjectType.GetNames(typeof(ObjectType)).Length; i++)
            ToolDictionary[i] = (ObjectType)(1 << i);

    
        Core = Instantiate(CorePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        Core.GetComponent<CubeControl>().Init(ObjectType.CUBE);
        Core.GetComponent<CubeControl>().SetCoordinate(Vector3.zero);    

        //테스트 변경중
        Core.transform.parent = GameObject.FindGameObjectWithTag("Core").transform;

        CubeDictionary = new Dictionary<Vector3, GameObject>
        {
            { Vector3.zero, Core}
        };

        MakingLog = new Stack<GameObject>();

        // 브러시 초기화
        Brush = transform.Find("Brush").gameObject;

        // 카메라 초기화
        MainCamera = Instantiate(MainCamera, new Vector3(4,2,-4), Quaternion.identity);
        cameraControl = MainCamera.GetComponent<CameraControl>();
        cameraControl.Initialize(CorePrefab.transform.position);

        objectUI = GameObject.FindWithTag("Object UI");
        isLookingObjectUI = false;

        GizmoList = new GameObject("Gizmo List");
        GizmoList.tag = "Gizmo List";
        GizmoList.transform.parent = UserObject.transform;
    }  

    // Update is called once per frame
    void Update()
    {
        GizmoActive(CurrentEditMode == ObjectType.CONNECTING_TOOL);

        

        if(Input.anyKeyDown)
        {
            CurrentEditMode = ToolDictionary[hotbar.selected];        
            
            // undo
            if(Input.GetKey(KeyCode.LeftControl))
            {
                if(Input.GetKeyDown(KeyCode.Z))
                {
                    GameObject lastCube = MakingLog.Pop();
                    CubeDictionary[lastCube.GetComponent<CubeControl>().coordinate] = null;
                    Destroy(lastCube);
                }                      
            }
            
            if(Input.GetKeyDown(KeyCode.P))
            {
                isStartTesting = !isStartTesting;
                if(isStartTesting)
                    StartTesting();
                else
                    EndTesting();
            }            
                
            if (Input.GetKey(KeyCode.Escape))
            {
                objectUI.GetComponent<ObjectUI>().DeactivateEditor();
                tool.SetActive(true);
                isLookingObjectUI = false;

                cameraControl.thirdPersonCam = !cameraControl.thirdPersonCam;
            }                                    

            if(CurrentEditMode == ObjectType.CONNECTING_TOOL)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // Object UI를 보고있는 상태가 아닐 경우
                    if (!isLookingObjectUI)
                    {
                        RaycastHit hit;
                        int layerMask = 1 << LayerMask.NameToLayer("Object");
                        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                        // 레이가 오브젝트에 충돌
                        if (Physics.Raycast(mousePosition, GameObject.FindWithTag("MainCamera").transform.forward, out hit, 10.0f, layerMask))
                        {         
                            if (hit.transform.GetComponent<MyObject>().objectType == ObjectType.SWITCH)
                            {
                                // Switch의 isOn을 변경
                                hit.transform.GetComponent<Switch>().ChangeIsOn();
                            }
                            else if (hit.transform.GetComponent<MyObject>().objectType == ObjectType.DRIVER_SEAT)
                            {
                                // Driver Seat의 isOn을 변경
                                hit.transform.GetComponent<DriverSeat>().isPlayerOn = !hit.transform.GetComponent<DriverSeat>().isPlayerOn;
                            }
                            else if (objectUI.GetComponent<ObjectUI>().ActivateEditor(hit.transform.gameObject))
                            {
                                // 툴을 비활성화
                                tool.SetActive(false);

                                isLookingObjectUI = true;

                                // 카메라 상태 변경
                                cameraControl.thirdPersonCam = !cameraControl.thirdPersonCam;
                            }
                        }
                    }
                    else
                    {
                        // 보고있던 Object UI를 비활성화 및 리셋
                        objectUI.GetComponent<ObjectUI>().DeactivateEditor();

                        // 툴을 활성화
                        tool.SetActive(true);

                        // 상태 변경 
                        isLookingObjectUI = false;

                        // 카메라 상태 변경
                        cameraControl.thirdPersonCam = !cameraControl.thirdPersonCam;
                    }
                }
            }
        }
    }

    public GameObject MakeCube(ObjectType objectType)
    {
        return hotbarList[(int)Mathf.Log((int)objectType,2)];
    }

    public void RemoveCube(Vector3 pos)
    {
        //CubeDictionary
        //MakingLog
    }

    public void StartTesting()
    { 
        UserObject.SetActive(false);  
        CopyUserObject = Instantiate(UserObject, transform.position, Quaternion.identity);
        CopyUserObject.SetActive(true);  
        
        foreach(Transform child in CopyUserObject.transform)
        {
            if(child.name == "Gizmo List"|| child.gameObject.layer == LayerMask.NameToLayer("Line"))
                continue;
            child.GetComponent<CoreControl>().isStart = true;
        }     
    }

    public void EndTesting()
    {   
        Destroy(CopyUserObject);
        UserObject.SetActive(true); 
    }

    private void GizmoActive(bool status)
    {
        foreach(Transform child in GizmoList.transform)
        {
            child.gameObject.SetActive(status);
        }  
    }
}