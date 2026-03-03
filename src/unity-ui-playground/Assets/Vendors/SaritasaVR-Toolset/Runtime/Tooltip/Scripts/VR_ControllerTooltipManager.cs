using System.Collections.Generic;
using UnityEngine;
using Saritasa.VRToolset.Tooltips;
using System;

/// <summary>
/// Simple data structure holder for easier inspector set up.
/// </summary>
[System.Serializable]
public class VR_ControllerGhostTooltipButton
{
    public VR_ControllerButtons Button;
    public TooltipBase VRBaseTooltip;
    public MeshRenderer ButtonMRenderer;
    public float IsVisible = 1;
    public float CurVisibility = 1;
    public int ShaderPropId;
}

/// <summary>
/// Manager for tooltips of controller with ghost transparent shader.
/// </summary>
public class VR_ControllerTooltipManager : MonoBehaviour
{
    /// <summary>
    /// If false controllers will be hidden if
    /// no any tooltip is shown.
    /// </summary>
    public bool ShowControllerWithNoTooltips = true;

    /// <summary>
    /// Duration of the fading effect of the tooltips.
    /// </summary>
    public float ControllersFadeDuration = 0.5f;

    /// <summary>
    /// True if tooltip manager was initialized.
    /// </summary>
    public bool Initialized => initialized;

    /// <summary>
    /// Calling, when tooltip manager initialized.
    /// </summary>
    public event Action ControllerTooltipManagerInitialized;

    [SerializeField]
    private MeshRenderer controllerMeshRenderer;

    [SerializeField]
    private List<VR_ControllerGhostTooltipButton> tooltips;

    private Dictionary<VR_ControllerButtons, VR_ControllerGhostTooltipButton> tooltipsDict;

    private Material controllerMaterial;

    private float isControllerVisible = 1; // true: 1, false: -1
    private float controllerVisibility = 1;
    private float allButtonsVisibility = 1;
    private readonly int shPropControllerVisibility = Shader.PropertyToID("_ControllerVisibility");
    private readonly int shPropAllButtonsVisibility = Shader.PropertyToID("_HgltsVisibility");

    private bool initialized = false;

    private void Awake()
    {
        controllerMaterial = controllerMeshRenderer.material;
        controllerMeshRenderer.material = controllerMaterial;
        tooltipsDict = new Dictionary<VR_ControllerButtons, VR_ControllerGhostTooltipButton>();
        foreach (var tooltip in tooltips)
        {
            tooltipsDict[tooltip.Button] = tooltip;
            tooltip.ButtonMRenderer.material = controllerMaterial;
            tooltip.ShaderPropId = GetShaderPropIdForButton(tooltip.Button);
            SetMeshVertexColorId(tooltip.ButtonMRenderer.GetComponent<MeshFilter>().mesh, (byte)tooltip.Button);
        }

        initialized = true;
        ControllerTooltipManagerInitialized?.Invoke();
    }

    private void Update()
    {
        foreach (var t in tooltips)
        {
            t.CurVisibility = GetNewVisibilityValue(t.CurVisibility, t.IsVisible);
            controllerMaterial.SetFloat(t.ShaderPropId, t.CurVisibility);
        }

        controllerVisibility = GetNewVisibilityValue(controllerVisibility, isControllerVisible);
        controllerMaterial.SetFloat(shPropControllerVisibility, controllerVisibility);

        // Pulse
        allButtonsVisibility = Mathf.PingPong(Time.time, 1);
        controllerMaterial.SetFloat(shPropAllButtonsVisibility, allButtonsVisibility);
    }

    private bool IsAnyTooltipShown()
    {
        foreach (var t in tooltips)
        {
            if (t.IsVisible > 0)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Set text to tooltip.
    /// </summary>
    /// <param name="button"></param>
    /// <param name="text"></param>
    public void SetTooltipText(VR_ControllerButtons button, string text)
    {
        if (tooltipsDict[button].VRBaseTooltip != null)
        {
            tooltipsDict[button].VRBaseTooltip.DisplayText = text;
        }
    }

    /// <summary>
    /// Show controller's tooltips.
    /// </summary>
    /// <param name="button"></param>
    public void ShowTooltip(VR_ControllerButtons button, float duration = 1f)
    {
        if (tooltipsDict[button].VRBaseTooltip != null)
        {
            tooltipsDict[button].VRBaseTooltip.Show(duration);
        }

        ShowButton(button, duration);
    }

    /// <summary>
    /// Show controller's button.
    /// </summary>
    /// <param name="button"></param>
    public void ShowButton(VR_ControllerButtons button, float duration = 1f)
    {
        tooltipsDict[button].IsVisible = 1;
        isControllerVisible = 1;

        if (duration <= 0f)
        {
            tooltipsDict[button].CurVisibility = 1;
            controllerVisibility = 1;
        }
    }

    /// <summary>
    /// Hide controller's tooltips.
    /// </summary>
    /// <param name="button"></param>
    public void HideTooltip(VR_ControllerButtons button, float duration = 1f)
    {
        if (tooltipsDict[button].VRBaseTooltip != null)
        {
            tooltipsDict[button].VRBaseTooltip.Hide(duration);
        }

        HideButton(button, duration);
    }

    /// <summary>
    /// Hide controller's button.
    /// </summary>
    /// <param name="button"></param>
    public void HideButton(VR_ControllerButtons button, float duration = 1f)
    {
        tooltipsDict[button].IsVisible = -1;
        if (duration <= 0f)
        {
            tooltipsDict[button].CurVisibility = 0;
        }

        if (!ShowControllerWithNoTooltips && !IsAnyTooltipShown())
        {
            isControllerVisible = -1;
            if (duration <= 0f)
            {
                controllerVisibility = 0;
            }
        }
    }

    /// <summary>
    /// Hide all controller's buttons.
    /// </summary>
    public void HideAllButtons(float duration = 1f)
    {
        foreach (var button in tooltipsDict.Keys)
        {
            HideButton(button, duration);
        }
    }

    /// <summary>
    /// Show all controller's buttons.
    /// </summary>
    public void ShowAllButtons(float duration = 1f)
    {
        foreach (var button in tooltipsDict.Keys)
        {
            ShowButton(button, duration);
        }
    }

    /// <summary>
    /// Show all controller's tooltips.
    /// </summary>
    public void ShowAllTooltips(float duration = 1f)
    {
        foreach (var button in tooltipsDict.Keys)
        {
            ShowTooltip(button, duration);
        }
    }

    /// <summary>
    /// Hide all controller's tooltips.
    /// </summary>
    public void HideAllTooltips(float duration = 1f)
    {
        foreach (var button in tooltipsDict.Keys)
        {
            HideTooltip(button, duration);
        }
    }

    private float GetNewVisibilityValue(float oldValue, float direction)
    {
        return Mathf.Clamp01(oldValue + Time.deltaTime / ControllersFadeDuration * direction);
    }

    private void SetMeshVertexColorId(Mesh mesh, byte id)
    {
        Color32 c = new Color32(id, 0, 0, 0);
        var vertexColors = new Color[mesh.vertexCount];
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            vertexColors[i] = c;
        }
        mesh.SetColors(vertexColors);
    }

    private int GetShaderPropIdForButton(VR_ControllerButtons button)
    {
        return button switch
        {
            VR_ControllerButtons.Touchpad => Shader.PropertyToID("_TouchpadVisibility"),
            VR_ControllerButtons.Trigger => Shader.PropertyToID("_TriggerVisibility"),
            VR_ControllerButtons.ButtonEnter => Shader.PropertyToID("_ButtoEnterVisibility"),
            VR_ControllerButtons.ButtonOne => Shader.PropertyToID("_ButtonOneVisibility"),
            VR_ControllerButtons.ButtonTwo => Shader.PropertyToID("_ButtonTwoVisibility"),
            VR_ControllerButtons.Grip => Shader.PropertyToID("_GripVisibility"),
            _ => throw new System.ComponentModel.InvalidEnumArgumentException("Wrong vr controller Buttons enum argument")
        };
    }
}