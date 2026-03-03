using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace SaritasaVRToolset
{
    /// <summary>
    /// Affordance component used in conjunction with a <see cref="UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing.ClimbTeleportInteractor"/> to display an object
    /// pointing at the target teleport destination while climbing.
    /// </summary>
    public class ClimbTeleportDestinationIndicator : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The interactor that drives the display and placement of the pointer object.")]
        private ClimbTeleportInteractor climbTeleportInteractor;

        [SerializeField]
        [Tooltip("The prefab to spawn when a teleport destination is chosen. The instance will spawn next to " +
                 "the destination and point its forward vector at the destination and its up vector at the camera.")]
        private GameObject pointerPrefab;

        [SerializeField]
        [Tooltip("The distance from the destination at which the pointer object spawns.")]
        private float pointerDistance = 0.3f;

        /// <summary>
        /// The interactor that drives the display and placement of the pointer object.
        /// </summary>
        public ClimbTeleportInteractor ClimbTeleportInteractor
        {
            get
            {
                return climbTeleportInteractor;
            }
            set
            {
                climbTeleportInteractor = value;
            }
        }

        /// <summary>
        /// The prefab to spawn when a teleport destination is chosen. The instance will spawn next to the destination
        /// and point its forward vector at the destination and its up vector at the camera.
        /// </summary>
        public GameObject PointerPrefab
        {
            get
            {
                return pointerPrefab;
            }
            set
            {
                pointerPrefab = value;
            }
        }

        /// <summary>
        /// The distance from the destination at which the pointer object spawns.
        /// </summary>
        public float PointerDistance
        {
            get
            {
                return pointerDistance;
            }
            set
            {
                pointerDistance = value;
            }
        }

        private TeleportationMultiAnchorVolume activeTeleportVolume;
        private Transform pointerInstance;

        /// <inheritdoc/>
        protected void OnEnable()
        {
            if (climbTeleportInteractor == null)
            {
                Debug.LogError($"Could not find {nameof(UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing.ClimbTeleportInteractor)}.");
                enabled = false;

                return;
            }

            climbTeleportInteractor.hoverEntered.AddListener(OnInteractorHoverEntered);
            climbTeleportInteractor.hoverExited.AddListener(OnInteractorHoverExited);
        }

        /// <inheritdoc/>
        protected void OnDisable()
        {
            HideIndicator();

            if (activeTeleportVolume != null)
            {
                activeTeleportVolume.destinationAnchorChanged -= OnClimbTeleportDestinationAnchorChanged;
                activeTeleportVolume = null;
            }

            if (climbTeleportInteractor != null)
            {
                climbTeleportInteractor.hoverEntered.RemoveListener(OnInteractorHoverEntered);
                climbTeleportInteractor.hoverExited.RemoveListener(OnInteractorHoverExited);
            }
        }

        private void OnInteractorHoverEntered(HoverEnterEventArgs args)
        {
            if (activeTeleportVolume != null || !(args.interactableObject is TeleportationMultiAnchorVolume teleportVolume))
            {
                return;
            }

            activeTeleportVolume = teleportVolume;

            if (activeTeleportVolume.destinationAnchor != null)
            {
                OnClimbTeleportDestinationAnchorChanged(activeTeleportVolume);
            }

            activeTeleportVolume.destinationAnchorChanged += OnClimbTeleportDestinationAnchorChanged;
        }

        private void OnInteractorHoverExited(HoverExitEventArgs args)
        {
            if (!(args.interactableObject is TeleportationMultiAnchorVolume teleportVolume) || teleportVolume != activeTeleportVolume)
            {
                return;
            }

            HideIndicator();
            activeTeleportVolume.destinationAnchorChanged -= OnClimbTeleportDestinationAnchorChanged;
            activeTeleportVolume = null;
        }

        private void OnClimbTeleportDestinationAnchorChanged(TeleportationMultiAnchorVolume teleportVolume)
        {
            HideIndicator();

            var destinationAnchor = teleportVolume.destinationAnchor;

            if (destinationAnchor == null)
            {
                return;
            }

            pointerInstance = Instantiate(pointerPrefab).transform;
            var cameraTrans = teleportVolume.teleportationProvider.mediator.xrOrigin.Camera.transform;
            var cameraPosition = cameraTrans.position;
            var destinationPosition = destinationAnchor.position;
            var destinationDirectionInScreenSpace = cameraTrans.InverseTransformDirection(destinationPosition - cameraPosition);
            destinationDirectionInScreenSpace.z = 0f;
            var pointerDirection = cameraTrans.TransformDirection(destinationDirectionInScreenSpace).normalized;
            pointerInstance.position = destinationPosition - pointerDirection * pointerDistance;
            pointerInstance.rotation = Quaternion.LookRotation(pointerDirection, -cameraTrans.forward);
        }

        private void HideIndicator()
        {
            if (pointerInstance != null)
            {
                Destroy(pointerInstance.gameObject);
            }
        }
    }
}
