//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/Input/InputXBoxController.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @InputXBoxController : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputXBoxController()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputXBoxController"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""2c35806c-c305-4602-ac82-0b6afff9cc9b"",
            ""actions"": [
                {
                    ""name"": ""Button A"",
                    ""type"": ""Button"",
                    ""id"": ""1cc97e17-a0f6-41c7-97a6-3e76785f1f1b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Button B"",
                    ""type"": ""Button"",
                    ""id"": ""c9c7b59d-d0e2-470c-a614-b773236321e5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Button Y"",
                    ""type"": ""Button"",
                    ""id"": ""aab5e639-fc37-4fa9-bc7d-74f1d1c784f0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Button X"",
                    ""type"": ""Button"",
                    ""id"": ""28a9d2f8-9a10-4a4d-9c7c-045678e5a94b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Thumbstick Left"",
                    ""type"": ""PassThrough"",
                    ""id"": ""5a1ecdaf-48c1-421c-96d2-782f987a4ca5"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Thumbstick Right"",
                    ""type"": ""PassThrough"",
                    ""id"": ""add81ac7-ad1d-4b87-9782-4ad7a2811a3e"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Trigger Left"",
                    ""type"": ""PassThrough"",
                    ""id"": ""925fbb91-f0c2-4b23-8e37-bbd8dca23b4e"",
                    ""expectedControlType"": ""Analog"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Trigger Right"",
                    ""type"": ""PassThrough"",
                    ""id"": ""e5b43130-ab2b-441d-bee5-3c8ecddbac14"",
                    ""expectedControlType"": ""Analog"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""733af87d-9d80-4e7a-8731-25e4dd7c72dd"",
                    ""path"": ""<XInputController>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XBox Control Scheme"",
                    ""action"": ""Thumbstick Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""edf837e8-3d1e-47b6-9c15-d5fe781dda93"",
                    ""path"": ""<XInputController>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XBox Control Scheme"",
                    ""action"": ""Button A"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f7985bdb-d69e-494b-98e7-7904ea3efba0"",
                    ""path"": ""<XInputController>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XBox Control Scheme"",
                    ""action"": ""Button B"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""929e57e2-6851-4c39-9c1f-73a26dec3298"",
                    ""path"": ""<XInputController>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XBox Control Scheme"",
                    ""action"": ""Button Y"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7c927411-653d-4b9d-89bb-a2404a75e7fe"",
                    ""path"": ""<XInputController>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XBox Control Scheme"",
                    ""action"": ""Button X"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f5a1a21b-aa07-4745-8d47-adbac87749c7"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XBox Control Scheme"",
                    ""action"": ""Thumbstick Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""efd86e90-634a-4b60-822a-700405f6795c"",
                    ""path"": ""<XInputController>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XBox Control Scheme"",
                    ""action"": ""Trigger Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""68d2e451-1208-4846-9975-78e05141c6af"",
                    ""path"": ""<XInputController>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XBox Control Scheme"",
                    ""action"": ""Trigger Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""XBox Control Scheme"",
            ""bindingGroup"": ""XBox Control Scheme"",
            ""devices"": [
                {
                    ""devicePath"": ""<XInputController>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_ButtonA = m_Player.FindAction("Button A", throwIfNotFound: true);
        m_Player_ButtonB = m_Player.FindAction("Button B", throwIfNotFound: true);
        m_Player_ButtonY = m_Player.FindAction("Button Y", throwIfNotFound: true);
        m_Player_ButtonX = m_Player.FindAction("Button X", throwIfNotFound: true);
        m_Player_ThumbstickLeft = m_Player.FindAction("Thumbstick Left", throwIfNotFound: true);
        m_Player_ThumbstickRight = m_Player.FindAction("Thumbstick Right", throwIfNotFound: true);
        m_Player_TriggerLeft = m_Player.FindAction("Trigger Left", throwIfNotFound: true);
        m_Player_TriggerRight = m_Player.FindAction("Trigger Right", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_ButtonA;
    private readonly InputAction m_Player_ButtonB;
    private readonly InputAction m_Player_ButtonY;
    private readonly InputAction m_Player_ButtonX;
    private readonly InputAction m_Player_ThumbstickLeft;
    private readonly InputAction m_Player_ThumbstickRight;
    private readonly InputAction m_Player_TriggerLeft;
    private readonly InputAction m_Player_TriggerRight;
    public struct PlayerActions
    {
        private @InputXBoxController m_Wrapper;
        public PlayerActions(@InputXBoxController wrapper) { m_Wrapper = wrapper; }
        public InputAction @ButtonA => m_Wrapper.m_Player_ButtonA;
        public InputAction @ButtonB => m_Wrapper.m_Player_ButtonB;
        public InputAction @ButtonY => m_Wrapper.m_Player_ButtonY;
        public InputAction @ButtonX => m_Wrapper.m_Player_ButtonX;
        public InputAction @ThumbstickLeft => m_Wrapper.m_Player_ThumbstickLeft;
        public InputAction @ThumbstickRight => m_Wrapper.m_Player_ThumbstickRight;
        public InputAction @TriggerLeft => m_Wrapper.m_Player_TriggerLeft;
        public InputAction @TriggerRight => m_Wrapper.m_Player_TriggerRight;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @ButtonA.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonA;
                @ButtonA.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonA;
                @ButtonA.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonA;
                @ButtonB.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonB;
                @ButtonB.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonB;
                @ButtonB.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonB;
                @ButtonY.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonY;
                @ButtonY.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonY;
                @ButtonY.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonY;
                @ButtonX.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonX;
                @ButtonX.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonX;
                @ButtonX.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnButtonX;
                @ThumbstickLeft.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnThumbstickLeft;
                @ThumbstickLeft.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnThumbstickLeft;
                @ThumbstickLeft.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnThumbstickLeft;
                @ThumbstickRight.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnThumbstickRight;
                @ThumbstickRight.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnThumbstickRight;
                @ThumbstickRight.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnThumbstickRight;
                @TriggerLeft.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTriggerLeft;
                @TriggerLeft.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTriggerLeft;
                @TriggerLeft.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTriggerLeft;
                @TriggerRight.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTriggerRight;
                @TriggerRight.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTriggerRight;
                @TriggerRight.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTriggerRight;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ButtonA.started += instance.OnButtonA;
                @ButtonA.performed += instance.OnButtonA;
                @ButtonA.canceled += instance.OnButtonA;
                @ButtonB.started += instance.OnButtonB;
                @ButtonB.performed += instance.OnButtonB;
                @ButtonB.canceled += instance.OnButtonB;
                @ButtonY.started += instance.OnButtonY;
                @ButtonY.performed += instance.OnButtonY;
                @ButtonY.canceled += instance.OnButtonY;
                @ButtonX.started += instance.OnButtonX;
                @ButtonX.performed += instance.OnButtonX;
                @ButtonX.canceled += instance.OnButtonX;
                @ThumbstickLeft.started += instance.OnThumbstickLeft;
                @ThumbstickLeft.performed += instance.OnThumbstickLeft;
                @ThumbstickLeft.canceled += instance.OnThumbstickLeft;
                @ThumbstickRight.started += instance.OnThumbstickRight;
                @ThumbstickRight.performed += instance.OnThumbstickRight;
                @ThumbstickRight.canceled += instance.OnThumbstickRight;
                @TriggerLeft.started += instance.OnTriggerLeft;
                @TriggerLeft.performed += instance.OnTriggerLeft;
                @TriggerLeft.canceled += instance.OnTriggerLeft;
                @TriggerRight.started += instance.OnTriggerRight;
                @TriggerRight.performed += instance.OnTriggerRight;
                @TriggerRight.canceled += instance.OnTriggerRight;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_XBoxControlSchemeSchemeIndex = -1;
    public InputControlScheme XBoxControlSchemeScheme
    {
        get
        {
            if (m_XBoxControlSchemeSchemeIndex == -1) m_XBoxControlSchemeSchemeIndex = asset.FindControlSchemeIndex("XBox Control Scheme");
            return asset.controlSchemes[m_XBoxControlSchemeSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnButtonA(InputAction.CallbackContext context);
        void OnButtonB(InputAction.CallbackContext context);
        void OnButtonY(InputAction.CallbackContext context);
        void OnButtonX(InputAction.CallbackContext context);
        void OnThumbstickLeft(InputAction.CallbackContext context);
        void OnThumbstickRight(InputAction.CallbackContext context);
        void OnTriggerLeft(InputAction.CallbackContext context);
        void OnTriggerRight(InputAction.CallbackContext context);
    }
}