using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Floor : UI_Base
{

    private void Start()
    {
        Init();
    }

    public int FloorID { get; set; }



    public override void Init()
    {
        Main.Instance.Floor[FloorID].UI_Floor = this;
    }


    public enum BuildMode
    {
        Build,
        Clear,
    }
    public BuildMode Mode { get; set; }



    //public void OpenPlacementType(PointerEventData data)
    //{
    //    Main.Instance.CurrentFloor = Main.Instance.Floor[FloorID];

    //    var popup = Managers.UI.ShowPopUpAlone<UI_Placement_TypeSelect>();
    //    var pos = Camera.main.ScreenToWorldPoint(data.position);
    //    popup.transform.localPosition = new Vector3(pos.x, pos.y, 5);
    //    popup.parents = this;
    //}



    public GameObject[,] TileList { get; set; }


    public void ShowTile_IgnoreEvent(Color color)
    {
        if (TileList != null) return;

        TileList = new GameObject[Main.Instance.Floor[FloorID].X_Size, Main.Instance.Floor[FloorID].Y_Size];

        for (int i = 0; i < TileList.GetLength(0); i++)
        {
            for (int k = 0; k < TileList.GetLength(1); k++)
            {
                BasementTile tile = null;
                if (Main.Instance.Floor[FloorID].TileMap.TryGetValue(new Vector2Int(i, k), out tile))
                {
                    if (tile.NonInteract_TileCheck())
                    {
                        continue;
                    }

                    var content = Managers.Resource.Instantiate("UI/PopUp/Element/Floor_Tile", transform);
                    content.GetComponent<RectTransform>().position = tile.worldPosition;

                    content.GetComponent<Image>().color = color;

                    content.GetComponent<UI_Floor_Tile>().Tile = tile;
                    content.GetComponent<UI_Floor_Tile>().isIgnore = true;
                    TileList[i, k] = content;
                }
            }
        }
    }

    public void ShowTile()
    {
        if (TileList != null) return;

        TileList = new GameObject[Main.Instance.Floor[FloorID].X_Size, Main.Instance.Floor[FloorID].Y_Size];

        for (int i = 0; i < TileList.GetLength(0); i++)
        {
            for (int k = 0; k < TileList.GetLength(1); k++)
            {
                BasementTile tile = null;
                if (Main.Instance.Floor[FloorID].TileMap.TryGetValue(new Vector2Int(i, k), out tile))
                {
                    if (tile.NonInteract_TileCheck())
                    {
                        continue;
                    }

                    var content = Managers.Resource.Instantiate("UI/PopUp/Element/Floor_Tile", transform);
                    content.GetComponent<RectTransform>().position = tile.worldPosition;

                    if (tile.tileType_Original == Define.TileType.Empty)
                    {
                        content.GetComponent<Image>().color = Define.Color_White;
                    }
                    else if (tile.tileType_Original == Define.TileType.Monster)
                    {
                        content.GetComponent<Image>().color = Define.Color_Blue;
                    }
                    else
                    {
                        content.GetComponent<Image>().color = Define.Color_Red;
                    }
                    content.GetComponent<UI_Floor_Tile>().Tile = tile;

                    TileList[i, k] = content;
                }
            }
        }
    }
    public void TileUpdate()
    {
        if (TileList == null) return;

        for (int i = 0; i < TileList.GetLength(0); i++)
        {
            for (int k = 0; k < TileList.GetLength(1); k++)
            {
                BasementTile tile = null;
                if (Main.Instance.Floor[FloorID].TileMap.TryGetValue(new Vector2Int(i, k), out tile))
                {
                    // 보여야될 타일 체크
                    if (tile.NonInteract_TileCheck())
                    {
                        continue;
                    }

                    var content = TileList[i, k];
                    if (tile.tileType_Original == Define.TileType.Empty)
                    {
                        content.GetComponent<Image>().color = Define.Color_White;
                    }
                    else if (tile.tileType_Original == Define.TileType.Monster)
                    {
                        content.GetComponent<Image>().color = Define.Color_Blue;
                    }
                    else
                    {
                        content.GetComponent<Image>().color = Define.Color_Red;
                    }
                }
            }
        }
    }



    //public bool NonInteract_TileCheck(BasementTile Tile)
    //{
    //    if (Tile.tileType_Original == Define.TileType.Non_Interaction || Tile.tileType_Original == Define.TileType.Player)
    //    {
    //        return true;
    //    }


    //    if (Tile.tileType_Original == Define.TileType.Facility)
    //    {
    //        var fa = Tile.Original as Facility;
    //        switch (fa.EventType)
    //        {
    //            case Facility.FacilityEventType.NPC_Interaction:
    //                return false;

    //            case Facility.FacilityEventType.NPC_Event:
    //                return false;

    //            case Facility.FacilityEventType.Player_Event:
    //                return true;

    //            case Facility.FacilityEventType.Non_Interaction:
    //                return true;
    //        }
    //    }

    //    return false;
    //}


}
