using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Teleport anchor that allows for teleportation to a specified point when interacted with.
/// Also allows to change teleport anchor color.
/// </summary>
public sealed class TeleportAnchor : MonoBehaviour
{
    [SerializeField]
    private XRSimpleInteractable interactable;

    [SerializeField]
    private Transform teleportPoint;

    [SerializeField]
    private MeshRenderer[] teleportMeshRenderers;

    [SerializeField]
    private Color normalColor = Color.white;

    [SerializeField]
    private Color hoverColor = Color.blue;

    private static readonly int ColorPropertyID = Shader.PropertyToID("_Color");
    private MaterialPropertyBlock propertyBlock;
    private Color colorValue;

    private void Awake()
    {
        propertyBlock = new();

        interactable.hoverEntered.AddListener(OnHoverEntered);
        interactable.hoverExited.AddListener(OnHoverExited);
    }

    private void Start()
    {
        // Initialize the teleport mesh renderers with the normal color.
        teleportMeshRenderers[0].GetPropertyBlock(propertyBlock);
        colorValue = propertyBlock.GetColor(ColorPropertyID);
        ApplyColor(normalColor);
    }

    private void OnDestroy()
    {
        interactable.hoverEntered.RemoveListener(OnHoverEntered);
        interactable.hoverExited.RemoveListener(OnHoverExited);
    }

    private void OnHoverExited(HoverExitEventArgs arg0)
    {
        ApplyColor(normalColor);
    }

    private void OnHoverEntered(HoverEnterEventArgs arg0)
    {
        ApplyColor(hoverColor);
    }

    private void ApplyColor(Color color)
    {
        colorValue = color;

        foreach (var renderer in teleportMeshRenderers)
        {
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(ColorPropertyID, colorValue);
            renderer.SetPropertyBlock(propertyBlock);
        }
    }
}