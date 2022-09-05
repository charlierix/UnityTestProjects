using PerfectlyNormalUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class ControllerListener : MonoBehaviour
{
    private const string MAP_LEFT = "left";
    private const string MAP_RIGHT = "right";

    public UnityEvent Down_Left_A = new UnityEvent();
    public UnityEvent Up_Left_A = new UnityEvent();

    public UnityEvent Down_Left_B = new UnityEvent();
    public UnityEvent Up_Left_B = new UnityEvent();

    public UnityEvent Down_Right_A = new UnityEvent();
    public UnityEvent Up_Right_A = new UnityEvent();

    public UnityEvent Down_Right_B = new UnityEvent();
    public UnityEvent Up_Right_B = new UnityEvent();

    public UnityEvent Down_Left_Touchpad = new UnityEvent();
    public UnityEvent Up_Left_Touchpad = new UnityEvent();

    public UnityEvent Down_Right_Touchpad = new UnityEvent();
    public UnityEvent Up_Right_Touchpad = new UnityEvent();

    public float Trigger_Left = 0f;
    public float Trigger_Right = 0f;

    public float Thumbstick_X_Left = 0f;       // thumbstick on index controller
    public float Thumbstick_Y_Left = 0f;

    public float Thumbstick_X_Right = 0f;
    public float Thumbstick_Y_Right = 0f;

    public float Touchpad_X_Left = 0f;     // touchpad on index controller
    public float Touchpad_Y_Left = 0f;

    public float Touchpad_X_Right = 0f;
    public float Touchpad_Y_Right = 0f;

    private float _prev_touchpad_x_left = 0f;
    private float _prev_touchpad_y_left = 0f;

    private float _prev_touchpad_x_right = 0f;
    private float _prev_touchpad_y_right = 0f;

    private List<InputDevice> _inputDevices_all = new List<InputDevice>();

    private readonly Dictionary<string, InputDevice> _inputDevices;
    private readonly Dictionary<(string, InputFeatureUsage<bool>), (UnityEvent down, UnityEvent up)> _map;
    private readonly Dictionary<(string, InputFeatureUsage<bool>), bool> _currentState;

    public ControllerListener()
    {
        _inputDevices = new Dictionary<string, InputDevice>();
        _inputDevices.Add(MAP_LEFT, new InputDevice());
        _inputDevices.Add(MAP_RIGHT, new InputDevice());

        _map = new Dictionary<(string, InputFeatureUsage<bool>), (UnityEvent down, UnityEvent up)>();
        //_map.Add((MAP_LEFT, CommonUsages.primaryButton), (Down_Left_A, Up_Left_A));
        //_map.Add((MAP_LEFT, CommonUsages.secondaryButton), (Down_Left_B, Up_Left_B));
        //_map.Add((MAP_RIGHT, CommonUsages.primaryButton), (Down_Right_A, Up_Right_A));
        //_map.Add((MAP_RIGHT, CommonUsages.secondaryButton), (Down_Right_B, Up_Right_B));
        _map.Add((MAP_LEFT, CommonUsages.primaryButton), (Down_Left_B, Up_Left_B));
        _map.Add((MAP_LEFT, CommonUsages.secondaryButton), (Down_Left_A, Up_Left_A));
        _map.Add((MAP_RIGHT, CommonUsages.primaryButton), (Down_Right_B, Up_Right_B));
        _map.Add((MAP_RIGHT, CommonUsages.secondaryButton), (Down_Right_A, Up_Right_A));

        _currentState = new Dictionary<(string, InputFeatureUsage<bool>), bool>();
        foreach (var key in _map.Keys)
            _currentState.Add(key, false);
    }

    void Start()
    {
        InputDevices.deviceConnected += InputDevices_DeviceConnectionChanged;
        RefreshDevices();
    }

    private void InputDevices_DeviceConnectionChanged(InputDevice obj)
    {
        RefreshDevices();
    }

    void Update()
    {
        // Regular Buttons
        foreach (var key in _map.Keys)
        {
            if (!_inputDevices[key.Item1].TryGetFeatureValue(CommonUsages.primaryButton, out bool button))
                continue;

            if (button == _currentState[key])
                continue;

            if (button)
                _map[key].down.Invoke();
            else
                _map[key].up.Invoke();

            _currentState[key] = button;
        }

        // Triggers
        Trigger_Left = _inputDevices[MAP_LEFT].TryGetFeatureValue(CommonUsages.trigger, out float trigger) ?
            trigger :
            0f;

        Trigger_Right = _inputDevices[MAP_RIGHT].TryGetFeatureValue(CommonUsages.trigger, out trigger) ?
            trigger :
            0f;

        // Thumbsticks
        if (_inputDevices[MAP_LEFT].TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 thumbstick))
        {
            Thumbstick_X_Left = thumbstick.x;
            Thumbstick_Y_Left = thumbstick.y;
        }
        else
        {
            Thumbstick_X_Left = 0;
            Thumbstick_Y_Left = 0;
        }

        if (_inputDevices[MAP_RIGHT].TryGetFeatureValue(CommonUsages.primary2DAxis, out thumbstick))
        {
            Thumbstick_X_Right = thumbstick.x;
            Thumbstick_Y_Right = thumbstick.y;
        }
        else
        {
            Thumbstick_X_Right = 0;
            Thumbstick_Y_Right = 0;
        }

        // Touchpads
        if (_inputDevices[MAP_LEFT].TryGetFeatureValue(CommonUsages.secondary2DAxis, out thumbstick))
        {
            Touchpad_X_Left = thumbstick.x;
            Touchpad_Y_Left = thumbstick.y;
        }
        else
        {
            Touchpad_X_Left = 0;
            Touchpad_Y_Left = 0;
        }

        if (_inputDevices[MAP_RIGHT].TryGetFeatureValue(CommonUsages.secondary2DAxis, out thumbstick))
        {
            Touchpad_X_Right = thumbstick.x;
            Touchpad_Y_Right = thumbstick.y;
        }
        else
        {
            Touchpad_X_Right = 0;
            Touchpad_Y_Right = 0;
        }

        DetectTouchpadChange();
    }

    private void DetectTouchpadChange()
    {
        // Detect Down
        bool down_left = (!Touchpad_X_Left.IsNearZero() || !Touchpad_Y_Left.IsNearZero());
        bool down_right = (!Touchpad_X_Right.IsNearZero() || !Touchpad_Y_Right.IsNearZero());
        bool down_left_prev = (!_prev_touchpad_x_left.IsNearZero() || !_prev_touchpad_y_left.IsNearZero());
        bool down_right_prev = (!_prev_touchpad_x_right.IsNearZero() || !_prev_touchpad_y_right.IsNearZero());

        // Left Events
        if (down_left && !down_left_prev)
            Down_Left_Touchpad.Invoke();

        else if (!down_left && down_left_prev)
            Up_Left_Touchpad.Invoke();

        // Right Events
        if (down_right && !down_right_prev)
            Down_Right_Touchpad.Invoke();

        else if (!down_right && down_right_prev)
            Up_Right_Touchpad.Invoke();

        // Store for next tick
        _prev_touchpad_x_left = Touchpad_X_Left;
        _prev_touchpad_y_left = Touchpad_Y_Left;

        _prev_touchpad_x_right = Touchpad_X_Right;
        _prev_touchpad_y_right = Touchpad_Y_Right;
    }

    private void RefreshDevices()
    {
        _inputDevices_all.Clear();
        InputDevices.GetDevices(_inputDevices_all);

        var results = new List<InputDevice>();
        //InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Camera | InputDeviceCharacteristics.HeadMounted, results);
        //_head = results.Count > 0 ?
        //    results[0] :
        //    new InputDevice();

        results.Clear();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left, results);
        _inputDevices[MAP_LEFT] = results.Count > 0 ?
            results[0] :
            new InputDevice();

        results.Clear();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, results);
        _inputDevices[MAP_RIGHT] = results.Count > 0 ?
            results[0] :
            new InputDevice();
    }
}
