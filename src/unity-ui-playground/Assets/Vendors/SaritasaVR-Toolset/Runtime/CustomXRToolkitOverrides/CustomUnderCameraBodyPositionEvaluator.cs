using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace SaritasaVRToolset
{
    /// <summary>
    /// Scriptable object that estimates the user's body position by projecting the position of the camera onto the
    /// XZ plane of the <see cref="XROrigin"/>.
    /// </summary>
    /// <remarks>
    /// This is the default <see cref="CustomXRBodyTransformer.bodyPositionEvaluator"/> for an <see cref="CustomXRBodyTransformer"/>.
    /// </remarks>
    [CreateAssetMenu(fileName = "Assets/Vendors/Saritasa VR-Toolset/Runtime/CustomXRToolkitOverrides/CustomUnderCameraBodyPositionEvaluator",
                                                                        menuName = "VR Toolset/Custom Under Camera Body Position Evaluator")]
    public class CustomUnderCameraBodyPositionEvaluator : ScriptableObject, IXRBodyPositionEvaluator
    {
        private CharacterController characterController = null;

        private HeadCollisionHandler headCollisionHandler;

        /// <summary>
        /// Set references for character controller and head collision handler.
        /// </summary>
        public void SetUpReferences(CharacterController characterController = null, HeadCollisionHandler headCollisionHandler = null)
        {
            if (this.characterController == null && characterController != null)
            {
                this.characterController = characterController;
            }

            if (this.headCollisionHandler == null && headCollisionHandler != null)
            {
                this.headCollisionHandler = headCollisionHandler;
            }
        }

        /// <summary>
        /// Clear references for character controller and head collision handler.
        /// </summary>
        public void UnSetUpReferences()
        {
            if (characterController != null)
            {
                characterController = null;
            }

            if (headCollisionHandler != null)
            {
                headCollisionHandler = null;
            }
        }

        /// <inheritdoc/>
        public Vector3 GetBodyGroundLocalPosition(XROrigin xrOrigin)
        {
            if (characterController != null &&
                headCollisionHandler != null &&
                headCollisionHandler.IsFaded)
            {
                var cameraPosition = xrOrigin.CameraInOriginSpacePos;
                var newCameraPos = characterController.center;
                newCameraPos.y = cameraPosition.y;

                xrOrigin.MoveCameraToWorldLocation(xrOrigin.transform.TransformPoint(newCameraPos));
            }

            var bodyPosition = xrOrigin.CameraInOriginSpacePos;
            bodyPosition.y = 0f;

            return bodyPosition;
        }
    }
}