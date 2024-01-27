using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TileView_Floor : UI_Base
{
    enum Contents
    {
        TileView_Floor,
    }
    public int FloorID { get; set; }


    public BasementTile CurrentTile;

    public override void Init()
    {
        Bind<GameObject>(typeof(Contents));
        GetObject((int)Contents.TileView_Floor).AddUIEvent((data) => InsteadOpenFloorEvent(data), Define.UIEvent.RightClick);
        //GetObject((int)Contents.TileView_Floor).GetComponent<Image>().color = Color.clear;

        GetObject((int)Contents.TileView_Floor).AddUIEvent((data) => MouseMoveEvent(data), Define.UIEvent.Move);
    }

    private void Start()
    {
        Init();

        ShowTile();
    }

    UI_TileView view;




    bool isClick;


    public void MouseMoveEvent(PointerEventData data)
    {
        if (isClick)
        {
            return;
        }

        if (CurrentTile == null)
        {
            if (view)
            {
                Managers.UI.ClosePopUp(view);
                view = null;
            }
            return;
        }

        if (CurrentTile.placementable == null)
        {
            if (view)
            {
                Managers.UI.ClosePopUp(view);
                view = null;
            }
            return;
        }


        if (view == null)
        {
            view = Managers.UI.ShowPopUpAlone<UI_TileView>();
        }

        var pos = Camera.main.ScreenToWorldPoint(data.position);
        view.transform.localPosition = new Vector3(pos.x, pos.y, 0);
        view.ViewContents($"[{CurrentTile.placementable.Name_KR}]" , $"{CurrentTile.placementable } 자세한 설명은 생략한다.");
    }



    public void SetFloorSize(Vector3 pos, Vector2 sizeXY)
    {
        RectTransform rect = GetComponent<RectTransform>();

        rect.anchoredPosition = pos;
        rect.sizeDelta = sizeXY;
    }



    public void InsteadOpenFloorEvent(PointerEventData data)
    {
        if (Main.Instance.Management)
        {
            StartCoroutine(FrameWait(data));
        }
    }

    IEnumerator FrameWait(PointerEventData data)
    {
        isClick = true;
        var ui = Managers.UI.ClearAndShowPopUp<UI_DungeonPlacement>();
        yield return new WaitForEndOfFrame();
        ui.uI_Floors[FloorID].OpenPlacementType(data);
        yield return new WaitForEndOfFrame();
        CurrentTile = null;
        isClick = false;
    }

    //public void OpenPlacementType(PointerEventData data)
    //{
    //    Main.Instance.CurrentFloor = Main.Instance.Floor[FloorID];

    //    var popup = Managers.UI.ShowPopUpAlone<UI_Placement_TypeSelect>();
    //    var pos = Camera.main.ScreenToWorldPoint(data.position);
    //    popup.transform.localPosition = new Vector3(pos.x, pos.y, 0);

    //    //popup.parents = this;
    //}


    public GameObject[,] TileList { get; set; }

    public void ShowTile()
    {
        if (TileList != null) return;

        TileList = new GameObject[Main.Instance.Floor[FloorID].TileMap.GetLength(0), Main.Instance.Floor[FloorID].TileMap.GetLength(1)];

        for (int i = 0; i < Main.Instance.Floor[FloorID].TileMap.GetLength(0); i++)
        {
            for (int k = 0; k < Main.Instance.Floor[FloorID].TileMap.GetLength(1); k++)
            {
                var content = Managers.Resource.Instantiate("UI/PopUp/Element/TileView_Tile", transform);

                content.GetComponent<RectTransform>().position = Main.Instance.Floor[FloorID].TileMap[i, k].worldPosition;

                //content.GetComponent<Image>().color = Color.clear;

                content.GetComponent<UI_TileView_Tile>().Tile = Main.Instance.Floor[FloorID].TileMap[i, k];

                TileList[i, k] = content;
            }
        }
    }


}
