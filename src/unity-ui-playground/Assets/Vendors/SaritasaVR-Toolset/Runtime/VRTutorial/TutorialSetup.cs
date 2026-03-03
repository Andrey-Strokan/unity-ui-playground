using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using System.Text;

/// <summary>
/// This object is responsible for storing and managing tutorial setup.
/// You can import tutorial steps from JSON file through context menu of the object.
/// </summary>
[CreateAssetMenu(fileName = "TutorialSetup", menuName = "Tutorial/Tutorial Setup", order = 1)]
public class TutorialSetup : ScriptableObject
{
    [Tooltip("A namespace of assembly in which action implementations are defined. You can specify your custom assembly.")]
    public string AssemblyName = "Assembly-CSharp";

    [Tooltip("A namespace in which all actions classes are located. Leave it empty to use global namespace.")]
    public string Namespace;

    [Tooltip("This name will be used as a base class for Tutorial Actions when generating script. Leave it empty to derive from TutorialActionBase.")]
    public string ActionBaseClassOverride;

    [SerializeField]
    [Tooltip("A collection of all steps for this tutorial.")]
    private List<TutorialStepData> steps;

    [SerializeField]
    [Tooltip("A collection action classes names for each step of tutorial.")]
    private string[] actionClassesNames = new string[0];

    /// <summary>
    /// Get a collection of all steps for this tutorial.
    /// </summary>
    public IReadOnlyList<TutorialStepData> Steps { get { return steps; } }

    /// <summary>
    /// Get a collection action classes names for each step of tutorial.
    /// </summary>
    public IReadOnlyList<string> ActionClassesNames { get { return actionClassesNames; } }

    /// <summary>
    /// Get a name of assembly in which implementation of tutorial actions are stored.
    /// </summary>
    public string GetAssemblyName()
    {
        return AssemblyName;
    }

#if UNITY_EDITOR
    /// <summary>
    /// This method will check if all steps have implementations of of their action classes.
    /// </summary>
    [ContextMenu("Validate Tutorial")]
    private void ValidateTutorialSteps()
    {
        bool failed = false;
        var assembly = AssemblyUtility.FindAssemblyByName(AssemblyName);

        if (assembly != null)
        {
            foreach (var name in ActionClassesNames)
            {
                Type type = assembly.GetType(name);
                if (type == null)
                {
                    Debug.LogError($"Tutorial step \"{name}\": missing Tutorial Action implementation for \"{name}\" in assembly \"{assembly.GetName().Name}\"");
                    failed = true;
                }
            }
        }

        if (!failed)
        {
            Debug.Log("Tutorial setup is correct");
        }
    }

    /// <summary>
    /// Save current tutorial setup to JSON file.
    /// </summary>
    [ContextMenu("Save As Json")]
    private void SaveAsJson()
    {
        var path = EditorUtility.SaveFilePanel("Save tutorial setup as Json", "", "tutorial.json", "json");

        if (path.Length != 0)
        {
            string json = JsonConvert.SerializeObject(steps, Formatting.Indented, new Newtonsoft.Json.Converters.StringEnumConverter());

            StreamWriter writer = new StreamWriter(path, false);
            writer.Write(json);
            writer.Close();
        }
    }

    /// <summary>
    /// Generate names of action classes for all current tutorial steps.
    /// </summary>
    [ContextMenu("Update Action Classes Signatures")]
    private void UpdateActionClasses()
    {
        actionClassesNames = new string[steps.Count];

        for (int i = 0; i < steps.Count; i++)
        {
            var step = steps[i];
            if (string.IsNullOrEmpty(Namespace))
            {
                actionClassesNames[i] = StringUtility.ToCamelCase(step.StepName);
            }
            else
            {
                actionClassesNames[i] = Namespace + "." + StringUtility.ToCamelCase(step.StepName);
            }
        }
    }

    /// <summary>
    /// Load new tutorial configuration from JSON file.
    /// This will validate loaded steps and generate names for action classes.
    /// </summary>
    [ContextMenu("Load From Json")]
    private void LoadFromJson()
    {
        var loadResult = LoadDataFromJson(out List<TutorialStepData> newData);
        switch (loadResult)
        {
            case LoadDataResult.Succeed:
                steps = newData;
                UpdateActionClasses();
                EditorUtility.SetDirty(this);
                ValidateTutorialSteps();
                break;

            case LoadDataResult.Failed:
                Debug.LogError("Failed to load tutorial setup");
                break;

            case LoadDataResult.Aborted:
                break;

            default:
                Debug.Assert(false);
                break;
        }
    }

    enum LoadDataResult
    {
        Succeed,
        Failed,
        Aborted,
    }

    private LoadDataResult LoadDataFromJson(out List<TutorialStepData> collection)
    {
        collection = null;
        var path = EditorUtility.OpenFilePanel("Load tutorial setup from Json", "", "json");

        if (path.Length != 0)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string text = sr.ReadToEnd();
                    if (text != null)
                    {
                        collection = JsonConvert.DeserializeObject<List<TutorialStepData>>(text);
                        return LoadDataResult.Succeed;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to read tutorial config from file {path}. Exception: {e.Message}");
                return LoadDataResult.Failed;
            }
        }

        return LoadDataResult.Aborted;
    }

    [ContextMenu("Generate Tutorial Script Template")]
    private void GenerateTutoialScriptTemplteFile()
    {
        var path = EditorUtility.SaveFilePanel("Save tutorial script template", "", "Tutorial.cs", "cs");

        if (path.Length != 0)
        {
            string template = GenerateTutorialScriptTemplate();

            StreamWriter writer = new StreamWriter(path, false);
            writer.Write(template);
            writer.Close();
        }
    }

    private string GenerateTutorialScriptTemplate()
    {
        CodeGeneratorUtility generator = new CodeGeneratorUtility();

        generator.AppendLine("using System.Collections;");
        generator.AppendLine("using System;");
        generator.AppendLine("using UnityEngine;");
        generator.AppendLine();

        bool hasNamespace = false;
        if (!string.IsNullOrEmpty(Namespace))
        {
            hasNamespace = true;
            generator.AppendFormat("namespace {0}\n", Namespace);
            generator.AppendLine("{");
            generator.Indent();
        }

        for (int i = 0; i < steps.Count; i++)
        {
            var step = steps[i];

            if (i != 0)
            {
                generator.AppendLine();
            }

            generator.AppendFormat("public class {0} : {1}\n",
                StringUtility.ToCamelCase(step.StepName),
                string.IsNullOrEmpty(ActionBaseClassOverride) ? "TutorialActionBase" :  ActionBaseClassOverride);

            generator.AppendLine("{");
            generator.Indent();

            generator.AppendLine("protected override void Begin() { }");
            generator.AppendLine();

            generator.AppendLine("public override IEnumerator ActionCoroutine()");
            generator.AppendLine("{");
            generator.Indent();
            generator.AppendLine("yield return null;");
            generator.AppendLine("manager.StepForward();");
            generator.Unindent();
            generator.AppendLine("}");
            generator.AppendLine();

            generator.AppendLine("protected override void End() { }");

            generator.Unindent();
            generator.AppendLine("}");
        }

        if (hasNamespace)
        {
            generator.Unindent();
            generator.AppendLine("}");
        }

        return generator.ToString();
    }
#endif
}
