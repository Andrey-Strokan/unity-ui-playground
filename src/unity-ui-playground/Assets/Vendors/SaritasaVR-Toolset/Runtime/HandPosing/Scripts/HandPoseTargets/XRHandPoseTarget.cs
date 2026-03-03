using HandPosing.Enumerations;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HandPosing.HandPoseTargets
{
    /// <summary>
    /// <see cref="HandPoseTarget"/> override for Unity XR.
    /// </summary>
    public class XRHandPoseTarget : HandPoseTarget
    {
        [Space]

        [SerializeField]
        private HandPose defaultPose;

        [SerializeField]
        private HandPose fistPose;

        [SerializeField]
        private HandPose okPose;

        [SerializeField]
        private InputActionProperty rightHand_PrimaryTouchAction;

        [SerializeField]
        private InputActionProperty leftHand_PrimaryTouchAction;

        [SerializeField]
        private InputActionProperty rightHand_SecondaryTouchAction;

        [SerializeField]
        private InputActionProperty leftHand_SecondaryTouchAction;

        [SerializeField]
        private InputActionProperty rightHand_ThumbTouchAction;

        [SerializeField]
        private InputActionProperty leftHand_ThumbTouchAction;

        [SerializeField]
        private InputActionProperty rightHand_GripAxisAction;

        [SerializeField]
        private InputActionProperty leftHand_GripAxisAction;

        [SerializeField]
        private InputActionProperty rightHand_TriggerAxisAction;

        [SerializeField]
        private InputActionProperty leftHand_TriggerAxisAction;

        /// <inheritdoc />
        protected override void UpdatePose()
        {
            bool isPrimaryTouched;
            bool isSecondaryTouched;
            bool isTouchpadTouched;
            float gripValue;
            float triggerValue;

            if (HandType == HandType.Right)
            {
                isPrimaryTouched = rightHand_PrimaryTouchAction.action.IsPressed();
                isSecondaryTouched = rightHand_SecondaryTouchAction.action.IsPressed();
                isTouchpadTouched = rightHand_ThumbTouchAction.action.IsPressed();

                gripValue = rightHand_GripAxisAction.action.ReadValue<float>();
                triggerValue = rightHand_TriggerAxisAction.action.ReadValue<float>();
            }
            else
            {
                isPrimaryTouched = leftHand_PrimaryTouchAction.action.IsPressed();
                isSecondaryTouched = leftHand_SecondaryTouchAction.action.IsPressed();
                isTouchpadTouched = leftHand_ThumbTouchAction.action.IsPressed();

                gripValue = leftHand_GripAxisAction.action.ReadValue<float>();
                triggerValue = leftHand_TriggerAxisAction.action.ReadValue<float>();
            }

            bool isGripPressed = gripValue > 0.01;
            bool isTriggerPressed = triggerValue > 0.01;
            bool isThumbPressed = isPrimaryTouched || isSecondaryTouched || isTouchpadTouched;

            float thumbValue = isThumbPressed ? 1 : 0;

            if (isTriggerPressed && isThumbPressed)
            {
                for (HandJointType joint = HandJointType.ThumbTrapezium; joint <= HandJointType.IndexDistal; ++joint)
                {
                    Quaternion from = Blend(defaultPose, okPose, joint, triggerValue);
                    Quaternion to = Blend(defaultPose, fistPose, joint, gripValue);

                    CurrentRotations[joint] = Quaternion.Lerp(from, to, gripValue);
                }
            }
            else if (isTriggerPressed)
            {
                ApplyPoseForRange(HandJointType.ThumbTrapezium, HandJointType.ThumbDistal, defaultPose);
                ApplyPoseForRange(HandJointType.IndexProximal, HandJointType.IndexDistal, defaultPose, fistPose,
                    triggerValue);
            }
            else if (isThumbPressed)
            {
                ApplyPoseForRange(HandJointType.ThumbTrapezium, HandJointType.ThumbDistal, defaultPose, fistPose,
                    thumbValue);
                ApplyPoseForRange(HandJointType.IndexProximal, HandJointType.IndexDistal, defaultPose);
            }
            else
            {
                ApplyPoseForRange(HandJointType.ThumbTrapezium, HandJointType.IndexDistal, defaultPose);
            }

            if (isGripPressed)
            {
                ApplyPoseForRange(HandJointType.MiddleProximal, HandJointType.PinkyDistal, defaultPose, fistPose,
                    gripValue);
            }
            else
            {
                ApplyPoseForRange(HandJointType.MiddleProximal, HandJointType.PinkyDistal, defaultPose);
            }

            base.UpdatePose();
        }

        private Quaternion Blend(HandPose from, HandPose to, HandJointType jointType, float t)
        {
            return Quaternion.Slerp(from[jointType], to[jointType], t);
        }

        private void ApplyPoseForRange(HandJointType min, HandJointType max, HandPose from, HandPose to, float t)
        {
            for (HandJointType joint = min; joint <= max; ++joint)
            {
                CurrentRotations[joint] = Blend(from, to, joint, t);
            }
        }

        private void ApplyPoseForRange(HandJointType min, HandJointType max, HandPose pose)
        {
            for (HandJointType joint = min; joint <= max; ++joint)
            {
                CurrentRotations[joint] = pose[joint];
            }
        }
    }
}