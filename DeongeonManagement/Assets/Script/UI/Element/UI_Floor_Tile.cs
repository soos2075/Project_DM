using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Floor_Tile : UI_Base
{

    enum Contents
    {
        Floor_Tile,
    }
    public BasementTile Tile { get; set; }


    UI_Floor parent;


    public override void Init()
    {
        Bind<GameObject>(typeof(Contents));
        parent = GetComponentInParent<UI_Floor>();

        gameObject.AddUIEvent((data) => TileCheckEvent(Main.Instance.CurrentBoundary), Define.UIEvent.Move);
        gameObject.AddUIEvent((data) => GetComponent<Image>().color = Define.Color_White, Define.UIEvent.Exit);

        gameObject.AddUIEvent((data) => ActionCheckEvent(), Define.UIEvent.LeftClick);

        gameObject.AddUIEvent((data) => ActionReturnEvent(), Define.UIEvent.RightClick);
    }



    void Start()
    {
        Init();
    }



    void ActionCheckEvent()
    {
        if (Main.Instance.CurrentAction != null)
            Main.Instance.CurrentAction.Invoke();
    }

    void ActionReturnEvent()
    {
        Main.Instance.CurrentBoundary = null;
        Main.Instance.CurrentAction = null;
        Main.Instance.CurrentTile = null;
        Main.Instance.PurchaseAction = null;
        Managers.UI.ClosePopupPick(GameObject.FindAnyObjectByType<UI_DungeonPlacement>());
        Managers.UI.PauseOpen();
        Time.timeScale = 0;
    }



    void TileCheckEvent(Vector2Int[] boundary)
    {
        if (Main.Instance.CurrentAction == null) return;
        if (parent.TileList == null) return;
        if (Tile == null) return;
        if (boundary == null) return;

        parent.TileUpdate();

        bool allClean = true;
        bool allEmpty = false;

        foreach (var item in boundary)
        {
            Vector2Int delta = Tile.index + item;

            BasementTile tile = null;
            if (Main.Instance.Floor[parent.FloorID].TileMap.TryGetValue(delta, out tile) == false)
            {
                allClean = false;
                break;
            }

            switch (parent.Mode)
            {
                case UI_Floor.BuildMode.Build:
                    allClean &= TileCheck(tile, Define.TileType.Empty);
                    break;
                case UI_Floor.BuildMode.Clear:
                    allEmpty |= TileCheck(tile, Define.TileType.Facility, Define.TileType.Entrance, Define.TileType.Exit);
                    break;
            }
        }

        if (allClean)
        {
            //? 실제 타일 변경 And 시설컨펌 준비
            switch (parent.Mode)
            {
                case UI_Floor.BuildMode.Build:
                    foreach (var item in boundary)
                    {
                        int _deltaX = Tile.index.x + item.x;
                        int _deltaY = Tile.index.y + item.y;

                        var content = parent.TileList[_deltaX, _deltaY];

                        content.GetComponent<Image>().color = Define.Color_Green;
                    }

                    Main.Instance.CurrentTile = Tile;
                    return;

                case UI_Floor.BuildMode.Clear:
                    foreach (var item in boundary)
                    {
                        int _deltaX = Tile.index.x + item.x;
                        int _deltaY = Tile.index.y + item.y;

                        var content = parent.TileList[_deltaX, _deltaY];
                        BasementTile tile = null;
                        if (Main.Instance.Floor[parent.FloorID].TileMap.TryGetValue(new Vector2Int(_deltaX, _deltaY), out tile))
                        {
                            if (TileCheck(tile, Define.TileType.Facility, Define.TileType.Entrance, Define.TileType.Exit))
                            {
                                content.GetComponent<Image>().color = Define.Color_Green;
                            }
                            else
                            {
                                content.GetComponent<Image>().color = Define.Color_Yellow;
                            }
                        }
                    }


                    if (allEmpty)
                    {
                        Main.Instance.CurrentTile = Tile;
                        return;
                    }
                    else
                    {
                        Main.Instance.CurrentTile = null;
                        return;
                    }
            }

        }
        else
        {
            Main.Instance.CurrentTile = null;
        }
    }
    //void TileCheckEvent(Vector2Int[] boundary)
    //{
    //    if (Main.Instance.CurrentAction == null) return;
    //    if (parent.TileList == null) return;
    //    if (Tile == null) return;
    //    if (boundary == null) return;

    //    parent.TileUpdate();

    //    bool allClean = true;
    //    bool allEmpty = false;

    //    foreach (var item in boundary)
    //    {
    //        int deltaX = Tile.index.x + item.x;
    //        int deltaY = Tile.index.y + item.y;

    //        if (deltaX < 0 || deltaX >= parent.TileList.GetLength(0))
    //        {
    //            allClean = false;
    //            break;
    //        }
    //        if (deltaY < 0 || deltaY >= parent.TileList.GetLength(1))
    //        {
    //            allClean = false;
    //            break;
    //        }

    //        switch (parent.Mode)
    //        {
    //            case UI_Floor.BuildMode.Build:
    //                allClean &= TileCheck(Main.Instance.Floor[parent.FloorID].TileMap[deltaX, deltaY], Define.TileType.Empty);
    //                break;
    //            case UI_Floor.BuildMode.Clear:
    //                //allClean &= TileCheck_Clear(Main.Instance.CurrentFloor.TileMap[deltaX, deltaY]);
    //                allEmpty |= TileCheck(Main.Instance.Floor[parent.FloorID].TileMap[deltaX, deltaY], 
    //                    Define.TileType.Facility, Define.TileType.Entrance, Define.TileType.Exit);
    //                break;
    //        }
    //    }

    //    if (allClean)
    //    {
    //        //? 실제 타일 변경 And 시설컨펌 준비
    //        switch (parent.Mode)
    //        {
    //            case UI_Floor.BuildMode.Build:
    //                foreach (var item in boundary)
    //                {
    //                    int _deltaX = Tile.index.x + item.x;
    //                    int _deltaY = Tile.index.y + item.y;

    //                    var content = parent.TileList[_deltaX, _deltaY];

    //                    content.GetComponent<Image>().color = Define.Color_Green;
    //                }

    //                Main.Instance.CurrentTile = Tile;
    //                return;

    //            case UI_Floor.BuildMode.Clear:
    //                foreach (var item in boundary)
    //                {
    //                    int _deltaX = Tile.index.x + item.x;
    //                    int _deltaY = Tile.index.y + item.y;

    //                    var content = parent.TileList[_deltaX, _deltaY];

    //                    if (TileCheck(Main.Instance.Floor[parent.FloorID].TileMap[_deltaX, _deltaY],
    //                        Define.TileType.Facility, Define.TileType.Entrance, Define.TileType.Exit))
    //                    {
    //                        content.GetComponent<Image>().color = Define.Color_Green;
    //                    }
    //                    else
    //                    {
    //                        content.GetComponent<Image>().color = Define.Color_Yellow;
    //                    }
    //                }
    //                if (allEmpty)
    //                {
    //                    Main.Instance.CurrentTile = Tile;
    //                    return;
    //                }
    //                else
    //                {
    //                    Main.Instance.CurrentTile = null;
    //                    return;
    //                }
    //        }

    //    }
    //    else
    //    {
    //        Main.Instance.CurrentTile = null;
    //    }
    //}


    bool TileCheck(BasementTile tile, Define.TileType type)
    {
        if (tile.tileType == type)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool TileCheck(BasementTile tile, Define.TileType type, Define.TileType type2, Define.TileType type3)
    {
        if (tile.tileType == type || tile.tileType == type2 || tile.tileType == type3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
