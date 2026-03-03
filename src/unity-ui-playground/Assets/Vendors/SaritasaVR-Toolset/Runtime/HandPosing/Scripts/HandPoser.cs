using System;
using System.Collections.Generic;
using HandPosing.Enumerations;
using UnityEngine;

namespace HandPosing
{
    /// <summary>
    /// Class responsible for managing poses.
    /// </summary>
    [RequireComponent(typeof(PoseEventRunner))]
    public class HandPoser : MonoBehaviour
    {
        [SerializeField]
        private PoseEventRunner eventRunner;

        private readonly List<HandPose> leftHandSelectPoses = new List<HandPose>();
        private readonly List<HandPose> rightHandSelectPoses = new List<HandPose>();

        private readonly List<HandPose> leftHandHoverPoses = new List<HandPose>();
        private readonly List<HandPose> rightHandHoverPoses = new List<HandPose>();

        private void Reset()
        {
            if (eventRunner == null)
            {
                eventRunner = GetComponent<PoseEventRunner>();
            }
        }

        private void Awake()
        {
            if (eventRunner == null)
            {
                ArgumentNullException exception = new ArgumentNullException(nameof(eventRunner));
                Debug.LogException(exception);
                return;
            }

            Initialize();
            Subscribe();
        }

        private void OnDestroy()
        {
            if (eventRunner == null)
            {
                return;
            }

            Unsubscribe();
        }

        private void Initialize()
        {
            var handPoses = GetComponentsInChildren<HandPose>(false);

            foreach (HandPose handPose in handPoses)
            {
                if (handPose.HandType == HandType.Left)
                {
                    if ((handPose.ActivationType & ActivationType.Select) != 0)
                    {
                        leftHandSelectPoses.Add(handPose);
                    }

                    if ((handPose.ActivationType & ActivationType.Hover) != 0)
                    {
                        leftHandHoverPoses.Add(handPose);
                    }
                }
                else
                {
                    if ((handPose.ActivationType & ActivationType.Select) != 0)
                    {
                        rightHandSelectPoses.Add(handPose);
                    }

                    if ((handPose.ActivationType & ActivationType.Hover) != 0)
                    {
                        rightHandHoverPoses.Add(handPose);
                    }
                }
            }
        }

        private void Subscribe()
        {
            eventRunner.Grabbed += OnGrabbed;
            eventRunner.UnGrabbed += OnUnGrabbed;

            eventRunner.Hovered += OnHovered;
            eventRunner.UnHovered += OnUnHovered;
        }

        private void Unsubscribe()
        {
            eventRunner.Grabbed -= OnGrabbed;
            eventRunner.UnGrabbed -= OnUnGrabbed;

            eventRunner.Hovered -= OnHovered;
            eventRunner.UnHovered -= OnUnHovered;
        }

        private void ApplyPose(PoseEventRunner.EventArgs args, IReadOnlyList<HandPose> leftHandPoses,
            IReadOnlyList<HandPose> rightHandPoses)
        {
            HandPose pose = GetHandPose(args.HandTransform, args.HandType, leftHandPoses, rightHandPoses);

            if (pose == null)
            {
                return;
            }

            args.HandPoseTarget.SetPose(pose);
            UpdateAttachTransform(pose.AttachTransformPosition, pose.AttachTransformRotation);
        }

        private void UpdateAttachTransform(Vector3 position, Quaternion rotation)
        {
            eventRunner.PosePositionOffset = position;
            eventRunner.PoseRotationOffset = rotation;
        }

        private void OnGrabbed(PoseEventRunner.EventArgs args)
        {
            ApplyPose(args, leftHandSelectPoses, rightHandSelectPoses);
        }

        private static void ClearPose(PoseEventRunner.EventArgs args)
        {
            args.HandPoseTarget.ClearPose();
        }

        private void OnUnGrabbed(PoseEventRunner.EventArgs args)
        {
            ClearPose(args);
        }

        private void OnHovered(PoseEventRunner.EventArgs args)
        {
            if (eventRunner.IsGrabbed)
            {
                return;
            }

            ApplyPose(args, leftHandHoverPoses, rightHandHoverPoses);
        }

        private void OnUnHovered(PoseEventRunner.EventArgs args)
        {
            if (eventRunner.IsGrabbed)
            {
                return;
            }

            ClearPose(args);
        }

        private HandPose GetHandPose(Transform controllerPoint, HandType handType,
            IReadOnlyList<HandPose> leftHandPoses,
            IReadOnlyList<HandPose> rightHandPoses)
        {
            var poses = handType switch
            {
                HandType.Left => leftHandPoses,
                HandType.Right => rightHandPoses,
                _ => throw new ArgumentOutOfRangeException(nameof(handType), handType, null)
            };

            switch (poses.Count)
            {
                case 0:
                {
                    return null;
                }
                case 1:
                {
                    return poses[0];
                }
                default:
                {
                    HandPose result = poses[0];
                    float minSimilarity = GetSimilarity(result, controllerPoint);

                    for (int i = 1; i < poses.Count; ++i)
                    {
                        HandPose pose = poses[i];
                        float similarity = GetSimilarity(pose, controllerPoint);

                        if (similarity < minSimilarity)
                        {
                            result = pose;
                            minSimilarity = similarity;
                        }
                    }

                    return result;
                }
            }
        }

        private float GetSimilarity(HandPose pose, Transform controllerPoint)
        {
            Matrix4x4 matrix = transform.localToWorldMatrix;

            Vector3 position = matrix.MultiplyPoint3x4(pose.AttachTransformPosition);
            Quaternion rotation = matrix.rotation * pose.AttachTransformRotation;

            float angle = Mathf.Abs(Quaternion.Angle(rotation, controllerPoint.rotation));
            float sqrDistance = (controllerPoint.position - position).sqrMagnitude;
            return sqrDistance * angle;
        }
    }
}