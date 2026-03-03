using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

namespace Saritasa.VRToolset.Tooltips.Editor
{
    /// <summary>
    /// Custom inspector to order tooltip's properties in inspector. This is only for comfortability and can be safely removed.
    /// </summary>
    [CustomEditor(typeof(TooltipBase), true)]
    [CanEditMultipleObjects]
    public class CustomInspectorAnimatedTooltips : UnityEditor.Editor
    {
        private const string StaticRefsFoldoutLabel = "Static References";
        private const string MeshRendererRefsFoldoutLabel = "Mesh renderer References";

        private bool foldOutRefsShown = false;
        private bool foldOutMeshRendererRefsShown = false;

        // Static Refs.
        private SerializedProperty txt_Front;
        private SerializedProperty txt_Back;
        private SerializedProperty container;
        private SerializedProperty containerRoot;
        private SerializedProperty background;
        private SerializedProperty lineParent;
        private SerializedProperty endPointParent;
        private SerializedProperty startPointParent;

        // Mesh Renderer Refs.
        private SerializedProperty Txt_Front_MR;
        private SerializedProperty Txt_Back_MR;
        private SerializedProperty lineRenderer;
        private SerializedProperty endPointRenderer;
        private SerializedProperty startPointRenderer;
        private SerializedProperty backgroundMeshFilter;
        private SerializedProperty backgroundMeshRenderer;

        private Dictionary<string, SerializedProperty> definedProperties = new Dictionary<string, SerializedProperty>();

        private void FindAndCacheProperty(string propertyName, out SerializedProperty storageVariable)
        {
            storageVariable = serializedObject.FindProperty(propertyName);
            if (storageVariable != null)
            {
                definedProperties.Add(propertyName, storageVariable);
            }
        }

        private void DrawValidProperty(SerializedProperty serializedProperty)
        {
            if (serializedProperty != null)
            {
                EditorGUILayout.PropertyField(serializedProperty);
            }
        }

        private void Awake()
        {
            // Static Refs.
            FindAndCacheProperty(nameof(txt_Front), out txt_Front);
            FindAndCacheProperty(nameof(txt_Back), out txt_Back);
            FindAndCacheProperty(nameof(container), out container);
            FindAndCacheProperty(nameof(containerRoot), out containerRoot);
            FindAndCacheProperty(nameof(background), out background);
            FindAndCacheProperty(nameof(lineParent), out lineParent);
            FindAndCacheProperty(nameof(endPointParent), out endPointParent);
            FindAndCacheProperty(nameof(startPointParent), out startPointParent);

            // Mesh Renderers.
            FindAndCacheProperty(nameof(Txt_Front_MR), out Txt_Front_MR);
            FindAndCacheProperty(nameof(Txt_Back_MR), out Txt_Back_MR);
            FindAndCacheProperty(nameof(lineRenderer), out lineRenderer);
            FindAndCacheProperty(nameof(endPointRenderer), out endPointRenderer);
            FindAndCacheProperty(nameof(startPointRenderer), out startPointRenderer);
            FindAndCacheProperty(nameof(backgroundMeshFilter), out backgroundMeshFilter);
            FindAndCacheProperty(nameof(backgroundMeshRenderer), out backgroundMeshRenderer);

            SceneView.duringSceneGui += OnDuringSceneGui;
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= OnDuringSceneGui;
        }

        private void OnDuringSceneGui(SceneView obj)
        {
            var tooltipWithLines = target as TooltipWithLines;

            if (!Application.isPlaying && tooltipWithLines != null)
            {
                tooltipWithLines.UpdateTooltipVisuals();
            }
        }

        public override void OnInspectorGUI()
        {
            DrawNotCachedProperties();

            foldOutRefsShown = EditorGUILayout.Foldout(foldOutRefsShown, StaticRefsFoldoutLabel);
            if (foldOutRefsShown)
            {
                EditorGUI.indentLevel++;
                DrawValidProperty(txt_Front);
                DrawValidProperty(txt_Back);
                DrawValidProperty(container);
                DrawValidProperty(containerRoot);
                DrawValidProperty(background);
                DrawValidProperty(lineParent);
                DrawValidProperty(endPointParent);
                DrawValidProperty(startPointParent);
                EditorGUI.indentLevel--;
            }

            foldOutMeshRendererRefsShown = EditorGUILayout.Foldout(foldOutMeshRendererRefsShown, MeshRendererRefsFoldoutLabel);
            if (foldOutMeshRendererRefsShown)
            {
                EditorGUI.indentLevel++;
                DrawValidProperty(backgroundMeshFilter);
                DrawValidProperty(Txt_Front_MR);
                DrawValidProperty(Txt_Back_MR);
                DrawValidProperty(lineRenderer);
                DrawValidProperty(endPointRenderer);
                DrawValidProperty(startPointRenderer);
                DrawValidProperty(backgroundMeshRenderer);
                EditorGUI.indentLevel--;
            }

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawNotCachedProperties()
        {
            var serializedProperties = new List<SerializedProperty>();

            var property = serializedObject.GetIterator();
            while (property.NextVisible(true))
            {
                if (definedProperties.ContainsKey(property.name) || property.name == "m_Script")
                {
                    continue;
                }
                serializedProperties.Add(property.Copy());
            }

            if (serializedProperties.Count <= 0)
                return;

            foreach (var serializedProperty in serializedProperties)
            {
                EditorGUILayout.PropertyField(serializedProperty);
            }
        }
    }
}