using System;
using HandPosing.Enumerations;
using HandPosing.HandPoseTargets;
using UnityEngine;

namespace HandPosing
{
    /// <summary>
    /// Base class that processes events for poses.
    /// </summary>
    public abstract class PoseEventRunner : MonoBehaviour
    {
        /// <summary>
        /// Event args used for pose handling.
        /// </summary>
        public struct EventArgs
        {
            /// <summary>
            /// Hand type.
            /// </summary>
            public readonly HandType HandType;

            /// <summary>
            /// Hand transform.
            /// </summary>
            public readonly Transform HandTransform;

            /// <summary>
            /// Hand pose target.
            /// </summary>
            public readonly HandPoseTarget HandPoseTarget;

            public EventArgs(HandType handType, Transform handTransform, HandPoseTarget handPoseTarget)
            {
                HandType = handType;
                HandTransform = handTransform;
                HandPoseTarget = handPoseTarget;
            }
        }

        /// <summary>
        /// Is this object grabbed or not.
        /// </summary>
        public bool IsGrabbed { get; private set; }

        /// <summary>
        /// Is this object hovered or not.
        /// </summary>
        public bool IsHovered { get; private set; }

        /// <summary>
        /// Event that fires upon object grab.
        /// </summary>
        public abstract event Action<EventArgs> Grabbed;

        /// <summary>
        /// Event that fires upon object ungrab.
        /// </summary>
        public abstract event Action<EventArgs> UnGrabbed;

        /// <summary>
        /// Event that fires upon object hover.
        /// </summary>
        public abstract event Action<EventArgs> Hovered;

        /// <summary>
        /// Event that fires upon object unhover.
        /// </summary>
        public abstract event Action<EventArgs> UnHovered;

        /// <summary>
        /// Attach transform position offset.
        /// </summary>
        public Vector3 PosePositionOffset { get; set; }

        /// <summary>
        /// Attach transform position rotation.
        /// </summary>
        public Quaternion PoseRotationOffset { get; set; }

        /// <inheritdoc />
        protected virtual void Awake()
        {
            Grabbed += OnGrabbed;
            UnGrabbed += OnUnGrabbed;

            Hovered += OnHovered;
            UnHovered += OnUnHovered;
        }

        /// <inheritdoc />
        protected virtual void OnDestroy()
        {
            Grabbed -= OnGrabbed;
            UnGrabbed -= OnUnGrabbed;

            Hovered -= OnHovered;
            UnHovered -= OnUnHovered;
        }

        private void OnGrabbed(EventArgs args)
        {
            IsGrabbed = true;
        }

        private void OnUnGrabbed(EventArgs args)
        {
            IsGrabbed = false;
        }

        private void OnHovered(EventArgs args)
        {
            IsHovered = true;
        }

        private void OnUnHovered(EventArgs args)
        {
            IsHovered = false;
        }
    }
}