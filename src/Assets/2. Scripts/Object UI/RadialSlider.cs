using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class RadialSlider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Controller controller;
    public GameObject target;
    public int horizontalIndex;
    public int value;

    private bool isPointerDown = false;
    private bool isClockwise;


    // Called when the pointer enters our GUI component.
    // Start tracking the mouse
    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine("TrackPointer");
    }

    // Called when the pointer exits our GUI component.
    // Stop tracking the mouse
    public void OnPointerExit(PointerEventData eventData)
    {
        StopCoroutine("TrackPointer");
    }

    // GUI위에서 마우스 버튼 다운시 호출
    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;

        var ray = GetComponentInParent<GraphicRaycaster>();
        var input = FindObjectOfType<StandaloneInputModule>();
        Vector2 localPos; // Mouse position  
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Input.mousePosition, ray.eventCamera, out localPos);

        isClockwise = localPos.x < 0 ? false : true;
    }

    // GUI위에서 마우스 버튼 업시 호출
    public void OnPointerUp(PointerEventData eventData)
    {
        //if(Input.GetMouseButtonUp(0))
        //isPointerDown = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            isPointerDown = false;

            controller.UpdateSettingValue(target, horizontalIndex, value);
        }
    }

    // mainloop
    IEnumerator TrackPointer()
    {
        var ray = GetComponentInParent<GraphicRaycaster>();     // UGUI
        var input = FindObjectOfType<StandaloneInputModule>();

        var text = GetComponentInChildren<Text>();

        if (ray != null && input != null)
        {
            while (Application.isPlaying)
            {

                // TODO: if mousebutton down
                if (isPointerDown)
                {


                    Vector2 localPos; // GUI위에서의 마우스 로컬좌표 
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Input.mousePosition, ray.eventCamera, out localPos);

                    // 좌측에서 시작하는 각도
                    // float angle = (Mathf.Atan2(-localPos.y, localPos.x)*180f/Mathf.PI+180f)/360f;

                    // 상단에서 시작하는 각도
                    float angle;
                    if (isClockwise)
                        angle = (Mathf.Atan2(-localPos.x, -localPos.y) * 180f / Mathf.PI + 180) / 360f;
                    else
                        angle = (Mathf.Atan2(localPos.x, -localPos.y) * 180f / Mathf.PI + 180) / 360f;


                    // 5도 단위로 변경
                    int unitAngle = (int)Mathf.Ceil(angle * 360) % 10;
                    if (unitAngle < 5)
                        unitAngle = unitAngle < (5 - unitAngle) ? 0 : 5;
                    else
                        unitAngle = (unitAngle - 5) < (10 - unitAngle) ? 5 : 10;
                    angle = ((int)Mathf.Ceil(angle * 360) / 10) * 10 + unitAngle;


                    GetComponent<Image>().fillClockwise = isClockwise;

                    GetComponent<Image>().fillAmount = angle / 360;

                    GetComponent<Image>().color = Color.Lerp(Color.green, Color.red, angle / 360);


                    angle *= isClockwise ? 1 : -1;
                    value = (int)angle;
                    text.text = (angle).ToString();
                }

                yield return null;
            }
        }
        else
            Debug.LogWarning("Could not find GraphicRaycaster and/or StandaloneInputModule");
    }





}
