using UnityEditor;
using UnityEditor.XR.Interaction.Toolkit.Locomotion;
using SaritasaVRToolset;

/// <summary>
/// Custom editor for an <see cref="CustomXRBodyTransformer"/>.
/// </summary>
[CustomEditor(typeof(CustomXRBodyTransformer), true)]
public class CustomXRBodyTransformerEditor : XRBodyTransformerEditor
{
    /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="CustomXRBodyTransformer.headCollisionHandler"/>.</summary>
    protected SerializedProperty headCollisionHandler;

    /// <inheritdoc/>
    protected new void OnEnable()
    {
        headCollisionHandler = serializedObject.FindProperty("headCollisionHandler");

        base.OnEnable();
    }

    /// <inheritdoc/>
    protected override void DrawInspector()
    {
        EditorGUILayout.PropertyField(headCollisionHandler);

        base.DrawInspector();
    }
}
