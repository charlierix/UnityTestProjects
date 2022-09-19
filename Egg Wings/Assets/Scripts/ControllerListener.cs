using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

//NOTE: This doesn't work for the xbox controller.  These methods seem to be specific to xr devices

public class ControllerListener : MonoBehaviour
{
    private const string MAP_XBOX = "xbox";

    public UnityEvent Down_XBox_A = new UnityEvent();
    public UnityEvent Up_XBox_A = new UnityEvent();

    private List<InputDevice> _inputDevices_all = new List<InputDevice>();

    private readonly Dictionary<string, InputDevice> _inputDevices;
    private readonly Dictionary<(string, InputFeatureUsage<bool>), (UnityEvent down, UnityEvent up)> _map;

    public ControllerListener()
    {
        _inputDevices = new Dictionary<string, InputDevice>();
        _inputDevices.Add(MAP_XBOX, new InputDevice());

        //_map.Add((MAP_XBOX, CommonUsages.primaryButton), (Down_Left_B, Up_Left_B));

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

        //results.Clear();
        //InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left, results);
        //_inputDevices[MAP_LEFT] = results.Count > 0 ?
        //    results[0] :
        //    new InputDevice();

        //results.Clear();
        //InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, results);
        //_inputDevices[MAP_RIGHT] = results.Count > 0 ?
        //    results[0] :
        //    new InputDevice();

        results.Clear();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller, results);
        //_inputDevices[MAP_LEFT] = results.Count > 0 ?
        //    results[0] :
        //    new InputDevice();
    }
}
