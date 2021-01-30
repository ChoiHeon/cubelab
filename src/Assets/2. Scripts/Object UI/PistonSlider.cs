using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PistonSlider : Slider
{
    [SerializeField] private Text valueText;

    public Controller controller;
    public GameObject target;
    public int horizontalIndex;

    protected override void Awake()
    {
        base.Awake();

        valueText = GetComponentInChildren<Text>();
    }

    protected override void Update()
    {
        base.Update();
    }

    public void SetValueOnStep()
    {
        value = Mathf.Round(value);
    }

    public void SetValueText()
    {
        valueText.text = ((int)value).ToString();
    }

    public void NotifyToController()
    {
        controller.UpdateSettingValue(target, horizontalIndex, (int)value);
    }

    public void NotifyRangeToPiston()
    {
        target.GetComponent<Piston>().range = value;
    }

    public void NotifySpeedToPiston()
    {
        target.GetComponent<Piston>().speed = value;
    }
}
