using System.Collections.Generic;
using HandPosing.Enumerations;
using UnityEngine;

namespace HandPosing.HandPoseTargets
{
    /// <summary>
    /// Base HandPoseTarget to use in posing system.
    /// </summary>
    public class HandPoseTarget : MonoBehaviour
    {
        [SerializeField]
        private HandType handType;

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Joint rotation interpolation speed.")]
        private float lerpSpeed = 0.3f;

        [SerializeField]
        private Transform thumbTrapezium;

        [SerializeField]
        private Transform thumbMeta;

        [SerializeField]
        private Transform thumbProximal;

        [SerializeField]
        private Transform thumbDistal;

        [SerializeField]
        private Transform indexProximal;

        [SerializeField]
        private Transform indexIntermediate;

        [SerializeField]
        private Transform indexDistal;

        [SerializeField]
        private Transform middleProximal;

        [SerializeField]
        private Transform middleIntermediate;

        [SerializeField]
        private Transform middleDistal;

        [SerializeField]
        private Transform ringProximal;

        [SerializeField]
        private Transform ringIntermediate;

        [SerializeField]
        private Transform ringDistal;

        [SerializeField]
        private Transform pinkyMeta;

        [SerializeField]
        private Transform pinkyProximal;

        [SerializeField]
        private Transform pinkyIntermediate;

        [SerializeField]
        private Transform pinkyDistal;

        [SerializeField]
        [Tooltip("Attach transform target position.")]
        private Transform target;

        [SerializeField]
        private HandAxisPreset axisPreset;

        /// <summary>
        /// Preset used by hand.
        /// </summary>
        public HandAxisPreset AxisPreset => axisPreset;

        /// <summary>
        /// Controller offset transform.
        /// </summary>
        public Transform Target => target;

        /// <summary>
        /// Hand type.
        /// </summary>
        public HandType HandType => handType;

        /// <summary>
        /// Joint rotation interpolation speed.
        /// </summary>
        public float LerpSpeed => lerpSpeed;

        /// <summary>
        /// Reference to ThumbTrapezium.
        /// </summary>
        public Transform ThumbTrapezium => thumbTrapezium;

        /// <summary>
        /// Reference to ThumbMeta.
        /// </summary>
        public Transform ThumbMeta => thumbMeta;

        /// <summary>
        /// Reference to ThumbProximal.
        /// </summary>
        public Transform ThumbProximal => thumbProximal;

        /// <summary>
        /// Reference to ThumbDistal.
        /// </summary>
        public Transform ThumbDistal => thumbDistal;

        /// <summary>
        /// Reference to IndexProximal.
        /// </summary>
        public Transform IndexProximal => indexProximal;

        /// <summary>
        /// Reference to IndexIntermediate.
        /// </summary>
        public Transform IndexIntermediate => indexIntermediate;

        /// <summary>
        /// Reference to IndexDistal.
        /// </summary>
        public Transform IndexDistal => indexDistal;

        /// <summary>
        /// Reference to MiddleProximal.
        /// </summary>
        public Transform MiddleProximal => middleProximal;

        /// <summary>
        /// Reference to MiddleIntermediate.
        /// </summary>
        public Transform MiddleIntermediate => middleIntermediate;

        /// <summary>
        /// Reference to MiddleDistal.
        /// </summary>
        public Transform MiddleDistal => middleDistal;

        /// <summary>
        /// Reference to RingProximal.
        /// </summary>
        public Transform RingProximal => ringProximal;

        /// <summary>
        /// Reference to RingIntermediate.
        /// </summary>
        public Transform RingIntermediate => ringIntermediate;

        /// <summary>
        /// Reference to RingDistal.
        /// </summary>
        public Transform RingDistal => ringDistal;

        /// <summary>
        /// Reference to PinkyMeta.
        /// </summary>
        public Transform PinkyMeta => pinkyMeta;

        /// <summary>
        /// Reference to PinkyProximal.
        /// </summary>
        public Transform PinkyProximal => pinkyProximal;

        /// <summary>
        /// Reference to PinkyIntermediate.
        /// </summary>
        public Transform PinkyIntermediate => pinkyIntermediate;

        /// <summary>
        /// Reference to PinkyDistal.
        /// </summary>
        public Transform PinkyDistal => pinkyDistal;

        private IReadOnlyList<Transform> joints;

        /// <summary>
        /// Joints collection that maps to <see cref="HandJointType"/>.
        /// </summary>
        public IReadOnlyList<Transform> Joints => joints ??= new[]
        {
            transform,
            thumbTrapezium,
            thumbMeta,
            thumbProximal,
            thumbDistal,
            indexProximal,
            indexIntermediate,
            indexDistal,
            middleProximal,
            middleIntermediate,
            middleDistal,
            ringProximal,
            ringIntermediate,
            ringDistal,
            pinkyMeta,
            pinkyProximal,
            pinkyIntermediate,
            pinkyDistal
        };

        /// <summary>
        /// Currently used pose.
        /// </summary>
        protected HandPose Pose { get; private set; }

        /// <summary>
        /// Current joint rotations.
        /// </summary>
        protected readonly HandRotations CurrentRotations = new HandRotations();

        [ContextMenu("Align With Target")]
        private void AlignWithTarget()
        {
            Quaternion rotationDelta = Quaternion.Inverse(transform.rotation) * Target.rotation;
            transform.localRotation = rotationDelta;

            Vector3 positionDelta = transform.position - Target.position;
            transform.localPosition = positionDelta;
        }

        /// <summary>
        /// Method used to set pose.
        /// </summary>
        /// <param name="pose">New pose to use.</param>
        public virtual void SetPose(HandPose pose)
        {
            Pose = pose;
        }

        /// <summary>
        /// Method used to clear pose.
        /// </summary>
        public virtual void ClearPose()
        {
            Pose = null;
        }

        /// <inheritdoc />
        protected virtual void Update()
        {
            UpdatePose();
        }

        /// <summary>
        /// Method that updates pose.
        /// </summary>
        protected virtual void UpdatePose()
        {
            if (Pose != null)
            {
                CurrentRotations[HandJointType.ThumbTrapezium] = Pose[HandJointType.ThumbTrapezium];
                CurrentRotations[HandJointType.ThumbMeta] = Pose[HandJointType.ThumbMeta];
                CurrentRotations[HandJointType.ThumbProximal] = Pose[HandJointType.ThumbProximal];
                CurrentRotations[HandJointType.ThumbDistal] = Pose[HandJointType.ThumbDistal];

                CurrentRotations[HandJointType.IndexProximal] = Pose[HandJointType.IndexProximal];
                CurrentRotations[HandJointType.IndexIntermediate] = Pose[HandJointType.IndexIntermediate];
                CurrentRotations[HandJointType.IndexDistal] = Pose[HandJointType.IndexDistal];

                CurrentRotations[HandJointType.MiddleProximal] = Pose[HandJointType.MiddleProximal];
                CurrentRotations[HandJointType.MiddleIntermediate] = Pose[HandJointType.MiddleIntermediate];
                CurrentRotations[HandJointType.MiddleDistal] = Pose[HandJointType.MiddleDistal];

                CurrentRotations[HandJointType.RingProximal] = Pose[HandJointType.RingProximal];
                CurrentRotations[HandJointType.RingIntermediate] = Pose[HandJointType.RingIntermediate];
                CurrentRotations[HandJointType.RingDistal] = Pose[HandJointType.RingDistal];

                CurrentRotations[HandJointType.PinkyMeta] = Pose[HandJointType.PinkyMeta];
                CurrentRotations[HandJointType.PinkyProximal] = Pose[HandJointType.PinkyProximal];
                CurrentRotations[HandJointType.PinkyIntermediate] = Pose[HandJointType.PinkyIntermediate];
                CurrentRotations[HandJointType.PinkyDistal] = Pose[HandJointType.PinkyDistal];
            }

            for (int i = 0; i < Joints.Count; ++i)
            {
                Transform joint = Joints[i];

                if (joint == null)
                {
                    continue;
                }

                Quaternion rotation = joint.localRotation;
                rotation = Quaternion.Slerp(rotation, CurrentRotations[(HandJointType)i], LerpSpeed);
                joint.localRotation = rotation;
            }
        }
    }
}