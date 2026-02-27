using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class USSClassRemover : MonoBehaviour
{
    [SerializeField]
    private UIDocument uiDocument;

    [SerializeField]
    private List<string> classNames;

    private void Awake()
    {
        var root = uiDocument.rootVisualElement;

        foreach (var className in classNames)
        {
            var elementsWithClass = root.Query(className).ToList();

            foreach (var element in elementsWithClass)
            {
                element.RemoveFromClassList(className);
            }
        }
    }
}
