using System;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

public class PaginationViewModel : MonoBehaviour
{
    [SerializeField]
    private UIDocument uiDocument;

    [SerializeField]
    private PaginationModel paginationModel;

    private CustomButton btn_Next;
    private CustomButton btn_Previous;

    private void Awake()
    {
        var root = uiDocument.rootVisualElement;
        root.dataSource = paginationModel;

        btn_Next = root.Query("btn_Next").Children<CustomButton>().First();
        btn_Previous = root.Query("btn_Previous").Children<CustomButton>().First();

        btn_Next.clicked += paginationModel.MoveNext;
        btn_Previous.clicked += paginationModel.MoveBack;
    }

    private void OnDestroy()
    {
        btn_Next.clicked -= paginationModel.MoveNext;
        btn_Previous.clicked -= paginationModel.MoveBack;
    }
}

[Serializable]
public class PaginationModel
{
    [SerializeField]
    private Page[] Pages;

    [CreateProperty]
    public Page CurrentPage => Pages[pageIndex];

    [CreateProperty]
    public string PageIndexText => $"{pageIndex + 1} / {Pages.Length}";

    private int pageIndex = 0;

    public PaginationModel(Page[] pages)
    {
        Pages = pages;
    }

    public void MoveNext()
    {
        if (pageIndex < Pages.Length - 1)
        {
            pageIndex++;
        }
        else
        {
            pageIndex = 0;
        }
    }

    public void MoveBack()
    {
        if (pageIndex > 0)
        {
            pageIndex--;
        }
        else
        {
            pageIndex = Pages.Length - 1;
        }
    }
}

[Serializable]
public class Page
{
    public string Title;
    public string Subtitle;

    [TextArea]
    public string Content;

    public Sprite Image;
}
