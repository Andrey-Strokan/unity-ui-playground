using System.Collections.Generic;
using Unity.XR.CoreUtils.Bindings;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace SaritasaVRToolset
{
    /// <summary>
    /// Use this class to mediate the interactors for a controller under different interaction states
    /// and the input actions used by them.
    /// </summary>
    /// <remarks>
    /// If the teleport ray input is engaged, the Ray Interactor used for distant manipulation is disabled
    /// and the Ray Interactor used for teleportation is enabled. If the Ray Interactor is selecting and it
    /// is configured to allow for attach transform manipulation, all locomotion input actions are disabled
    /// (teleport ray, move, and turn controls) to prevent input collision with the manipulation inputs used
    /// by the ray interactor.
    /// <br />
    /// A typical hierarchy also includes an XR Interaction Group component to mediate between interactors.
    /// The interaction group ensures that the Direct and Ray Interactors cannot interact at the same time,
    /// with the Direct Interactor taking priority over the Ray Interactor.
    /// </remarks>
    public class ControllerInputActionManager : MonoBehaviour
    {
        [Space]
        [Header("Controllers model parent")]

        [SerializeField]
        private Transform controllersModelParent;

        [Space]
        [Header("Interactors")]

        [SerializeField]
        [Tooltip("The interactor used for distant/ray manipulation. Use this or Near-Far Interactor, not both.")]
        private XRRayInteractor rayInteractor;

        [SerializeField]
        [Tooltip("Near-Far Interactor used for distant/ray manipulation. Use this or Ray Interactor, not both.")]
        private NearFarInteractor nearFarInteractor;

        [SerializeField]
        [Tooltip("The interactor used for teleportation.")]
        private XRRayInteractor teleportInteractor;

        [Space]
        [Header("Controller Actions")]

        [SerializeField]
        [Tooltip("The reference to the action to start the teleport aiming mode for this controller.")]
        [FormerlySerializedAs("m_TeleportModeActivate")]
        private InputActionReference teleportMode;

        [SerializeField]
        [Tooltip("The reference to the action to cancel the teleport aiming mode for this controller.")]
        private InputActionReference teleportModeCancel;

        [SerializeField]
        [Tooltip("The reference to the action of continuous turning the XR Origin with this controller.")]
        private InputActionReference turn;

        [SerializeField]
        [Tooltip("The reference to the action of snap turning the XR Origin with this controller.")]
        private InputActionReference snapTurn;

        [SerializeField]
        [Tooltip("The reference to the action of moving the XR Origin with this controller.")]
        private InputActionReference move;

        [SerializeField]
        [Tooltip("The reference to the action of scrolling UI with this controller.")]
        private InputActionReference uiScroll;

        [SerializeField]
        [Tooltip("The reference to the action of open the pause menu with this controller.")]
        private InputActionReference openMenu;

        [Space]
        [Header("Locomotion Settings")]

        [SerializeField]
        [Tooltip("If true, continuous movement will be enabled. If false, teleport will be enabled.")]
        private bool smoothMotionEnabled;

        [SerializeField]
        [Tooltip("If true, continuous turn will be enabled. If false, snap turn will be enabled. Note: If smooth motion is enabled and enable strafe is enabled on the continuous move provider, turn will be overriden in favor of strafe.")]
        private bool smoothTurnEnabled;

        [SerializeField]
        [Tooltip("If true, teleportation will be enabled. If false, continuous move will be enabled. Note: If smooth motion is enabled and enable strafe is enabled on the continuous move provider, turn will be overriden in favor of strafe.")]
        private bool snapTurnEnabled;

        [SerializeField]
        [Tooltip("If true, snap turn will be enabled. If false, continuous turn will be enabled. Note: If smooth motion is enabled and enable strafe is enabled on the continuous move provider, turn will be overriden in favor of strafe.")]
        private bool teleportationEnabled;

        [SerializeField]
        [Tooltip("With the Near-Far Interactor, if true, teleport will be enabled during near interaction. If false, teleport will be disabled during near interaction.")]
        private bool nearFarEnableTeleportDuringNearInteraction = true;

        [Space]
        [Header("UI Settings")]

        [SerializeField]
        [Tooltip("If true, UI scrolling will be enabled. Locomotion will be disabled when pointing at UI to allow it to be scrolled.")]
        private bool uiScrollingEnabled = true;

        [Space]
        [Header("Mediation Events")]

        [SerializeField]
        [Tooltip("Event fired when the active ray interactor changes between interaction and teleport.")]
        private UnityEvent<IXRRayProvider> rayInteractorChanged;

        /// <summary>
        /// True if locomotion actions was initialized.
        /// </summary>
        public bool LocomotionInitialized => locomotionInitialized;

        /// <summary>
        /// Field for control smooth motion interactions.
        /// </summary>
        public bool SmoothMotionEnabled
        {
            get
            {
                return smoothMotionEnabled;
            }
            set
            {
                smoothMotionEnabled = value;
                UpdateLocomotionActions();
            }
        }

        /// <summary>
        /// Field for control smooth turn interactions.
        /// </summary>
        public bool SmoothTurnEnabled
        {
            get
            {
                return smoothTurnEnabled;
            }
            set
            {
                smoothTurnEnabled = value;
                UpdateLocomotionActions();
            }
        }

        /// <summary>
        /// Field for control snap turn interactions.
        /// </summary>
        public bool SnapTurnEnabled
        {
            get
            {
                return snapTurnEnabled;
            }
            set
            {
                snapTurnEnabled = value;
                UpdateLocomotionActions();
            }
        }

        /// <summary>
        /// Field for control teleportation interactions.
        /// </summary>
        public bool TeleportationEnabled
        {
            get
            {
                return teleportationEnabled;
            }
            set
            {
                teleportationEnabled = value;
                UpdateLocomotionActions();
            }
        }

        /// <summary>
        /// Field for control ui scrolling interactions.
        /// </summary>
        public bool UIScrollingEnabled
        {
            get
            {
                return uiScrollingEnabled;
            }
            set
            {
                uiScrollingEnabled = value;
                UpdateUIActions();
            }
        }

        /// <summary>
        /// Transform of parent for controllers model.
        /// </summary>
        public Transform ControllersModelParent => controllersModelParent;

        private bool startCalled;
        private bool postponedDeactivateTeleport;
        private bool hoveringScrollableUI;
        private bool locomotionInitialized;
        private bool menuOpened = false;

        private readonly HashSet<InputAction> locomotionUsers = new HashSet<InputAction>();
        private readonly BindingsGroup bindingsGroup = new BindingsGroup();

        /// <summary>
        /// Get value from teleport mode actions.
        /// </summary>
        public Vector2 GetTeleportModeValue()
        {
            return teleportMode.action.ReadValue<Vector2>();
        }

        /// <summary>
        /// Get value from move actions.
        /// </summary>
        public Vector2 GetMoveValue()
        {
            return move.action.ReadValue<Vector2>();
        }

        private void SetupInteractorEvents()
        {
            if (nearFarInteractor != null)
            {
                nearFarInteractor.uiHoverEntered.AddListener(OnUIHoverEntered);
                nearFarInteractor.uiHoverExited.AddListener(OnUIHoverExited);
                bindingsGroup.AddBinding(nearFarInteractor.selectionRegion.Subscribe(OnNearFarSelectionRegionChanged));
            }

            if (rayInteractor != null)
            {
                rayInteractor.selectEntered.AddListener(OnRaySelectEntered);
                rayInteractor.selectExited.AddListener(OnRaySelectExited);
                rayInteractor.uiHoverEntered.AddListener(OnUIHoverEntered);
                rayInteractor.uiHoverExited.AddListener(OnUIHoverExited);
            }

            var teleportModeAction = GetInputAction(teleportMode);
            if (teleportModeAction != null)
            {
                teleportModeAction.performed += OnStartTeleport;
                teleportModeAction.performed += OnStartLocomotion;
                teleportModeAction.canceled += OnCancelTeleport;
                teleportModeAction.canceled += OnStopLocomotion;
            }

            var teleportModeCancelAction = GetInputAction(teleportModeCancel);
            if (teleportModeCancelAction != null)
            {
                teleportModeCancelAction.performed += OnCancelTeleport;
            }

            var moveAction = GetInputAction(move);
            if (moveAction != null)
            {
                moveAction.started += OnStartLocomotion;
                moveAction.canceled += OnStopLocomotion;
            }

            var turnAction = GetInputAction(turn);
            if (turnAction != null)
            {
                turnAction.started += OnStartLocomotion;
                turnAction.canceled += OnStopLocomotion;
            }

            var snapTurnAction = GetInputAction(snapTurn);
            if (snapTurnAction != null)
            {
                snapTurnAction.started += OnStartLocomotion;
                snapTurnAction.canceled += OnStopLocomotion;
            }

            var openMenuAction = GetInputAction(openMenu);
            if (openMenuAction != null)
            {
                openMenuAction.performed += OnMenuOpened;
            }
        }

        private void TeardownInteractorEvents()
        {
            bindingsGroup.Clear();

            if (nearFarInteractor != null)
            {
                nearFarInteractor.uiHoverEntered.RemoveListener(OnUIHoverEntered);
                nearFarInteractor.uiHoverExited.RemoveListener(OnUIHoverExited);
            }

            if (rayInteractor != null)
            {
                rayInteractor.selectEntered.RemoveListener(OnRaySelectEntered);
                rayInteractor.selectExited.RemoveListener(OnRaySelectExited);
                rayInteractor.uiHoverEntered.RemoveListener(OnUIHoverEntered);
                rayInteractor.uiHoverExited.RemoveListener(OnUIHoverExited);
            }

            var teleportModeAction = GetInputAction(teleportMode);
            if (teleportModeAction != null)
            {
                teleportModeAction.performed -= OnStartTeleport;
                teleportModeAction.performed -= OnStartLocomotion;
                teleportModeAction.canceled -= OnCancelTeleport;
                teleportModeAction.canceled -= OnStopLocomotion;
            }

            var teleportModeCancelAction = GetInputAction(teleportModeCancel);
            if (teleportModeCancelAction != null)
            {
                teleportModeCancelAction.performed -= OnCancelTeleport;
            }

            var moveAction = GetInputAction(move);
            if (moveAction != null)
            {
                moveAction.started -= OnStartLocomotion;
                moveAction.canceled -= OnStopLocomotion;
            }

            var turnAction = GetInputAction(turn);
            if (turnAction != null)
            {
                turnAction.started -= OnStartLocomotion;
                turnAction.canceled -= OnStopLocomotion;
            }

            var snapTurnAction = GetInputAction(snapTurn);
            if (snapTurnAction != null)
            {
                snapTurnAction.started -= OnStartLocomotion;
                snapTurnAction.canceled -= OnStopLocomotion;
            }

            var openMenuAction = GetInputAction(openMenu);
            if (openMenuAction != null)
            {
                openMenuAction.performed -= OnMenuOpened;
            }
        }

        private void OnStartTeleport(InputAction.CallbackContext context)
        {
            postponedDeactivateTeleport = false;

            if (teleportInteractor != null)
            {
                teleportInteractor.gameObject.SetActive(true);
            }

            if (rayInteractor != null)
            {
                rayInteractor.gameObject.SetActive(false);
            }

            if (nearFarInteractor != null && nearFarInteractor.selectionRegion.Value != NearFarInteractor.Region.Near)
            {
                nearFarInteractor.gameObject.SetActive(false);
            }

            rayInteractorChanged?.Invoke(teleportInteractor);
        }

        private void OnCancelTeleport(InputAction.CallbackContext context)
        {
            // Do not deactivate the teleport interactor in this callback.
            // We delay turning off the teleport interactor in this callback so that
            // the teleport interactor has a chance to complete the teleport if needed.
            // OnAfterInteractionEvents will handle deactivating its GameObject.
            postponedDeactivateTeleport = true;

            if (rayInteractor != null)
            {
                rayInteractor.gameObject.SetActive(true);
            }

            if (nearFarInteractor != null)
            {
                nearFarInteractor.gameObject.SetActive(true);
            }

            rayInteractorChanged?.Invoke(rayInteractor);
        }

        private void OnNearFarSelectionRegionChanged(NearFarInteractor.Region selectionRegion)
        {
            if (selectionRegion == NearFarInteractor.Region.Far ||
                (selectionRegion == NearFarInteractor.Region.Near && !nearFarEnableTeleportDuringNearInteraction))
            {
                DisableTeleportActions();
            }
            else if (!menuOpened)
            {
                UpdateLocomotionActions();
            }
        }

        private void OnStartLocomotion(InputAction.CallbackContext context)
        {
            locomotionUsers.Add(context.action);
        }

        private void OnStopLocomotion(InputAction.CallbackContext context)
        {
            locomotionUsers.Remove(context.action);

            if (locomotionUsers.Count == 0 && hoveringScrollableUI)
            {
                DisableAllLocomotionActions();
                UpdateUIActions();
            }
        }

        private void OnRaySelectEntered(SelectEnterEventArgs args)
        {
            if (rayInteractor.manipulateAttachTransform)
            {
                // Disable locomotion and turn actions.
                DisableAllLocomotionActions();
            }
        }

        private void OnRaySelectExited(SelectExitEventArgs args)
        {
            if (rayInteractor.manipulateAttachTransform && !menuOpened)
            {
                // Re-enable the locomotion and turn actions.
                UpdateLocomotionActions();
            }
        }

        private void OnUIHoverEntered(UIHoverEventArgs args)
        {
            hoveringScrollableUI = uiScrollingEnabled && args.deviceModel.isScrollable;
            UpdateUIActions();

            // If locomotion is occurring, wait.
            if (hoveringScrollableUI && locomotionUsers.Count == 0)
            {
                // Disable locomotion and turn actions.
                DisableAllLocomotionActions();
            }
        }

        private void OnUIHoverExited(UIHoverEventArgs args)
        {
            hoveringScrollableUI = false;
            UpdateUIActions();

            if (!menuOpened)
            {
                // Re-enable the locomotion and turn actions.
                UpdateLocomotionActions();
            }
        }

        private void OnMenuOpened(InputAction.CallbackContext callbackContext)
        {
            if (menuOpened)
            {
                // Re-enable the locomotion and turn actions.
                UpdateLocomotionActions();
                menuOpened = false;

                return;
            }

            // If locomotion is occurring, wait.
            if (locomotionUsers.Count == 0)
            {
                // Disable locomotion and turn actions.
                DisableAllLocomotionActions();
                menuOpened = true;
            }
        }

        protected void OnEnable()
        {
            if (rayInteractor != null && nearFarInteractor != null)
            {
                Debug.LogWarning("Both Ray Interactor and Near-Far Interactor are assigned. Only one should be assigned, not both. Clearing Ray Interactor.", this);
                rayInteractor = null;
            }

            if (teleportInteractor != null)
            {
                teleportInteractor.gameObject.SetActive(false);
            }

            // Allow the actions to be refreshed when this component is re-enabled.
            // See comments in Start for why we wait until Start to enable/disable actions.
            if (startCalled)
            {
                UpdateLocomotionActions();
                UpdateUIActions();
            }

            SetupInteractorEvents();
        }

        protected void OnDisable()
        {
            TeardownInteractorEvents();
        }

        protected void Start()
        {
            startCalled = true;

            // Ensure the enabled state of locomotion and turn actions are properly set up.
            // Called in Start so it is done after the InputActionManager enables all input actions earlier in OnEnable.
            UpdateLocomotionActions();
            UpdateUIActions();

            locomotionInitialized = true;
        }

        protected void Update()
        {
            // Start the coroutine that executes code after the Update phase (during yield null).
            // Since this behavior has the default execution order, it runs after the XRInteractionManager,
            // so selection events have been finished by now this frame. This means that the teleport interactor
            // has had a chance to process its select interaction event and teleport if needed.
            if (postponedDeactivateTeleport)
            {
                if (teleportInteractor != null)
                {
                    teleportInteractor.gameObject.SetActive(false);
                }

                postponedDeactivateTeleport = false;
            }
        }

        private void UpdateLocomotionActions()
        {
            // Disable/enable Teleport and Turn when Move is enabled/disabled.
            SetEnabled(move, !TeleportationEnabled && SmoothMotionEnabled);
            SetEnabled(teleportMode, !SmoothMotionEnabled && TeleportationEnabled);
            SetEnabled(teleportModeCancel, !SmoothMotionEnabled && TeleportationEnabled);

            // Disable ability to turn when using continuous movement.
            SetEnabled(turn, !SmoothMotionEnabled && !SnapTurnEnabled && SmoothTurnEnabled);
            SetEnabled(snapTurn, !SmoothMotionEnabled && !SmoothTurnEnabled && SnapTurnEnabled);
        }

        private void DisableTeleportActions()
        {
            DisableAction(teleportMode);
            DisableAction(teleportModeCancel);
        }

        private void DisableMoveAndTurnActions()
        {
            DisableAction(move);
            DisableAction(turn);
            DisableAction(snapTurn);
        }

        private void DisableAllLocomotionActions()
        {
            DisableTeleportActions();
            DisableMoveAndTurnActions();
        }

        private void UpdateUIActions()
        {
            SetEnabled(uiScroll, UIScrollingEnabled && hoveringScrollableUI && locomotionUsers.Count == 0);
        }

        private static void SetEnabled(InputActionReference actionReference, bool enabled)
        {
            if (enabled)
            {
                EnableAction(actionReference);
            }
            else
            {
                DisableAction(actionReference);
            }
        }

        private static void EnableAction(InputActionReference actionReference)
        {
            var action = GetInputAction(actionReference);

            if (action != null && !action.enabled)
            {
                action.Enable();
            }
        }

        private static void DisableAction(InputActionReference actionReference)
        {
            var action = GetInputAction(actionReference);

            if (action != null && action.enabled)
            {
                action.Disable();
            }
        }

        private static InputAction GetInputAction(InputActionReference actionReference)
        {
#pragma warning disable IDE0031 // Use null propagation -- Do not use for UnityEngine.Object types.
            return actionReference != null ? actionReference.action : null;
#pragma warning restore IDE0031
        }
    }
}
