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


    public override void Init()
    {
        Bind<GameObject>(typeof(Contents));
        parent = GetComponentInParent<UI_TileView_Floor>();


        //gameObject.AddUIEvent((data) => MouseMoveEvent(), Define.UIEvent.Move);

        gameObject.AddUIEvent((data) => parent.ChildMoveEvent(Tile, data), Define.UIEvent.Move);
        gameObject.AddUIEvent((data) => parent.ChildExitEvent(), Define.UIEvent.Exit);


        gameObject.AddUIEvent((data) => TileClickEvent(data), Define.UIEvent.LeftClick);
    }



    void TileClickEvent(PointerEventData _data)
    {
        if (Tile.placementable == null)
        {
            parent.InsteadOpenFloorEvent(_data);
        }
        else
        {
            parent.ChildExitEvent();
            Tile.placementable.MouseClickEvent();
        }
    }


}
