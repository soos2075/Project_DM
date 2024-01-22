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
        gameObject.AddUIEvent((data) => ActionCheckEvent(), Define.UIEvent.LeftClick);

        gameObject.AddUIEvent((data) => Managers.UI.PauseOpen(), Define.UIEvent.RightClick);
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

    void TileCheckEvent(Vector2Int[] boundary)
    {
        if (Main.Instance.CurrentAction == null) return;
        if (parent.TileList == null) return;
        if (Tile == null) return;
        if (boundary == null) return;

        parent.TileUpdate();

        bool allClean = true;

        foreach (var item in boundary)
        {
            int deltaX = Tile.index.x + item.x;
            int deltaY = Tile.index.y + item.y;

            if (deltaX < 0 || deltaX >= parent.TileList.GetLength(0))
            {
                allClean = false;
                break;
            }
            if (deltaY < 0 || deltaY >= parent.TileList.GetLength(1))
            {
                allClean = false;
                break;
            }

            allClean &= TileCheck(Main.Instance.CurrentFloor.TileMap[deltaX, deltaY]);
        }

        if (allClean)
        {
            //? 실제 타일 변경 And 시설컨펌 준비
            foreach (var item in boundary)
            {
                int _deltaX = Tile.index.x + item.x;
                int _deltaY = Tile.index.y + item.y;

                var content = parent.TileList[_deltaX, _deltaY];

                content.GetComponent<Image>().color = Define.Color_Green;
            }

            Main.Instance.CurrentTile = Tile;
        }
        else
        {
            Main.Instance.CurrentTile = null;
        }
    }


    bool TileCheck(BasementTile tile)
    {
        if (tile.tileType == Define.TileType.Empty)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


}
