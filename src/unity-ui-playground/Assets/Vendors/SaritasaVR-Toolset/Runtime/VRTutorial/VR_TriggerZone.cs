using System;
using UnityEngine;

/// <summary>
/// A trigger zone which raises event when player enters it.
/// </summary>
public class VR_TriggerZone : MonoBehaviour
{
    [SerializeField]
    private bool isHighlightTriggerZone;

    /// <summary>
    /// This event will be triggered when player will enter this zone.
    /// </summary>
    public event Action OnPlayerEnteredZone;

    private bool isPlayerEnteredZone = false;

    private Material material;
    private Color color;

    /// <summary>
    /// Is player already entered this zone.
    /// </summary>
    public bool IsPlayerEnteredZone
    {
        get
        {
            return isPlayerEnteredZone;
        }
    }

    private int pulseDirection = 1;
    private float pulseMax = 1f;
    private float pulseMin = 0.2f;

    private void OnEnable()
    {
        if (!isHighlightTriggerZone)
        {
            return;
        }

        material = GetComponent<MeshRenderer>().material;
        color = material.color;
    }

    private void Update()
    {
        if (!isHighlightTriggerZone)
        {
            return;
        }

        Pulse();
    }

    private void Pulse()
    {
        var alpha = Mathf.Clamp(material.color.a + Time.deltaTime * pulseDirection, pulseMin, pulseMax);
        material.color = new Color(color.r, color.g, color.b, alpha);
        if (material.color.a == pulseMax)
        {
            pulseDirection = -1;
        }
        else if (material.color.a == pulseMin)
        {
            pulseDirection = 1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        if (other.tag.Equals("Player"))
        {
            isPlayerEnteredZone = true;
            OnPlayerEnteredZone?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
