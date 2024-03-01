using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler, IPointerMoveHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler
{
	public Action<PointerEventData> OnLeftClickHandler = null;
	public Action<PointerEventData> OnRightClickHandler = null;

	public void OnPointerClick(PointerEventData eventData)
	{
		if (OnLeftClickHandler != null && eventData.pointerId == -1)
        {
			OnLeftClickHandler.Invoke(eventData);
			SoundManager.Instance.ReplaceSound("LeftClick");
		}
			
        if (OnRightClickHandler != null && eventData.pointerId == -2)
        {
			OnRightClickHandler.Invoke(eventData);
			SoundManager.Instance.ReplaceSound("RightClick");
		}
	}



	public Action<PointerEventData> OnDragHandler = null;
	public Action<PointerEventData> OnBeginDragHandler = null;
	public Action<PointerEventData> OnEndDragHandler = null;

	public void OnDrag(PointerEventData eventData)
	{
		if (OnDragHandler != null)
			OnDragHandler.Invoke(eventData);
	}
	public void OnBeginDrag(PointerEventData eventData)
	{
		if (OnBeginDragHandler != null)
			OnBeginDragHandler.Invoke(eventData);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (OnEndDragHandler != null)
			OnEndDragHandler.Invoke(eventData);
	}




	public Action<PointerEventData> OnMoveHandler = null;
	public void OnPointerMove(PointerEventData eventData)
    {
		if (OnMoveHandler != null)
			OnMoveHandler.Invoke(eventData);
	}


	public Action<PointerEventData> OnPointerEnterHandler = null;
	public void OnPointerEnter(PointerEventData eventData)
    {
		if (OnPointerEnterHandler != null)
        {
			OnPointerEnterHandler?.Invoke(eventData);
			SoundManager.Instance.ReplaceSound("UI_Enter");
		}
	}


	public Action<PointerEventData> OnPointerExitHandler = null;
	public void OnPointerExit(PointerEventData eventData)
    {
		OnPointerExitHandler?.Invoke(eventData);
	}




	public void ClearAllAction()
	{
		OnLeftClickHandler = null;
		OnRightClickHandler = null;
		OnDragHandler = null;
		OnMoveHandler = null;
		OnPointerEnterHandler = null;
		OnPointerExitHandler = null;
	}


}
