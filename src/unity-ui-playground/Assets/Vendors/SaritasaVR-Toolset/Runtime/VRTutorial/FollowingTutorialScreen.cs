using System.Collections;
using Saritasa.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.UI;

/// <summary>
/// An implementation of TutorialScreen which follow player position. Also has hide button.
/// </summary>
public class FollowingTutorialScreen : TutorialScreenBase
{
    [Tooltip("Text component that shows screen title.")]
    [SerializeField]
    private TMP_Text txt_Title;

    [Tooltip("Text component that shows screen message.")]
    [SerializeField]
    private TMP_Text txt_Message;

    [SerializeField]
    [Tooltip("Reference to a canvas of the screen.")]
    private Canvas canvas;

    [SerializeField]
    [Tooltip("Time for screen fade/unfade.")]
    private float fadeTime = 0.5f;

    [SerializeField]
    [Tooltip("Device manager reference.")]
    private ControllersManager controllersManager;

    private Coroutine coroutine;

    private CanvasGroup canvasGroup;

    private Collider uiCollider;

    private LazyFollow headsetFollower;

    /// <summary>
    /// Enable this screen with animation.
    /// </summary>
    public override void Show(bool show)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        float dAlpha = 1.0f / fadeTime;

        if (show)
        {
            canvasGroup.alpha = 0.0001f;
            coroutine = StartCoroutine(FadeCoroutine(dAlpha));
            EnableCanvas(true);
        }
        else
        {
            canvasGroup.alpha = 1.0f;
            coroutine = StartCoroutine(FadeCoroutine(-dAlpha));
            EnableCanvas(false);
        }
    }

    /// <summary>
    /// Set the title and the message for this screen. Set string to
    /// null or empty to hide title or message.
    /// </summary>
    public override void SetText(string title, string message)
    {
        if (string.IsNullOrEmpty(title))
        {
            txt_Title.text = "";
            txt_Title.gameObject.SetActive(false);
        }
        else
        {
            txt_Title.gameObject.SetActive(true);
            txt_Title.text = title;
        }

        if (string.IsNullOrEmpty(message))
        {
            txt_Message.text = "";
            txt_Message.gameObject.SetActive(false);
        }
        else
        {
            txt_Message.gameObject.SetActive(true);
            txt_Message.text = message;
        }
    }

    private IEnumerator FadeCoroutine(float dAlpha)
    {
        float alpha = canvasGroup.alpha;
        while (true)
        {
            alpha += dAlpha * Time.deltaTime;

            if (alpha >= 1.0f)
            {
                canvasGroup.alpha = 1.0f;
                coroutine = null;
                yield break;
            }

            if (alpha <= 0.0f)
            {
                canvasGroup.alpha = 0.0f;
                coroutine = null;
                yield break;
            }

            canvasGroup.alpha = alpha;

            yield return null;
        }
    }

    private void EnableCanvas(bool enable)
    {
        canvasGroup.interactable = enable;
        canvasGroup.blocksRaycasts = enable;
        uiCollider.enabled = enable;
    }

    private void OnEnable()
    {
        canvasGroup = canvas.gameObject.GetComponent<CanvasGroup>();
        Debug.Assert(canvasGroup != null, "Unable to get CanvasGroup");

        uiCollider = canvas.gameObject.GetComponent<Collider>();
        Debug.Assert(uiCollider != null, "Unable to get UI Collider");

        controllersManager.ControllersInitialized += OnSdkSetupChanged;

        headsetFollower = GetComponent<LazyFollow>();
        Debug.Assert(headsetFollower != null, "Unable to get HeadsetTransformFollow");
    }

    private void OnDestroy()
    {
        controllersManager.ControllersInitialized -= OnSdkSetupChanged;
    }

    private void OnSdkSetupChanged()
    {
        
        var headset = Camera.main;
        if (headset == null)
        {
            Debug.Log("Unable to find Headset");
            headsetFollower.target = null;
        }
        else
        {
            headsetFollower.target = headset.transform;
        }
    }

    public void OnBthHideClick()
    {
        Show(false);
    }
}
