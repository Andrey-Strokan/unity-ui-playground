using System.Globalization;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Sample class that demonstrates how to bind to a UI Toolkit button click event.
/// </summary>
public class TimeViewModel : MonoBehaviour
{
    [SerializeField]
    private UIDocument uiDocument;

    private TimeModel model;
    private Button btn_UpdateTime;

    private void Awake()
    {
        var root = uiDocument.rootVisualElement;

        model = new TimeModel();
        root.dataSource = model;

        btn_UpdateTime = root.Query<VisualElement>("Modal").First().Q<Button>();

        btn_UpdateTime.clicked += OnClicked;
    }

    private void OnDestroy()
    {
        btn_UpdateTime.clicked -= OnClicked;
    }

    private void OnClicked()
    {
        model.UpdateTime();
    }
}

/// <summary>
/// Modal model class that contains the data to be displayed in the UI.
/// </summary>
public class TimeModel
{
    [CreateProperty]
    private string time;

    /// <summary>
    /// Updates the time property with the current time.
    /// </summary>
    public void UpdateTime()
    {
        time = Time.time.ToString(CultureInfo.InvariantCulture);
    }
}
