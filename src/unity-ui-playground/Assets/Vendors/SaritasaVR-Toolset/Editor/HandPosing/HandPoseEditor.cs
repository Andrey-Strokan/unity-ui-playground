using HandPosing.Editor.PropertyWrappers;
using HandPosing.Enumerations;
using HandPosing.HandPoseTargets;
using HandPosing.Utils;
using UnityEditor;
using UnityEngine;

namespace HandPosing.Editor
{
    /// <summary>
    /// Editor class for editing <see cref="HandPose"/>.
    /// </summary>
    [CustomEditor(typeof(HandPose))]
    public class HandPoseEditor : UnityEditor.Editor
    {
        private HandPose handPose;

        private HandPoseTarget handInstance;

        private PropertyWrapper<bool> initialized;
        private PropertyWrapper<bool> autoAlignAttachTransform;

        private PropertyWrapper<Vector3> attachTransformPosition;
        private PropertyWrapper<Quaternion> attachTransformRotation;

        private HandPoseTarget HandPrefab => HandPoserSettings.Instance.HandPoseTarget;

        private bool InitializedEditor => handPose;

        private HandAxisPreset AxisPreset => handInstance.AxisPreset;

        private float[] distances;

        private float[] Distances
        {
            get
            {
                if (distances == null)
                {
                    distances = new float[(int)HandJointType.Count];

                    for (int i = 0; i < distances.Length; ++i)
                    {
                        distances[i] = 0.02f;
                    }
                }

                return distances;
            }
        }

        private void CreateHand()
        {
            if (handInstance == null)
            {
                handInstance = Instantiate(HandPrefab, handPose.transform);
                Transform handTransform = handInstance.transform;
                Vector3 scale = handTransform.lossyScale;
                handTransform.localScale = new Vector3(1 / scale.x, 1 / scale.y, 1 / scale.z);

                handInstance.gameObject.hideFlags = HideFlags.HideAndDontSave;

                UpdateHandVisuals();
            }
        }

        private void DestroyHand()
        {
            if (handInstance != null)
            {
                DestroyImmediate(handInstance.gameObject);
            }
        }

        private void UpdateHandVisuals()
        {
            if (initialized.Value)
            {
                handPose.CopyToTarget(handInstance);
            }
            else
            {
                handPose.CopyFromTarget(HandPrefab);

                serializedObject.Update();
                initialized.Value = true;

                serializedObject.ApplyModifiedProperties();
            }

            Transform transform = handInstance.transform;

            Vector3 scale = transform.localScale;
            if (handPose.HandType == HandType.Left)
            {
                scale[AxisPreset.RightIndex] = -Mathf.Abs(scale[AxisPreset.RightIndex]);
            }
            else
            {
                scale[AxisPreset.RightIndex] = Mathf.Abs(scale[AxisPreset.RightIndex]);
            }

            transform.localScale = scale;
        }

        private void Initialize()
        {
            handPose = (HandPose)target;

            ClearChildren();

            initialized = new BoolPropertyWrapper(serializedObject, nameof(initialized));
            autoAlignAttachTransform = new BoolPropertyWrapper(serializedObject, nameof(autoAlignAttachTransform));

            attachTransformPosition = new Vector3PropertyWrapper(serializedObject, nameof(attachTransformPosition));
            attachTransformRotation = new QuaternionPropertyWrapper(serializedObject, nameof(attachTransformRotation));

            CreateHand();

            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }

        private void ClearChildren()
        {
            for (int i = 0; i < handPose.transform.childCount; ++i)
            {
                Transform child = handPose.transform.GetChild(i);
                if (child.GetComponent<HandPoseTarget>())
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        private void UnInitialize()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            DestroyHand();
        }

        private void Awake()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            UnInitialize();
        }

        private void OnUndoRedoPerformed()
        {
            DestroyHand();
            CreateHand();
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            if (!InitializedEditor)
            {
                Initialize();
            }

            base.OnInspectorGUI();

            autoAlignAttachTransform.Value =
                EditorGUILayout.Toggle(autoAlignAttachTransform.PropertyName, autoAlignAttachTransform.Value);

            if (autoAlignAttachTransform.HasChanged)
            {
                serializedObject.ApplyModifiedProperties();
            }

            if (!autoAlignAttachTransform.Value)
            {
                attachTransformPosition.Value = EditorGUILayout.Vector3Field(attachTransformPosition.PropertyName,
                    attachTransformPosition.Value);

                Vector3 attachTransformEulerAngles = EditorGUILayout.Vector3Field(attachTransformRotation.PropertyName,
                    attachTransformRotation.Value.eulerAngles);
                attachTransformRotation.Value = Quaternion.Euler(attachTransformEulerAngles);

                if (attachTransformPosition.HasChanged || attachTransformRotation.HasChanged)
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }

            if (GUILayout.Button(ObjectNames.NicifyVariableName(nameof(CreateMirroredHand))))
            {
                CreateMirroredHand();
            }

            if (!autoAlignAttachTransform.Value)
            {
                if (GUILayout.Button(ObjectNames.NicifyVariableName(nameof(CalculateAttachTransform))))
                {
                    CalculateAttachTransform();
                    serializedObject.ApplyModifiedProperties();
                }
            }

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void CreateMirroredHand()
        {
            DestroyHand();

            Undo.IncrementCurrentGroup();

            HandPose mirrored = Instantiate(handPose);
            mirrored.name = mirrored.name.Replace("(Clone)", "_mirrored");

            Undo.RegisterCreatedObjectUndo(mirrored.gameObject, "Create Hand Clone");
            Undo.SetTransformParent(mirrored.transform, handPose.transform.parent, false, "Change Clone Parent");

            mirrored.HandType = 1 - mirrored.HandType;

            Transform transform = mirrored.transform;

            Vector3 rotation = transform.localEulerAngles;
            rotation[AxisPreset.UpIndex] *= -1;
            rotation[AxisPreset.ForwardIndex] *= -1;

            transform.localEulerAngles = rotation;

            Vector3 position = transform.localPosition;
            position[AxisPreset.RightIndex] *= -1;

            transform.localPosition = position;

            Undo.RegisterCompleteObjectUndo(mirrored, "Set all variables");
            Undo.SetCurrentGroupName("Create Mirrored Hand");

            Selection.objects = new Object[] { mirrored.gameObject };
        }

        private Transform GetParent()
        {
            Transform result;
            HandPoser poser = handPose.GetComponentInParent<HandPoser>();

            if (poser)
            {
                result = poser.transform;
            }
            else
            {
                result = handPose.transform;
            }

            if (result.parent)
            {
                result = result.parent;
            }

            return result;
        }

        private bool CalculateAttachTransform()
        {
            Transform parent = GetParent();

            Matrix4x4 matrix = parent.worldToLocalMatrix;
            Vector3 attachPosition = matrix.MultiplyPoint3x4(handInstance.Target.position);
            Quaternion attachRotation = matrix.rotation * handInstance.Target.rotation;

            attachTransformPosition.Value = attachPosition;
            attachTransformRotation.Value = attachRotation;

            return attachTransformPosition.HasChanged || attachTransformRotation.HasChanged;
        }

        private bool isThumbActive;
        private bool isIndexActive;
        private bool isMiddleActive;
        private bool isRingActive;
        private bool isPinkyActive;

        private void OnSceneGUI()
        {
            if (!InitializedEditor)
            {
                Initialize();
            }

            if (autoAlignAttachTransform.Value)
            {
                if (CalculateAttachTransform())
                {
                    serializedObject.ApplyModifiedPropertiesWithoutUndo();
                }
            }
            else
            {
                using EditorGUI.ChangeCheckScope changeScope = new EditorGUI.ChangeCheckScope();

                Transform poseParent = GetParent();

                Matrix4x4 matrix = poseParent.worldToLocalMatrix;
                Matrix4x4 invMatrix = matrix.inverse;

                Vector3 attachPosition = invMatrix.MultiplyPoint(attachTransformPosition.Value);
                Quaternion attachRotation = attachTransformRotation.Value.normalized * invMatrix.rotation;

                Handles.TransformHandle(ref attachPosition, ref attachRotation);

                attachTransformPosition.Value = matrix.MultiplyPoint(attachPosition);
                attachTransformRotation.Value = attachRotation * matrix.rotation;

                if (changeScope.changed && (attachTransformPosition.HasChanged || attachTransformRotation.HasChanged))
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }

            DrawToggleButton(HandJointType.ThumbTrapezium, ref isThumbActive);
            DrawToggleButton(HandJointType.IndexProximal, ref isIndexActive);
            DrawToggleButton(HandJointType.MiddleProximal, ref isMiddleActive);
            DrawToggleButton(HandJointType.RingProximal, ref isRingActive);
            DrawToggleButton(HandJointType.PinkyMeta, ref isPinkyActive);

            if (isThumbActive)
            {
                DrawJointRange(HandJointType.ThumbTrapezium, HandJointType.ThumbDistal);
            }

            if (isIndexActive)
            {
                DrawJointRange(HandJointType.IndexProximal, HandJointType.IndexDistal);
            }

            if (isMiddleActive)
            {
                DrawJointRange(HandJointType.MiddleProximal, HandJointType.MiddleDistal);
            }

            if (isRingActive)
            {
                DrawJointRange(HandJointType.RingProximal, HandJointType.RingDistal);
            }

            if (isPinkyActive)
            {
                DrawJointRange(HandJointType.PinkyMeta, HandJointType.PinkyDistal);
            }
        }

        private void DrawJointRange(HandJointType from, HandJointType to)
        {
            using EditorGUI.ChangeCheckScope changeScope = new EditorGUI.ChangeCheckScope();

            for (HandJointType jointType = from; jointType <= to; ++jointType)
            {
                HandJointType type = jointType;
                Transform joint = GetClosestJoint(ref jointType);

                if (joint == null)
                {
                    continue;
                }

                Quaternion rotation = DrawDiscHandles(type, joint);
                rotation = DrawTangentHandles(type, joint.localToWorldMatrix, rotation);

                joint.rotation = rotation;
            }

            if (changeScope.changed)
            {
                Undo.RecordObject(handPose, "Joint Changed");

                handPose.CopyFromTarget(handInstance);
                serializedObject.Update();
            }
        }

        private Quaternion DrawTangentHandles(HandJointType jointType, Matrix4x4 jointMatrix, Quaternion rotation)
        {
            Handles.color = Color.cyan;

            int index = (int)jointType;
            Vector3 position = jointMatrix.GetPosition();
            Matrix4x4 worldToLocal = jointMatrix.inverse;

            Vector3 handlePoint = position + rotation * AxisPreset.ForwardVector * Distances[index];
            Handles.DrawLine(position, handlePoint);

            Vector3 newPosition =
                Handles.FreeMoveHandle(handlePoint, 0.004f, Vector3.zero, Handles.SphereHandleCap);

            Vector3 handleLocal = worldToLocal.MultiplyPoint3x4(handlePoint);
            Vector3 newLocal = worldToLocal.MultiplyPoint3x4(newPosition);

            if (!HandJointUtils.JointsWithFreeRotation.Contains(jointType))
            {
                newLocal[AxisPreset.RightIndex] = handleLocal[AxisPreset.RightIndex];
            }

            handlePoint = jointMatrix.MultiplyPoint3x4(newLocal);

            Vector3 delta = handlePoint - position;
            Distances[index] = delta.magnitude;

            Quaternion lookAngle = Quaternion.LookRotation(delta.normalized, rotation * AxisPreset.UpVector) *
                                   Quaternion.LookRotation(AxisPreset.ForwardVector, AxisPreset.UpVector);

            return lookAngle;
        }

        private Quaternion DrawDiscHandles(HandJointType jointType, Transform joint)
        {
            Vector3 position = joint.position;
            Quaternion rotation = joint.rotation;

            if (HandJointUtils.JointsWithFreeRotation.Contains(jointType))
            {
                Handles.color = Color.red;
                rotation = Handles.Disc(rotation, position, rotation * AxisPreset.ForwardVector, 0.0075f, false, 0);

                Handles.color = Color.green;
                rotation = Handles.Disc(rotation, position, rotation * AxisPreset.UpVector, 0.0075f, false, 0);

                Handles.color = Color.blue;
                rotation = Handles.Disc(rotation, position, rotation * AxisPreset.RightVector, 0.0075f, false, 0);
            }
            else
            {
                Handles.color = Color.blue;
                rotation = Handles.Disc(rotation, position, rotation * AxisPreset.RightVector, 0.0075f, false, 0);
            }

            return rotation;
        }

        private Transform GetClosestJoint(ref HandJointType jointType)
        {
            while (jointType < HandJointType.Count)
            {
                Transform result = handInstance.Joints[(int)jointType];
                if (result)
                {
                    return result;
                }

                ++jointType;
            }

            return null;
        }

        private void DrawToggleButton(HandJointType jointType, ref bool value)
        {
            Transform root = GetClosestJoint(ref jointType);

            if (!root)
            {
                return;
            }

            using (new Handles.DrawingScope(value ? Color.yellow : Color.green))
            {
                if (Handles.Button(root.position, root.rotation, 0.005f, 0.0025f, Handles.SphereHandleCap))
                {
                    value = !value;
                }
            }
        }
    }
}