using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler, IPointerMoveHandler
{
	public Action<PointerEventData> OnLeftClickHandler = null;
	public Action<PointerEventData> OnRightClickHandler = null;

	public Action<PointerEventData> OnDragHandler = null;
	public Action<PointerEventData> OnMoveHandler = null;

	public void OnPointerClick(PointerEventData eventData)
	{
		if (OnLeftClickHandler != null && eventData.pointerId == -1)
        {
			OnLeftClickHandler.Invoke(eventData);
		}
			
        if (OnRightClickHandler != null && eventData.pointerId == -2)
        {
			OnRightClickHandler.Invoke(eventData);
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (OnDragHandler != null)
			OnDragHandler.Invoke(eventData);
	}

    public void OnPointerMove(PointerEventData eventData)
    {
		if (OnMoveHandler != null)
			OnMoveHandler.Invoke(eventData);
	}
}
