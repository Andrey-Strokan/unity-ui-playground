using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static TutorialControllerTooltipData;

/// <summary>
/// A core component of the tutotrial system. This class manages tutorial steps, their state and
/// transitions between then. It's also responsible for for creating Tutorial Action classes and
/// setting up tutorial UI and controller tooltips.
/// </summary>
[RequireComponent(typeof(TutorialContextBase))]
public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to a TutorialSetup which holds data for this tutorial.")]
    private List<TutorialSetup> tutorialSetups;

    private int currentTutorialSetup = 0;

    [SerializeField]
    [Tooltip("Reference to TutorialContextBase on scene.")]
    private TutorialContextBase context;

    [SerializeField]
    [Tooltip("Reference to a component derived from TutorialScreenBase which will be used to display this tutorial.")]
    private TutorialScreenBase screen;

    [SerializeField]
    [Tooltip("Start tutorial on load scene.")]
    private bool startTutorialOnSceneLoad;

    private bool isTutorialStarted;

    private Coroutine logicCoroutine;

    private TutorialActionBase currentAction;

    private int currentStage;

    public int CurrentStepIndex => currentStage;

    private Assembly tutorialImplAssembly;

    /// <summary>
    /// Start tutorial sequence from the begining.
    /// </summary>
    private void StartTutorialInternal()
    {
        if (isTutorialStarted)
        {
            Debug.LogError("Tutorial already started");
        }

        var current = tutorialSetups[currentTutorialSetup].Steps[currentStage];
        var actionName = tutorialSetups[currentTutorialSetup].ActionClassesNames[currentStage];

        tutorialImplAssembly = AssemblyUtility.FindAssemblyByName(tutorialSetups[currentTutorialSetup].GetAssemblyName());
        if (tutorialImplAssembly == null)
        {
            Debug.LogError("Missing tutorial steps implementation assembly");
        }

        isTutorialStarted = true;
        ShowMainScreen(true);
        ActivateTooltipsForSubstep(0);

        currentAction = CreateActionByName(actionName);

        if (currentAction == null)
        {
            Debug.LogError($"Failed to create instance of a Tutorial Action class {actionName}");
        }

        currentAction.InvokeBegin();
        logicCoroutine = StartCoroutine(currentAction.ActionCoroutine());
    }

    /// <summary>
    /// Hide tutorial screen.
    /// </summary>
    public void HideTutorialScreen()
    {
        ShowMainScreen(false);
    }

    /// <summary>
    /// Start tutorial sequence with tutorial setup by number.
    /// </summary>
    /// <param name="tutorialSetupNumber"></param>
    public void StartTutorial(int tutorialSetupNumber)
    {
        currentTutorialSetup = tutorialSetupNumber;
        StartTutorial();
    }

    /// <summary>
    /// Stop tutorial.
    /// </summary>
    public void StopTutorial()
    {
        if (isTutorialStarted)
        {
            if (logicCoroutine != null)
            {
                StopCoroutine(logicCoroutine);
            }

            currentStage = 0;
            isTutorialStarted = false;
            currentAction.InvokeEnd();
        }
    }

    /// <summary>
    /// Activate tooltips from the specified substep.
    /// </summary>
    public void ActivateTooltipsForSubstep(int substepIndex)
    {
        var current = tutorialSetups[currentTutorialSetup].Steps[currentStage];
        ActivateTooltipsForStep(current, substepIndex);
    }

    /// <summary>
    /// Force sets the next tutorial step and substep.
    /// </summary>
    public void SetStepByIndex(int stepIndex, int substepIndex = 0)
    {
        HideControllerTooltips();

        if (logicCoroutine != null)
        {
            StopCoroutine(logicCoroutine);
        }
        logicCoroutine = null;

        currentAction.InvokeEnd();
        currentAction = null;

        currentStage = stepIndex;
        if (currentStage < tutorialSetups[currentTutorialSetup].Steps.Count && currentStage >= 0)
        {
            var actionName = tutorialSetups[currentTutorialSetup].ActionClassesNames[currentStage];

            ShowMainScreen(true);
            ActivateTooltipsForSubstep(substepIndex);

            currentAction = CreateActionByName(actionName);
            if (currentAction == null)
            {
                Debug.LogError($"Failed to create instance of a Tutorial Action class {actionName}");
            }

            currentAction.InvokeBegin();
            logicCoroutine = StartCoroutine(currentAction.ActionCoroutine());
        }
        else
        {
            isTutorialStarted = false;
            currentStage = 0;
        }
    }

    /// <summary>
    /// Switch to the next tutorial step. Intended to be called in Tutorial Action coroutine, when
    /// all step conditions are satisfied.
    /// </summary>
    public void StepForward()
    {
        if (!isTutorialStarted)
        {
            return;
        }
        SetStepByIndex(++currentStage);
    }

    /// <summary>
    /// Switch to the previous tutorial step.
    /// </summary>
    public void StepBackward()
    {
        if (!isTutorialStarted)
        {
            return;
        }
        SetStepByIndex(--currentStage);
    }

    /// <summary>
    /// Set tooltip state.
    /// <param name="hand">A hand for which the tooltip should be enabled.</param>
    /// <param name="button">A button for which the tooltip should be enabled.</param>
    /// <param name="substepIndex">A substep from which the tooltip should be taken.</param>
    /// <param name="active">True to activate tooltip, false to hide</param>
    /// </summary>
    public void SetControllerTooltipActive(ControllerHands hand, VR_ControllerButtons button, int substepIndex,
        bool active)
    {
        if (!isTutorialStarted)
        {
            return;
        }

        var current = tutorialSetups[currentTutorialSetup].Steps[currentStage];

        if (substepIndex <= 0 && substepIndex >= current.SubSteps.Length)
        {
            return;
        }

        foreach (var tooltip in current.SubSteps[substepIndex].Tooltips)
        {
            if (tooltip.ControllerHand == hand && tooltip.ControllerButton == button)
            {
                switch (tooltip.ControllerHand)
                {
                    case ControllerHands.Right:
                    {
                        var controller = context.RightTooltipManager;
                        SetControllerTooltipActive(controller, tooltip, active);
                    }
                    break;
                    case ControllerHands.Left:
                    {
                        var controller = context.LeftTooltipManager;
                        SetControllerTooltipActive(controller, tooltip, active);
                    }
                    break;
                    case ControllerHands.Both:
                    {
                        var leftController = context.LeftTooltipManager;
                        var rightController = context.RightTooltipManager;
                        SetControllerTooltipActive(leftController, tooltip, active);
                        SetControllerTooltipActive(rightController, tooltip, active);
                    }
                    break;

                    default:
                    {
                        throw new Exception("Invalid type");
                    }
                }
            }
        }
    }

    private void SetControllerTooltipActive(VR_ControllerTooltipManager tooltipManager,
        TutorialControllerTooltipData tooltip, bool active)
    {
        if (active)
        {
            tooltipManager.ShowTooltip(tooltip.ControllerButton);
            tooltipManager.SetTooltipText(tooltip.ControllerButton, tooltip.Text);
        }
        else
        {
            tooltipManager.HideTooltip(tooltip.ControllerButton);
        }
    }

    private TutorialActionBase CreateActionByName(string name)
    {
        TutorialActionBase result = null;

        if (name != null)
        {
            if (tutorialImplAssembly == null)
            {
                Debug.LogError("Missing assembly");
            }

            Type type = tutorialImplAssembly.GetType(name);
            if (type != null)
            {
                if (typeof(TutorialActionBase).IsAssignableFrom(type))
                {
                    var constructor = type.GetConstructor(Type.EmptyTypes);
                    result = (TutorialActionBase)constructor.Invoke(new object[] { });
                    result.SetReferences(this, context);
                }
            }
        }

        return result;
    }

    private void ShowMainScreen(bool show)
    {
        if (show)
        {
            var current = tutorialSetups[currentTutorialSetup].Steps[currentStage];
            screen?.SetText(current.Title, current.Message);
        }

        screen?.Show(show);
    }

    private void ActivateTooltipsForStep(TutorialStepData stage, int substepIndex)
    {
        HideControllerTooltips();
        if (substepIndex >= 0 && substepIndex < stage.SubSteps.Length)
        {
            foreach (var t in stage.SubSteps[substepIndex].Tooltips)
            {
                SetControllerTooltipActive(t.ControllerHand, t.ControllerButton, substepIndex, true);
            }
        }
    }

    private void HideControllerTooltips()
    {
        context.LeftTooltipManager.HideAllTooltips();
        context.RightTooltipManager.HideAllTooltips();
    }

    private void Awake()
    {
        if (context == null)
        {
            Debug.LogError("Unable to find TutorialContextBase component. It should be placed on the same GameObject");
        }
    }

    private void Start()
    {
        screen?.Show(false);
        if (startTutorialOnSceneLoad)
        {
            StartTutorial();
        }
    }

    /// <summary>
    /// Wrapper for tutorial start. Need make sure that controllers initialized.
    /// </summary>
    public void StartTutorial()
    {
        if (context.ControllersManager.IsControllersInitialized)
        {
            StartTutorialInternal();
        }
        else
        {
            context.ControllersManager.ControllersInitialized += OnControllersInitialized;
        }
    }

    private void OnControllersInitialized()
    {
        context.ControllersManager.ControllersInitialized -= OnControllersInitialized;
        StartTutorialInternal();
    }

    private void OnDestroy()
    {
        if (!context.ControllersManager.IsControllersInitialized)
        {
            context.ControllersManager.ControllersInitialized -= OnControllersInitialized;
        }
        StopTutorial();
    }
}