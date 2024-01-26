using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler, IPointerMoveHandler, IPointerEnterHandler, IPointerExitHandler
{
	public Action<PointerEventData> OnLeftClickHandler = null;
	public Action<PointerEventData> OnRightClickHandler = null;

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



	public Action<PointerEventData> OnDragHandler = null;

	public void OnDrag(PointerEventData eventData)
	{
		if (OnDragHandler != null)
			OnDragHandler.Invoke(eventData);
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
		OnPointerEnterHandler?.Invoke(eventData);
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
