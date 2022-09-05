using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoggingCanvasState : MonoBehaviour
{
    public Toggle Radio_Left_TPose;
    public Toggle Radio_Left_Down;
    public Toggle Radio_Left_Up;
    public Toggle Radio_Left_Forward;
    public Toggle Radio_Left_Back;

    public Toggle Radio_Right_TPose;
    public Toggle Radio_Right_Down;
    public Toggle Radio_Right_Up;
    public Toggle Radio_Right_Forward;
    public Toggle Radio_Right_Back;

    public Toggle Radio_Head_StraightAhead;
    public Toggle Radio_Head_Right;
    public Toggle Radio_Head_Left;
    public Toggle Radio_Head_Up;
    public Toggle Radio_Head_Down;

    public Toggle Radio_Feet_StraightAhead;
    public Toggle Radio_Feet_Right45;
    public Toggle Radio_Feet_Right90;
    public Toggle Radio_Feet_Left45;
    public Toggle Radio_Feet_Left90;

    public Toggle PreviousLocked;

    public LoggingState_Hand State_Current_Left { get; private set; }
    public LoggingState_Hand State_Current_Right { get; private set; }
    public LoggingState_Head State_Current_Head { get; private set; }
    public LoggingState_Feet State_Current_Feet { get; private set; }

    public LoggingState_Hand State_Previous_Left { get; private set; }
    public LoggingState_Hand State_Previous_Right { get; private set; }
    public LoggingState_Head State_Previous_Head { get; private set; }
    public LoggingState_Feet State_Previous_Feet { get; private set; }

    void Start()
    {
        var state = GetState();

        State_Current_Left = state.left;
        State_Previous_Left = state.left;

        State_Current_Right = state.right;
        State_Previous_Right = state.right;

        State_Current_Head = state.head;
        State_Previous_Head = state.head;

        State_Current_Feet = state.feet;
        State_Previous_Feet = state.feet;
    }

    void Update()
    {
        var state = GetState();

        if (state.left != State_Current_Left)
        {
            if (!PreviousLocked.isOn)
                State_Previous_Left = State_Current_Left;

            State_Current_Left = state.left;
        }

        if (state.right != State_Current_Right)
        {
            if (!PreviousLocked.isOn)
                State_Previous_Right = State_Current_Right;

            State_Current_Right = state.right;
        }

        if (state.head != State_Current_Head)
        {
            if (!PreviousLocked.isOn)
                State_Previous_Head = State_Current_Head;

            State_Current_Head = state.head;
        }

        if (state.feet != State_Current_Feet)
        {
            if (!PreviousLocked.isOn)
                State_Previous_Feet = State_Current_Feet;

            State_Current_Feet = state.feet;
        }
    }

    public void SetPrevious_Clicked()
    {
        State_Previous_Left = State_Current_Left;
        State_Previous_Right = State_Current_Right;
        State_Previous_Head = State_Current_Head;
    }

    private (LoggingState_Hand left, LoggingState_Hand right, LoggingState_Head head, LoggingState_Feet feet) GetState()
    {
        // Left
        LoggingState_Hand left;

        if (Radio_Left_TPose.isOn)
            left = LoggingState_Hand.TPose;

        else if (Radio_Left_Down.isOn)
            left = LoggingState_Hand.Down;

        else if (Radio_Left_Up.isOn)
            left = LoggingState_Hand.Up;

        else if (Radio_Left_Forward.isOn)
            left = LoggingState_Hand.Forward;

        else if (Radio_Left_Back.isOn)
            left = LoggingState_Hand.Back;

        else
            left = LoggingState_Hand.TPose;     // else should never hit

        // Right
        LoggingState_Hand right;

        if (Radio_Right_TPose.isOn)
            right = LoggingState_Hand.TPose;

        else if (Radio_Right_Down.isOn)
            right = LoggingState_Hand.Down;

        else if (Radio_Right_Up.isOn)
            right = LoggingState_Hand.Up;

        else if (Radio_Right_Forward.isOn)
            right = LoggingState_Hand.Forward;

        else if (Radio_Right_Back.isOn)
            right = LoggingState_Hand.Back;

        else
            right = LoggingState_Hand.TPose;

        // Head
        LoggingState_Head head;

        if (Radio_Head_StraightAhead.isOn)
            head = LoggingState_Head.StraightAhead;

        else if (Radio_Head_Right.isOn)
            head = LoggingState_Head.Right;

        else if (Radio_Head_Left.isOn)
            head = LoggingState_Head.Left;

        else if (Radio_Head_Up.isOn)
            head = LoggingState_Head.Up;

        else if (Radio_Head_Down.isOn)
            head = LoggingState_Head.Down;

        else
            head = LoggingState_Head.StraightAhead;

        // Feet
        LoggingState_Feet feet;

        if (Radio_Feet_StraightAhead.isOn)
            feet = LoggingState_Feet.StraightAhead;

        else if (Radio_Feet_Right45.isOn)
            feet = LoggingState_Feet.Right45;

        else if (Radio_Feet_Right90.isOn)
            feet = LoggingState_Feet.Right90;

        else if (Radio_Feet_Left45.isOn)
            feet = LoggingState_Feet.Left45;

        else if (Radio_Feet_Left90.isOn)
            feet = LoggingState_Feet.Left90;

        else
            feet = LoggingState_Feet.StraightAhead;

        return (left, right, head, feet);
    }
}

public enum LoggingState_Hand
{
    TPose,
    Down,
    Up,
    Forward,
    Back,
}
public enum LoggingState_Head
{
    StraightAhead,
    Right,
    Left,
    Up,
    Down,
}
public enum LoggingState_Feet
{
    StraightAhead,
    Right45,
    Right90,
    Left45,
    Left90,
}
