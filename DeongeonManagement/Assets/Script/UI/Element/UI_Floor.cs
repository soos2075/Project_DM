using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Floor : UI_Base
{


    private void Awake()
    {
        //Init();
    }
    private void Start()
    {
        Init();
    }


    enum Contents
    {
        Floor,
    }
    public int FloorID { get; set; }



    public override void Init()
    {
        Bind<GameObject>(typeof(Contents));
        GetObject((int)Contents.Floor).AddUIEvent((data) => OpenPlacementType(data), Define.UIEvent.LeftClick);
        GetObject((int)Contents.Floor).AddUIEvent((data) => OpenPlacementType(data), Define.UIEvent.RightClick);

        Main.Instance.Floor[FloorID].UI_Floor = this;
    }


    public enum BuildMode
    {
        Build,
        Clear,
    }
    public BuildMode Mode { get; set; }


    public void SetFloorSize(Vector3 pos, Vector2 sizeXY)
    {
        RectTransform rect = GetComponent<RectTransform>();

        rect.anchoredPosition = pos;
        rect.sizeDelta = sizeXY;
    }



    public void OpenPlacementType(PointerEventData data)
    {
        Main.Instance.CurrentFloor = Main.Instance.Floor[FloorID];

        var popup = Managers.UI.ShowPopUpAlone<UI_Placement_TypeSelect>();
        var pos = Camera.main.ScreenToWorldPoint(data.position);
        popup.transform.localPosition = new Vector3(pos.x, pos.y, 0);
        popup.parents = this;

        //ShowTile();
        PanelSelect();
    }

    void PanelSelect()
    {
        var floorList = FindObjectsOfType<UI_Floor>();
        foreach (var item in floorList)
        {
            item.GetComponent<Image>().color = Define.Color_White;
        }
        
        GetObject((int)Contents.Floor).GetComponent<Image>().color = Define.Color_Blue;
    }

    public void PanelDisable()
    {
        var floorList = FindObjectsOfType<UI_Floor>();
        foreach (var item in floorList)
        {
            item.GetComponent<Image>().enabled = false;
        }
    }



    public GameObject[,] TileList { get; set; }

    public void ShowTile()
    {
        if (TileList != null) return;

        TileList = new GameObject[Main.Instance.Floor[FloorID].TileMap.GetLength(0), Main.Instance.Floor[FloorID].TileMap.GetLength(1)];

        for (int i = 0; i < Main.Instance.Floor[FloorID].TileMap.GetLength(0); i++)
        {
            for (int k = 0; k < Main.Instance.Floor[FloorID].TileMap.GetLength(1); k++)
            {
                var content = Managers.Resource.Instantiate("UI/PopUp/Element/Floor_Tile", transform);

                content.GetComponent<RectTransform>().position = Main.Instance.Floor[FloorID].TileMap[i, k].worldPosition;
                if (Main.Instance.Floor[FloorID].TileMap[i, k].tileType == Define.TileType.Empty)
                {
                    content.GetComponent<Image>().color = Define.Color_White;
                }
                else if (Main.Instance.Floor[FloorID].TileMap[i, k].tileType == Define.TileType.Monster)
                {
                    content.GetComponent<Image>().color = Define.Color_Blue;
                }
                else
                {
                    content.GetComponent<Image>().color = Define.Color_Red;
                }
                content.GetComponent<UI_Floor_Tile>().Tile = Main.Instance.Floor[FloorID].TileMap[i, k];

                TileList[i, k] = content;
            }
        }
    }

    public void TileUpdate()
    {
        if (TileList == null) return;

        for (int i = 0; i < Main.Instance.Floor[FloorID].TileMap.GetLength(0); i++)
        {
            for (int k = 0; k < Main.Instance.Floor[FloorID].TileMap.GetLength(1); k++)
            {
                var content = TileList[i, k];
                if (Main.Instance.Floor[FloorID].TileMap[i, k].tileType == Define.TileType.Empty)
                {
                    content.GetComponent<Image>().color = Define.Color_White;
                }
                else if (Main.Instance.Floor[FloorID].TileMap[i, k].tileType == Define.TileType.Monster)
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