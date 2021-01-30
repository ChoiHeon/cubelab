using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectUI : MonoBehaviour
{
    [SerializeField] private GameObject engineEditor;
    [SerializeField] private GameObject controllerEditor;
    [SerializeField] private GameObject timerEditor;
    [SerializeField] private GameObject pistonEditor;
    [SerializeField] private GameObject logicGateEditor;

    [SerializeField] private GameObject controllerEditorContent;

    [SerializeField] private GameObject timerEditorTimes;

    private List<GameObject> editors;


    private void Start()
    {
        InitializeEditors();
    }

    private void InitializeEditors()
    {
        editors = new List<GameObject>();

        editors.Add(engineEditor);
        editors.Add(controllerEditor);
        editors.Add(timerEditor);
        editors.Add(pistonEditor);
        editors.Add(logicGateEditor);
    }

    public bool ActivateEditor(GameObject obj)
    {
        bool ret = true;
        ObjectType type = obj.GetComponent<MyObject>().objectType;

        foreach (GameObject editor in editors)
            editor.SetActive(false);


        switch (type)
        {
            case ObjectType.ENGINE:
                SetEngineEditor(obj.GetComponent<Engine>());
                engineEditor.SetActive(true);
                break;

            case ObjectType.CONTROLLER:
                SetControllerEditor(obj.GetComponent<Controller>());
                controllerEditor.SetActive(true);
                break;

            case ObjectType.TIMER:
                SetTimerEditor(obj.GetComponent<Timer>());
                timerEditor.SetActive(true);
                break;

            case ObjectType.LOGIC_GATE:
                SetLogicGateEditor(obj.GetComponent<LogicGate>());
                logicGateEditor.SetActive(true);
                break;

            case ObjectType.PISTON:
                SetPistonEditor(obj);
                pistonEditor.SetActive(true);
                break;

            default:
                ret = false;
                break;
        }

        return ret;
    }

    public void DeactivateEditor()
    {
        foreach (GameObject editor in editors)
            editor.SetActive(false);
    }

    private void SetEngineEditor(Engine engine)
    {
        // Engine Editor에서 Slider 컴포넌트 탐색
        Slider slider = engineEditor.GetComponentInChildren<Slider>();

        // Slider에 등록된 리스너 전부 삭제
        slider.onValueChanged.RemoveAllListeners();

        // 이 오브젝트의 엔진에 대한 리스터 등록
        slider.onValueChanged.AddListener(value => engine.SetEngineSpeed(slider.value));

        // slider의 value를 engineSpeed로 변경
        slider.value = engine.engineSpeed / engine.maxEngineSpeed;
    }

    private void SetControllerEditor(Controller controller)
    {
        // content에 자식이 이미 있을 경우, 자식들을 전부 삭제
        foreach (Transform child in controllerEditorContent.transform)
            Destroy(child.gameObject);

        // for문을 통해 contorller에 연결된 오브젝트별로 row를 인스턴스화 (오브젝트 타입은 베어링, 피스톤에 한정)
        for (int i = 0; i < controller.connectedObjects.Count; i++)
        {
            GameObject target = controller.connectedObjects[i];
            GameObject row;

            if (target.GetComponent<MyObject>().objectType == ObjectType.BEARING)
            {
                row = (GameObject)Instantiate(Resources.Load("Prefabs/Bearing Control Row"));
                row.GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();

                RadialSlider[] sliders = row.GetComponentsInChildren<RadialSlider>();

                for (int j = 0; j < sliders.Length; j++)
                {
                    sliders[j].controller = controller;
                    sliders[j].target = target;
                    sliders[j].horizontalIndex = j;
                    sliders[j].value = controller.dictObjectInfo[target].settingValues[j];
                }

                row.transform.SetParent(controllerEditorContent.transform);
            }
            else
            {
                row = (GameObject)Instantiate(Resources.Load("Prefabs/Piston Control Row"));
                row.GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();

                PistonSlider[] sliders = row.GetComponentsInChildren<PistonSlider>();

                for (int j = 0; j < sliders.Length; j++)
                {
                    sliders[j].controller = controller;
                    sliders[j].target = target;
                    sliders[j].horizontalIndex = j;
                    sliders[j].value = controller.dictObjectInfo[target].settingValues[j];
                }

                row.transform.SetParent(controllerEditorContent.transform);
            }
        }

        // 슬라이더 리스너 등록 & 값 초기화
        Slider slider = controllerEditor.GetComponentInChildren<Slider>();
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(value => controller.SetTimeStep(slider.value));
        slider.value = controller.timeStep / controller.maxTimeStep;

        // 토글 리스터 등록 & 값 초기화
        Toggle toggle = controllerEditor.GetComponentInChildren<Toggle>();
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(inOn => controller.SetIsOn(toggle.isOn));
        toggle.isOn = controller.isOn;
    }

    private void SetTimerEditor(Timer timer)
    {
        // 텍스트 순서: 세컨드 -> 밀리세컨드 -> 틱
        TextMeshProUGUI[] texts = timerEditorTimes.GetComponentsInChildren<TextMeshProUGUI>();

        timer.secondText = texts[0];
        timer.millisecondText = texts[1];
        timer.ticksText = texts[2];

        // 텍스트 값 초기화
        timer.SetSecondText();
        timer.SetMillisecondText();
        timer.SetTicksText();

        // 슬라이더 순서: 세컨드 -> 틱
        Slider[] sliders = timerEditor.GetComponentsInChildren<Slider>();

        foreach (Slider slider in sliders)
            slider.onValueChanged.RemoveAllListeners();

        sliders[0].onValueChanged.AddListener((value) => timer.SetSecond(value));
        sliders[1].onValueChanged.AddListener((value) => timer.SetTicks(value));

        sliders[0].value = (float)timer.second / 60;
        sliders[1].value = (float)timer.ticks / 40;
    }

    private void SetLogicGateEditor(LogicGate logicGate)
    {
        Toggle[] toggles = logicGateEditor.GetComponentsInChildren<Toggle>();

        toggles[(int)logicGate.logic].isOn = true;

        foreach (Toggle toggle in toggles)
            toggle.onValueChanged.RemoveAllListeners();

        toggles[0].onValueChanged.AddListener((value) => logicGate.SetLogicToAND(value));
        toggles[1].onValueChanged.AddListener((value) => logicGate.SetLogicToOR(value));
        toggles[2].onValueChanged.AddListener((value) => logicGate.SetLogicToXOR(value));
        toggles[3].onValueChanged.AddListener((value) => logicGate.SetLogicToNAND(value));
        toggles[4].onValueChanged.AddListener((value) => logicGate.SetLogicToXOR(value));
        toggles[5].onValueChanged.AddListener((value) => logicGate.SetLogicToNXOR(value));
    }

    private void SetPistonEditor(GameObject target)
    {
        Piston piston = target.GetComponent<Piston>();
        PistonSlider[] sliders = pistonEditor.GetComponentsInChildren<PistonSlider>();

        foreach(PistonSlider slider in sliders)
            slider.target = target;

        sliders[0].value = piston.range;
        sliders[1].value = piston.speed;
    }
}
