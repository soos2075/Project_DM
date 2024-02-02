using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContentManager
{
    public Dictionary<string, ContentData> EventContentsDic { get; set; }


    public List<ContentData> Contents { get; set; } //? 딕셔너리로 관리해도 좋을거같긴한데 일단은 구현부터

    public void Init()
    {
        EventContentsDic = new Dictionary<string, ContentData>();
        Contents = new List<ContentData>();
        AddContents();
        //AddEventContents();
    }

    //void AddEventContents()
    //{
    //    ContentData content = new ContentData("EggAppear");
    //    content.AddOption("비밀방 / 통상설치 불가능", Define.Boundary_1x1,
    //        (data) => CreateOnlyOne("EggEntrance"));


    //    EventContentsDic.Add(content.contentName, content);
    //}

    void AddContents()
    {
        {
            ContentData content = new ContentData("Clear");
            content.SetName("비우기", "힘들게 설치한 시설을 철거합니다. 마나와 골드는 회수할 수 없지만, 철거비용을 안받는게 어디에요.");
            content.SetCondition(0, 0, 1);
            content.sprite = Managers.Sprite.GetSprite("Nothing");
            content.AddOption("\n적용 범위는 1 x 1 입니다.", Define.Boundary_1x1,
                (data) => SetBoundary(Define.Boundary_1x1, () => ClearAll(), UI_Floor.BuildMode.Clear));

            content.AddOption("\n적용 범위는 3 x 3 입니다.", Define.Boundary_3x3,
                (data) => SetBoundary(Define.Boundary_3x3, () => ClearAll(), UI_Floor.BuildMode.Clear));

            content.AddOption("\n적용 범위는 5 x 5 입니다.", Define.Boundary_5x5,
                (data) => SetBoundary(Define.Boundary_5x5, () => ClearAll(), UI_Floor.BuildMode.Clear));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Entrance");
            content.SetName("입구", "플레이어가 들어올 입구를 지정합니다. 만약 입구가 없으면 랜덤위치에 자동으로 지정돼요. 입구는 층 당 한개만 존재할 수 있어요.");
            content.SetCondition(10, 0, 1);
            content.sprite = Managers.Sprite.GetSprite("Entrance");
            content.AddOption("\n적용 범위는 1 x 1 입니다.", Define.Boundary_1x1,
                data => SetBoundary(Define.Boundary_1x1, () => CreateOnlyOne("Entrance", useMana: 10)));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Exit");
            content.SetName("출구", "플레이어가 돌아갈 출구를 지정합니다. 만약 출구가 없으면 랜덤위치에 자동으로 지정돼요. 출구는 층 당 한개만 존재할 수 있어요.");
            content.SetCondition(10, 0, 1);
            content.sprite = Managers.Sprite.GetSprite("Exit");
            content.AddOption("\n적용 범위는 1 x 1 입니다.", Define.Boundary_1x1,
                data => SetBoundary(Define.Boundary_1x1, () => CreateOnlyOne("Exit", useMana: 10)));

            Contents.Add(content);
        }


        {
            ContentData content = new ContentData("Herb_Low");
            content = new ContentData("Herb_Low");
            content.SetName("흔한 약초밭", "흔한 약초밭을 설치합니다. 흔하긴 해도 마나를 지니고 있기 때문에 쓰임새는 많은 편이에요. " +
                "대량으로 설치하면 좀 더 효율적이에요.");
            content.SetCondition(25, 0, 1);
            content.sprite = Managers.Sprite.GetSprite("Herb_Low");

            content.AddOption("\n적용 범위는 2 x 2 입니다.", Define.Boundary_2x2,
                data => SetBoundary(Define.Boundary_2x2, () => CreateAll("Herb_Low", useMana: 25)));
            content.AddOption("\n적용 범위는 3 x 3 입니다.", Define.Boundary_3x3,
                data => SetBoundary(Define.Boundary_3x3, () => CreateAll("Herb_Low", useMana: 45)),
                mana: 20);
            content.AddOption("\n적용 범위는 5 x 5 입니다.", Define.Boundary_5x5,
                data => SetBoundary(Define.Boundary_5x5, () => CreateAll("Herb_Low", useMana: 100)),
                mana:75);

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Herb_High");
            content = new ContentData("Herb_High");
            content.SetName("귀한 약초밭", "귀한 약초밭을 설치합니다. 쉽게 구할 수 없는 귀한 약초입니다. 누구든지 발견하면 꼭 챙기려고 할 거에요.");
            content.SetCondition(40, 0, 2);
            content.sprite = Managers.Sprite.GetSprite("Nothing");
            content.AddOption("\n적용 범위는 1 x 3 입니다.", Define.Boundary_1x3,
                data => SetBoundary(Define.Boundary_1x3, () => CreateAll("Herb_High")));
            content.AddOption("\n적용 범위는 3 x 1 입니다.", Define.Boundary_3x1,
                data => SetBoundary(Define.Boundary_3x1, () => CreateAll("Herb_High")));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Mineral");
            content = new ContentData("Mineral");
            content.SetName("광맥", "유용한 물질을 얻을 수 있는 광맥을 설치합니다. 귀한 물질도 조금 섞여있어요.");
            content.SetCondition(40, 0, 2);
            content.sprite = Managers.Sprite.GetSprite("Mineral");
            content.AddOption("\n적용 범위는 작은 십자모양 입니다.", Define.Boundary_Cross_1,
                data => SetBoundary(Define.Boundary_Cross_1, () =>
                CreateTwo("Mineral_Diamond", "Mineral_Rock", Define.Boundary_1x1, Define.Boundary_Side_Cross, useMana: 40)));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Trap");
            content.SetName("함정", "뻔히 보이는 고전식 발밑 함정. 이런거에 걸리는 사람이 있을까요?");
            content.SetCondition(0, 50, 2);
            content.sprite = Managers.Sprite.GetSprite("Nothing");

            content.AddOption("\n적용 범위는 1 x 1 입니다.", Define.Boundary_1x1,
                data => SetBoundary(Define.Boundary_1x1, () => CreateAll("Trap", true, useGold: 50)));
            content.AddOption("\n적용 범위는 1 x 3 입니다.", Define.Boundary_1x3,
                data => SetBoundary(Define.Boundary_1x3, () => CreateAll("Trap", true, useGold: 100)),
                mana: 0, gold: 50);
            content.AddOption("\n적용 범위는 3 x 1 입니다.", Define.Boundary_3x1,
                data => SetBoundary(Define.Boundary_3x1, () => CreateAll("Trap", true, useGold: 100)),
                mana: 0, gold: 50);

            Contents.Add(content);
        }
    }





    #region Clear 메서드
    void ClearAll()
    {
        if (Clear(Main.Instance.CurrentBoundary))
        {
            ClearOver();
        }
        else
        {
            Debug.Log("삭제할 수 없음");
        }
    }
    bool Clear(Vector2Int[] boundary)
    {
        if (Main.Instance.CurrentTile == null) return false;

        var tile = Main.Instance.CurrentTile;
        foreach (var item in boundary)
        {
            int _deltaX = tile.index.x + item.x;
            int _deltaY = tile.index.y + item.y;

            var content = Main.Instance.CurrentFloor.TileMap[_deltaX, _deltaY];

            if (content.tileType == Define.TileType.Facility || content.tileType == Define.TileType.Entrance || content.tileType == Define.TileType.Exit)
            {
                Managers.Placement.PlacementClear_Completely(content.placementable);
            }
        }

        return true;
    }

    void ClearOver()
    {
        Debug.Log($"{Main.Instance.CurrentTile.index} 타일 삭제");
        //Managers.UI.PauseClose();
        //Managers.UI.ClosePopUp();
        ResetAction();
        Managers.UI.CloseAll();
    }

    #endregion


    #region Create 메서드
    void CreateAll(string prefab, bool isUnchangeable = false, int useMana = 0, int useGold = 0)
    {
        if (Create(prefab, Main.Instance.CurrentBoundary, isUnchangeable))
        {
            CreateOver(useMana, useGold);
        }
        else
        {
            Debug.Log("배치할 수 없음");
        }
    }

    void CreateOnlyOne(string prefab, int useMana = 0, int useGold = 0)
    {
        if (CreateUnique(prefab, Main.Instance.CurrentBoundary))
        {
            CreateOver(useMana, useGold);
        }
        else
        {
            Debug.Log("배치할 수 없음");
        }
    }


    void CreateTwo(string prefab1, string prefab2, Vector2Int[] boundary1, Vector2Int[] boundary2, int useMana = 0, int useGold = 0)
    {
        if (Create(prefab1, boundary1) && Create(prefab2, boundary2))
        {
            CreateOver(useMana, useGold);
        }
        else
        {
            Debug.Log("배치할 수 없음");
        }
    }
    //void CreateThree(string prefab1, string prefab2, string prefab3, Vector2Int[] boundary1, Vector2Int[] boundary2, Vector2Int[] boundary3)
    //{
    //    if (Create(prefab1, boundary1) && Create(prefab2, boundary2) && Create(prefab3, boundary3))
    //    {
    //        CreateOver();
    //    }
    //    else
    //    {
    //        Debug.Log("배치할 수 없음");
    //    }
    //}

    #endregion UIEvent 함수


    #region 실제 구현 함수

    void SetBoundary(Vector2Int[] vector2Ints, Action action, UI_Floor.BuildMode buildMode = UI_Floor.BuildMode.Build)
    {
        Main.Instance.CurrentBoundary = vector2Ints;
        Main.Instance.CurrentAction = action;

        //parents.ShowTile();
        //parents.Mode = buildMode;

        Main.Instance.CurrentFloor.UI_Floor.ShowTile();
        Main.Instance.CurrentFloor.UI_Floor.Mode = buildMode;

        Managers.UI.PausePopUp();
        //Managers.UI.ClosePopUp(this);
    }

    void ResetAction()
    {
        Main.Instance.CurrentBoundary = null;
        Main.Instance.CurrentAction = null;
        Main.Instance.CurrentTile = null;
    }

    void CreateOver(int mana, int gold)
    {
        Debug.Log($"{Main.Instance.CurrentTile.index} : {Main.Instance.CurrentTile.placementable.Name_KR} 설치완료");


        Main.Instance.CurrentDay.SubtractMana(mana);
        Main.Instance.CurrentDay.SubtractGold(gold);

        //Managers.UI.PauseClose();
        //Managers.UI.ClosePopUp();
        ResetAction();
        Managers.UI.CloseAll();
    }

    bool Create(string prefab, Vector2Int[] boundary, bool isUnChangeable = false)
    {
        if (Main.Instance.CurrentTile == null) return false;

        var tile = Main.Instance.CurrentTile;
        foreach (var item in boundary)
        {
            int _deltaX = tile.index.x + item.x;
            int _deltaY = tile.index.y + item.y;

            var content = Main.Instance.CurrentFloor.TileMap[_deltaX, _deltaY];
            var info = new PlacementInfo(Main.Instance.CurrentFloor, content);

            Managers.Facility.CreateFacility(prefab, info, isUnChangeable);

            //var newObj = Managers.Placement.CreatePlacementObject($"Facility/{prefab}", null, Define.PlacementType.Facility);
            //Managers.Placement.PlacementConfirm(newObj, new PlacementInfo(Main.Instance.CurrentFloor, content), isUnChangeable);
        }

        return true;
    }


    bool CreateUnique(string prefab, Vector2Int[] boundary)
    {
        if (Main.Instance.CurrentTile == null) return false;

        var tile = Main.Instance.CurrentTile;
        foreach (var item in boundary)
        {
            int _deltaX = tile.index.x + item.x;
            int _deltaY = tile.index.y + item.y;

            var content = Main.Instance.CurrentFloor.TileMap[_deltaX, _deltaY];
            var info = new PlacementInfo(Main.Instance.CurrentFloor, content);

            var newObj = Managers.Facility.CreateFacility_OnlyOne($"{prefab}", info, true);
        }

        return true;
    }

    #endregion



}


public class ContentData
{
    public Sprite sprite;
    public string name_Placement;
    public string name_Detail;

    public int need_Mana;
    public int need_Gold;
    public int need_LV;

    public string contentName;
    public string path;

    public List<Option> boundaryOption;
    public ContentData(string _contentName)
    {
        contentName = _contentName;
        boundaryOption = new List<Option>();
    }

    public void AddOption(string _text, Vector2Int[] _option, Action<PointerEventData> _action, int mana = 0, int gold = 0)
    {
        boundaryOption.Add(new Option(_text, _option, _action, mana, gold));
    }

    public void SetName(string title, string box)
    {
        name_Placement = title;
        name_Detail = box;
    }
    public void SetCondition(int mana, int gold, int lv)
    {
        need_Mana = mana;
        need_Gold = gold;
        need_LV = lv;
    }
}

public class Option
{
    public string optionText;
    public Vector2Int[] boundary;

    public int addMana;
    public int addGold;
    public Action<PointerEventData> Action { get; set; }

    public Option(string option, Vector2Int[] _bounds, Action<PointerEventData> _action, int _mana, int _gold)
    {
        optionText = option;
        boundary = _bounds;
        Action = _action;
        addMana = _mana;
        addGold = _gold;
    }
}