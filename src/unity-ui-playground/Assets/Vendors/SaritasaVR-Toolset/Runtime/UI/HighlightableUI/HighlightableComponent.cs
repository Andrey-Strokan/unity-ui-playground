using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Saritasa.VRToolset
{
    /// <summary>
    /// This is a standalone implementation of default Unity UI transitions
    /// but adjusted for our needs. Use instead of built-in UI controls transitions.
    /// Sources are mostly based on UnityEngine.UI.Selectable.
    /// </summary>
    [ExecuteInEditMode]
    public class HighlightableComponent :
        InteractableStateTracker,
        IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler,
        ISelectHandler, IDeselectHandler
    {
        /// <summary>
        /// Internal buffer. Used as temporary storage to avoid exra allocations.
        /// </summary>
        private static List<Graphic> graphicsTempBuffer = new List<Graphic>();

        [SerializeField]
        [Tooltip("Allow selected state for this component.")]
        private bool isSelectedStateAllowed = false;

        private bool isEnableCalled = false;

        /// <summary>
        ///Transition mode for a Selectable.
        /// </summary>
        public enum TransitionType
        {
            /// <summary>
            /// No Transition.
            /// </summary>
            None,

            /// <summary>
            /// Use an color tint transition.
            /// </summary>
            ColorTint,

            /// <summary>
            /// Use a sprite swap transition.
            /// </summary>
            SpriteSwap
        }

                /// <summary>
        /// An enumeration of selected states of objects
        /// </summary>
        protected enum SelectionState
        {
            /// <summary>
            /// The UI object can be selected.
            /// </summary>
            Normal,

            /// <summary>
            /// The UI object is highlighted.
            /// </summary>
            Highlighted,

            /// <summary>
            /// The UI object is pressed.
            /// </summary>
            Pressed,

            /// <summary>
            /// The UI object is selected
            /// </summary>
            Selected,

            /// <summary>
            /// The UI object cannot be selected.
            /// </summary>
            Disabled,
        }

        [SerializeField]
        [Tooltip("Type of the transition that occurs when the button state changes.")]
        private TransitionType transition = TransitionType.ColorTint;

        [SerializeField]
        [Tooltip("Colors used for a color tint-based transition")]
        private ColorBlock colors = ColorBlock.defaultColorBlock;

        [SerializeField]
        [Tooltip("Sprites used for a Image swap-based transition.")]
        private SpriteState spriteState;

        // Graphic that will be colored.
        [SerializeField]
        [Tooltip("Graphic that will be colored.")]
        private Graphic[] targetGraphics = new Graphic[0];

        [SerializeField]
        [Tooltip("If it is true then the component also will fade children graphics.")]
        private bool isChildrenIncluded = false;

        /// <summary>
        /// If it is true then the component also will fade children graphics.
        /// </summary>
        public bool IsChildrenIncluded
        {
            get => isChildrenIncluded;
            set => isChildrenIncluded = value;
        }

        [SerializeField]
        [Tooltip("Enable or disable graphics effects for this highlightable. If it is set to false the highlightable would still process all events but it will not change graphics colors or sprites.")]
        private bool isGraphicsEffectsEnabled = true;

        /// <summary>
        /// Enable or disable graphics effects fot this highlightable.
        /// If it is set to false the highlightable would still process
        /// all events but it will not change graphics colors or sprites.
        /// </summary>
        public bool IsGraphicsEffectsEnabled
        {
            get => isGraphicsEffectsEnabled;
            set
            {
                isGraphicsEffectsEnabled = value;
                SetGraphicsEffectsState(value);
            }
        }

        /// <summary>
        /// The type of transition that will be applied to the targetGraphic when the state changes.
        /// </summary>
        public Selectable TargetSelectable
        {
            get => targetSelectable;
            set
            {
                if (SetPropertyUtility.SetClass(ref targetSelectable, value))
                {
                    UpdateState(true);
                }
            }
        }

        /// <summary>
        /// The type of transition that will be applied to the targetGraphic when the state changes.
        /// </summary>
        public Graphic[] TargetGraphics
        {
            get => targetGraphics;
            set
            {
                targetGraphics = value;
                UpdateState(true);
            }
        }

        /// <summary>
        /// The type of transition that will be applied to the targetGraphic when the state changes.
        /// </summary>
        public TransitionType Transition
        {
            get => transition;
            set
            {
                if (SetPropertyUtility.SetStruct(ref transition, value))
                {
                    UpdateState(true);
                }
            }
        }

        /// <summary>
        /// The ColorBlock for this selectable object.
        /// </summary>
        public ColorBlock Colors
        {
            get => colors;
            set
            {
                if (SetPropertyUtility.SetStruct(ref colors, value))
                {
                    UpdateState(true);
                }
            }
        }

        /// <summary>
        /// The SpriteState for this selectable object.
        /// </summary>
        public SpriteState SpriteState
        {
            get => spriteState;
            set
            {
                if (SetPropertyUtility.SetStruct(ref spriteState, value))
                {
                    UpdateState(true);
                }
            }
        }

        private bool isPointerInside;
        private bool isPointerDown;
        private bool hasSelection;

        protected override void Awake()
        {
            if (targetSelectable == null)
            {
                targetSelectable = GetComponent<Selectable>();
            }
        }

        protected override void Start()
        {
            // Update state after all parent canvas groups are initialized
            // Canvas groups are not yet initialized in Awake so
            // Selectable's m_GroupsAllowInteraction is false at that time.
            // So we update state in Start to avoid getting false Disabled state.
            UpdateState(false);
        }

        // Select on enable and add to the list.
        protected override void OnEnable()
        {
            StartColorTween(Color.white, true);
            DoStateTransition(currentSelectionState, true);

            //Check to avoid multiple OnEnable() calls for each selectable
            if (isEnableCalled)
            {
                return;
            }

            base.OnEnable();

            isPointerDown = false;

            isEnableCalled = true;
        }

        protected override void OnDisable()
        {
            //Check to avoid multiple OnDisable() calls for each selectable
            if (!isEnableCalled)
            {
                return;
            }

            InstantClearState();
            base.OnDisable();

            isEnableCalled = false;
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();

            // If our parenting changes figure out if we are under a new CanvasGroup.
            OnCanvasGroupChanged();
        }

        protected override void OnInteractableStateChanged()
        {
            UpdateState(true);
        }

        public void UpdateState(bool instant)
        {
            var selectionState = targetSelectable != null ? currentSelectionState : SelectionState.Disabled;
#if UNITY_EDITOR
            DoStateTransition(selectionState, instant || !UnityEngine.Application.isPlaying);
#else
            DoStateTransition(selectionState, instant);
#endif
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            colors.fadeDuration = Mathf.Max(colors.fadeDuration, 0.0f);

            DoSpriteSwap(null);

            // If the transition mode got changed, we need to clear all the transitions, since we don't know what the old transition mode was.
            StartColorTween(Color.white, true);

            // And now go to the right state.
            DoStateTransition(currentSelectionState, true);

            SetGraphicsEffectsState(isGraphicsEffectsEnabled);
        }

        protected override void Reset()
        {
            targetSelectable = GetComponent<Selectable>();
        }

#endif // if UNITY_EDITOR

        protected SelectionState currentSelectionState
        {
            get
            {
                if (!IsInteractable)
                {
                    return SelectionState.Disabled;
                }

                if (isPointerDown)
                {
                    return SelectionState.Pressed;
                }

                if (hasSelection && isSelectedStateAllowed)
                {
                    return SelectionState.Selected;
                }

                if (isPointerInside)
                {
                    return SelectionState.Highlighted;
                }

                return SelectionState.Normal;
            }
        }

        /// <summary>
        /// Clear any internal state from the Selectable (used when disabling).
        /// </summary>
        protected virtual void InstantClearState()
        {
            isPointerInside = false;
            isPointerDown = false;
            hasSelection = false;

            switch (transition)
            {
                case TransitionType.ColorTint:
                    StartColorTween(Color.white, true);
                    break;
                case TransitionType.SpriteSwap:
                    DoSpriteSwap(null);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Transition the Selectable to the entered state.
        /// </summary>
        /// <param name="state">State to transition to</param>
        /// <param name="instant">Should the transition occur instantly.</param>
        protected virtual void DoStateTransition(SelectionState state, bool instant)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            if (!isGraphicsEffectsEnabled)
            {
                return;
            }

            Color tintColor;
            Sprite transitionSprite;

            switch (state)
            {
                case SelectionState.Normal:
                    tintColor = colors.normalColor;
                    transitionSprite = null;
                    break;
                case SelectionState.Highlighted:
                    tintColor = colors.highlightedColor;
                    transitionSprite = spriteState.highlightedSprite;
                    break;
                case SelectionState.Pressed:
                    tintColor = colors.pressedColor;
                    transitionSprite = spriteState.pressedSprite;
                    break;
                case SelectionState.Selected:
                    tintColor = colors.selectedColor;
                    transitionSprite = spriteState.selectedSprite;
                    break;

                case SelectionState.Disabled:
                    tintColor = colors.disabledColor;
                    transitionSprite = spriteState.disabledSprite;
                    break;
                default:
                    tintColor = Color.black;
                    transitionSprite = null;
                    break;
            }

            switch (transition)
            {
                case TransitionType.ColorTint:
                    StartColorTween(tintColor * colors.colorMultiplier, instant);
                    break;
                case TransitionType.SpriteSwap:
                    DoSpriteSwap(transitionSprite);
                    break;
            }
        }

        private void StartColorTween(Color targetColor, bool instant)
        {
            foreach (var graphic in targetGraphics)
            {
                if (isChildrenIncluded)
                {
                    graphicsTempBuffer.Clear();
                    graphic.gameObject.GetComponentsInChildren<Graphic>(graphicsTempBuffer);
                    foreach (var childGraphic in graphicsTempBuffer)
                    {
                        StartColorTween(childGraphic, targetColor, instant);
                    }
                }
                else
                {
                    StartColorTween(graphic, targetColor, instant);
                }
            }
        }

        private void StartColorTween(Graphic graphic, Color targetColor, bool instant)
        {
            if (instant)
            {
                graphic?.canvasRenderer.SetColor(targetColor);
            }
            else
            {
                graphic?.CrossFadeColor(targetColor, colors.fadeDuration, true, true);
            }
        }

        private void DoSpriteSwap(Sprite newSprite)
        {
            foreach (var graphic in targetGraphics)
            {
                if (isChildrenIncluded)
                {
                    graphicsTempBuffer.Clear();
                    graphic.gameObject.GetComponentsInChildren<Graphic>(graphicsTempBuffer);
                    foreach (var childGraphic in graphicsTempBuffer)
                    {
                        DoSpriteSwap(childGraphic, newSprite);
                    }
                }
                else
                {
                    DoSpriteSwap(graphic, newSprite);
                }
            }
        }

        private void DoSpriteSwap(Graphic graphic, Sprite newSprite)
        {
            var img = graphic as Image;
            if (img != null)
            {
                img.overrideSprite = newSprite;
            }
        }


        // Change the button to the correct state
        private void EvaluateAndTransitionToSelectionState()
        {
            if (!targetSelectable.IsActive() || !IsInteractable)
            {
                return;
            }

            DoStateTransition(currentSelectionState, false);
        }

        private void SetGraphicsEffectsState(bool enable)
        {
            if (enable)
            {
                DoStateTransition(currentSelectionState, true);
            }
            else
            {
                switch (transition)
                {
                    case TransitionType.ColorTint:
                        StartColorTween(Color.white, true);
                        break;
                    case TransitionType.SpriteSwap:
                        DoSpriteSwap(null);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Evaluate current state and transition to pressed state.
        /// </summary>
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            isPointerDown = true;
            EvaluateAndTransitionToSelectionState();
        }

        /// <summary>
        /// Evaluate eventData and transition to appropriate state.
        /// </summary>
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
            EvaluateAndTransitionToSelectionState();
        }

        /// <summary>
        /// Evaluate current state and transition to appropriate state.
        /// New state could be pressed or hover depending on pressed state.
        /// </summary>
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            isPointerInside = true;
            EvaluateAndTransitionToSelectionState();
        }

        /// <summary>
        /// Evaluate current state and transition to normal state.
        /// </summary>
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            isPointerInside = false;
            EvaluateAndTransitionToSelectionState();
        }

                /// <summary>
        /// Set selection and transition to appropriate state.
        /// </summary>
        public virtual void OnSelect(BaseEventData eventData)
        {
            hasSelection = true;
            EvaluateAndTransitionToSelectionState();
        }

        /// <summary>
        /// Unset selection and transition to appropriate state.
        /// </summary>
        public virtual void OnDeselect(BaseEventData eventData)
        {
            hasSelection = false;
            EvaluateAndTransitionToSelectionState();
        }
    }
}