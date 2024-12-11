using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
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
                Managers.UI.ClosePopupPick(view);
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



    public void ChildMoveEvent_CurrentData(IPlacementable current)
    {
        if (current as Facility != null)  //? 퍼실리티가 들어오면 바로 리턴
        {
            return;
        }

        CurrentTile = current.PlacementInfo.Place_Tile;
        if (view == null)
        {
            view = Managers.UI.ShowPopUpAlone<UI_TileView>();
        }

        string _title = current.Name_Color;
        switch (current.PlacementType)
        {
            case PlacementType.Monster:
                var monster = current as Monster;
                _title += $" LV.{monster.LV}".SetTextColorTag(Define.TextColor.monster_green);
                break;

            case PlacementType.NPC:
                break;
        }
        view.ViewContents(_title, current.Detail_KR);

        switch (current.PlacementType)
        {
            case PlacementType.Monster:
                var monster = current as Monster;
                view.ViewDetail($"{monster.B_HP}/{monster.B_HP_Max}".SetTextColorTag(Define.TextColor.HeavyGreen));
                break;

            case PlacementType.NPC:
                var npc = current as NPC;
                view.ViewDetail($"{npc.B_HP}/{npc.B_HP_Max}".SetTextColorTag(Define.TextColor.npc_red));
                string trait = "";
                foreach (var item in npc.Data.NPC_TraitList)
                {
                    trait += $"{GameManager.Trait.GetData(item).labelName}  ";
                }
                view.View_State(trait);
                break;
        }
    }

    public void ChildMoveEvent(BasementTile child, PointerEventData data)
    {
        CurrentTile = child;
        if (child.Original == null)
        {
            if (view)
            {
                Managers.UI.ClosePopupPick(view);
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

            //var pos = Camera.main.ScreenToWorldPoint(data.position);
            //float offset = Camera.main.GetComponent<PixelPerfectCamera>().assetsPPU * 0.04f;

            //view.transform.localPosition = new Vector3(pos.x , pos.y, 0);

            string _title = CurrentTile.Original.Name_Color;
            string _detail = CurrentTile.Original.Detail_KR;
            switch (CurrentTile.Original.PlacementType)
            {
                case PlacementType.Monster:
                    var monster = CurrentTile.Original as Monster;
                    _title += $" LV.{monster.LV}".SetTextColorTag(Define.TextColor.monster_green);
                    break;

                case PlacementType.NPC:
                    break;

                case PlacementType.Facility:
                    var facil = CurrentTile.Original as Facility;
                    if (facil.GetType() == typeof(Obstacle) || facil.GetType() == typeof(Obstacle_Wall) 
                        || facil.GetType() == typeof(RemoveableObstacle) || facil.GetType() == typeof(Custom_Wall))
                    {
                        Managers.UI.ClosePopupPick(view);
                        return;
                    }

                    if (facil is Clone_Facility) //? 만약 클론이면 오리지널 데이터에 따라결정
                    {
                        var clone = facil as Clone_Facility;
                        if (clone.OriginalTarget.GetType() == typeof(Obstacle) || clone.OriginalTarget.GetType() == typeof(Obstacle_Wall) 
                            || clone.OriginalTarget.GetType() == typeof(RemoveableObstacle) || clone.OriginalTarget.GetType() == typeof(Custom_Wall))
                        {
                            Managers.UI.ClosePopupPick(view);
                            return;
                        }
                    }

                    //_title = _title.SetTextColorTag(Define.TextColor.white);
                    break;
            }

            view.ViewContents(_title, _detail);
            view.ViewDetail("");


            switch (CurrentTile.Original.PlacementType)
            {
                case PlacementType.Facility:
                    var facil = CurrentTile.Original as Facility;
                    if (facil.EventType == Facility.FacilityEventType.NPC_Interaction)
                    {
                        if (facil.Data == null || facil.GetType() == typeof(SpecialEgg))
                        {
                            return;
                        }
                        if (facil is Clone_Facility)
                        {
                            var clone = facil as Clone_Facility;
                            if (clone.OriginalTarget is SpecialEgg)
                            {
                                return;
                            }
                        }
                        view.ViewDetail($"{facil.InteractionOfTimes + facil.IOT_Temp}/{facil.Data.interactionOfTimes}".SetTextColorTag(Define.TextColor.HeavyYellow));
                    }
                    break;

                case PlacementType.Monster:
                    var monster = CurrentTile.Original as Monster;
                    view.ViewDetail($"{monster.B_HP}/{monster.B_HP_Max}".SetTextColorTag(Define.TextColor.HeavyGreen));
                    break;

                case PlacementType.NPC:
                    var npc = CurrentTile.Original as NPC;
                    view.ViewDetail($"{npc.B_HP}/{npc.B_HP_Max}".SetTextColorTag(Define.TextColor.npc_red));
                    string trait = "";
                    foreach (var item in npc.Data.NPC_TraitList)
                    {
                        trait += $"{GameManager.Trait.GetData(item).labelName}  ";
                    }
                    view.View_State(trait);
                    break;
            }
        }
    }


    //public void ChildEnterEvent()
    //{
    //    //CurrentTile = null;
    //}
    public void ChildExitEvent()
    {
        CurrentTile = null;
    }





    public void ChildColorChange(Color32 color32)
    {
        for (int i = 0; i < childList.Count; i++)
        {
            if (childList[i].Tile.NonInteract_TileCheck())
            {
                continue;
            }

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
        //var ui = Managers.UI.ClearAndShowPopUp<UI_DungeonPlacement>();

        OpenPlacementType(data);

        yield return new WaitForEndOfFrame();

        FindObjectOfType<UI_Management>().FloorPanelActive();
        ChildColorChange(Define.Color_Blue);

        //ui.uI_Floors[FloorID].OpenPlacementType(data);
        
        yield return new WaitForEndOfFrame();

        CurrentTile = null;
    }


    public void OpenPlacementType(PointerEventData data)
    {
        Main.Instance.CurrentFloor = Main.Instance.Floor[FloorID];

        //Debug.Log(Managers.UI._popupStack);
        //var popup = Managers.UI.ShowPopUpAlone<UI_Placement_TypeSelect>();
        var popup = Managers.UI.ClearAndShowPopUp<UI_Placement_TypeSelect>();

        var pos = Camera.main.ScreenToWorldPoint(data.position);
        popup.transform.localPosition = new Vector3(pos.x, pos.y, 5);
        //popup.parents = this;
    }

}
