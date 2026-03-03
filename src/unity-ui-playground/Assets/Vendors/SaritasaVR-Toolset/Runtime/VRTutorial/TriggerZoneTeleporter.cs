using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

/// <summary>
/// Teleporter for trigger zone.
/// </summary>
public class TriggerZoneTeleporter : MonoBehaviour
{
    [SerializeField]
    private TeleportationAnchor teleportationAnchor;

    [SerializeField]
    private VR_TriggerZone triggerZone;

    private void OnEnable()
    {
        triggerZone.OnPlayerEnteredZone += OnPlayerEnteredZone;
    }

    private void OnDisable()
    {
        triggerZone.OnPlayerEnteredZone -= OnPlayerEnteredZone;
    }

    private void OnPlayerEnteredZone()
    {
        teleportationAnchor.RequestTeleport();
    }
}
