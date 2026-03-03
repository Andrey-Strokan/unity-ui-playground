using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Sets the sorting order parameter of any Renderer component.
/// This parameter is hidden in the inspector for MeshRenderer, though it has crucial
/// value for mesh objects rendering queue adjustments (instead of material queue).
/// Also allows to apply selected value to the renderer in edit mode. This change can be saved
/// in a prefab or a scene.
/// </summary>
public class SetRendererSortingOrder : MonoBehaviour
{
    [SerializeField]
    private int sortingOrder;

    void Awake()
    {
        SetSortingOrderOnRenderers(sortingOrder);
    }

    public void SetSortingOrderOnRenderers(int order)
    {
        foreach(var r in GetComponentsInChildren<Renderer>(true))
        {
            r.sortingOrder = order;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Apply Sorting Order")]
    private void ApplySortingOrder()
    {
        SetSortingOrderOnRenderers(sortingOrder);
        EditorUtility.SetDirty(gameObject);
    }
#endif
}
