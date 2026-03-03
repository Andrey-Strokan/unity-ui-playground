using SaritasaVRToolset;
using System.Collections;
using UnityEngine;

/// <summary>
/// Locomotion input manager.
/// </summary>
public class XRLocomotionInput : MonoBehaviour
{
    [SerializeField]
    private ControllerInputActionManager leftControllerInputActionManager;

    [SerializeField]
    private ControllerInputActionManager rightControllerInputActionManager;

    private bool initialized;
    private bool rightTeleportModeEnabled;
    private bool rightMoveEnabled;
    private bool rightTurnEnabled;
    private bool rightSnapTurnEnabled;

    /// <summary>
    /// True if locomotion input was initialized.
    /// </summary>
    public bool Initialized => initialized;

    private IEnumerator Start()
    {
        if (!Initialized)
        {
            while (!leftControllerInputActionManager.LocomotionInitialized ||
                   !rightControllerInputActionManager.LocomotionInitialized)
            {
                yield return null;
            }

            rightTeleportModeEnabled = rightControllerInputActionManager.TeleportationEnabled;
            rightMoveEnabled = rightControllerInputActionManager.SmoothMotionEnabled;
            rightTurnEnabled = rightControllerInputActionManager.SmoothTurnEnabled;
            rightSnapTurnEnabled = rightControllerInputActionManager.SnapTurnEnabled;

            initialized = true;
        }
    }

    /// <summary>
    /// Disable locomotion actions.
    /// </summary>
    public void DisableLocomotionActions()
    {
        if (!initialized)
        {
            return;
        }

        SetActiveTeleportActions(false);
        SetActiveMoveActions(false);
        SetActiveTurnActions(false);
        SetActiveSnapTurnActions(false);
    }

    /// <summary>
    /// Enable locomotion actions.
    /// </summary>
    public void EnableLocomotionActions()
    {
        if (!initialized)
        {
            return;
        }

        SetActiveTeleportActions(true);
        SetActiveMoveActions(true);
        SetActiveTurnActions(true);
        SetActiveSnapTurnActions(true);
    }

    /// <summary>
    /// Set teleport actions.
    /// </summary>
    public void SetActiveTeleportActions(bool value)
    {
        if (!initialized)
        {
            return;
        }

        if ( rightTeleportModeEnabled)
        {
            rightControllerInputActionManager.TeleportationEnabled = value;
        }
        else
        {
            leftControllerInputActionManager.TeleportationEnabled = value;
        }
    }

    /// <summary>
    /// Set move actions.
    /// </summary>
    public void SetActiveMoveActions(bool value)
    {
        if (!initialized)
        {
            return;
        }

        if (rightMoveEnabled)
        {
            rightControllerInputActionManager.SmoothMotionEnabled = value;
        }
        else
        {
            leftControllerInputActionManager.SmoothMotionEnabled = value;
        }
    }

    /// <summary>
    /// Set turn actions.
    /// </summary>
    public void SetActiveTurnActions(bool value)
    {
        if (!initialized)
        {
            return;
        }

        if (rightTurnEnabled || rightSnapTurnEnabled)
        {
            rightControllerInputActionManager.SmoothTurnEnabled = value;
        }
        else
        {
            leftControllerInputActionManager.SmoothTurnEnabled = value;
        }
    }

    /// <summary>
    /// Set snap turn actions.
    /// </summary>
    public void SetActiveSnapTurnActions(bool value)
    {
        if (!initialized)
        {
            return;
        }

        if (rightTurnEnabled || rightSnapTurnEnabled)
        {
            rightControllerInputActionManager.SnapTurnEnabled = value;
        }
        else
        {
            leftControllerInputActionManager.SnapTurnEnabled = value;
        }
    }

    /// <summary>
    /// Get value from move actions.
    /// </summary>
    public Vector2 GetMoveValue()
    {
        return leftControllerInputActionManager.GetMoveValue() + rightControllerInputActionManager.GetMoveValue();
    }

    /// <summary>
    /// Get value from teleport mode actions.
    /// </summary>
    public Vector2 GetTeleportModeValue()
    {
        return leftControllerInputActionManager.GetTeleportModeValue() + rightControllerInputActionManager.GetTeleportModeValue();
    }
}
