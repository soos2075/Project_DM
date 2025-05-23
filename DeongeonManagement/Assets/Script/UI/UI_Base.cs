﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UI_Base : MonoBehaviour
{
	protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();
	public abstract void Init();

	protected void Bind<T>(Type type) where T : UnityEngine.Object
	{
		string[] names = Enum.GetNames(type);
		UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
		_objects.Add(typeof(T), objects);

		for (int i = 0; i < names.Length; i++)
		{
			if (typeof(T) == typeof(GameObject))
				objects[i] = Util.FindChild(gameObject, names[i], true);
			else
				objects[i] = Util.FindChild<T>(gameObject, names[i], true);

			if (objects[i] == null)
				Debug.Log($"Failed to bind({names[i]})");
		}
	}

	protected T Get<T>(int idx) where T : UnityEngine.Object
	{
		UnityEngine.Object[] objects = null;
		if (_objects.TryGetValue(typeof(T), out objects) == false)
			return null;

		return objects[idx] as T;
	}

	protected GameObject GetObject(int idx) { return Get<GameObject>(idx); }
	//protected Text GetText(int idx) { return Get<Text>(idx); }
	protected TextMeshProUGUI GetTMP(int idx) { return Get<TextMeshProUGUI>(idx); }
	protected Button GetButton(int idx) { return Get<Button>(idx); }
	protected Image GetImage(int idx) { return Get<Image>(idx); }

	protected int GetCount(Type type)
	{
		UnityEngine.Object[] objects = null;
        if (_objects.TryGetValue(type, out objects) == false)
        {
			return 0;
        }
		return objects.Length;
	}



    public static void AddUIEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.LeftClick)
    {
        UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

        switch (type)
        {
            case Define.UIEvent.LeftClick:
                evt.OnLeftClickHandler -= action;
                evt.OnLeftClickHandler += action;
                break;

			case Define.UIEvent.RightClick:
				evt.OnRightClickHandler -= action;
				evt.OnRightClickHandler += action;
				break;

			case Define.UIEvent.Drag:
                evt.OnDragHandler -= action;
                evt.OnDragHandler += action;
                break;

			case Define.UIEvent.BeginDrag:
				evt.OnBeginDragHandler -= action;
				evt.OnBeginDragHandler += action;
				break;

			case Define.UIEvent.EndDrag:
				evt.OnEndDragHandler -= action;
				evt.OnEndDragHandler += action;
				break;

			case Define.UIEvent.Move:
				evt.OnMoveHandler -= action;
				evt.OnMoveHandler += action;
				break;

			case Define.UIEvent.Enter:
				evt.OnPointerEnterHandler -= action;
				evt.OnPointerEnterHandler += action;
				break;
			case Define.UIEvent.Exit:
				evt.OnPointerExitHandler -= action;
				evt.OnPointerExitHandler += action;
				break;


			case Define.UIEvent.Down:
				evt.OnPointerDownHandler -= action;
				evt.OnPointerDownHandler += action;
				break;
			case Define.UIEvent.Up:
				evt.OnPointerUpHandler -= action;
				evt.OnPointerUpHandler += action;
				break;
		}
    }


	public static void RemoveUIEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.LeftClick)
    {
		UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

		evt.OnLeftClickHandler -= action;
	}
	public static void RemoveUIEventAll(GameObject go)
    {
		UI_EventHandler evt = go.GetComponent<UI_EventHandler>();

		evt?.ClearAllAction();
	}


	public event Action CallbackAction;
	public void InteractionCallback(bool isOnlyOnetiem)
    {
        if (CallbackAction != null)
        {
			CallbackAction.Invoke();
		}

        if (isOnlyOnetiem)
        {
			CallbackAction = null;
		}
    }



	#region AddNotice
	public void AddNotice_UI(string label, UI_Base parent, string findName, string setBoolName)
	{
		UserData.Instance.FileConfig.SetBoolValue(setBoolName, true);
		StartCoroutine(WaitFrame(label, parent, findName, setBoolName));
	}
	IEnumerator WaitFrame(string label, UI_Base parent, string findName, string setBoolName)
	{
		yield return null;

		var obj = Util.FindChild(parent.gameObject, findName, true);

		var overlay = Managers.Resource.Instantiate("UI/PopUp/Element/OverlayImage", obj.transform);
		var ui = overlay.GetComponent<UI_Overlay>();
		ui.SetOverlay(Managers.Sprite.Get_SLA(SpriteManager.Library.UI, "Overlay_Icon", label), obj, setBoolName);
	}

	public void AddNotice_NoClear(string label, UI_Base parent, string findName, string setBoolName)
	{
		UserData.Instance.FileConfig.SetBoolValue(setBoolName, true);
		StartCoroutine(WaitFrame_NoClear(label, parent, findName));
	}
	IEnumerator WaitFrame_NoClear(string label, UI_Base parent, string findName)
	{
		yield return null;

		var obj = Util.FindChild(parent.gameObject, findName, true);

		var overlay = Managers.Resource.Instantiate("UI/PopUp/Element/OverlayImage", obj.transform);
		var ui = overlay.GetComponent<UI_Overlay>();
		ui.SetOverlay_DontDest(Managers.Sprite.Get_SLA(SpriteManager.Library.UI, "Overlay_Icon", label), obj);
	}

	public void RemoveNotice_UI(GameObject btn)
	{
		var overlay = btn.GetComponentInChildren<UI_Overlay>();
		if (overlay != null)
		{
			overlay.SelfDestroy();
		}
	}
	#endregion
}
