using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class CameraControl : MonoBehaviour
{
    public float dist;
    private Vector3 Core;
    private Vector3 CenterOfRotation;    
    private Vector3 MouseStart;
    public bool pause;
    public bool thirdPersonCam = false;
    public float maxFOV = 60.0f;
    public float minFOV = 20.0f;

	[SerializeField] float MoveSpeed;
	[SerializeField] float MoveAccelerationSpeed;

	[SerializeField] float RotateSpeed;

	Vector3 Velocity;
	Vector3 MousePos;

    Camera mainCamera;
    Camera gizmoCamera;
    Camera lineCamera;

    GraphicRaycaster raycaster;

    void Start () 
    {
        mainCamera = GetComponent<Camera>();
        gizmoCamera = transform.Find("Gizmo Camera").GetComponent<Camera>();
        lineCamera = transform.Find("Line Camera").GetComponent<Camera>();
        dist = 10;  // Distance camera is above map   
        MoveSpeed = 0.1f;
	    MoveAccelerationSpeed = 0.8f;
        RotateSpeed = 2;
        pause = false;

        raycaster = GetComponent<GraphicRaycaster>();
    }

    public void Initialize(Vector3 _CorePos)
    {
        Core = _CorePos;
        CenterOfRotation = _CorePos;     
        transform.LookAt(Core);
    }

    void Update () 
    {
        if(pause)
            return;


        if (Input.GetKeyDown(KeyCode.P))
        {
            gizmoCamera.enabled = !gizmoCamera.enabled;
            lineCamera.enabled = !lineCamera.enabled;
        }

        if(thirdPersonCam)
            CameraMove();
        else
           thirdPersonCameraMove();

        CameraZoom();
        CameraRelookat();
    }
    private void thirdPersonCameraMove()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Velocity.z = Mathf.MoveTowards (Velocity.z, Input.GetAxis ("Vertical") * MoveSpeed, MoveAccelerationSpeed * Time.deltaTime);
        Velocity.x = Mathf.MoveTowards (Velocity.x, Input.GetAxis ("Horizontal") * MoveSpeed, MoveAccelerationSpeed * Time.deltaTime);
        Velocity.y = Mathf.MoveTowards (Velocity.y, (Input.GetKey(KeyCode.R)? 1: Input.GetKey (KeyCode.Q)? -1: 0) * MoveSpeed * 0.3f, MoveAccelerationSpeed * Time.deltaTime);

        transform.position += transform.TransformDirection (Velocity);

        var mousePosDelta = Input.mousePosition - MousePos;
        MousePos = Input.mousePosition;

        transform.rotation *= Quaternion.AngleAxis (Input.GetAxis ("Mouse Y") * RotateSpeed, Vector3.left);
        transform.rotation *= Quaternion.AngleAxis (Input.GetAxis ("Mouse X") * RotateSpeed, Vector3.up);

        var eulerAngles = transform.eulerAngles;
        eulerAngles.z = 0;
        transform.eulerAngles = eulerAngles;

        if (Input.GetKey (KeyCode.LeftAlt))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    private void CameraMove()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (Input.GetMouseButtonDown (2)) {
            MouseStart = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
            MouseStart = Camera.main.ScreenToWorldPoint (MouseStart);
        } 
        else if (Input.GetMouseButton (2)) 
        {
            var MouseMove = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
            MouseMove = Camera.main.ScreenToWorldPoint (MouseMove);            
            transform.position = transform.position - (MouseMove - MouseStart);
            CenterOfRotation = CenterOfRotation - (MouseMove - MouseStart);
        }

        if(Input.GetMouseButton(1))
        {
            transform.RotateAround(CenterOfRotation, transform.up, Input.GetAxis("Mouse X")*1.4f);
            transform.RotateAround(CenterOfRotation, transform.right, -Input.GetAxis("Mouse Y")*1.4f);
            transform.LookAt(CenterOfRotation);
        }
    }

    private void CameraZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel")*-15.0f;
        if (mainCamera.fieldOfView <= minFOV && scroll < 0)
        {
            mainCamera.fieldOfView = minFOV;
            gizmoCamera.fieldOfView = minFOV;
            lineCamera.fieldOfView = minFOV;
        }
        else if (mainCamera.fieldOfView >= maxFOV && scroll > 0)
        {
            mainCamera.fieldOfView = maxFOV;
            gizmoCamera.fieldOfView = maxFOV;
            lineCamera.fieldOfView = maxFOV;
        }
        else
        {
            mainCamera.fieldOfView += scroll;
            gizmoCamera.fieldOfView += scroll;
            lineCamera.fieldOfView += scroll;
        }    
    }
    private void CameraRelookat()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            transform.LookAt(Core);
            Debug.Log(Core);
            CenterOfRotation = Core;
        }        
    }
}