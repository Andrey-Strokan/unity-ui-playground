using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Saritasa.VRToolset
{
    /// <summary>
    /// Base class for components which need to track state of Unity UI Selectable.
    /// This is done via polling Selectables variables in update loop since Selectable does
    /// not have any events for that.
    /// </summary>
    public abstract class InteractableStateTracker : UnityEngine.EventSystems.UIBehaviour
    {
        [SerializeField]
        [Tooltip("A Selectable which state will be tracked for this component.")]
        protected Selectable targetSelectable;

        // Cached state if targetSelectable != null. Used for optimizations
        private bool isSelectableSet;

        private bool isSelectableInteractable = true;

        private bool isRegistered;

        /// <summary>
        /// Is this component interactable.
        /// </summary>
        public bool IsSelectableInteractable
        {
            get => isSelectableInteractable;
            private set
            {
                if (isSelectableInteractable != value)
                {
                    isSelectableInteractable = value;
                    OnInteractableStateChanged();
                }
            }
        }

        public bool IsInteractable
        {
            get => IsSelectableInteractable && groupsAllowInteraction;
        }

        // Track parent canvas group interaction states.
        protected bool groupsAllowInteraction { get; private set; } = true;

        private readonly List<CanvasGroup> canvasGroupCache = new List<CanvasGroup>();

        /// <summary>
        /// Update component interactable state. This method should be only called by InteractableTrackersDispatcher
        /// </summary>
        public virtual void UpdateInteractableState()
        {
            // Using cached bool instead of directly calling costly virtual comparsion method in a hot code path.
            if (isSelectableSet)
            {
                IsSelectableInteractable = targetSelectable.IsInteractable();
            }
            else
            {
               IsSelectableInteractable = true;
            }
        }

        /// <summary>
        /// Called when interactable state of target selectable is changed.
        /// </summary>
        protected virtual void OnInteractableStateChanged()
        {
        }

        protected override void OnEnable()
        {
            isSelectableSet = !object.Equals(targetSelectable, null);

            if (!isRegistered && UnityEngine.Application.isPlaying)
            {
                InteractableTrackersDispatcher.Instance?.Register(this);
                isRegistered = true;
            }
        }

        protected override void OnDisable()
        {
            if (isRegistered && UnityEngine.Application.isPlaying)
            {
                isRegistered = false;
                InteractableTrackersDispatcher.Instance?.Unregister(this);
            }

            IsSelectableInteractable = false;
        }

        protected override void OnDestroy()
        {
            if (isRegistered && UnityEngine.Application.isPlaying)
            {
                isRegistered = false;
                InteractableTrackersDispatcher.Instance?.Unregister(this);
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            isSelectableSet = !object.Equals(targetSelectable, null);
        }
#endif

        /// <see cref="UnityEngine.EventSystems.Selectable.OnCanvasGroupChanged"/>
        protected override void OnCanvasGroupChanged()
        {
            UpdateParentCanvasGroupsState();
        }

        private void UpdateParentCanvasGroupsState()
        {
            // Figure out if parent groups allow interaction.
            // If no interaction is allowed then we need to not do that.
            var groupAllowInteraction = true;
            Transform t = transform;
            while (t != null)
            {
                t.GetComponents(canvasGroupCache);
                bool shouldBreak = false;
                for (var i = 0; i < canvasGroupCache.Count; i++)
                {
                    // If the parent group does not allow interaction
                    // we need to break.
                    if (!canvasGroupCache[i].interactable)
                    {
                        groupAllowInteraction = false;
                        shouldBreak = true;
                    }
                    // If this is a 'fresh' group, then break
                    // as we should not consider parents.
                    if (canvasGroupCache[i].ignoreParentGroups)
                    {
                        shouldBreak = true;
                    }
                }

                if (shouldBreak)
                {
                    break;
                }

                t = t.parent;
            }

            if (groupAllowInteraction != groupsAllowInteraction)
            {
                groupsAllowInteraction = groupAllowInteraction;
                OnInteractableStateChanged();
            }
        }
    }
}