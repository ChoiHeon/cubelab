using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
public class Quad : MonoBehaviour
{
    int blockType = 0; // enum으로 변경 필요
   
    public GameObject Brush;
    public Vector3 normal;

    private GameManager gameManager;
    private Transform pTransform;
    private CubeControl pCubeControl;
    // Start is called before the first frame update
    void Start()
    {     
        pTransform = transform.parent;
        pCubeControl = pTransform.GetComponent<CubeControl>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Brush = gameManager.Brush;
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

    // Update is called once per frame
    void Update()
    {


    }

    public void setBlockType()
    {
        // 추가
    }

    
    private void OnMouseDown()
    {
        Vector3 normalPlane = new Vector3(normal.x == 0 ? 1 : 0,normal.y == 0 ? 1 : 0,normal.z == 0 ? 1 : 0);
        int testInt = normal.x == 0 ? 1 : 0;

        if (gameManager.CurrentEditMode == ObjectType.CUBE)
        {    
            if(gameManager.CubeDictionary.ContainsKey(pCubeControl.coordinate + normal))
                return;
     
            GameObject newCube = Instantiate(gameManager.MakeCube(gameManager.CurrentEditMode), pTransform.position + normal/4, Quaternion.identity);
            newCube.GetComponent<CubeControl>().SetCoordinate(pCubeControl.coordinate + normal);
            newCube.GetComponent<CubeControl>().Init(ObjectType.CUBE);

            if(pCubeControl.objectType == ObjectType.BEARING)
                newCube.transform.parent = pTransform;
            else
                newCube.transform.parent = pTransform.parent;

            gameManager.CubeDictionary[pCubeControl.coordinate + normal] = newCube;
            gameManager.MakingLog.Push(newCube);
        }        
        else if (gameManager.CurrentEditMode == ObjectType.ENGINE)
        {    
            if(gameManager.CubeDictionary.ContainsKey(pCubeControl.coordinate + normal))
                return;
     
            GameObject newCube = Instantiate(gameManager.MakeCube(gameManager.CurrentEditMode), pTransform.position + normal/4, Quaternion.LookRotation(normal));
            newCube.GetComponent<CubeControl>().SetCoordinate(pCubeControl.coordinate + normal);
            newCube.GetComponent<CubeControl>().Init(ObjectType.ENGINE);

            if(pCubeControl.objectType == ObjectType.BEARING)
                newCube.transform.parent = pTransform;
            else
                newCube.transform.parent = pTransform.parent;

            gameManager.CubeDictionary[pCubeControl.coordinate + normal] = newCube;
            gameManager.MakingLog.Push(newCube);
        }
        else if (gameManager.CurrentEditMode == ObjectType.DRIVER_SEAT)
        {    
            if(gameManager.CubeDictionary.ContainsKey(pCubeControl.coordinate + normal))
                return;
     
            GameObject newCube = Instantiate(gameManager.MakeCube(gameManager.CurrentEditMode), pTransform.position + normal/4, Quaternion.LookRotation(normal));
            newCube.GetComponent<CubeControl>().SetCoordinate(pCubeControl.coordinate + normal);
            newCube.GetComponent<CubeControl>().Init(ObjectType.DRIVER_SEAT);

            if(pCubeControl.objectType == ObjectType.BEARING)
                newCube.transform.parent = pTransform;
            else
                newCube.transform.parent = pTransform.parent;

            gameManager.CubeDictionary[pCubeControl.coordinate + normal] = newCube;
            gameManager.MakingLog.Push(newCube);
        }
        else if (gameManager.CurrentEditMode == ObjectType.SWITCH)
        {    
            if(gameManager.CubeDictionary.ContainsKey(pCubeControl.coordinate + normal))
                return;
     
            GameObject newCube = Instantiate(gameManager.MakeCube(gameManager.CurrentEditMode), pTransform.position + normal/4, Quaternion.LookRotation(normal));
            newCube.GetComponent<CubeControl>().SetCoordinate(pCubeControl.coordinate + normal);
            newCube.GetComponent<CubeControl>().Init(ObjectType.SWITCH);

            if(pCubeControl.objectType == ObjectType.BEARING)
                newCube.transform.parent = pTransform;
            else
                newCube.transform.parent = pTransform.parent;

            gameManager.CubeDictionary[pCubeControl.coordinate + normal] = newCube;
            gameManager.MakingLog.Push(newCube);
        }
        else if (gameManager.CurrentEditMode == ObjectType.WHEEL)
        {
            for(int x = -1; x <= 1; x++)            
                for(int y = -1; y <= 1; y++)                
                    for(int z = -1; z <= 1; z++)                    
                        if(gameManager.CubeDictionary.ContainsKey(pCubeControl.coordinate + normal + Vector3.Scale(new Vector3(x,y,z), normalPlane)))
                            return;

            GameObject newCube = Instantiate(gameManager.MakeCube(gameManager.CurrentEditMode), pTransform.position + normal/4, Quaternion.LookRotation(normal));
            newCube.GetComponent<CubeControl>().SetCoordinate(pCubeControl.coordinate + normal);
            newCube.GetComponent<CubeControl>().Init(ObjectType.WHEEL);

            Debug.Log(pCubeControl.objectType);
            if(pCubeControl.objectType == ObjectType.BEARING)              
                newCube.transform.parent = pTransform;          
            else
                newCube.transform.parent = pTransform.parent;

            for(int x = -1; x <= 1; x++)            
                for(int y = -1; y <= 1; y++)                
                    for(int z = -1; z <= 1; z++)                                       
                        gameManager.CubeDictionary[pCubeControl.coordinate + normal + Vector3.Scale(new Vector3(x,y,z), normalPlane)] = newCube;    

            gameManager.MakingLog.Push(newCube);   
        }
        else if (gameManager.CurrentEditMode == ObjectType.BEARING)
        {    
            if(gameManager.CubeDictionary.ContainsKey(pCubeControl.coordinate + normal))
                return;
            if(pCubeControl.objectType == ObjectType.BEARING)
                return;

            GameObject newCube = Instantiate(gameManager.MakeCube(gameManager.CurrentEditMode), pTransform.position + normal/4, Quaternion.LookRotation(normal));
            newCube.GetComponent<CubeControl>().SetCoordinate(pCubeControl.coordinate + normal);
            newCube.GetComponent<CubeControl>().SetNormal(normal);

            newCube.GetComponent<HingeJoint>().connectedBody = pTransform.parent.GetComponent<Rigidbody>();
            newCube.GetComponent<HingeJoint>().axis = new Vector3(0,0,1);
            newCube.GetComponent<CubeControl>().Init(ObjectType.BEARING);

            newCube.AddComponent<CoreControl>();

            //newCube.GetComponent<FixedJoint>().connectedBody = pTransform.parent.GetComponent<Rigidbody>();


            //newCube.transform.parent = pTransform.parent;
            newCube.transform.parent = GameObject.FindGameObjectWithTag("Core").transform.parent;

            gameManager.CubeDictionary[pCubeControl.coordinate + normal] = newCube;
            gameManager.MakingLog.Push(newCube);
            transform.gameObject.SetActive(false);
        } 
        else if (gameManager.CurrentEditMode == ObjectType.PISTON)
        {    
            if(gameManager.CubeDictionary.ContainsKey(pCubeControl.coordinate + normal))
                return;

            GameObject newCube = Instantiate(gameManager.MakeCube(gameManager.CurrentEditMode), pTransform.position + normal/4, Quaternion.LookRotation(normal));
            newCube.GetComponent<CubeControl>().SetCoordinate(pCubeControl.coordinate + normal);
            newCube.GetComponent<CubeControl>().Init(ObjectType.PISTON);

            if(pCubeControl.objectType == ObjectType.BEARING)
                newCube.transform.parent = pTransform;
            else
                newCube.transform.parent = pTransform.parent;

            gameManager.CubeDictionary[pCubeControl.coordinate + normal] = newCube;
            gameManager.MakingLog.Push(newCube);
        } 
        else if (gameManager.CurrentEditMode == ObjectType.CONTROLLER)
        {    
            if(gameManager.CubeDictionary.ContainsKey(pCubeControl.coordinate + normal))
                return;

            GameObject newCube = Instantiate(gameManager.MakeCube(gameManager.CurrentEditMode), pTransform.position + normal/4, Quaternion.LookRotation(normal));
            newCube.GetComponent<CubeControl>().SetCoordinate(pCubeControl.coordinate + normal);
            newCube.GetComponent<CubeControl>().Init(ObjectType.CONTROLLER);

            if(pCubeControl.objectType == ObjectType.BEARING)
                newCube.transform.parent = pTransform;
            else
                newCube.transform.parent = pTransform.parent;

            gameManager.CubeDictionary[pCubeControl.coordinate + normal] = newCube;
            gameManager.MakingLog.Push(newCube);
        } 
    }

    // 마우스가 위로 올라왔을 때
    private void OnMouseEnter()
    {                
        if(gameManager.CubeDictionary.ContainsKey(pCubeControl.coordinate + normal))
                return;
        Brush.GetComponent<BrushControl>().brushOn();
        Brush.transform.position = pTransform.position + normal/4;//2*transform.localPosition;
        Brush.transform.rotation = Quaternion.LookRotation(normal);
    }

    private void OnMouseExit()
    {
        Brush.GetComponent<BrushControl>().brushOff();
    }

    public void SetNormal(Vector3 norm)
    {
        normal = norm;
    }
}
