using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TileView_Floor : UI_Scene, IWorldSpaceUI
{
    private void Start()
    {
        Init();
    }

    private void LateUpdate()
    {
        if (CurrentTile == null && view != null)
        {
            if (view)
            {
                Managers.UI.ClosePopUp(view);
                view = null;
            }
        }
    }

    public void SetCanvasWorldSpace()
    {
        Managers.UI.SetCanvas(gameObject, RenderMode.WorldSpace, false);
    }
    public override void Refresh()
    {
        ChildColorChange(Color.clear);
    }

    public int FloorID { get; set; }

    public override void Init()
    {
        SetCanvasWorldSpace();
        ShowTile();
    }


    List<UI_TileView_Tile> childList;
    UI_TileView view;
    public BasementTile CurrentTile;

    public void ChildMoveEvent(BasementTile child, PointerEventData data)
    {
        CurrentTile = child;
        if (child.placementable == null)
        {
            if (view)
            {
                Managers.UI.ClosePopUp(view);
                view = null;
                CurrentTile = null;
            }
        }
        else
        {
            if (view == null)
            {
                view = Managers.UI.ShowPopUpAlone<UI_TileView>();
            }

            var pos = Camera.main.ScreenToWorldPoint(data.position);
            view.transform.localPosition = new Vector3(pos.x, pos.y, 0);
            view.ViewContents($"[{CurrentTile.placementable.Name_KR}]", $"{CurrentTile.placementable.Detail_KR } ");
        }
    }
    public void ChildExitEvent()
    {
        CurrentTile = null;
    }




    public void ChildColorChange(Color32 color32)
    {
        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].GetComponent<Image>().color = color32;
        }
    }


    public GameObject[,] TileList { get; set; }

    public void ShowTile()
    {
        if (TileList != null) return;

        childList = new List<UI_TileView_Tile>();
        TileList = new GameObject[Main.Instance.Floor[FloorID].X_Size, Main.Instance.Floor[FloorID].Y_Size];

        for (int i = 0; i < TileList.GetLength(0); i++)
        {
            for (int k = 0; k < TileList.GetLength(1); k++)
            {
                BasementTile tile = null;
                if (Main.Instance.Floor[FloorID].TileMap.TryGetValue(new Vector2Int(i, k), out tile))
                {
                    var content = Managers.Resource.Instantiate("UI/PopUp/Element/TileView_Tile", transform);

                    content.GetComponent<RectTransform>().position = tile.worldPosition;
                    content.GetComponent<UI_TileView_Tile>().Tile = tile;

                    TileList[i, k] = content;

                    childList.Add(content.GetComponent<UI_TileView_Tile>());
                }
            }
        }
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
        var ui = Managers.UI.ClearAndShowPopUp<UI_DungeonPlacement>();

        yield return new WaitForEndOfFrame();

        FindObjectOfType<UI_Management>().FloorPanelActive();
        ChildColorChange(Define.Color_Blue);
        ui.uI_Floors[FloorID].OpenPlacementType(data);

        yield return new WaitForEndOfFrame();

        CurrentTile = null;
    }


}
