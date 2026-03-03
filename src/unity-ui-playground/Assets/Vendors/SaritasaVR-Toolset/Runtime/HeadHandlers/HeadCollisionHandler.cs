using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Fade camera on collision.
/// </summary>
public class HeadCollisionHandler : MonoBehaviour
{
    private static readonly int fadeOpacity = Shader.PropertyToID("_FadeOpacity");

    private MaterialPropertyBlock fadePropertyBlock;

    [SerializeField]
    [Tooltip("Fader mesh renderer.")]
    private MeshRenderer meshRender;

    [SerializeField]
    [Tooltip("Representation of head real position.")]
    private Transform realHead;

    [SerializeField]
    [Tooltip("Rigidbody attached to head physical representation.")]
    private Rigidbody physicalHeadRgb;

    [SerializeField]
    [Range(0, 1)]
    [Tooltip("Threshold where we start to fade screen.")]
    private float minDistanceThreshold = 0.08f;

    [SerializeField]
    [Range(0, 1)]
    [Tooltip("Threshold where we fade screen completely.")]
    private float maxDistanceThreshold = 0.085f;

    [SerializeField]
    [Range(1, 10)]
    [Tooltip("Limit distance to call overcame event.")]
    private float limit = 1f;

    private Coroutine fadeCoroutine;

    /// <summary>
    /// Is fade by collision active.
    /// </summary>
    public bool IsFaded { get; private set; }

    /// <summary>
    /// Calls when physical head on distance more then limit.
    /// </summary>
    public event Action LimitCrossed;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        physicalHeadRgb.transform.parent = null;
        fadeCoroutine = StartCoroutine(FadeCoroutine());
    }

    private void LateUpdate()
    {
        SyncHeadPosition();
    }

    private void SyncHeadPosition()
    {
        var distance = (realHead.position - physicalHeadRgb.position).magnitude;
        if (distance == 0)
        {
            return;
        }

        if (distance > limit)
        {
            LimitCrossed?.Invoke();
            return;
        }

        if (!physicalHeadRgb.SweepTest(realHead.position - physicalHeadRgb.position, out _, distance))
        {
            physicalHeadRgb.position = realHead.position;
        }
    }

    private IEnumerator FadeCoroutine()
    {
        while (true)
        {
            // Wait until next frame. We can't wait for physics, because it is bound to timeScale.
            yield return new WaitForEndOfFrame();

            var magnitude = (realHead.position - physicalHeadRgb.position).magnitude;

            var alpha = 0f;
            if (physicalHeadRgb.SweepTest(realHead.position - physicalHeadRgb.position, out _, magnitude))
            {
                alpha = Mathf.InverseLerp(minDistanceThreshold, maxDistanceThreshold,
                    Vector3.Distance(physicalHeadRgb.position, realHead.position));
            }

            if (fadePropertyBlock == null)
            {
                fadePropertyBlock = new MaterialPropertyBlock();
            }

            meshRender.GetPropertyBlock(fadePropertyBlock);
            fadePropertyBlock.SetFloat(fadeOpacity, alpha);
            meshRender.SetPropertyBlock(fadePropertyBlock);

            IsFaded = alpha > 0;
        }
    }

    private void OnDestroy()
    {
        StopCoroutine(fadeCoroutine);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Reset physical head position.
    /// </summary>
    public void ResetHead()
    {
        physicalHeadRgb.position = realHead.position;
    }
}