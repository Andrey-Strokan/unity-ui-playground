using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Feedback;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

/// <inheritdoc />
public class CustomHapticFeedback : SimpleHapticFeedback
{
    /// <summary>
    /// Whether to play a haptic impulse when the interactor activate.
    /// </summary>
    [field: Space]
    [field: SerializeField]
    public bool PlayActivated { get; set; }

    /// <summary>
    /// The haptic impulse to play when the interactor activate.
    /// </summary>
    [field: SerializeField]
    public HapticImpulseData ActivatedFeedback { get; set; } = new() { amplitude = 0.5f, duration = 0.1f };

    /// <summary>
    /// Whether to play a haptic impulse when the interactor deactivate.
    /// </summary>
    [field: Space]
    [field: SerializeField]
    public bool PlayDeactivated { get; set; }

    /// <summary>
    /// The haptic impulse to play when the interactor deactivate.
    /// </summary>
    [field: SerializeField]
    public HapticImpulseData DeactivatedFeedback { get; set; } = new() { amplitude = 0.5f, duration = 0.1f };

    /// <summary>
    /// Whether to play a haptic impulse when the interactor cick ui element.
    /// </summary>
    [field: Header("UI")]
    [field: Space]
    [field: SerializeField]
    public bool PlayPointerClick { get; set; }

    /// <summary>
    /// The haptic impulse to play when the interactor click on ui element.
    /// </summary>
    [field: SerializeField]
    public HapticImpulseData PointerClickFeedback { get; set; } = new() { amplitude = 0.5f, duration = 0.1f };

    /// <summary>
    /// Whether to play a haptic impulse when the interactor hover on ui element.
    /// </summary>
    [field: Space]
    [field: SerializeField]
    public bool PlayPointerEntered { get; set; }

    /// <summary>
    /// The haptic impulse to play when the interactor hover on ui element.
    /// </summary>
    [field: SerializeField]
    public HapticImpulseData PointerEnteredFeedback { get; set; } = new() { amplitude = 0.5f, duration = 0.1f };

    /// <summary>
    /// Whether to play a haptic impulse when the interactor unhover on ui element.
    /// </summary>
    [field: Space]
    [field: SerializeField]
    public bool PlayPointerExited { get; set; }

    /// <summary>
    /// The haptic impulse to play when the interactor unhover on ui element.
    /// </summary>
    [field: SerializeField]
    public HapticImpulseData PointerExitedFeedback { get; set; } = new() { amplitude = 0.5f, duration = 0.1f };

    private XRUIInputModule currentInputModule;
    private EventSystem currentEventSystem;

    private XRUIInputModule GetCurrentInputModule
    {
        get
        {
            if (currentEventSystem == EventSystem.current)
                return currentInputModule;

            if (currentInputModule != null)
            {
                currentInputModule.pointerEnter -= OnPointerEnter;
                currentInputModule.pointerExit -= OnPointerExit;
                currentInputModule.pointerClick -= OnPointerClick;
            }

            currentEventSystem = EventSystem.current;
            currentInputModule = currentEventSystem != null ?
                currentEventSystem.GetComponent<XRUIInputModule>() :
                null;

            return currentInputModule;
        }
    }

    private IXRInteractor interactor;

    /// <summary>
    /// See <see cref="MonoBehaviour"/>.
    /// </summary>
    protected new void OnEnable()
    {
        interactor = GetInteractorSource();
        Subscribe(interactor);
        base.OnEnable();
    }

    /// <summary>
    /// See <see cref="MonoBehaviour"/>.
    /// </summary>
    protected new void OnDisable()
    {
        Unsubscribe(interactor);
        base.OnDisable();
    }

    private void Subscribe(IXRInteractor interactor)
    {
        if (interactor == null || (interactor is Object interactorObject && interactorObject == null))
            return;

        if (interactor is IXRSelectInteractor selectInteractor)
        {
            selectInteractor.selectEntered.AddListener(OnSelectEntered);
            selectInteractor.selectExited.AddListener(OnSelectExited);
        }

        if (interactor is IXRHoverInteractor hoverInteractor)
        {
            hoverInteractor.hoverEntered.AddListener(OnHoverEntered);
            hoverInteractor.hoverExited.AddListener(OnHoverExited);
        }

        GetCurrentInputModule.pointerEnter += OnPointerEnter;
        GetCurrentInputModule.pointerExit += OnPointerExit;
        GetCurrentInputModule.pointerClick += OnPointerClick;
    }

    private void Unsubscribe(IXRInteractor interactor)
    {
        if (interactor == null || (interactor is Object interactorObject && interactorObject == null))
            return;

        if (interactor is IXRSelectInteractor selectInteractor)
        {
            selectInteractor.selectEntered.RemoveListener(OnSelectEntered);
            selectInteractor.selectExited.RemoveListener(OnSelectExited);
        }

        if (interactor is IXRHoverInteractor hoverInteractor)
        {
            hoverInteractor.hoverEntered.RemoveListener(OnHoverEntered);
            hoverInteractor.hoverExited.RemoveListener(OnHoverExited);
        }

        GetCurrentInputModule.pointerEnter -= OnPointerEnter;
        GetCurrentInputModule.pointerExit -= OnPointerExit;
        GetCurrentInputModule.pointerClick -= OnPointerClick;
    }

    private void OnPointerEnter(GameObject target, PointerEventData eventData)
    {
        if (target == null || !PlayPointerEntered)
        {
            return;
        }

        if (!target.TryGetComponent(out IEventSystemHandler _))
        {
            return;
        }

        if (interactor is IUIInteractor uiInteractor && uiInteractor == GetCurrentInputModule.GetInteractor(eventData.pointerId))
        {
            SendHapticImpulse(PointerEnteredFeedback);
        }
    }

    private void OnPointerExit(GameObject target, PointerEventData eventData)
    {
        if (target == null || !PlayPointerExited)
        {
            return;
        }

        if (target.GetComponent<IEventSystemHandler>() == null)
        {
            return;
        }

        if (interactor is IUIInteractor uiInteractor && uiInteractor == GetCurrentInputModule.GetInteractor(eventData.pointerId))
        {
            SendHapticImpulse(PointerExitedFeedback);
        }
    }

    private void OnPointerClick(GameObject target, PointerEventData eventData)
    {
        if (target == null || !PlayPointerClick)
        {
            return;
        }

        if (target.GetComponent<IEventSystemHandler>() == null)
        {
            return;
        }

        if (interactor is IUIInteractor uiInteractor && uiInteractor == GetCurrentInputModule.GetInteractor(eventData.pointerId))
        {
            SendHapticImpulse(PointerClickFeedback);
        }
    }

    private void OnActivated(ActivateEventArgs args)
    {
        if (PlayActivated && interactor is IXRActivateInteractor activateInteractor && activateInteractor == args.interactorObject)
        {
            SendHapticImpulse(ActivatedFeedback);
        }
    }

    private void OnDeactivated(DeactivateEventArgs args)
    {
        if (PlayActivated && interactor is IXRActivateInteractor activateInteractor && activateInteractor == args.interactorObject)
        {
            SendHapticImpulse(DeactivatedFeedback);
        }
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (interactor is XRBaseInputInteractor baseInputInteractor &&
            baseInputInteractor.allowHoveredActivate)
        {
            if (args.interactableObject is IXRActivateInteractable interactable)
            {
                interactable.activated.AddListener(OnActivated);
                interactable.deactivated.AddListener(OnDeactivated);
            }
        }
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        if (args.interactableObject is IXRSelectInteractable selectInteractable &&
            selectInteractable.IsSelectableBy(interactor as IXRSelectInteractor))
        {
            return;
        }

        if (args.interactableObject is IXRActivateInteractable interactable)
        {
            interactable.activated.RemoveListener(OnActivated);
            interactable.deactivated.RemoveListener(OnDeactivated);
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactableObject is IXRActivateInteractable interactable)
        {
            interactable.activated.AddListener(OnActivated);
            interactable.deactivated.AddListener(OnDeactivated);
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (interactor is XRBaseInputInteractor baseInputInteractor &&
            baseInputInteractor.allowHoveredActivate &&
            args.interactableObject is IXRHoverInteractable hoverInteractable &&
            hoverInteractable.IsHoverableBy(interactor as IXRHoverInteractor))
        {
            return;
        }

        if (args.interactableObject is IXRActivateInteractable interactable)
        {
            interactable.activated.RemoveListener(OnActivated);
            interactable.deactivated.RemoveListener(OnDeactivated);
        }
    }
}