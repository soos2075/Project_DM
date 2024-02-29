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
    }


    void AddContents()
    {
        {
            ContentData content = new ContentData("Clear");
            content.SetName("철거 (1x1)", "힘들게 설치한 시설을 철거합니다. 마나와 골드는 회수할 수 없지만, 철거비용을 안받는게 어디에요." +
                "\n적용 범위는 1 x 1 입니다.");
            content.SetCondition(0, 0, 1, Facility_Priority.System);
            content.sprite = Managers.Sprite.GetSprite("Nothing");
            content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => ClearAll(), UI_Floor.BuildMode.Clear));

            Contents.Add(content);
        }
        {
            ContentData content = new ContentData("Clear");
            content.SetName("철거 (3x3)", "힘들게 설치한 시설을 철거합니다. 마나와 골드는 회수할 수 없지만, 철거비용을 안받는게 어디에요." +
                "\n적용 범위는 3 x 3 입니다.");
            content.SetCondition(0, 0, 1, Facility_Priority.System);
            content.sprite = Managers.Sprite.GetSprite("Nothing");
            content.SetAction(() => SetBoundary(Define.Boundary_3x3, () => ClearAll(), UI_Floor.BuildMode.Clear));
            Contents.Add(content);
        }


        {
            ContentData content = new ContentData("Herb_Low");
            content = new ContentData("Herb");
            content.SetName("하급 약초밭(소)", "하급 약초가 자라나는 약초밭을 설치합니다. 흔하긴 해도 마나를 지니고 있기 때문에 쓰임새는 많은 편이에요." +
                "\n적용 범위는 2 x 2 입니다.");
            content.SetCondition(25, 0, 1, Facility_Priority.Herb);
            content.sprite = Managers.Sprite.GetSprite_SLA("Low");
            content.SetAction(() => SetBoundary(Define.Boundary_2x2, () => CreateAll<Herb>("Herb", (int)Herb.HerbType.Low)));

            Contents.Add(content);
        }
        {
            ContentData content = new ContentData("Herb_Low");
            content = new ContentData("Herb");
            content.SetName("흔한 약초밭(중)", "하급 약초가 자라나는 약초밭을 설치합니다. 흔하긴 해도 마나를 지니고 있기 때문에 쓰임새는 많은 편이에요." +
                "\n적용 범위는 3 x 3 입니다.");
            content.SetCondition(50, 0, 1, Facility_Priority.Herb);
            content.sprite = Managers.Sprite.GetSprite_SLA("Low");
            content.SetAction(() => SetBoundary(Define.Boundary_3x3, () => CreateAll<Herb>("Herb", (int)Herb.HerbType.Low)));

            Contents.Add(content);
        }
        {
            ContentData content = new ContentData("Herb_Low");
            content = new ContentData("Herb_Low");
            content.SetName("흔한 약초밭(대)", "하급 약초가 자라나는 약초밭을 설치합니다. 흔하긴 해도 마나를 지니고 있기 때문에 쓰임새는 많은 편이에요." +
                "\n적용 범위는 5 x 5 입니다.");
            content.SetCondition(100, 0, 1, Facility_Priority.Herb);
            content.sprite = Managers.Sprite.GetSprite_SLA("Low");
            content.SetAction(() => SetBoundary(Define.Boundary_5x5, () => CreateAll<Herb>("Herb", (int)Herb.HerbType.Low)));

            Contents.Add(content);
        }


        {
            ContentData content = new ContentData("Herb_High");
            content = new ContentData("Herb_High");
            content.SetName("고급 약초밭", "고급 약초가 자라나는 약초밭을 설치합니다. 쉽게 구할 수 없는 귀한 약초입니다. 마나 효율이 높아요." +
                "\n적용 범위는 1 x 3 입니다.");
            content.SetCondition(75, 0, 1, Facility_Priority.Herb);
            content.sprite = Managers.Sprite.GetSprite_SLA("High");
            content.SetAction(() => SetBoundary(Define.Boundary_1x3, () => CreateAll<Herb>("Herb", (int)Herb.HerbType.High)));

            Contents.Add(content);
        }


        {
            ContentData content = new ContentData("Mineral");
            content = new ContentData("Mineral");
            content.SetName("작은 광맥", "유용한 물질을 얻을 수 있는 광맥을 설치합니다. 여러 광물이 섞여있어요." +
                "\n적용 범위는 작은 십자 입니다.");
            content.SetCondition(50, 0, 1, Facility_Priority.Mineral);
            content.sprite = Managers.Sprite.GetSprite("Mineral");
            content.SetAction(() => SetBoundary(Define.Boundary_Cross_1, () =>
                CreateCustom(Create<Mineral>("Mineral", Define.Boundary_1x1, (int)Mineral.MienralCategory.Stone),
                        Create<Mineral>("Mineral", Define.Boundary_Side_Cross, (int)Mineral.MienralCategory.Rock))));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Mineral");
            content = new ContentData("Mineral");
            content.SetName("큰 광맥", "유용한 물질을 얻을 수 있는 광맥을 설치합니다. 여러 광물이 섞여있어요." +
                "\n적용 범위는 큰 마름모 입니다.");
            content.SetCondition(100, 0, 1, Facility_Priority.Mineral);
            content.sprite = Managers.Sprite.GetSprite("Mineral");
            content.SetAction(() => SetBoundary(Define.Boundary_Cross_3, () =>
                    CreateCustom(Create<Mineral>("Mineral", Define.Boundary_Cross_1, (int)Mineral.MienralCategory.Rock),
                        Create<Mineral>("Mineral", Define.Boundary_Side_X, (int)Mineral.MienralCategory.Stone),
                        Create<Mineral>("Mineral", Define.Boundary_Side_Cross_2, (int)Mineral.MienralCategory.Sand))));

            Contents.Add(content);
        }



        {
            ContentData content = new ContentData("Trap_Fallen_1");
            content.SetName("낙하 함정", "고전식 발밑 함정을 설치합니다. 발동되면 시간과 체력에 피해를 줄 수 있어요." +
                "\n적용 범위는 1 x 1 입니다.");
            content.SetCondition(0, 50, 1, Facility_Priority.Trap);
            content.sprite = Managers.Sprite.GetSprite_SLA("Fallen_1");
            content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateAll<Trap>("Trap", (int)Trap.TrapType.Fallen_1)));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Sword");
            content = new ContentData("Sword");
            content.SetName("쓸만한 무기", "모험가들이 좋아할만한 무기를 설치합니다.\n" +
                "모험가에게 발견되면 던전의 인기도가 조금 올라요.");
            content.SetCondition(0, 30, 1, Facility_Priority.Treasure);
            content.sprite = Managers.Sprite.GetSprite_SLA("Sword");
            content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateAll<Treasure>("Treasure", (int)Treasure.TreasureCategory.Sword)));

            Contents.Add(content);
        }
    }

    public void AddLevel2()
    {
        {
            ContentData content = new ContentData("Herb_Pumpkin");
            content = new ContentData("Herb_Pumpkin");
            content.SetName("마나 호박밭", "마나를 지닌 호박입니다. 최고급 식재료로도 사용이 되며 기타 여러 합성에 재료로 사용됩니다." +
                "\n적용 범위는 작은 X 입니다.");
            content.SetCondition(125, 0, 1, Facility_Priority.Herb);
            content.sprite = Managers.Sprite.GetSprite_SLA("Pumpkin");
            content.SetAction(() => SetBoundary(Define.Boundary_X_1, () => CreateAll<Herb>("Herb", (int)Herb.HerbType.Pumpkin)));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Entrance");
            content.SetName("입구", "플레이어가 들어올 입구를 지정합니다. 만약 입구가 없으면 랜덤위치에 자동으로 생성됩니다. 입구는 층 당 한개만 존재할 수 있어요.");
            content.SetCondition(50, 0, 2, Facility_Priority.System);
            content.sprite = Managers.Sprite.GetSprite("Entrance");

            content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateOnlyOne("Entrance")));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Exit");
            content.SetName("출구", "플레이어가 나갈 출구를 지정합니다. 만약 출구가 없으면 랜덤위치에 자동으로 생성됩니다. 출구는 층 당 한개만 존재할 수 있어요.");
            content.SetCondition(50, 0, 2, Facility_Priority.System);
            content.sprite = Managers.Sprite.GetSprite("Exit");

            content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateOnlyOne("Exit")));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Mineral_Lv2");
            content = new ContentData("Mineral_Lv2");
            content.SetName("단단한 광맥", "유용한 물질을 얻을 수 있는 광맥을 설치합니다. 마나를 많이 소모 할 수록 더 단단하고 귀한 물질로 되어있어요." +
                "\n적용 범위는 3 x 1 입니다.");
            content.SetCondition(100, 0, 2, Facility_Priority.Mineral);
            content.sprite = Managers.Sprite.GetSprite_SLA("Stone");
            content.SetAction(() => SetBoundary(Define.Boundary_3x1, () => CreateAll<Mineral>("Mineral", (int)Mineral.MienralCategory.Stone)));

            Contents.Add(content);
        }
        {
            ContentData content = new ContentData("Mineral_Lv2");
            content = new ContentData("Mineral_Lv2");
            content.SetName("철 광맥", "유용한 물질을 얻을 수 있는 광맥을 설치합니다. 마나를 많이 소모 할 수록 더 단단하고 귀한 물질로 되어있어요." +
                "\n적용 범위는 3 x 1 입니다.");
            content.SetCondition(150, 0, 2, Facility_Priority.Mineral);
            content.sprite = Managers.Sprite.GetSprite_SLA("Iron");
            content.SetAction(() => SetBoundary(Define.Boundary_3x1, () => CreateAll<Mineral>("Mineral", (int)Mineral.MienralCategory.Iron)));

            Contents.Add(content);
        }
        {
            ContentData content = new ContentData("Mineral_Lv2");
            content = new ContentData("Mineral_Lv2");
            content.SetName("석탄 광맥", "유용한 물질을 얻을 수 있는 광맥을 설치합니다. 마나를 많이 소모 할 수록 더 단단하고 귀한 물질로 되어있어요." +
                "\n적용 범위는 3 x 1 입니다.");
            content.SetCondition(200, 0, 2, Facility_Priority.Mineral);
            content.sprite = Managers.Sprite.GetSprite_SLA("Coal");
            content.SetAction(() => SetBoundary(Define.Boundary_3x1, () => CreateAll<Mineral>("Mineral", (int)Mineral.MienralCategory.Coal)));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Trap_Fallen_2");
            content.SetName("강화 낙화 함정", "고전식 발밑 함정을 설치합니다. 기존의 함정보다조금 더 효과가 강해졌어요." +
                "\n적용 범위는 1 x 1 입니다.");
            content.SetCondition(0, 100, 2, Facility_Priority.Trap);
            content.sprite = Managers.Sprite.GetSprite_SLA("Fallen_2");
            content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateAll<Trap>("Trap", (int)Trap.TrapType.Fallen_2)));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Trap_Awl");
            content.SetName("송곳 함정", "바닥에서 송곳이 올라오는 무시무시한 함정을 설치합니다. 발동되면 큰 피해를 입힙니다." +
                "\n적용 범위는 1 x 1 입니다.");
            content.SetCondition(0, 150, 2, Facility_Priority.Trap);
            content.sprite = Managers.Sprite.GetSprite_SLA("Awl_1");

            content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateAll<Trap>("Trap", (int)Trap.TrapType.Awl_1)));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Ring");
            content = new ContentData("Ring");
            content.SetName("매직 링", "던전의 마나를 사용해서 만들어낸 마법 반지입니다. 착용자의 마나를 조금 흡수하는 효과가 걸려있습니다.\n" +
                "모험가에게 발견되면 던전의 인기도가 조금 올라요.");
            content.SetCondition(0, 50, 1, Facility_Priority.Treasure);
            content.sprite = Managers.Sprite.GetSprite_SLA("Ring");
            content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateAll<Treasure>("Treasure", (int)Treasure.TreasureCategory.Ring)));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Scroll");
            content = new ContentData("Scroll");
            content.SetName("마법 스크롤", "고대의 주문이 적힌 스크롤입니다. 고위 마법사들이 마법을 배우기 위해 필요한 아이템으로 인기가 많습니다.");
            content.SetCondition(0, 75, 1, Facility_Priority.Treasure);
            content.sprite = Managers.Sprite.GetSprite_SLA("Scroll");
            content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateAll<Treasure>("Treasure", (int)Treasure.TreasureCategory.Scroll)));

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
                if (temp.tileType_Original == Define.TileType.Facility)
                {
                    var facil = temp.Original as Facility;
                    if (facil.isClearable)
                    {
                        GameManager.Facility.RemoveFacility(temp.Original as Facility);
                    }
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
    void CreateAll<T>(string prefab, int _optionIndex) where T : Facility
    {
        if (Create<T>(prefab, Main.Instance.CurrentBoundary, _optionIndex))
        {
            CreateOver();
        }
        else
        {
            Debug.Log("배치할 수 없음");
        }
    }

    void CreateCustom(bool _create1, bool _create2 = true, bool _create3 = true)
    {
        if (_create1 && _create2 && _create3)
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

    
    //void CreateTrap(string prefab, Trap.TrapType trapType, bool isUnchangeable = true)
    //{
    //    IPlacementable[] traparray = null;
    //    if (Create_Trap(prefab, Main.Instance.CurrentBoundary, isUnchangeable, out traparray))
    //    {
    //        CreateOver();
    //        foreach (var item in traparray)
    //        {
    //            var trap = item as Trap;
    //            trap.trapType = trapType;
    //        }
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
        Time.timeScale = 1;

        GameManager.Instance.StartCoroutine(ShowAllFloor(vector2Ints, action, buildMode));

        //if (GameObject.FindObjectOfType<UI_Placement_Facility>().Mode == UI_Placement_Facility.FacilityMode.All)
        //{
        //    GameManager.Instance.StartCoroutine(ShowAllFloor(vector2Ints, action, buildMode));
        //    return;
        //}

        //Main.Instance.CurrentBoundary = vector2Ints;
        //Main.Instance.CurrentAction += action;

        //Main.Instance.CurrentFloor.UI_Floor.ShowTile();
        //Main.Instance.CurrentFloor.UI_Floor.Mode = buildMode;
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
        Main.Instance.PurchaseAction = null;
        Managers.UI.ClosePopupPick(GameObject.FindAnyObjectByType<UI_DungeonPlacement>());
        Managers.UI.PauseOpen();
        Time.timeScale = 0;
    }

    void CreateOver()
    {
        Main.Instance.PurchaseAction.Invoke();

        ResetAction();
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
                GameManager.Facility.CreateFacility(prefab, info);
            }
        }

        return true;
    }

    bool Create<T>(string prefab, Vector2Int[] boundary, int _optionIndex) where T : Facility
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
                GameManager.Facility.CreateFacility(prefab, info, _optionIndex);
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

public enum Facility_Priority // 상점에서 contents 정렬시 우선순위
{
    System = 0,

    Herb = 100,

    Mineral = 200,

    Trap = 300,



    Treasure = 400,

    Artifact = 500,

    Etc = 900,
}
// 상품 종류별 정렬, 숫자가 낮을수록 우선순위가 높음 
// 1순위비교 - Facility_Priority
// 2순위 - 요구레벨
// 3순위 - 필요 마나
// 4순위 - 필요 골드
public class ContentComparer : IComparer<ContentData>
{
    public int Compare(ContentData x, ContentData y)
    {
        if (x.priority > y.priority)
        {
            return 1;
        }
        else if (x.priority < y.priority)
        {
            return -1;
        }
        else
        {
            var compare_lv = x.need_LV.CompareTo(y.need_LV);
            if (compare_lv == 0)
            {
                var compare_mana = x.need_Mana.CompareTo(y.need_Mana);
                if (compare_mana == 0)
                {
                    return x.need_Gold.CompareTo(y.need_Gold);
                }
                else
                {
                    return compare_mana;
                }
            }
            else
            {
                return compare_lv;
            }
        }
    }
}

public class ContentData
{
    public Facility_Priority priority;

    public Sprite sprite;
    public string name_Placement;
    public string name_Detail;

    public int need_Mana;
    public int need_Gold;
    public int need_LV;

    public string contentName;
    public string path;

    public Action contentAction;

    //public List<Option> boundaryOption;
    public ContentData(string _contentName)
    {
        contentName = _contentName;
        //boundaryOption = new List<Option>();
    }

    //public void AddOption(string _text, Vector2Int[] _option, Action _action, int mana = 0, int gold = 0)
    //{
    //    boundaryOption.Add(new Option(_text, _option, _action, mana, gold));
    //}

    public void SetName(string title, string box)
    {
        name_Placement = title;
        name_Detail = box;
    }
    public void SetCondition(int mana, int gold, int lv, Facility_Priority _Priority)
    {
        need_Mana = mana;
        need_Gold = gold;
        need_LV = lv;
        priority = _Priority;
    }

    public void SetAction(Action _action)
    {
        contentAction = _action;
    }

}

public class Option
{
    public string optionText;
    public Vector2Int[] boundary;

    public int addMana;
    public int addGold;
    public Action Action { get; set; }

    public Option(string option, Vector2Int[] _bounds, Action _action, int _mana, int _gold)
    {
        optionText = option;
        boundary = _bounds;
        Action = _action;
        addMana = _mana;
        addGold = _gold;
    }
}