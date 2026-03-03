using System;
using UnityEngine;

/// <summary>
/// This class is a data model for tutorial step. Contains a description of a single tutorial step
/// </summary>
[Serializable]
public struct TutorialStepData
{
    [Tooltip("Name of this step. A name of Tutorial Action Class will be generated based on this name.")]
    public string StepName;

    [TextArea]
    [Tooltip("Title for this tutorial step.")]
    public string Title;

    [TextArea]
    [Tooltip("Message fot this tutorial step.")]
    public string Message;

    [Tooltip("An array of substeps for this step.")]
    public TutorialSubStep[] SubSteps;
}

/// <summary>
/// Data model for Sub-step of Tutorial Step. Tutorial Step may consist of multiple substeps.
/// Each substep has it's own controller tooltips. You can swich between substeps in Tutorial Action.
/// </summary>
[Serializable]
public struct TutorialSubStep
{
    [Tooltip("An array of tooltips for this substep.")]
    public TutorialControllerTooltipData[] Tooltips;
}

/// <summary>
/// Data model for a controller tooltip.
/// </summary>
[Serializable]
public struct TutorialControllerTooltipData
{
    public enum ControllerHands
    {
        Left, Right, Both
    }

    [Tooltip("A hand on which this tooltip will be shown.")]
    public ControllerHands ControllerHand;

    [Tooltip("A button for which this tooltip will be shown.")]
    public VR_ControllerButtons ControllerButton;

    [TextArea]
    [Tooltip("A text of the tooltip.")]
    public string Text;
}
