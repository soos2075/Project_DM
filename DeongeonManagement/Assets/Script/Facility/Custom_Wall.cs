using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Custom_Wall : Facility, IWall
{
    public override void Init_Personal()
    {
        Size = (SizeOption)CategoryIndex;

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


        var ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
        ui.SetText($"{UserData.Instance.LocaleText("Confirm_RemoveObstacle")}",
            () => GameManager.Facility.RemoveFacility(this));
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

    public void Set_ObstacleOption(Define.Boundary _size)
    {
        //Size = _size;
        //categoryName = _category;
        //labelName = _label;
        switch (_size)
        {
            case Define.Boundary.Boundary_1x1:
                break;
            case Define.Boundary.Boundary_1x2:
                break;
            case Define.Boundary.Boundary_1x3:
                break;
            case Define.Boundary.Boundary_2x1:
                break;
            case Define.Boundary.Boundary_2x2:
                Size = SizeOption._2x2;
                categoryName = "Floor3";
                labelName = "6";
                break;
            case Define.Boundary.Boundary_2x3:
                break;
            case Define.Boundary.Boundary_3x1:
                break;
            case Define.Boundary.Boundary_3x2:
                break;
            case Define.Boundary.Boundary_3x3:
                BasementTile temp = null;
                if (PlacementInfo.Place_Floor.TileMap.TryGetValue(PlacementInfo.Place_Tile.index + new Vector2Int(-1, -1), out temp))
                {
                    PlacementInfo newInfo = new PlacementInfo(PlacementInfo.Place_Floor, temp);
                    GameManager.Placement.PlacementConfirm(this, newInfo, true);
                }
                Size = SizeOption._3x3;
                categoryName = "Floor3";
                labelName = "7";
                break;
        }



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
