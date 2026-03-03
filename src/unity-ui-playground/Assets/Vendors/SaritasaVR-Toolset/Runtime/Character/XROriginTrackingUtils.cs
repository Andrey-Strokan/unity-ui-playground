using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;

/// <summary>
/// HACK:
/// Utils for fixed xr origin tracking mode (https://discussions.unity.com/t/issues-setting-player-height-in-xr-quest-2/934042).
/// </summary>
public class XROriginTrackingUtils : MonoBehaviour
{
#if UNITY_EDITOR

    [SerializeField]
    private XROrigin xrOrigin;

    private bool initialized = false;

    private void Start()
    {
        if (!initialized)
        {
            StartCoroutine(ActionCoroutine());
        }
    }

    private IEnumerator ActionCoroutine()
    {
        initialized = true;

        yield return new WaitForSeconds(0.1f);

        xrOrigin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Device;

        yield return new WaitForSeconds(0.1f);

        xrOrigin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Floor;
    }

#endif
}
