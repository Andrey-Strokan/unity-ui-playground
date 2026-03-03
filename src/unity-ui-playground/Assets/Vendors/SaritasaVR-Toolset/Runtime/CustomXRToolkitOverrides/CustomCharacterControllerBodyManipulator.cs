using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace SaritasaVRToolset
{
    /// <inheritdoc/>
    [CreateAssetMenu(fileName = "Assets/Vendors/Saritasa VR-Toolset/Runtime/CustomXRToolkitOverrides/CustomCharacterControllerBodyManipulator",
                                                                         menuName = "VR Toolset/Custom Character Controller Body Manipulator")]
    public class CustomCharacterControllerBodyManipulator : ScriptableConstrainedBodyManipulator
    {
        /// <inheritdoc/>
        public override CollisionFlags lastCollisionFlags => CharacterController != null ? CharacterController.collisionFlags : CollisionFlags.None;

        /// <inheritdoc/>
        public override bool isGrounded => CharacterController == null || CharacterController.isGrounded;

        /// <summary>
        /// The character controller attached to the <see cref="XRMovableBody.originTransform"/> of the
        /// <see cref="IConstrainedXRBodyManipulator.linkedBody"/>. This is <see langword="null"/> if
        /// <see cref="IConstrainedXRBodyManipulator.linkedBody"/> is <see langword="null"/>.
        /// </summary>
        public CharacterController CharacterController { get; private set; }

        /// <inheritdoc/>
        public override void OnLinkedToBody(XRMovableBody body)
        {
            base.OnLinkedToBody(body);

            var xrOrigin = body.xrOrigin;
            var origin = xrOrigin.Origin;

            // Try on the Origin GameObject first, and then fallback to the XR Origin GameObject (if different)
            if (!origin.TryGetComponent(out CharacterController foundController) && origin != xrOrigin.gameObject)
                xrOrigin.TryGetComponent(out foundController);

            if (foundController != null)
            {
                CharacterController = foundController;
                return;
            }

            Debug.LogWarning($"No CharacterController found. Adding one to Origin GameObject '{origin.name}'.", this);
            CharacterController = origin.AddComponent<CharacterController>();
        }

        /// <inheritdoc/>
        public override void OnUnlinkedFromBody()
        {
            base.OnUnlinkedFromBody();
            CharacterController = null;
        }

        /// <inheritdoc/>
        public override CollisionFlags MoveBody(Vector3 motion)
        {
            if (linkedBody == null || CharacterController == null)
                return CollisionFlags.None;

            var xrOrigin = linkedBody.xrOrigin;
            var bodyGroundPosition = linkedBody.GetBodyGroundLocalPosition();
            var capsuleHeight = xrOrigin.CameraInOriginSpaceHeight - bodyGroundPosition.y;
            CharacterController.height = capsuleHeight;
            CharacterController.center = new Vector3(bodyGroundPosition.x,
                bodyGroundPosition.y + capsuleHeight * 0.5f + CharacterController.skinWidth, bodyGroundPosition.z);

            return CharacterController.Move(motion);
        }
    }
}