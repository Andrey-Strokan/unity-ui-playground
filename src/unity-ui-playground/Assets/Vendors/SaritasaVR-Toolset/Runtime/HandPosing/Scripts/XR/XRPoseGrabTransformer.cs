using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

namespace HandPosing.XR
{
    /// <summary>
    /// Pose grab transformer for proper attach transform positioning.
    /// </summary>
    public class XRPoseGrabTransformer : XRBaseGrabTransformer
    {
        /// <summary>
        /// Attach transform position offset.
        /// </summary>
        public Vector3 PosePositionOffset { get; set; }

        /// <summary>
        /// Attach transform rotation offset.
        /// </summary>
        public Quaternion PoseRotationOffset { get; set; }

        /// <inheritdoc />
        public override void Process(XRGrabInteractable grabInteractable,
            XRInteractionUpdateOrder.UpdatePhase updatePhase, ref Pose targetPose, ref Vector3 localScale)
        {
            if (updatePhase != XRInteractionUpdateOrder.UpdatePhase.Dynamic &&
                updatePhase != XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender)
            {
                return;
            }

            if (grabInteractable.interactorsSelecting.Count == 1)
            {
                IXRSelectInteractor interactor = grabInteractable.interactorsSelecting[0];
                Transform interactorAttachTransform = interactor.GetAttachTransform(grabInteractable);

                Matrix4x4 interactorMatrix = interactorAttachTransform.localToWorldMatrix;

                Quaternion rotation = interactorMatrix.rotation;
                Vector3 position = interactorMatrix.GetPosition();

                Quaternion offsetRotation = Quaternion.Inverse(PoseRotationOffset);
                Vector3 offsetPosition = rotation * offsetRotation * -PosePositionOffset.Multiply(localScale);

                targetPose.rotation = rotation * offsetRotation;
                targetPose.position = position + offsetPosition;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}