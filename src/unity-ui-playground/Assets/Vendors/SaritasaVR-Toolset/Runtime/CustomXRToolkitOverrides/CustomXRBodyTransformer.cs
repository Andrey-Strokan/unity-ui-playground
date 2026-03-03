using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace SaritasaVRToolset
{
    /// <inheritdoc />
    public class CustomXRBodyTransformer : XRBodyTransformer
    {
        [SerializeField]
        private HeadCollisionHandler headCollisionHandler;

        private CustomUnderCameraBodyPositionEvaluator customBodyPositionEvaluator;
        private CustomCharacterControllerBodyManipulator customCharacterControllerBodyManipulator;

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!enabled)
            {
                return;
            }

            if (constrainedBodyManipulator is CustomCharacterControllerBodyManipulator customCharacterControllerBodyManipulator)
            {
                this.customCharacterControllerBodyManipulator = customCharacterControllerBodyManipulator;
            }

            if (bodyPositionEvaluator is CustomUnderCameraBodyPositionEvaluator customBodyPositionEvaluator)
            {
                this.customBodyPositionEvaluator = customBodyPositionEvaluator;

                if (this.customCharacterControllerBodyManipulator != null)
                {
                    customBodyPositionEvaluator.SetUpReferences(this.customCharacterControllerBodyManipulator.CharacterController, headCollisionHandler);
                }
                else
                {
                    customBodyPositionEvaluator.SetUpReferences((constrainedBodyManipulator as CharacterControllerBodyManipulator).characterController, headCollisionHandler);
                }

                headCollisionHandler.LimitCrossed += OnCollisionLimitCrossed;
            }
        }

        /// <inheritdoc />
        protected override void OnDisable()
        {
            if (customBodyPositionEvaluator != null)
            {
                headCollisionHandler.LimitCrossed -= OnCollisionLimitCrossed;
                customBodyPositionEvaluator.UnSetUpReferences();
            }

            base.OnDisable();
        }

        private void OnCollisionLimitCrossed()
        {
            customBodyPositionEvaluator.GetBodyGroundLocalPosition(xrOrigin);
        }
    }
}
