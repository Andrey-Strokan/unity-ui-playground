using System;
using HandPosing.HandPoseTargets;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace HandPosing.XR
{
    /// <summary>
    /// Event runner override for XR system.
    /// </summary>
    public class XRPoseEventRunner : PoseEventRunner
    {
        [SerializeField]
        private XRGrabInteractable grabInteractable;

        [SerializeField]
        private XRPoseGrabTransformer grabTransformer;

        /// <inheritdoc />
        public override event Action<EventArgs> Grabbed;

        /// <inheritdoc />
        public override event Action<EventArgs> UnGrabbed;

        /// <inheritdoc />
        public override event Action<EventArgs> Hovered;

        /// <inheritdoc />
        public override event Action<EventArgs> UnHovered;

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();

            grabInteractable.selectEntered.AddListener(OnSelectEntered);
            grabInteractable.selectExited.AddListener(OnSelectExited);

            grabInteractable.hoverEntered.AddListener(OnHoverEntered);
            grabInteractable.hoverExited.AddListener(OnHoverExited);
        }

        /// <inheritdoc />
        protected override void OnDestroy()
        {
            grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
            grabInteractable.selectExited.RemoveListener(OnSelectExited);

            grabInteractable.hoverEntered.RemoveListener(OnHoverEntered);
            grabInteractable.hoverExited.RemoveListener(OnHoverExited);

            base.OnDestroy();
        }

        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            EventArgs eventArgs = GetEventArgs(args);

            if (eventArgs.HandPoseTarget == null)
            {
                return;
            }

            Grabbed?.Invoke(eventArgs);

            grabTransformer.PosePositionOffset = PosePositionOffset;
            grabTransformer.PoseRotationOffset = PoseRotationOffset;
        }

        private void OnSelectExited(SelectExitEventArgs args)
        {
            EventArgs eventArgs = GetEventArgs(args);

            if (eventArgs.HandPoseTarget == null)
            {
                return;
            }

            UnGrabbed?.Invoke(eventArgs);
        }

        private void OnHoverEntered(HoverEnterEventArgs args)
        {
            EventArgs eventArgs = GetEventArgs(args);

            if (eventArgs.HandPoseTarget == null)
            {
                return;
            }

            Hovered?.Invoke(eventArgs);

            grabTransformer.PosePositionOffset = PosePositionOffset;
            grabTransformer.PoseRotationOffset = PoseRotationOffset;
        }

        private void OnHoverExited(HoverExitEventArgs args)
        {
            EventArgs eventArgs = GetEventArgs(args);

            if (eventArgs.HandPoseTarget == null)
            {
                return;
            }

            UnHovered?.Invoke(eventArgs);
        }

        private static EventArgs GetEventArgs(BaseInteractionEventArgs args)
        {
            var interactor = GetInteractorFromArgs(args, out HandPoseRelay poseRelay);

            return interactor == null ?
                   default :
                   new EventArgs(poseRelay.Target.HandType, interactor.transform, poseRelay.Target);
        }

        private static IXRInteractor GetInteractorFromArgs(BaseInteractionEventArgs args, out HandPoseRelay poseTarget)
        {
            var interactor = args.interactorObject;

            if (interactor == null)
            {
                poseTarget = null;
                return null;
            }

            poseTarget = interactor.transform.GetComponentInParent<HandPoseRelay>();
            return interactor;
        }
    }
}