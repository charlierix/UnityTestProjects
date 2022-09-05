using PerfectlyNormalUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseLogger : MonoBehaviour
{
    private const string CATEGORY_HEAD = "head";
    private const string CATEGORY_LEFT = "left";
    private const string CATEGORY_RIGHT = "right";

    public string Folder = @"d:\temp";

    public Transform HeadController;
    public Transform LeftController;
    public Transform RightController;

    public LoggingCanvasState CanvasState;

    public ControllerListener ControllerListener;

    private DebugLogger _logger = null;
    private bool _isRightADown = false;

    void Start()
    {
        ControllerListener.Down_Left_A.AddListener(LeftA);
        ControllerListener.Down_Right_A.AddListener(RightADown);
        ControllerListener.Up_Right_A.AddListener(RightAUp);

        _logger = new DebugLogger(Folder, true);
        _logger.DefineCategory(CATEGORY_HEAD, UtilityUnity.ColorFromHex("EBEBEB"), 2);     // top is white
        _logger.DefineCategory(CATEGORY_LEFT, UtilityUnity.ColorFromHex("A33F37"), 2);     // port is red
        _logger.DefineCategory(CATEGORY_RIGHT, UtilityUnity.ColorFromHex("23A157"), 2);     // starboard is green
    }

    private void LeftA()
    {
        LogPositions();

        _logger.Save($"{CanvasState.State_Current_Left} - {CanvasState.State_Current_Right} - {CanvasState.State_Current_Head} - {CanvasState.State_Current_Feet}");
    }

    private void RightADown()
    {
        _isRightADown = true;
    }
    private void RightAUp()
    {
        _isRightADown = false;

        string from = $"{CanvasState.State_Previous_Left} - {CanvasState.State_Previous_Right} - {CanvasState.State_Previous_Head} - {CanvasState.State_Previous_Feet}";
        string to = $"{CanvasState.State_Current_Left} - {CanvasState.State_Current_Right} - {CanvasState.State_Current_Head} - {CanvasState.State_Current_Feet}";

        if (from == to)
            _logger.Save(from);     // when they are the same, it's just the user doing a single pose, but moving around slightly
        else
            _logger.Save($"{from} to {to}");
    }

    private void Update()
    {
        if (_isRightADown)
        {
            _logger.NewFrame();
            LogPositions();
        }
    }

    private void LogPositions()
    {
        const float AXIS_SIZE = 0.25f;

        _logger.Add_Dot(HeadController.position, CATEGORY_HEAD);
        _logger.Add_AxisLines(HeadController.position, HeadController.rotation, AXIS_SIZE, CATEGORY_HEAD);

        _logger.Add_Dot(LeftController.position, CATEGORY_LEFT);
        _logger.Add_AxisLines(LeftController.position, LeftController.rotation, AXIS_SIZE, CATEGORY_LEFT);

        _logger.Add_Dot(RightController.position, CATEGORY_RIGHT);
        _logger.Add_AxisLines(RightController.position, RightController.rotation, AXIS_SIZE, CATEGORY_RIGHT);

        _logger.Add_AxisLines(new Vector3(), Quaternion.identity, 1);
    }
}
