using UnityEngine;

public class UI_TileView_Tile : UI_Base
{
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
        gameObject.AddUIEvent((data) => MouseMoveEvent(), Define.UIEvent.Move);
        gameObject.AddUIEvent((data) => MouseExit(), Define.UIEvent.Exit);


        gameObject.AddUIEvent((data) => parent.InsteadOpenFloorEvent(data), Define.UIEvent.LeftClick);
    }

    void Start()
    {
        Init();
    }



    void MouseMoveEvent()
    {
        if (Tile.placementable != null)
        {
            parent.CurrentTile = Tile;
        }
        else
        {
            parent.CurrentTile = null;
        }
    }

    void MouseExit()
    {
        parent.CurrentTile = null;
    }
}
