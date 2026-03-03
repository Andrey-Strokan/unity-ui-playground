using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

/// <inheritdoc/>
public class CustomTeleportationProvider : TeleportationProvider
{
    [SerializeField]
    private HeadCollisionHandler headCollisionHandler;

    /// <inheritdoc/>
    public override bool canStartMoving => delayTime <= 0f || Time.time - delayStartTime >= delayTime;

    private float delayStartTime;

    /// <summary>
    /// The transformation that is used by this component to apply physical head positioning movement.
    /// </summary>
    public DelegateXRBodyTransformation HeadPositionTransformation { get; set; } = new DelegateXRBodyTransformation();

    /// <summary>
    /// The transformation that is used by this component to apply physical body movement.
    /// </summary>
    public XROriginMovement Transformation { get; set; } = new XROriginMovement();

    private void OnEnable()
    {
        HeadPositionTransformation.transformation += ResetPhysicalHead;
    }

    private void OnDisable()
    {
        HeadPositionTransformation.transformation -= ResetPhysicalHead;
    }

    /// <inheritdoc/>
    protected override void Update()
    {
        if (!validRequest)
            return;

        if (locomotionState == LocomotionState.Idle)
        {
            if (delayTime > 0f)
            {
                if (TryPrepareLocomotion())
                    delayStartTime = Time.time;
            }
            else
            {
                TryStartLocomotionImmediately();
            }
        }

        if (locomotionState == LocomotionState.Moving)
        {
            switch (currentRequest.matchOrientation)
            {
                case MatchOrientation.WorldSpaceUp:
                    upTransformation.targetUp = Vector3.up;
                    TryQueueTransformation(upTransformation);
                    break;
                case MatchOrientation.TargetUp:
                    upTransformation.targetUp = currentRequest.destinationRotation * Vector3.up;
                    TryQueueTransformation(upTransformation);
                    break;
                case MatchOrientation.TargetUpAndForward:
                    upTransformation.targetUp = currentRequest.destinationRotation * Vector3.up;
                    TryQueueTransformation(upTransformation);
                    forwardTransformation.targetDirection = currentRequest.destinationRotation * Vector3.forward;
                    TryQueueTransformation(forwardTransformation);
                    break;
                case MatchOrientation.None:
                    // Change nothing. Maintain current origin rotation.
                    break;
                default:
                    Assert.IsTrue(false, $"Unhandled {nameof(MatchOrientation)}={currentRequest.matchOrientation}.");
                    break;
            }

            positionTransformation.targetPosition = currentRequest.destinationPosition;
            var direction = mediator.xrOrigin.transform.position - currentRequest.destinationPosition;
            Transformation.motion = direction.normalized * 0.01f;

            TryQueueTransformation(positionTransformation);
            TryQueueTransformation(Transformation);
            TryQueueTransformation(HeadPositionTransformation);

            TryEndLocomotion();
            validRequest = false;
        }
    }

    private void ResetPhysicalHead(XRMovableBody body)
    {
        headCollisionHandler.ResetHead();
    }
}
