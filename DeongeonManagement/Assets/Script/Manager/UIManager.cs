using System;
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

    public void SetCanvas(GameObject go, RenderMode renderMode = RenderMode.ScreenSpaceOverlay, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = renderMode;
        if (renderMode != RenderMode.ScreenSpaceOverlay)
        {
            canvas.worldCamera = Camera.main;
        }

        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _sortOrder;
            _sortOrder++;
        }
        else
        {
            //canvas.sortingOrder = 0;
        }
    }


    public T ShowSceneUI<T>(string name)
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
        T sceneUI = go.GetComponent<T>();
        go.transform.SetParent(UI_Root.transform);

        _sceneList.Add(sceneUI as UI_Scene);
        return sceneUI;
    }

    List<UI_Scene> _sceneList = new List<UI_Scene>();

    void SceneUIRefresh()
    {
        foreach (var item in _sceneList)
        {
            item.Refresh();
        }
    }




    #region Popup

    public Stack<UI_PopUp> _popupStack = new Stack<UI_PopUp>();

    public UI_PopUp _paused;



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
            SetCanvas(uiComponent.gameObject);
        }
        return uiComponent;
    }

    public T ShowPopUp<T>(Transform parents, string name = null) where T : UI_PopUp
    {
        T component = ShowPopUp<T>(name);
        component.transform.SetParent(parents);
        return component;
    }

    public T ClearAndShowPopUp<T>(string name = null) where T : UI_PopUp
    {
        CloseAll();
        return ShowPopUp<T>(name);
    }

    public T ShowPopUpAlone<T>(string name = null) where T : UI_PopUp
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }
        GameObject go = Managers.Resource.Instantiate($"UI/PopUp/{name}", UI_Root.transform);
        T uiComponent = Util.GetOrAddComponent<T>(go);

        if (_popupStack.Count > 0 && _popupStack.Peek().GetType() == uiComponent.GetType())
        {
            ClosePopUp();
        }

        _popupStack.Push(uiComponent);

        return uiComponent;
    }
    public T ShowPopUpAlone<T>(Transform parents, string name = null) where T : UI_PopUp
    {
        T component = ShowPopUpAlone<T>(name);
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
        PauseClose();

        //SceneUIRefresh();
    }


    public void PausePopUp()
    {
        if (_popupStack.Count == 0)
        {
            return;
        }

        _paused = _popupStack.Pop();
        for (int i = 0; i < _paused.transform.childCount; i++)
        {
            _paused.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    public void PausePopUp(UI_PopUp popup)
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

        _paused = _popupStack.Pop();
        for (int i = 0; i < _paused.transform.childCount; i++)
        {
            _paused.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void PauseClose()
    {
        if (_paused == null) return;

        Managers.Resource.Destroy(_paused.gameObject);
        _paused = null;
    }

    public void PauseOpen()
    {
        if (_paused == null) return;

        for (int i = 0; i < _paused.transform.childCount; i++)
        {
            _paused.transform.GetChild(i).gameObject.SetActive(true);
        }

        _popupStack.Push(_paused);
        _paused = null;
    }



    #endregion



}
