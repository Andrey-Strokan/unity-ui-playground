using System;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// This sample class demonstrates how to bind to a UI Toolkit toggle value change event
/// and update the theme of the UI accordingly.
/// </summary>
public class ThemeViewModel : MonoBehaviour
{
    [SerializeField]
    private UIDocument uiDocument;

    [SerializeField]
    private ThemeModel model;

    private Toggle tgl_SwitchTheme;

    private void Awake()
    {
        var root = uiDocument.rootVisualElement;

        tgl_SwitchTheme = root.Query("ThemeSwitch").First().Q<Toggle>();
        tgl_SwitchTheme.RegisterValueChangedCallback(OnValueChanged);
        SwitchTheme(tgl_SwitchTheme.value);
    }

    private void OnDestroy()
    {
        tgl_SwitchTheme.UnregisterValueChangedCallback(OnValueChanged);
    }

    private void OnValueChanged(ChangeEvent<bool> evt)
    {
        SwitchTheme(evt.newValue);
    }

    private void SwitchTheme(bool isDark)
    {
        var theme = isDark ? ThemeModel.ThemeVariant.Dark : ThemeModel.ThemeVariant.Light;
        model.SetTheme(theme);

        tgl_SwitchTheme.text = theme.ToString();
    }
}

/// <summary>
/// The model class that contains selected theme data.
/// </summary>
[Serializable]
public class ThemeModel
{
    public enum ThemeVariant
    {
        Light,
        Dark
    }

    [CreateProperty]
    public ThemeVariant Theme { get; private set; }

    [CreateProperty]
    public string ThemeVariantName => Theme.ToString();

    [SerializeField]
    [DontCreateProperty]
    private PanelSettings[] panelSettings = Array.Empty<PanelSettings>();

    [SerializeField]
    [DontCreateProperty]
    private ThemeStyleSheet lightThemeStyleSheet;

    [SerializeField]
    [DontCreateProperty]
    private ThemeStyleSheet darkThemeStyleSheet;

    public void SetTheme(ThemeVariant newTheme)
    {
        var styleSheet = newTheme switch
        {
            ThemeVariant.Light => lightThemeStyleSheet,
            ThemeVariant.Dark => darkThemeStyleSheet,
            _ => throw new ArgumentOutOfRangeException(nameof(newTheme), newTheme, null)
        };

        Theme = newTheme;

        foreach (var panelSettings in panelSettings)
        {
            panelSettings.themeStyleSheet = styleSheet;
        }
    }
}
