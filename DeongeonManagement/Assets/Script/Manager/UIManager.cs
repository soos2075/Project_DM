using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    public GameObject UI_Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
            {
                root = new GameObject { name = "@UI_Root" };
            }
            return root;
        }
    }

    int _sortOrder = 10;

    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _sortOrder;
            _sortOrder++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }


    public T ShowSceneUI<T>(string name)
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
        T sceneUI = go.GetComponent<T>();
        go.transform.SetParent(UI_Root.transform);

        return sceneUI;
    }




    #region Popup

    Stack<UI_PopUp> _popupStack = new Stack<UI_PopUp>();

    public T ShowPopUp<T>(string name = null) where T : UI_PopUp
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }
        GameObject go = Managers.Resource.Instantiate($"UI/PopUp/{name}", UI_Root.transform);
        T uiComponent = Util.GetOrAddComponent<T>(go);
        _popupStack.Push(uiComponent);

        return uiComponent;
    }

    public T ShowPopUp<T>(string name, bool setCanvasOrder) where T : UI_PopUp
    {
        var uiComponent = ShowPopUp<T>(name);
        if (setCanvasOrder)
        {
            SetCanvas(uiComponent.gameObject, true);
        }
        return uiComponent;
    }

    public T ShowPopUp<T>(Transform parents, string name = null) where T : UI_PopUp
    {
        T component = ShowPopUp<T>(name);
        component.transform.SetParent(parents);
        return component;
    }

    public void ClosePopUp()
    {
        if (_popupStack.Count == 0)
        {
            return;
        }

        UI_PopUp uiObject = _popupStack.Pop();
        Managers.Resource.Destroy(uiObject.gameObject);
        uiObject = null;
        //_sortOrder--;
    }
    public void ClosePopUp(UI_PopUp popup)
    {
        if (_popupStack.Count == 0)
        {
            return;
        }

        if (_popupStack.Peek() != popup)
        {
            Debug.Log($"Close Popup Failed : {popup.name}");
            return;
        }

        ClosePopUp();
    }


    public void CloseAll()
    {
        while (_popupStack.Count > 0)
        {
            ClosePopUp();
        }
    }

    #endregion



}
