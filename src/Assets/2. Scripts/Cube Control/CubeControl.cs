using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeControl : MonoBehaviour
{
    [HideInInspector]
    public GameObject gameManagerObject;
    private GameObject quadPrefab;
    [HideInInspector]
    public GameManager gameManager;
    [HideInInspector]
    public GameObject[] Quads;
    //private Dictionary<int, Vector3> NormalDict;
    public Vector3 coordinate;

    public Vector3 normal;

    public ObjectType objectType;

    public void SetCoordinate(Vector3 c)
    {
        coordinate = c;
    }
    public void SetNormal(Vector3 c)
    {
        normal = c;
    }
    private Vector3[] normalMat =
    {            
        new Vector3(1,0,0),
        new Vector3(0,1,0),
        new Vector3(0,0,1),
        new Vector3(-1,0,0),
        new Vector3(0,-1,0),
        new Vector3(0,0,-1)
    };
    private Vector3[] rotationMat =
    {            
        new Vector3(0,-90,0),
        new Vector3(90,0,0),
        new Vector3(0,180,0),
        new Vector3(0,90,0),
        new Vector3(-90,0,0),
        new Vector3(0,0,0)
    };

    // Start is called before the first frame update
    void Start()
    {        

    }

    public void Init(ObjectType _objectType)
    {
        gameManagerObject = GameObject.Find("GameManager"); 
        gameManager = gameManagerObject.GetComponent<GameManager>();
        quadPrefab = gameManager.QuadPrefab;
        objectType = _objectType;
        switch(objectType)
        {
            case ObjectType.CUBE:
                Quads = new GameObject[6];
                for(int i=0; i<6; i++)
                {
                    Quads[i] = Instantiate(quadPrefab, normalMat[i]/8, Quaternion.identity);
                    Quads[i].transform.SetParent(transform, false);
                    Quads[i].transform.Rotate(rotationMat[i],Space.Self);
                    Quads[i].GetComponent<Quad>().SetNormal(normalMat[i]);
                }
                break;
            case ObjectType.WHEEL:
                break;
            case ObjectType.CONTROLLER:
                break;
            case ObjectType.DRIVER_SEAT:
                Quads = new GameObject[1];
                Quads[0] = Instantiate(quadPrefab, new Vector3(0,-1,0)/8, Quaternion.LookRotation(new Vector3(0,-1,0)));
                Quads[0].transform.SetParent(transform, false);
                Quads[0].GetComponent<Quad>().SetNormal(new Vector3(0,-1,0));
                break;
            case ObjectType.ENGINE:
                break;
            case ObjectType.PISTON:
                Quads = new GameObject[1];
                Quads[0] = Instantiate(quadPrefab, new Vector3(0,0,1)/8, Quaternion.identity);
                Quads[0].transform.SetParent(transform, false);
                Quads[0].GetComponent<Quad>().SetNormal(normal);
                break;
            case ObjectType.SWITCH:
                break;
            case ObjectType.BEARING:
                Quads = new GameObject[1];
                Quads[0] = Instantiate(quadPrefab, new Vector3(0,0,1)/8, Quaternion.identity);
                Quads[0].transform.SetParent(transform, false);
                Quads[0].GetComponent<Quad>().SetNormal(normal);
                //Quads[0].GetComponent<Quad>().SetNormal(normal

                /*
                // 베어링 쿼드에 리지드바디와 힌지조인트 추가
                Quads[0].GetComponent<Rigidbody>().isKinematic = false;
                //Quads[0].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                Quads[0].AddComponent<HingeJoint>();
                Quads[0].GetComponent<HingeJoint>().connectedBody = transform.GetComponent<Rigidbody>();
                Quads[0].GetComponent<HingeJoint>().axis = new Vector3(0,0,1);
                */
                break;
        }
    }


}
