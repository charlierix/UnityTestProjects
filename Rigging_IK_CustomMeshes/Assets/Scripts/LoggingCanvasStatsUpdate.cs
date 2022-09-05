using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoggingCanvasStatsUpdate : MonoBehaviour
{
    public LoggingCanvasState State;

    public GameObject PreviousCurrentLabel;
    private TextMeshProUGUI _label_prevcur;

    void Start()
    {
        _label_prevcur = PreviousCurrentLabel.GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        string prev = $"previous:\t{State.State_Previous_Left}\t{State.State_Previous_Right}\t{State.State_Previous_Head}\t{State.State_Previous_Feet}";
        string cur = $"current:\t{State.State_Current_Left}\t{State.State_Current_Right}\t{State.State_Current_Head}\t{State.State_Current_Feet}";
        _label_prevcur.text = $"{prev}\r\n{cur}";
    }
}
