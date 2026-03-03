using System;
using HandPosing.Enumerations;
using HandPosing.HandPoseTargets;
using UnityEngine;

namespace HandPosing
{
    /// <summary>
    /// Class that contains hand pose.
    /// </summary>
    public class HandPose : MonoBehaviour
    {
        /// <summary>
        /// Class that contains finger data.
        /// </summary>
        [Serializable]
        public class FingerData
        {
            [SerializeField]
            private Quaternion[] jointRotations;

            /// <summary>
            /// Joint rotations.
            /// </summary>
            public Quaternion[] JointRotations => jointRotations;

            public FingerData(int length)
            {
                jointRotations = new Quaternion[length];
            }
        }

        [SerializeField]
        private ActivationType activationType = ActivationType.Select;

        // I need hidden serialized fields in order to use default inspector and then draw fields as I prefer without code duplication.

        [SerializeField]
        [HideInInspector]
        private HandType handType;

        [SerializeField]
        [HideInInspector]
        private Vector3 attachTransformPosition = Vector3.zero;

        [SerializeField]
        [HideInInspector]
        private Quaternion attachTransformRotation = Quaternion.identity;

        [SerializeField]
        [HideInInspector]
        private FingerData thumbData = new FingerData(4);

        [SerializeField]
        [HideInInspector]
        private FingerData indexData = new FingerData(3);

        [SerializeField]
        [HideInInspector]
        private FingerData middleData = new FingerData(3);

        [SerializeField]
        [HideInInspector]
        private FingerData ringData = new FingerData(3);

        [SerializeField]
        [HideInInspector]
        private FingerData pinkyData = new FingerData(4);

        [SerializeField]
        [HideInInspector]
        private bool initialized;

        /// <summary>
        /// Hand type.
        /// </summary>
        public HandType HandType
        {
            get => handType;
            set => handType = value;
        }

        /// <summary>
        /// Activation type.
        /// </summary>
        public ActivationType ActivationType => activationType;

        /// <summary>
        /// Position of attach transform.
        /// </summary>
        public Vector3 AttachTransformPosition => attachTransformPosition;

        /// <summary>
        /// Rotation of attach transform.
        /// </summary>
        public Quaternion AttachTransformRotation => attachTransformRotation;

        /// <summary>
        /// Thumb finger data.
        /// </summary>
        public FingerData ThumbData => thumbData;

        /// <summary>
        /// Index finger data.
        /// </summary>
        public FingerData IndexData => indexData;

        /// <summary>
        /// Middle finger data.
        /// </summary>
        public FingerData MiddleData => middleData;

        /// <summary>
        /// Ring finger data.
        /// </summary>
        public FingerData RingData => ringData;

        /// <summary>
        /// Pinky finger data.
        /// </summary>
        public FingerData PinkyData => pinkyData;

#if UNITY_EDITOR

        /// <summary>
        /// Method used to copy joint data from target.
        /// </summary>
        /// <param name="target">Target.</param>
        public void CopyFromTarget(HandPoseTarget target)
        {
            TryCopyFromTarget(out thumbData.JointRotations[0], target.ThumbTrapezium);
            TryCopyFromTarget(out thumbData.JointRotations[1], target.ThumbMeta);
            TryCopyFromTarget(out thumbData.JointRotations[2], target.ThumbProximal);
            TryCopyFromTarget(out thumbData.JointRotations[3], target.ThumbDistal);

            TryCopyFromTarget(out indexData.JointRotations[0], target.IndexProximal);
            TryCopyFromTarget(out indexData.JointRotations[1], target.IndexIntermediate);
            TryCopyFromTarget(out indexData.JointRotations[2], target.IndexDistal);

            TryCopyFromTarget(out middleData.JointRotations[0], target.MiddleProximal);
            TryCopyFromTarget(out middleData.JointRotations[1], target.MiddleIntermediate);
            TryCopyFromTarget(out middleData.JointRotations[2], target.MiddleDistal);

            TryCopyFromTarget(out ringData.JointRotations[0], target.RingProximal);
            TryCopyFromTarget(out ringData.JointRotations[1], target.RingIntermediate);
            TryCopyFromTarget(out ringData.JointRotations[2], target.RingDistal);

            TryCopyFromTarget(out pinkyData.JointRotations[0], target.PinkyMeta);
            TryCopyFromTarget(out pinkyData.JointRotations[1], target.PinkyProximal);
            TryCopyFromTarget(out pinkyData.JointRotations[2], target.PinkyIntermediate);
            TryCopyFromTarget(out pinkyData.JointRotations[3], target.PinkyDistal);
        }

        /// <summary>
        /// Method used to copy joint data to target.
        /// </summary>
        /// <param name="target">Target.</param>
        public void CopyToTarget(HandPoseTarget target)
        {
            TryCopyToTarget(in thumbData.JointRotations[0], target.ThumbTrapezium);
            TryCopyToTarget(in thumbData.JointRotations[1], target.ThumbMeta);
            TryCopyToTarget(in thumbData.JointRotations[2], target.ThumbProximal);
            TryCopyToTarget(in thumbData.JointRotations[3], target.ThumbDistal);

            TryCopyToTarget(in indexData.JointRotations[0], target.IndexProximal);
            TryCopyToTarget(in indexData.JointRotations[1], target.IndexIntermediate);
            TryCopyToTarget(in indexData.JointRotations[2], target.IndexDistal);

            TryCopyToTarget(in middleData.JointRotations[0], target.MiddleProximal);
            TryCopyToTarget(in middleData.JointRotations[1], target.MiddleIntermediate);
            TryCopyToTarget(in middleData.JointRotations[2], target.MiddleDistal);

            TryCopyToTarget(in ringData.JointRotations[0], target.RingProximal);
            TryCopyToTarget(in ringData.JointRotations[1], target.RingIntermediate);
            TryCopyToTarget(in ringData.JointRotations[2], target.RingDistal);

            TryCopyToTarget(in pinkyData.JointRotations[0], target.PinkyMeta);
            TryCopyToTarget(in pinkyData.JointRotations[1], target.PinkyProximal);
            TryCopyToTarget(in pinkyData.JointRotations[2], target.PinkyIntermediate);
            TryCopyToTarget(in pinkyData.JointRotations[3], target.PinkyDistal);
        }

        private bool TryCopyFromTarget(out Quaternion rotation, Transform target)
        {
            if (!target)
            {
                rotation = Quaternion.identity;
                return false;
            }

            rotation = target.localRotation;
            return true;
        }

        private bool TryCopyToTarget(in Quaternion rotation, Transform target)
        {
            if (!target)
            {
                return false;
            }

            target.localRotation = rotation;
            return true;
        }
#endif

        /// <summary>
        /// Indexer for joint rotations.
        /// </summary>
        /// <param name="jointType">Joint to get rotation for.</param>
        public Quaternion this[HandJointType jointType] => jointType switch
        {
            HandJointType.Wrist => transform.localRotation,
            HandJointType.ThumbTrapezium => ThumbData.JointRotations[0],
            HandJointType.ThumbMeta => ThumbData.JointRotations[1],
            HandJointType.ThumbProximal => ThumbData.JointRotations[2],
            HandJointType.ThumbDistal => ThumbData.JointRotations[3],
            HandJointType.IndexProximal => IndexData.JointRotations[0],
            HandJointType.IndexIntermediate => IndexData.JointRotations[1],
            HandJointType.IndexDistal => IndexData.JointRotations[2],
            HandJointType.MiddleProximal => MiddleData.JointRotations[0],
            HandJointType.MiddleIntermediate => MiddleData.JointRotations[1],
            HandJointType.MiddleDistal => MiddleData.JointRotations[2],
            HandJointType.RingProximal => RingData.JointRotations[0],
            HandJointType.RingIntermediate => RingData.JointRotations[1],
            HandJointType.RingDistal => RingData.JointRotations[2],
            HandJointType.PinkyMeta => PinkyData.JointRotations[0],
            HandJointType.PinkyProximal => PinkyData.JointRotations[1],
            HandJointType.PinkyIntermediate => PinkyData.JointRotations[2],
            HandJointType.PinkyDistal => PinkyData.JointRotations[3],
            _ => Quaternion.identity
        };
    }
}