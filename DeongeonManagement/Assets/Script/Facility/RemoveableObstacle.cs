using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class RemoveableObstacle : Facility, IWall
{
    public override void Init_Personal()
    {
        Size = (SizeOption)CategoryIndex;
        //if (isInit)
        //{
        //    CategorySelect(categoryName, labelName);
        //}

        Show_Sprite();
    }
    public override void Init_FacilityEgo()
    {
        isOnlyOne = false;
        isClearable = true;
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        throw new System.NotImplementedException();
    }

    public override void Load_Data(Save_FacilityData _data)
    {
        base.Load_Data(_data);
        categoryName = _data.categoryName;
        labelName = _data.labelName;
        CategorySelect(categoryName, labelName);
    }


    public void Show_Sprite()
    {
        if (Main.Instance.ActiveFloor_Basement <= PlacementInfo.Place_Floor.FloorIndex)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
        else
        {
            GetComponentInChildren<SpriteRenderer>().enabled = true;
        }
    }





    public override void MouseClickEvent()
    {
        if (Main.Instance.Management == false) return;
        if (Main.Instance.CurrentAction != null) return;

        int ap = 0;
        //int mana = 0;
        //int gold = 0;

        int value = 45 + (PlacementInfo.Place_Floor.FloorIndex * 5);

        switch (Size)
        {
            case SizeOption._1x2:
            case SizeOption._2x1:
                value *= 2;
                break;
            case SizeOption._1x3:
            case SizeOption._3x1:
                value *= 3;
                break;

            case SizeOption._2x2:
                value *= 4;
                ap = 1;
                break;

            case SizeOption._2x3:
            case SizeOption._3x2:
                value *= 6;
                ap = 2;
                break;

            case SizeOption._3x3:
                value *= 8;
                ap = 2;
                break;
        }

        var ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
        ui.SetText($"{UserData.Instance.LocaleText("Confirm_RemoveObstacle")}", () => ConfirmUI_Action(ap, value, value));
        ui.SetMode_Calculation(0, $"{value}", $"+{value}", $"{ap}");
    }


    void ConfirmUI_Action(int _ap, int _mana, int _gold)
    {
        if (ConfirmCheck(ap: _ap, gold: _gold, mana: _mana))
        {
            //Main.Instance.CurrentDay.AddGold(_gold, Main.DayResult.EventType.Etc);
            Main.Instance.CurrentDay.AddGold(_gold, Main.DayResult.EventType.Etc);
            Main.Instance.CurrentDay.SubtractMana(_mana, Main.DayResult.EventType.Etc);
            Main.Instance.Player_AP -= _ap;

            GameManager.Facility.RemoveFacility(this);
            //Debug.Log($"장애물 제거 {ObstacleType}");
        }
    }

    bool ConfirmCheck(int mana = 0, int gold = 0, int lv = 0, int ap = 0)
    {
        if (Main.Instance.DungeonRank < lv)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = UserData.Instance.LocaleText("Message_No_Rank");
            return false;
        }
        if (Main.Instance.Player_AP < ap)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = UserData.Instance.LocaleText("Message_No_AP");
            return false;
        }
        if (Main.Instance.Player_Mana < mana)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = UserData.Instance.LocaleText("Message_No_Mana");
            return false;
        }
        //if (Main.Instance.Player_Gold < gold)
        //{
        //    var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
        //    msg.Message = UserData.Instance.LocaleText("Message_No_Gold");
        //    return false;
        //}

        return true;
    }


    public override void MouseMoveEvent()
    {
        if (Main.Instance.Management == false) return;
        if (Main.Instance.CurrentAction != null) return;
        CategorySelect($"{categoryName}_Outline", labelName);
    }
    public override void MouseExitEvent()
    {
        if (Main.Instance.Management == false) return;
        if (Main.Instance.CurrentAction != null) return;
        CategorySelect(categoryName, labelName);
    }


    void CategorySelect(string category, string label)
    {
        var resolver = GetComponentInChildren<SpriteResolver>();
        resolver.SetCategoryAndLabel(category, label);
    }



    public enum SizeOption
    {
        _1x1 = 3531,
        _1x2,
        _1x3,
        _2x1,
        _2x2,
        _2x3,
        _3x1,
        _3x2,
        _3x3,
    }

    public SizeOption Size;

    public void Set_ObstacleOption(SizeOption _size, string _category, string _label)
    {
        Size = _size;
        categoryName = _category;
        labelName = _label;

        isInit = true;
        Data = GameManager.Facility.GetData($"RO{Size.ToString()}");
        SetData();
        CategorySelect(categoryName, labelName);
        Create_Clone();
    }


    void Create_Clone()
    {
        switch (Size)
        {
            case SizeOption._1x2:
                SetClone(CreateClone(PlacementInfo, new Vector2Int(0, 1)));
                break;

            case SizeOption._1x3:
                SetClone(CreateClone(PlacementInfo, new Vector2Int(0, 1)));
                SetClone(CreateClone(PlacementInfo, new Vector2Int(0, 2)));
                break;

            case SizeOption._2x1:
                SetClone(CreateClone(PlacementInfo, new Vector2Int(1, 0)));
                break;

            case SizeOption._2x2:
                SetClone(CreateClone(PlacementInfo, new Vector2Int(1, 0)));

                SetClone(CreateClone(PlacementInfo, new Vector2Int(0, 1)));
                SetClone(CreateClone(PlacementInfo, new Vector2Int(1, 1)));
                break;

            case SizeOption._2x3:
                SetClone(CreateClone(PlacementInfo, new Vector2Int(1, 0)));

                SetClone(CreateClone(PlacementInfo, new Vector2Int(0, 1)));
                SetClone(CreateClone(PlacementInfo, new Vector2Int(1, 1)));

                SetClone(CreateClone(PlacementInfo, new Vector2Int(0, 2)));
                SetClone(CreateClone(PlacementInfo, new Vector2Int(1, 2)));
                break;

            case SizeOption._3x1:
                SetClone(CreateClone(PlacementInfo, new Vector2Int(1, 0)));
                SetClone(CreateClone(PlacementInfo, new Vector2Int(2, 0)));
                break;

            case SizeOption._3x2:
                SetClone(CreateClone(PlacementInfo, new Vector2Int(1, 0)));
                SetClone(CreateClone(PlacementInfo, new Vector2Int(2, 0)));

                SetClone(CreateClone(PlacementInfo, new Vector2Int(0, 1)));
                SetClone(CreateClone(PlacementInfo, new Vector2Int(1, 1)));
                SetClone(CreateClone(PlacementInfo, new Vector2Int(2, 1)));
                break;

            case SizeOption._3x3:
                SetClone(CreateClone(PlacementInfo, new Vector2Int(1, 0)));
                SetClone(CreateClone(PlacementInfo, new Vector2Int(2, 0)));

                SetClone(CreateClone(PlacementInfo, new Vector2Int(0, 1)));
                SetClone(CreateClone(PlacementInfo, new Vector2Int(1, 1)));
                SetClone(CreateClone(PlacementInfo, new Vector2Int(2, 1)));

                SetClone(CreateClone(PlacementInfo, new Vector2Int(0, 2)));
                SetClone(CreateClone(PlacementInfo, new Vector2Int(1, 2)));
                SetClone(CreateClone(PlacementInfo, new Vector2Int(2, 2)));
                break;
        }
    }


    IPlacementable CreateClone(PlacementInfo data, Vector2Int offset)
    {
        BasementTile newTile = null;
        data.Place_Floor.TileMap.TryGetValue(data.Place_Tile.index + offset, out newTile);
        PlacementInfo info = new PlacementInfo(data.Place_Floor, newTile);
        IPlacementable obj = GameManager.Facility.CreateFacility("Clone_Facility_Wall", info);
        return obj;
    }

    void SetClone(IPlacementable clone)
    {
        var cf = clone as Clone_Facility_Wall;
        cf.OriginalTarget = this;
    }

}
