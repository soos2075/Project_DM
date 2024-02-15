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
                () => SetBoundary(Define.Boundary_1x1, () => ClearAll(), UI_Floor.BuildMode.Clear));

            content.AddOption("\n적용 범위는 3 x 3 입니다.", Define.Boundary_3x3,
                () => SetBoundary(Define.Boundary_3x3, () => ClearAll(), UI_Floor.BuildMode.Clear));

            content.AddOption("\n적용 범위는 5 x 5 입니다.", Define.Boundary_5x5,
                () => SetBoundary(Define.Boundary_5x5, () => ClearAll(), UI_Floor.BuildMode.Clear));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Entrance");
            content.SetName("입구", "플레이어가 들어올 입구를 지정합니다. 만약 입구가 없으면 랜덤위치에 자동으로 지정돼요. 입구는 층 당 한개만 존재할 수 있어요.");
            content.SetCondition(50, 0, 1);
            content.sprite = Managers.Sprite.GetSprite("Entrance");
            content.AddOption("\n적용 범위는 1 x 1 입니다.", Define.Boundary_1x1,
                () => SetBoundary(Define.Boundary_1x1, () => CreateOnlyOne("Entrance")));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Exit");
            content.SetName("출구", "플레이어가 돌아갈 출구를 지정합니다. 만약 출구가 없으면 랜덤위치에 자동으로 지정돼요. 출구는 층 당 한개만 존재할 수 있어요.");
            content.SetCondition(50, 0, 1);
            content.sprite = Managers.Sprite.GetSprite("Exit");
            content.AddOption("\n적용 범위는 1 x 1 입니다.", Define.Boundary_1x1,
                () => SetBoundary(Define.Boundary_1x1, () => CreateOnlyOne("Exit")));

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
                () => SetBoundary(Define.Boundary_2x2, () => CreateAll("Herb_Low")));
            content.AddOption("\n적용 범위는 3 x 3 입니다.", Define.Boundary_3x3,
                () => SetBoundary(Define.Boundary_3x3, () => CreateAll("Herb_Low")), mana: 20);
            content.AddOption("\n적용 범위는 5 x 5 입니다.", Define.Boundary_5x5,
                () => SetBoundary(Define.Boundary_5x5, () => CreateAll("Herb_Low")), mana: 50, ap: 1);

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Herb_High");
            content = new ContentData("Herb_High");
            content.SetName("귀한 약초밭", "귀한 약초밭을 설치합니다. 쉽게 구할 수 없는 귀한 약초입니다. 누구든지 발견하면 꼭 챙기려고 할 거에요.");
            content.SetCondition(40, 0, 1);
            content.sprite = Managers.Sprite.GetSprite("Nothing");
            content.AddOption("\n적용 범위는 1 x 3 입니다.", Define.Boundary_1x3,
                () => SetBoundary(Define.Boundary_1x3, () => CreateAll("Herb_High")));
            content.AddOption("\n적용 범위는 3 x 1 입니다.", Define.Boundary_3x1,
                () => SetBoundary(Define.Boundary_3x1, () => CreateAll("Herb_High")));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Mineral");
            content = new ContentData("Mineral");
            content.SetName("광맥", "유용한 물질을 얻을 수 있는 광맥을 설치합니다. 귀한 물질도 조금 섞여있어요.");
            content.SetCondition(40, 0, 1);
            content.sprite = Managers.Sprite.GetSprite("Mineral");
            content.AddOption("\n적용 범위는 작은 십자모양 입니다.", Define.Boundary_Cross_1,
                () => SetBoundary(Define.Boundary_Cross_1, () =>
                CreateTwo("Mineral_Diamond", "Mineral_Rock", Define.Boundary_1x1, Define.Boundary_Side_Cross)));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Trap_Fallen_1");
            content.SetName("발밑 함정", "뻔히 보이는 고전식 발밑 함정을 설치합니다. 하지만 이런거에 걸리는 사람이 있을까요? 동시에 여러개를 설치할 수도 있어요.");
            content.SetCondition(0, 50, 1);
            content.sprite = Managers.Sprite.GetSprite("Nothing");

            content.AddOption("\n적용 범위는 1 x 1 입니다.", Define.Boundary_1x1,
                () => SetBoundary(Define.Boundary_1x1, () => CreateAll("Trap_Fallen_1", true)));
            content.AddOption("\n적용 범위는 1 x 3 입니다.", Define.Boundary_1x3,
                () => SetBoundary(Define.Boundary_1x3, () => CreateAll("Trap_Fallen_1", true)), mana: 0, gold: 50);
            content.AddOption("\n적용 범위는 3 x 1 입니다.", Define.Boundary_3x1,
                () => SetBoundary(Define.Boundary_3x1, () => CreateAll("Trap_Fallen_1", true)), mana: 0, gold: 50);

            Contents.Add(content);
        }
    }

    public void AddLevel2()
    {
        {
            ContentData content = new ContentData("Trap_Fallen_2");
            content.SetName("강화 발밑 함정", "뻔히 보이는 고전식 발밑 함정을 설치합니다. 조금 더 효과가 강해졌어요. 하지만 겉모습은 별로 달라진게 없어보이네요.");
            content.SetCondition(100, 50, 2);
            content.sprite = Managers.Sprite.GetSprite("Nothing");

            content.AddOption("\n적용 범위는 1 x 1 입니다.", Define.Boundary_1x1,
                () => SetBoundary(Define.Boundary_1x1, () => CreateTrap("Trap_Fallen_2", Trap_Base.TrapType.Fallen_2)));
            content.AddOption("\n적용 범위는 1 x 3 입니다.", Define.Boundary_1x3,
                () => SetBoundary(Define.Boundary_1x3, () => CreateTrap("Trap_Fallen_2", Trap_Base.TrapType.Fallen_2)), mana: 100, gold: 50);
            content.AddOption("\n적용 범위는 3 x 1 입니다.", Define.Boundary_3x1,
                () => SetBoundary(Define.Boundary_3x1, () => CreateTrap("Trap_Fallen_2", Trap_Base.TrapType.Fallen_2)), mana: 100, gold: 50);

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Trap_Awl");
            content.SetName("송곳 함정", "바닥에서 송곳이 올라오는 무시무시한 함정을 설치합니다. 웬만하면 여길 지나가길 원하는 모험가는 없을거에요.");
            content.SetCondition(50, 100, 2);
            content.sprite = Managers.Sprite.GetSprite("Nothing");

            content.AddOption("\n적용 범위는 1 x 1 입니다.", Define.Boundary_1x1,
                () => SetBoundary(Define.Boundary_1x1, () => CreateTrap("Trap_Awl_1", Trap_Base.TrapType.Awl_1)));
            content.AddOption("\n적용 범위는 1 x 3 입니다.", Define.Boundary_1x3,
                () => SetBoundary(Define.Boundary_1x3, () => CreateTrap("Trap_Awl_1", Trap_Base.TrapType.Awl_1)), mana: 50, gold: 100);
            content.AddOption("\n적용 범위는 3 x 1 입니다.", Define.Boundary_3x1,
                () => SetBoundary(Define.Boundary_3x1, () => CreateTrap("Trap_Awl_1", Trap_Base.TrapType.Awl_1)), mana: 50, gold: 100);

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
            Vector2Int delta = new Vector2Int(_deltaX, _deltaY);

            BasementTile temp = null;
            if (Main.Instance.CurrentTile.floor.TileMap.TryGetValue(delta, out temp))
            {
                if (temp.tileType == Define.TileType.Facility || temp.tileType == Define.TileType.Entrance || temp.tileType == Define.TileType.Exit)
                {
                    GameManager.Facility.RemoveFacility(temp.placementable as Facility);
                }
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
    void CreateAll(string prefab, bool isUnchangeable = false)
    {
        if (Create(prefab, Main.Instance.CurrentBoundary, isUnchangeable))
        {
            CreateOver();
        }
        else
        {
            Debug.Log("배치할 수 없음");
        }
    }

    void CreateOnlyOne(string prefab)
    {
        if (CreateUnique(prefab, Main.Instance.CurrentBoundary))
        {
            CreateOver();
        }
        else
        {
            Debug.Log("배치할 수 없음");
        }
    }


    void CreateTwo(string prefab1, string prefab2, Vector2Int[] boundary1, Vector2Int[] boundary2)
    {
        if (Create(prefab1, boundary1) && Create(prefab2, boundary2))
        {
            CreateOver();
        }
        else
        {
            Debug.Log("배치할 수 없음");
        }
    }

    
    void CreateTrap(string prefab, Trap_Base.TrapType trapType, bool isUnchangeable = true)
    {
        IPlacementable[] traparray = null;
        if (Create_Trap(prefab, Main.Instance.CurrentBoundary, isUnchangeable, out traparray))
        {
            CreateOver();
            foreach (var item in traparray)
            {
                var trap = item as Trap_Base;
                trap.trapType = trapType;
            }
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
        Managers.UI.PausePopUp();

        if (GameObject.FindObjectOfType<UI_Placement_Facility>().Mode == UI_Placement_Facility.FacilityMode.All)
        {
            GameManager.Instance.StartCoroutine(ShowAllFloor(vector2Ints, action, buildMode));
            return;
        }

        Main.Instance.CurrentBoundary = vector2Ints;
        Main.Instance.CurrentAction += action;

        Main.Instance.CurrentFloor.UI_Floor.ShowTile();
        Main.Instance.CurrentFloor.UI_Floor.Mode = buildMode;
    }

    IEnumerator ShowAllFloor(Vector2Int[] vector2Ints, Action action, UI_Floor.BuildMode buildMode = UI_Floor.BuildMode.Build)
    {
        Managers.UI.ShowPopUpAlone<UI_DungeonPlacement>();
        yield return new WaitForEndOfFrame();

        Main.Instance.CurrentBoundary = vector2Ints;
        Main.Instance.CurrentAction += action;

        for (int i = 0; i < Main.Instance.ActiveFloor_Basement; i++)
        {
            Main.Instance.Floor[i].UI_Floor.ShowTile();
            Main.Instance.Floor[i].UI_Floor.Mode = buildMode;
        }
    }

    void ResetAction()
    {
        Main.Instance.CurrentBoundary = null;
        Main.Instance.CurrentAction = null;
        Main.Instance.CurrentTile = null;
    }

    void CreateOver()
    {
        Debug.Log($"{Main.Instance.CurrentTile.index} : {Main.Instance.CurrentTile.placementable.Name_KR} 설치완료");

        //Main.Instance.CurrentDay.SubtractMana(mana);
        //Main.Instance.CurrentDay.SubtractGold(gold);

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
            Vector2Int delta = tile.index + item;
            BasementTile temp = null;
            if (Main.Instance.CurrentTile.floor.TileMap.TryGetValue(delta, out temp))
            {
                var info = new PlacementInfo(Main.Instance.CurrentTile.floor, temp);
                GameManager.Facility.CreateFacility(prefab, info, isUnChangeable);
            }
        }

        return true;
    }
    bool Create_Trap(string prefab, Vector2Int[] boundary, bool isUnChangeable, out IPlacementable[] placementable)
    {
        if (Main.Instance.CurrentTile == null)
        {
            placementable = null;
            return false; 
        }

        placementable = new IPlacementable[boundary.Length];
        int index = 0;

        var tile = Main.Instance.CurrentTile;
        foreach (var item in boundary)
        {
            Vector2Int delta = tile.index + item;
            BasementTile temp = null;
            if (Main.Instance.CurrentTile.floor.TileMap.TryGetValue(delta, out temp))
            {
                var info = new PlacementInfo(Main.Instance.CurrentTile.floor, temp);
                placementable[index] = GameManager.Facility.CreateFacility(prefab, info, isUnChangeable);
                index++;
            }
        }

        return true;
    }


    bool CreateUnique(string prefab, Vector2Int[] boundary)
    {
        if (Main.Instance.CurrentTile == null) return false;

        var tile = Main.Instance.CurrentTile;
        foreach (var item in boundary)
        {
            Vector2Int delta = tile.index + item;
            BasementTile temp = null;
            if (Main.Instance.CurrentTile.floor.TileMap.TryGetValue(delta, out temp))
            {
                var info = new PlacementInfo(Main.Instance.CurrentTile.floor, temp);
                GameManager.Facility.CreateFacility_OnlyOne(prefab, info, true);
            }
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
    public int need_AP;

    public string contentName;
    public string path;

    public List<Option> boundaryOption;
    public ContentData(string _contentName)
    {
        contentName = _contentName;
        boundaryOption = new List<Option>();
    }

    public void AddOption(string _text, Vector2Int[] _option, Action _action, int mana = 0, int gold = 0, int ap = 0)
    {
        boundaryOption.Add(new Option(_text, _option, _action, mana, gold, ap));
    }

    public void SetName(string title, string box)
    {
        name_Placement = title;
        name_Detail = box;
    }
    public void SetCondition(int mana, int gold, int lv, int ap = 0)
    {
        need_Mana = mana;
        need_Gold = gold;
        need_LV = lv;
        need_AP = ap;
    }
}

public class Option
{
    public string optionText;
    public Vector2Int[] boundary;

    public int addMana;
    public int addGold;
    public int addAP;
    public Action Action { get; set; }

    public Option(string option, Vector2Int[] _bounds, Action _action, int _mana, int _gold, int _ap)
    {
        optionText = option;
        boundary = _bounds;
        Action = _action;
        addMana = _mana;
        addGold = _gold;
        addAP = _ap;
    }
}