using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

namespace SaritasaVRToolset
{
    /// <summary>
    /// A version of continuous movement that automatically controls the frame of reference that
    /// determines the forward direction of movement based on user preference for each hand.
    /// For example, can configure to use head relative movement for the left hand and controller relative movement for the right hand.
    /// </summary>
    public class DynamicMoveProvider : ContinuousMoveProvider
    {
        /// <summary>
        /// Defines which transform the XR Origin's movement direction is relative to.
        /// </summary>
        /// <seealso cref="LeftHandMovementDirection"/>
        /// <seealso cref="RightHandMovementDirection"/>
        public enum MovementDirection
        {
            /// <summary>
            /// Use the forward direction of the head (camera) as the forward direction of the XR Origin's movement.
            /// </summary>
            HeadRelative,

            /// <summary>
            /// Use the forward direction of the hand (controller) as the forward direction of the XR Origin's movement.
            /// </summary>
            HandRelative,
        }

        [Space, Header("Movement Direction")]
        [SerializeField]
        [Tooltip("Directs the XR Origin's movement when using the head-relative mode. If not set, will automatically find and use the XR Origin Camera.")]
        private Transform headTransform;

        [SerializeField]
        [Tooltip("Directs the XR Origin's movement when using the hand-relative mode with the left hand.")]
        private Transform leftControllerTransform;

        [SerializeField]
        [Tooltip("Directs the XR Origin's movement when using the hand-relative mode with the right hand.")]
        private Transform rightControllerTransform;

        [SerializeField]
        [Tooltip("Whether to use the specified head transform or left controller transform to direct the XR Origin's movement for the left hand.")]
        private MovementDirection leftHandMovementDirection;

        [SerializeField]
        [Tooltip("Whether to use the specified head transform or right controller transform to direct the XR Origin's movement for the right hand.")]
        private MovementDirection rightHandMovementDirection;

        /// <summary>
        /// Directs the XR Origin's movement when using the head-relative mode. If not set, will automatically find and use the XR Origin Camera.
        /// </summary>
        public Transform HeadTransform
        {
            get => headTransform;
            set => headTransform = value;
        }

        /// <summary>
        /// Directs the XR Origin's movement when using the hand-relative mode with the left hand.
        /// </summary>
        public Transform LeftControllerTransform
        {
            get => leftControllerTransform;
            set => leftControllerTransform = value;
        }

        /// <summary>
        /// Directs the XR Origin's movement when using the hand-relative mode with the right hand.
        /// </summary>
        public Transform RightControllerTransform
        {
            get => rightControllerTransform;
            set => rightControllerTransform = value;
        }

        /// <summary>
        /// Whether to use the specified head transform or controller transform to direct the XR Origin's movement for the left hand.
        /// </summary>
        /// <seealso cref="MovementDirection"/>
        public MovementDirection LeftHandMovementDirection
        {
            get => leftHandMovementDirection;
            set => leftHandMovementDirection = value;
        }

        /// <summary>
        /// Whether to use the specified head transform or controller transform to direct the XR Origin's movement for the right hand.
        /// </summary>
        /// <seealso cref="MovementDirection"/>
        public MovementDirection RightHandMovementDirection
        {
            get => rightHandMovementDirection;
            set => rightHandMovementDirection = value;
        }

        private Transform combinedTransform;
        private Pose leftMovementPose = Pose.identity;
        private Pose rightMovementPose = Pose.identity;

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();

            combinedTransform = new GameObject("[Dynamic Move Provider] Combined Forward Source").transform;
            combinedTransform.SetParent(transform, false);
            combinedTransform.localPosition = Vector3.zero;
            combinedTransform.localRotation = Quaternion.identity;

            forwardSource = combinedTransform;
        }

        /// <inheritdoc />
        protected override Vector3 ComputeDesiredMove(Vector2 input)
        {
            // Don't need to do anything if the total input is zero.
            // This is the same check as the base method.
            if (input == Vector2.zero)
                return Vector3.zero;

            // Initialize the Head Transform if necessary, getting the Camera from XR Origin
            if (headTransform == null)
            {
                var xrOrigin = mediator.xrOrigin;
                if (xrOrigin != null)
                {
                    var xrCamera = xrOrigin.Camera;
                    if (xrCamera != null)
                        headTransform = xrCamera.transform;
                }
            }

            // Get the forward source for the left hand input
            switch (leftHandMovementDirection)
            {
                case MovementDirection.HeadRelative:
                    if (headTransform != null)
                        leftMovementPose = headTransform.GetWorldPose();

                    break;

                case MovementDirection.HandRelative:
                    if (leftControllerTransform != null)
                        leftMovementPose = leftControllerTransform.GetWorldPose();

                    break;

                default:
                    Assert.IsTrue(false, $"Unhandled {nameof(MovementDirection)}={leftHandMovementDirection}");
                    break;
            }

            // Get the forward source for the right hand input
            switch (rightHandMovementDirection)
            {
                case MovementDirection.HeadRelative:
                    if (headTransform != null)
                        rightMovementPose = headTransform.GetWorldPose();

                    break;

                case MovementDirection.HandRelative:
                    if (rightControllerTransform != null)
                        rightMovementPose = rightControllerTransform.GetWorldPose();

                    break;

                default:
                    Assert.IsTrue(false, $"Unhandled {nameof(MovementDirection)}={rightHandMovementDirection}");
                    break;
            }

            // Combine the two poses into the forward source based on the magnitude of input
            var leftHandValue = leftHandMoveInput.ReadValue();
            var rightHandValue = rightHandMoveInput.ReadValue();

            var totalSqrMagnitude = leftHandValue.sqrMagnitude + rightHandValue.sqrMagnitude;
            var leftHandBlend = 0.5f;
            if (totalSqrMagnitude > Mathf.Epsilon)
                leftHandBlend = leftHandValue.sqrMagnitude / totalSqrMagnitude;

            var combinedPosition = Vector3.Lerp(rightMovementPose.position, leftMovementPose.position, leftHandBlend);
            var combinedRotation = Quaternion.Slerp(rightMovementPose.rotation, leftMovementPose.rotation, leftHandBlend);
            combinedTransform.SetPositionAndRotation(combinedPosition, combinedRotation);

            return base.ComputeDesiredMove(input);
        }
    }
}
