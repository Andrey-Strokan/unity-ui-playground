using Saritasa.Controllers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

/// <summary>
/// Container for references used for tutorial. Passed to your tutorial steps.
/// Override this class to add your custom references in order to get acess to them
/// in your tutorial steps. You should pass reference to instance of inherited class
/// to TutorialManager.
/// </summary>
public abstract class TutorialContextBase : MonoBehaviour
{
    // TODO: Review if it is required to move base references to tutorial manager.

    /// <summary>
    /// Controllers manager.
    /// </summary>
    [field: SerializeField]
    [Tooltip("Controllers Manager reference.")]
    public ControllersManager ControllersManager { get; private set; }

    /// <summary>
    /// Menu confirm button actions reference.
    /// </summary>
    [field: SerializeField]
    public InputActionProperty NextStepMenuButtonAction { get; private set; }

    /// <summary>
    /// Left hand tooltip manager.
    /// </summary>
    public VR_ControllerTooltipManager LeftTooltipManager { get; private set; }

    /// <summary>
    /// Right hand tooltip manager.
    /// </summary>
    public VR_ControllerTooltipManager RightTooltipManager { get; private set; }

    private void Awake()
    {
        ControllersManager.ControllersInitialized += SetControllerTooltips;
    }

    private void OnDestroy()
    {
        ControllersManager.ControllersInitialized -= SetControllerTooltips;
    }

    private void SetControllerTooltips()
    {
        LeftTooltipManager = ControllersManager.LeftTooltipManager;
        RightTooltipManager = ControllersManager.RightTooltipManager;
    }
}