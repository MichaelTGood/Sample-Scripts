// GENERATED AUTOMATICALLY FROM 'Assets/Inputs/MainInputs.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Squads.Inputs
{
    public class @MainInputs : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @MainInputs()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""MainInputs"",
    ""maps"": [
        {
            ""name"": ""Character"",
            ""id"": ""8324e189-8bd9-4d84-bb14-50118caba071"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""be061739-143f-4f3d-b257-8b472f1506ec"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""ef5164f2-dfd6-4eac-b974-526969c19d0f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Walk"",
                    ""type"": ""Button"",
                    ""id"": ""6e50e440-e511-4f9a-a1a8-7604757c551c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""Button"",
                    ""id"": ""73635cb1-7195-4a65-a11e-e9028d8e115a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""9b72dcdf-e2b0-40ae-ba18-f849cbcd3681"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Quit"",
                    ""type"": ""Button"",
                    ""id"": ""7d129f86-dc24-46fc-82e4-e3ca0378d657"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Deactivate"",
                    ""type"": ""PassThrough"",
                    ""id"": ""f7c1d7d3-a603-4510-8f5a-e982a0cdf456"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Fire"",
                    ""type"": ""Value"",
                    ""id"": ""8858694a-0202-4944-a1cc-cc9cc355b5e7"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""d744ccb5-8e5a-4041-88a2-e8fe9ddbdc0c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""6a51a81f-a1cf-4569-b11c-bdd642e60f28"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone"",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""9ec9c6ee-aad9-418e-a675-bd20e1deaa12"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""50db114e-e177-4138-957d-84599c40691b"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f381e6c9-5622-452a-96f6-47a3c85e9a6a"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""d4264501-e145-4c77-b2cc-4ccf8565f2a7"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""4e56dd08-0875-43d8-94eb-bc7abc25eda3"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""b26c2112-4a7a-40f3-a10e-bbaa73df8ece"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4521ddb4-24bc-4a1c-9531-d7beb24de247"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Walk"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e419619d-27f7-4ec6-b279-c9cd401120bb"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bd4a6dc2-d0c1-4d27-96f7-29e7b1f76059"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""37da29b8-2a70-4311-bc6d-6fd1a937c8e7"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""42989e91-49ce-4fc7-bd70-207865267402"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""aedcca4e-ae8a-4082-b674-43c4ecb1e70f"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Quit"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""76084ef8-b467-42ba-b0ea-f9497830c3bc"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Quit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""afe4d322-ffe2-47c0-a754-666054354421"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Quit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""9eb9cc68-d7fe-44c0-ba90-4d681c649263"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Deactivate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0e3e3442-6e1a-4a5e-8991-9a0a21d43514"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": ""MultiTap"",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Deactivate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c3160744-a9ba-4569-9a3f-5d317a3ca8f8"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b443d034-9426-4c96-b78e-6750948ebace"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3414a3cb-e6e5-4e07-b057-3923db685f11"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7c46f16f-ab01-4a4e-b3d0-569ce2e1d434"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Commander"",
            ""id"": ""e05617fb-8a7d-4055-b3ca-e13cf694c4b4"",
            ""actions"": [
                {
                    ""name"": ""SwitchCharacter"",
                    ""type"": ""Button"",
                    ""id"": ""b2e909c2-60eb-40af-b14e-36879aba35e1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Quit"",
                    ""type"": ""Button"",
                    ""id"": ""d7e619a4-cdf9-4d73-8255-418e44c1cc04"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""dbc9ca04-16a9-4f31-849e-75aa4dabd53d"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""SwitchCharacter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d6936aca-02f5-4831-9a7d-82937dadbcf6"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""SwitchCharacter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""28fcbbd4-5e76-44f7-b788-832b584a073a"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": ""Hold(duration=0.75)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Quit"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""aa43fda1-b6ad-444a-b12b-ac4bb7ebad65"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Quit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""4c8be5b8-a314-4f9c-b7f4-11608dce93db"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Quit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Keyboard and Mouse"",
            ""bindingGroup"": ""Keyboard and Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // Character
            m_Character = asset.FindActionMap("Character", throwIfNotFound: true);
            m_Character_Move = m_Character.FindAction("Move", throwIfNotFound: true);
            m_Character_Look = m_Character.FindAction("Look", throwIfNotFound: true);
            m_Character_Walk = m_Character.FindAction("Walk", throwIfNotFound: true);
            m_Character_Aim = m_Character.FindAction("Aim", throwIfNotFound: true);
            m_Character_Jump = m_Character.FindAction("Jump", throwIfNotFound: true);
            m_Character_Quit = m_Character.FindAction("Quit", throwIfNotFound: true);
            m_Character_Deactivate = m_Character.FindAction("Deactivate", throwIfNotFound: true);
            m_Character_Fire = m_Character.FindAction("Fire", throwIfNotFound: true);
            m_Character_Interact = m_Character.FindAction("Interact", throwIfNotFound: true);
            // Commander
            m_Commander = asset.FindActionMap("Commander", throwIfNotFound: true);
            m_Commander_SwitchCharacter = m_Commander.FindAction("SwitchCharacter", throwIfNotFound: true);
            m_Commander_Quit = m_Commander.FindAction("Quit", throwIfNotFound: true);
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

        // Character
        private readonly InputActionMap m_Character;
        private ICharacterActions m_CharacterActionsCallbackInterface;
        private readonly InputAction m_Character_Move;
        private readonly InputAction m_Character_Look;
        private readonly InputAction m_Character_Walk;
        private readonly InputAction m_Character_Aim;
        private readonly InputAction m_Character_Jump;
        private readonly InputAction m_Character_Quit;
        private readonly InputAction m_Character_Deactivate;
        private readonly InputAction m_Character_Fire;
        private readonly InputAction m_Character_Interact;
        public struct CharacterActions
        {
            private @MainInputs m_Wrapper;
            public CharacterActions(@MainInputs wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_Character_Move;
            public InputAction @Look => m_Wrapper.m_Character_Look;
            public InputAction @Walk => m_Wrapper.m_Character_Walk;
            public InputAction @Aim => m_Wrapper.m_Character_Aim;
            public InputAction @Jump => m_Wrapper.m_Character_Jump;
            public InputAction @Quit => m_Wrapper.m_Character_Quit;
            public InputAction @Deactivate => m_Wrapper.m_Character_Deactivate;
            public InputAction @Fire => m_Wrapper.m_Character_Fire;
            public InputAction @Interact => m_Wrapper.m_Character_Interact;
            public InputActionMap Get() { return m_Wrapper.m_Character; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(CharacterActions set) { return set.Get(); }
            public void SetCallbacks(ICharacterActions instance)
            {
                if (m_Wrapper.m_CharacterActionsCallbackInterface != null)
                {
                    @Move.started -= m_Wrapper.m_CharacterActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_CharacterActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_CharacterActionsCallbackInterface.OnMove;
                    @Look.started -= m_Wrapper.m_CharacterActionsCallbackInterface.OnLook;
                    @Look.performed -= m_Wrapper.m_CharacterActionsCallbackInterface.OnLook;
                    @Look.canceled -= m_Wrapper.m_CharacterActionsCallbackInterface.OnLook;
                    @Walk.started -= m_Wrapper.m_CharacterActionsCallbackInterface.OnWalk;
                    @Walk.performed -= m_Wrapper.m_CharacterActionsCallbackInterface.OnWalk;
                    @Walk.canceled -= m_Wrapper.m_CharacterActionsCallbackInterface.OnWalk;
                    @Aim.started -= m_Wrapper.m_CharacterActionsCallbackInterface.OnAim;
                    @Aim.performed -= m_Wrapper.m_CharacterActionsCallbackInterface.OnAim;
                    @Aim.canceled -= m_Wrapper.m_CharacterActionsCallbackInterface.OnAim;
                    @Jump.started -= m_Wrapper.m_CharacterActionsCallbackInterface.OnJump;
                    @Jump.performed -= m_Wrapper.m_CharacterActionsCallbackInterface.OnJump;
                    @Jump.canceled -= m_Wrapper.m_CharacterActionsCallbackInterface.OnJump;
                    @Quit.started -= m_Wrapper.m_CharacterActionsCallbackInterface.OnQuit;
                    @Quit.performed -= m_Wrapper.m_CharacterActionsCallbackInterface.OnQuit;
                    @Quit.canceled -= m_Wrapper.m_CharacterActionsCallbackInterface.OnQuit;
                    @Deactivate.started -= m_Wrapper.m_CharacterActionsCallbackInterface.OnDeactivate;
                    @Deactivate.performed -= m_Wrapper.m_CharacterActionsCallbackInterface.OnDeactivate;
                    @Deactivate.canceled -= m_Wrapper.m_CharacterActionsCallbackInterface.OnDeactivate;
                    @Fire.started -= m_Wrapper.m_CharacterActionsCallbackInterface.OnFire;
                    @Fire.performed -= m_Wrapper.m_CharacterActionsCallbackInterface.OnFire;
                    @Fire.canceled -= m_Wrapper.m_CharacterActionsCallbackInterface.OnFire;
                    @Interact.started -= m_Wrapper.m_CharacterActionsCallbackInterface.OnInteract;
                    @Interact.performed -= m_Wrapper.m_CharacterActionsCallbackInterface.OnInteract;
                    @Interact.canceled -= m_Wrapper.m_CharacterActionsCallbackInterface.OnInteract;
                }
                m_Wrapper.m_CharacterActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                    @Look.started += instance.OnLook;
                    @Look.performed += instance.OnLook;
                    @Look.canceled += instance.OnLook;
                    @Walk.started += instance.OnWalk;
                    @Walk.performed += instance.OnWalk;
                    @Walk.canceled += instance.OnWalk;
                    @Aim.started += instance.OnAim;
                    @Aim.performed += instance.OnAim;
                    @Aim.canceled += instance.OnAim;
                    @Jump.started += instance.OnJump;
                    @Jump.performed += instance.OnJump;
                    @Jump.canceled += instance.OnJump;
                    @Quit.started += instance.OnQuit;
                    @Quit.performed += instance.OnQuit;
                    @Quit.canceled += instance.OnQuit;
                    @Deactivate.started += instance.OnDeactivate;
                    @Deactivate.performed += instance.OnDeactivate;
                    @Deactivate.canceled += instance.OnDeactivate;
                    @Fire.started += instance.OnFire;
                    @Fire.performed += instance.OnFire;
                    @Fire.canceled += instance.OnFire;
                    @Interact.started += instance.OnInteract;
                    @Interact.performed += instance.OnInteract;
                    @Interact.canceled += instance.OnInteract;
                }
            }
        }
        public CharacterActions @Character => new CharacterActions(this);

        // Commander
        private readonly InputActionMap m_Commander;
        private ICommanderActions m_CommanderActionsCallbackInterface;
        private readonly InputAction m_Commander_SwitchCharacter;
        private readonly InputAction m_Commander_Quit;
        public struct CommanderActions
        {
            private @MainInputs m_Wrapper;
            public CommanderActions(@MainInputs wrapper) { m_Wrapper = wrapper; }
            public InputAction @SwitchCharacter => m_Wrapper.m_Commander_SwitchCharacter;
            public InputAction @Quit => m_Wrapper.m_Commander_Quit;
            public InputActionMap Get() { return m_Wrapper.m_Commander; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(CommanderActions set) { return set.Get(); }
            public void SetCallbacks(ICommanderActions instance)
            {
                if (m_Wrapper.m_CommanderActionsCallbackInterface != null)
                {
                    @SwitchCharacter.started -= m_Wrapper.m_CommanderActionsCallbackInterface.OnSwitchCharacter;
                    @SwitchCharacter.performed -= m_Wrapper.m_CommanderActionsCallbackInterface.OnSwitchCharacter;
                    @SwitchCharacter.canceled -= m_Wrapper.m_CommanderActionsCallbackInterface.OnSwitchCharacter;
                    @Quit.started -= m_Wrapper.m_CommanderActionsCallbackInterface.OnQuit;
                    @Quit.performed -= m_Wrapper.m_CommanderActionsCallbackInterface.OnQuit;
                    @Quit.canceled -= m_Wrapper.m_CommanderActionsCallbackInterface.OnQuit;
                }
                m_Wrapper.m_CommanderActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @SwitchCharacter.started += instance.OnSwitchCharacter;
                    @SwitchCharacter.performed += instance.OnSwitchCharacter;
                    @SwitchCharacter.canceled += instance.OnSwitchCharacter;
                    @Quit.started += instance.OnQuit;
                    @Quit.performed += instance.OnQuit;
                    @Quit.canceled += instance.OnQuit;
                }
            }
        }
        public CommanderActions @Commander => new CommanderActions(this);
        private int m_GamepadSchemeIndex = -1;
        public InputControlScheme GamepadScheme
        {
            get
            {
                if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
                return asset.controlSchemes[m_GamepadSchemeIndex];
            }
        }
        private int m_KeyboardandMouseSchemeIndex = -1;
        public InputControlScheme KeyboardandMouseScheme
        {
            get
            {
                if (m_KeyboardandMouseSchemeIndex == -1) m_KeyboardandMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard and Mouse");
                return asset.controlSchemes[m_KeyboardandMouseSchemeIndex];
            }
        }
        public interface ICharacterActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnLook(InputAction.CallbackContext context);
            void OnWalk(InputAction.CallbackContext context);
            void OnAim(InputAction.CallbackContext context);
            void OnJump(InputAction.CallbackContext context);
            void OnQuit(InputAction.CallbackContext context);
            void OnDeactivate(InputAction.CallbackContext context);
            void OnFire(InputAction.CallbackContext context);
            void OnInteract(InputAction.CallbackContext context);
        }
        public interface ICommanderActions
        {
            void OnSwitchCharacter(InputAction.CallbackContext context);
            void OnQuit(InputAction.CallbackContext context);
        }
    }
}
