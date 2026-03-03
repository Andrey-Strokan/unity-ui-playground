using SaritasaVRToolset;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

namespace Saritasa.Controllers
{
    /// <summary>
    /// Manager of controllers.
    /// Setup models with tooltips.
    /// </summary>
    public class ControllersManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Reference to left controller")]
        private ControllerInputActionManager lController;

        [SerializeField]
        [Tooltip("Reference to right controller")]
        private ControllerInputActionManager rController;

        [SerializeField]
        [Tooltip("Show controllers if tooltips are turned off")]
        private bool showControllersWithoutTooltips = false;

        private bool isControllersInitialized = false;

        private bool leftControllerTooltipsInitialized = false;
        private bool rightControllerTooltipsInitialized = false;

        private bool leftControllerInitialized = false;
        private bool rightControllerInitialized = false;

        /// <summary>
        /// If controllers already initialized returns true.
        /// </summary>
        public bool IsControllersInitialized => isControllersInitialized;

        /// <summary>
        /// Show controllers if tooltips are turned off.
        /// </summary>
        public bool ShowControllersWithoutTooltips
        {
            get => showControllersWithoutTooltips;
        }

        /// <summary>
        /// Left controller tooltip manager.
        /// </summary>
        public VR_ControllerTooltipManager LeftTooltipManager { get; private set; }

        /// <summary>
        /// Right controller tooltip manager.
        /// </summary>
        public VR_ControllerTooltipManager RightTooltipManager { get; private set; }

        private readonly ControllerPreset[] controllerPresets =
        {
            new OculusQuestControllerPreset(),
            new OculusQuest2ControllerPreset(),
            new OculusQuest3ControllerPreset(),
            new PicoNeo2ControllerPreset(),
            new PicoG2ControllerPreset(),
            new OculusRiftSControllerPreset()
        };

        /// <summary>
        /// Called when headset was selected.
        /// </summary>
        public event Action ControllersInitialized;

        private void OnEnable()
        {
            InputDevices.deviceConnected += DeviceConnected;
            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevices(devices);

            foreach (var device in devices)
            {
                DeviceConnected(device);
            }
        }

        private void OnDisable()
        {
            InputDevices.deviceConnected -= DeviceConnected;

            if (LeftTooltipManager != null)
            {
                LeftTooltipManager.ControllerTooltipManagerInitialized -= OnLeftControllerTooltipMangerInitialized;
            }

            if (RightTooltipManager != null)
            {
                RightTooltipManager.ControllerTooltipManagerInitialized -= OnRightControllerTooltipMangerInitialized;
            }
        }

        private void OnLeftControllerTooltipMangerInitialized()
        {
            LeftTooltipManager.ControllerTooltipManagerInitialized -= OnLeftControllerTooltipMangerInitialized;
            LeftTooltipManager.HideAllTooltips(0f);
            leftControllerTooltipsInitialized = true;
            Debug.Log("Left tooltip manager assigned");
            OnInitialized();
        }

        private void OnRightControllerTooltipMangerInitialized()
        {
            RightTooltipManager.ControllerTooltipManagerInitialized -= OnRightControllerTooltipMangerInitialized;
            RightTooltipManager.HideAllTooltips(0f);
            rightControllerTooltipsInitialized = true;
            Debug.Log("Right tooltip manager assigned");
            OnInitialized();
        }

        private void DeviceConnected(InputDevice device)
        {
            // Oculus controllers must have characteristics,
            // so we use them to identify the right and left
            // The Left Hand
            if ((device.characteristics & InputDeviceCharacteristics.Left) != 0)
            {
                leftControllerInitialized = true;
                var preset = GetControllers(device.name);
                var controller = preset.GetLeftControllerPrefabTransform();
                Instantiate(controller, lController.ControllersModelParent);
                LeftTooltipManager = lController.GetComponentInChildren<VR_ControllerTooltipManager>(true);

                if (LeftTooltipManager == null)
                    return;

                LeftTooltipManager.ShowControllerWithNoTooltips = ShowControllersWithoutTooltips;

                if (LeftTooltipManager.Initialized)
                {
                    LeftTooltipManager.HideAllTooltips(0f);
                    leftControllerTooltipsInitialized = true;
                    Debug.Log("Left tooltip manager assigned");
                    OnInitialized();
                }
                else
                {
                    LeftTooltipManager.ControllerTooltipManagerInitialized += OnLeftControllerTooltipMangerInitialized;
                }
            }
            // The Right hand
            else if ((device.characteristics & InputDeviceCharacteristics.Right) != 0)
            {
                rightControllerInitialized = true;
                var preset = GetControllers(device.name);
                var controller = preset.GetRightControllerPrefabTransform();
                Instantiate(controller, rController.ControllersModelParent);
                RightTooltipManager = rController.GetComponentInChildren<VR_ControllerTooltipManager>(true);

                if (RightTooltipManager == null)
                    return;

                RightTooltipManager.ShowControllerWithNoTooltips = ShowControllersWithoutTooltips;

                if (RightTooltipManager.Initialized)
                {
                    RightTooltipManager.HideAllTooltips(0f);
                    rightControllerTooltipsInitialized = true;
                    Debug.Log("Right tooltip manager assigned");
                    OnInitialized();
                }
                else
                {
                    RightTooltipManager.ControllerTooltipManagerInitialized += OnRightControllerTooltipMangerInitialized;
                }
            }

            if (leftControllerInitialized && rightControllerInitialized)
            {
                InputDevices.deviceConnected -= DeviceConnected;
            }
        }

        private void OnInitialized()
        {
            if (leftControllerTooltipsInitialized && rightControllerTooltipsInitialized)
            {
                ControllersInitialized?.Invoke();
                isControllersInitialized = true;
            }
        }

        private ControllerPreset GetControllers(string controllersName)
        {
            if (TryGetControllerPresetByName(controllersName, out var preset))
            {
                return preset;
            }

            var deviceSimulator = FindObjectOfType(typeof(XRDeviceSimulator));

            if (deviceSimulator == null)
            {
                Debug.LogError("Headset is not found! Force Oculus Rift.");
            }
            else
            {
                Debug.Log("Device simulator was found! Force Oculus Rift.");
            }

            return new OculusRiftSControllerPreset();
        }

        private bool TryGetControllerPresetByName(string controllerName, out ControllerPreset preset)
        {
            Debug.Log($"Try to get preset for {controllerName}");
            preset = controllerPresets[0];
            foreach (ControllerPreset controllerPreset in controllerPresets)
            {
                foreach (var nameVariant in controllerPreset.HeadSetNameVariants)
                {
                    if (nameVariant.Equals(controllerName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Debug.Log(nameVariant + " Found");
                        preset = controllerPreset;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}