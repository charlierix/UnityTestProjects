using System.Collections;
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
        _map.Add((MAP_LEFT, CommonUsages.primaryButton), (Down_Left_A, Up_Left_A));
        _map.Add((MAP_LEFT, CommonUsages.secondaryButton), (Down_Left_B, Up_Left_B));
        _map.Add((MAP_RIGHT, CommonUsages.primaryButton), (Down_Right_A, Up_Right_A));
        _map.Add((MAP_RIGHT, CommonUsages.secondaryButton), (Down_Right_B, Up_Right_B));

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
