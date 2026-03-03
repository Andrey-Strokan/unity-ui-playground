using UnityEditor;

namespace Saritasa.VRToolset.Highlight.Editor
{
    [CustomEditor(typeof(HighlightSystem))]
    public class HighlightSystemDrawer : UnityEditor.Editor
    {
        private SerializedProperty isHighlighted;
        private SerializedProperty highlightProvider;

        private SerializedProperty highlightStarted;
        private SerializedProperty highlightFinished;
        private SerializedProperty unhighlightStarted;
        private SerializedProperty unhighlightFinished;
        private SerializedProperty animationStarted;
        private SerializedProperty animationFinished;

        private bool foldout;

        private void Awake()
        {
            isHighlighted = serializedObject.FindProperty(nameof(isHighlighted));
            highlightProvider = serializedObject.FindProperty(nameof(highlightProvider));

            highlightStarted = serializedObject.FindProperty(nameof(highlightStarted));
            highlightFinished = serializedObject.FindProperty(nameof(highlightFinished));
            unhighlightStarted = serializedObject.FindProperty(nameof(unhighlightStarted));
            unhighlightFinished = serializedObject.FindProperty(nameof(unhighlightFinished));
            animationStarted = serializedObject.FindProperty(nameof(animationStarted));
            animationFinished = serializedObject.FindProperty(nameof(animationFinished));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(isHighlighted);
            EditorGUILayout.PropertyField(highlightProvider);

            foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, "Events");

            if (foldout)
            {
                EditorGUILayout.PropertyField(highlightStarted);
                EditorGUILayout.PropertyField(highlightFinished);
                EditorGUILayout.PropertyField(unhighlightStarted);
                EditorGUILayout.PropertyField(unhighlightFinished);
                EditorGUILayout.PropertyField(animationStarted);
                EditorGUILayout.PropertyField(animationFinished);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}