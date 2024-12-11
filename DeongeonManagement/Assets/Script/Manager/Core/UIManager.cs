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


    public void SetCanvas_SubCamera(GameObject go, RenderMode renderMode = RenderMode.ScreenSpaceCamera, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = renderMode;
        canvas.worldCamera = GameObject.FindAnyObjectByType<Camera_SubCam>().GetComponent<Camera>();
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



    public void SceneChange()
    {
        _sceneList.Clear();

        _popupStack.Clear();

        _paused = null;
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
            if (item.isActiveAndEnabled)
            {
                item.Refresh();
            }
        }
    }

    public void SceneUI_Clear()
    {
        _sceneList = new List<UI_Scene>();
    }



    #region Popup

    public Stack<UI_PopUp> _popupStack = new Stack<UI_PopUp>();

    public UI_PopUp _paused;



    public Queue<Action> _reservationQueue = new Queue<Action>();
    Coroutine ReservationCor;

    public void Popup_Reservation(Action action)
    {
        if (_popupStack.Count > 0)
        {
            _reservationQueue.Enqueue(action);
            if (ReservationCor == null)
            {
                ReservationCor = Managers.Instance.StartCoroutine(Show_Reservation());
            }
        }
        else
        {
            action.Invoke();
        }
    }

    IEnumerator Show_Reservation()
    {
        while (_reservationQueue.Count > 0)
        {
            yield return new WaitUntil(() => _popupStack.Count == 0);
            var res = _reservationQueue.Dequeue();
            res.Invoke();
        }

        ReservationCor = null;
        Debug.Log("예약팝업 끝");
    }

    public void Stop_Reservation()
    {
        Managers.Instance.StopCoroutine(ReservationCor);
        ReservationCor = null;
        _reservationQueue.Clear();
    }








    public T ShowPopUpNonPush<T>(string name = null) where T : UI_PopUp
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }
        GameObject go = Managers.Resource.Instantiate($"UI/PopUp/{name}", UI_Root.transform);
        T uiComponent = Util.GetOrAddComponent<T>(go);
        return uiComponent;
    }
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
        uiObject.PopupCloseCallback();
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
    public void ClosePopupPick(UI_PopUp popup)
    {
        if (_popupStack.Contains(popup))
        {
            Stack<UI_PopUp> tempStack = new Stack<UI_PopUp>();

            // 스택에서 해당 팝업을 찾을 때까지 팝업을 임시 스택에 복사
            while (_popupStack.Peek() != popup)
            {
                tempStack.Push(_popupStack.Pop());
            }

            // 해당 팝업을 제거
            UI_PopUp uiObject = _popupStack.Pop();
            uiObject.PopupCloseCallback();
            Managers.Resource.Destroy(uiObject.gameObject);
            uiObject = null;


            // 임시 스택에 있는 나머지 팝업을 다시 원래 스택에 추가
            while (tempStack.Count > 0)
            {
                _popupStack.Push(tempStack.Pop());
            }
        }
    }
    public void ClosePopupPickType(Type uiType)
    {
        foreach (var item in _popupStack)
        {
            if (item.GetType() == uiType)
            {
                ClosePopupPick(item);
                return;
            }
        }
    }




    public void CloseAll()
    {
        while (_popupStack.Count > 0)
        {
            ClosePopUp();
        }
        PauseClose();

        if (Managers.Scene.GetCurrentScene() == SceneName._2_Management)
        {
            SceneUIRefresh();
        }

        if (Time.timeScale == 0 && Managers.FindAnyObjectByType<UI_Stop>() == null)
        {
            UserData.Instance.GamePlay();
        }
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

        _popupStack.Push(_paused);
        for (int i = 0; i < _paused.transform.childCount; i++)
        {
            _paused.transform.GetChild(i).gameObject.SetActive(true);
            _paused.PauseRefresh();
        }

        _paused = null;
    }


    public bool ContainsPopup(Type uiType)
    {
        foreach (var item in _popupStack)
        {
            if (item.GetType() == uiType)
            {
                return true;
            }
        }
        return false;
    }






    #endregion




    #region Hide And Show All UI


    //public List<Canvas> CurrentCanvasList { get; set; }

    //public void HideCanvasAll()
    //{
    //    if (CurrentCanvasList != null)
    //    {
    //        Debug.Log("활성화된 Canvas가 이미 존재함");
    //        return;
    //    }
    //    CurrentCanvasList = new List<Canvas>();
    //    CurrentCanvasList.AddRange(GameObject.FindObjectsOfType<Canvas>());

    //    // 설치하는곳에서 숨기는거랑 겹치는 경우
    //    var main_Can = GameObject.FindAnyObjectByType<UI_Management>().GetComponent<Canvas>();
    //    if (!main_Can.isActiveAndEnabled)
    //    {
    //        CurrentCanvasList.Remove(main_Can);
    //    }

    //    foreach (var item in CurrentCanvasList)
    //    {
    //        item.enabled = false;
    //    }
    //}

    //public void ShowCanvasAll()
    //{
    //    if (CurrentCanvasList == null)
    //    {
    //        Debug.Log("활성화된 Canvas가 없음");
    //        return;
    //    }

    //    if (WaitForShowCanvas_Cor != null)
    //    {
    //        return;
    //    }
    //    else
    //    {
    //        WaitForShowCanvas_Cor = Managers.Instance.StartCoroutine(WaitForShowCanvas());
    //    }
    //}

    //Coroutine WaitForShowCanvas_Cor;

    //IEnumerator WaitForShowCanvas()
    //{
    //    yield return null;
    //    yield return new WaitUntil(() => Managers.Dialogue.GetState() == DialogueManager.DialogueState.None);

    //    foreach (var item in CurrentCanvasList)
    //    {
    //        if (item != null)
    //        {
    //            item.enabled = true;
    //        }
    //    }
    //    CurrentCanvasList = null;
    //    WaitForShowCanvas_Cor = null;
    //}


    #endregion


}
