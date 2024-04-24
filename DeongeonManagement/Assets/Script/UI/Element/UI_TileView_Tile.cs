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

        gameObject.AddUIEvent((data) => TileMoveEvent(data), Define.UIEvent.Move);
        gameObject.AddUIEvent((data) => TileExitEvent(data), Define.UIEvent.Exit);
        gameObject.AddUIEvent((data) => TileClickEvent(data), Define.UIEvent.LeftClick);
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
        parent.ChildMoveEvent(Tile, _data);

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


}
