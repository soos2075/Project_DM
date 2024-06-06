using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_TileView_Tile : UI_Base
{
    void Start()
    {
        Init();
    }


    enum Contents
    {
        TileView_Tile,
    }
    public BasementTile Tile { get; set; }
    UI_TileView_Floor parent;


    IPlacementable CurrentTempData { get; set; }



    public override void Init()
    {
        Bind<GameObject>(typeof(Contents));
        parent = GetComponentInParent<UI_TileView_Floor>();

        gameObject.AddUIEvent((data) => TileMoveEvent(data), Define.UIEvent.Move);
        gameObject.AddUIEvent((data) => TileExitEvent(data), Define.UIEvent.Exit);
        gameObject.AddUIEvent((data) => TileClickEvent(data), Define.UIEvent.LeftClick);

        gameObject.AddUIEvent((data) => TileDownEvent(data), Define.UIEvent.Down);
        gameObject.AddUIEvent((data) => TileUpEvent(data), Define.UIEvent.Up);
    }



    void TileClickEvent(PointerEventData _data)
    {
        if (Tile.Original == null)
        {
            parent.InsteadOpenFloorEvent(_data);
        }
        else
        {
            parent.ChildExitEvent();
            Tile.Original.MouseClickEvent();
        }
    }

    void TileMoveEvent(PointerEventData _data)
    {
        if (isDownEvent) return;
        if (Managers.UI._popupStack.Count > 0) return;

        //Debug.Log(Managers.UI._popupStack.Count);


        if (CurrentTempData != null)
        {
            if (CurrentTempData.GetObject().activeInHierarchy)
            {
                parent.ChildMoveEvent_CurrentData(CurrentTempData);
            }
            else
            {
                CurrentTempData = null;
            }
        }
        else
        {
            parent.ChildMoveEvent(Tile, _data);
            if (Tile.Current != null)
            {
                if (Tile.Current.PlacementType == PlacementType.Monster || Tile.Current.PlacementType == PlacementType.NPC)
                {
                    CurrentTempData = Tile.Current;
                    StartCoroutine(TempData());
                }
            }
        }

        if (Tile.Original != null)
        {
            Tile.Original.MouseMoveEvent();
        }
    }


    void TileExitEvent(PointerEventData _data)
    {
        parent.ChildExitEvent();

        if (Tile.Original != null)
        {
            Tile.Original.MouseExitEvent();
        }
    }



    IEnumerator TempData()
    {
        yield return new WaitForSeconds(2.0f);
        CurrentTempData = null;
    }



    bool isDownEvent;

    void TileDownEvent(PointerEventData _data)
    {
        if (Tile.Original != null)
        {
            Tile.Original.MouseDownEvent();
            isDownEvent = true;
        }
    }
    void TileUpEvent(PointerEventData _data)
    {
        if (Tile.Original != null)
        {
            Tile.Original.MouseUpEvent();
            isDownEvent = false;
        }
    }

}
