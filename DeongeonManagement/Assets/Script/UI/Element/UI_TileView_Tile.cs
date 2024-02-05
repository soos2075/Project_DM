using UnityEngine;

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


        gameObject.AddUIEvent((data) => parent.InsteadOpenFloorEvent(data), Define.UIEvent.LeftClick);
    }




    //void MouseMoveEvent()
    //{
    //    Debug.Log("자식 무브");

    //    if (Tile.placementable != null)
    //    {
    //        parent.CurrentTile = Tile;
    //    }
    //    else
    //    {
    //        parent.CurrentTile = null;
    //    }
    //}

    //void MouseExit()
    //{
    //    parent.CurrentTile = null;
    //}
}
