using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContentManager
{
    public void Init()
    {
        Init_LocalData();
        //AddContents();
    }


    #region SO Data
    Dictionary<string, SO_Contents> Contents_Dictionary { get; set; }
    SO_Contents[] so_data;

    public void Init_LocalData()
    {
        Contents_Dictionary = new Dictionary<string, SO_Contents>();

        so_data = Resources.LoadAll<SO_Contents>("Data/Contents");
        foreach (var item in so_data)
        {
            string[] datas = null;
            switch (UserData.Instance.Language)
            {
                case Define.Language.EN:
                    Managers.Data.ObjectsLabel_EN.TryGetValue(item.id, out datas);
                    break;

                case Define.Language.KR:
                    Managers.Data.ObjectsLabel_KR.TryGetValue(item.id, out datas);
                    break;

                case Define.Language.JP:
                    Managers.Data.ObjectsLabel_JP.TryGetValue(item.id, out datas);
                    break;
            }

            if (datas == null)
            {
                Debug.Log($"{item.id} : CSV Data Not Exist");
                continue;
            }

            item.labelName = datas[0];
            item.detail = datas[1];
            item.boundary = datas[2];


            if (item.Options.Count == 1)
            {
                if (item.isOnlyOne)
                {
                    Vector2Int[] boundary = Util.GetBoundary(item.Boundary_All);
                    item.action = () => SetBoundary(boundary, () => CreateOnlyOne(item.Options[0].FacilityKeyName));
                }
                else
                {
                    Vector2Int[] boundary = Util.GetBoundary(item.Boundary_All);
                    item.action = () => SetBoundary(boundary, () => CreateAll(item.Options[0].FacilityKeyName));
                }
            }
            else
            {
                List<Func<bool>> customFuncList = new List<Func<bool>>();

                if (item.Options.Count > 0)
                {
                    Vector2Int[] boundary = Util.GetBoundary(item.Options[0].Boundary);
                    customFuncList.Add(() => Create(item.Options[0].FacilityKeyName, boundary));
                }
                if (item.Options.Count > 1)
                {
                    Vector2Int[] boundary = Util.GetBoundary(item.Options[1].Boundary);
                    customFuncList.Add(() => Create(item.Options[1].FacilityKeyName, boundary));
                }
                if (item.Options.Count > 2)
                {
                    Vector2Int[] boundary = Util.GetBoundary(item.Options[2].Boundary);
                    customFuncList.Add(() => Create(item.Options[2].FacilityKeyName, boundary));
                }

                item.action = () => SetBoundary(Util.GetBoundary(item.Boundary_All), () => CreateCustom(customFuncList));
            }


            Contents_Dictionary.Add(item.keyName, item);
        }
    }

    public List<SO_Contents> GetContentsList(int _DungeonRank = 1)
    {
        List<SO_Contents> list = new List<SO_Contents>();

        foreach (var item in so_data)
        {
            if (item.UnlockRank <= _DungeonRank)
            {
                list.Add(item);
            }
        }

        list.Sort((a, b) => a.id.CompareTo(b.id));

        //list.Sort(new ContentComparer());
        return list;
    }


    public SO_Contents GetData(string _keyName)
    {
        SO_Contents content = null;
        if (Contents_Dictionary.TryGetValue(_keyName, out content))
        {
            return content;
        }

        Debug.Log($"{_keyName}: Data Not Exist");
        return null;
    }


    #endregion




    //public Dictionary<string, ContentData> EventContentsDic { get; set; } = new Dictionary<string, ContentData>();


    //public List<ContentData> Contents { get; set; } = new List<ContentData>();




    //void AddContents()
    //{
    //    {
    //        ContentData content = new ContentData("Clear");
    //        content.SetName("철거 (1x1)", "힘들게 설치한 시설을 철거합니다. 마나와 골드는 회수할 수 없지만, 철거비용을 안받는게 어디에요." +
    //            "\n적용 범위는 1 x 1 입니다.");
    //        content.SetCondition(0, 0, 1, Facility_Priority.System);
    //        content.sprite = Managers.Sprite.GetSprite("Nothing");
    //        content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => ClearAll(), UI_Floor.BuildMode.Clear));

    //        Contents.Add(content);
    //    }
    //    {
    //        ContentData content = new ContentData("Clear");
    //        content.SetName("철거 (3x3)", "힘들게 설치한 시설을 철거합니다. 마나와 골드는 회수할 수 없지만, 철거비용을 안받는게 어디에요." +
    //            "\n적용 범위는 3 x 3 입니다.");
    //        content.SetCondition(0, 0, 1, Facility_Priority.System);
    //        content.sprite = Managers.Sprite.GetSprite("Nothing");
    //        content.SetAction(() => SetBoundary(Define.Boundary_3x3, () => ClearAll(), UI_Floor.BuildMode.Clear));
    //        Contents.Add(content);
    //    }


    //    //{
    //    //    ContentData content = new ContentData("Herb_Low");
    //    //    content = new ContentData("Herb");
    //    //    content.SetName("흔한 약초밭(소)", "하급 약초가 자라나는 약초밭을 설치합니다. 흔하긴 해도 마나를 지니고 있기 때문에 쓰임새는 많은 편이에요." +
    //    //        "\n적용 범위는 2 x 2 입니다.");
    //    //    content.SetCondition(25, 0, 1, Facility_Priority.Herb);
    //    //    content.sprite = Managers.Sprite.GetSprite_SLA("Low");
    //    //    content.SetAction(() => SetBoundary(Define.Boundary_2x2, () => CreateAll<Herb>("Herb", (int)Herb.HerbType.Low)));

    //    //    Contents.Add(content);
    //    //}
    //    {
    //        ContentData content = new ContentData("Herb_Low");
    //        content = new ContentData("Herb");
    //        content.SetName("작은 약초밭", "하급 약초가 자라나는 약초밭을 설치합니다. 흔하긴 해도 마나를 지니고 있기 때문에 쓰임새는 많은 편이에요." +
    //            "\n적용 범위는 3 x 3 입니다.");
    //        content.SetCondition(50, 0, 1, Facility_Priority.Herb);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("Low");
    //        content.SetAction(() => SetBoundary(Define.Boundary_3x3, () => CreateAll<Herb>("Herb_Low")));

    //        Contents.Add(content);
    //    }

    //    {
    //        ContentData content = new ContentData("Herb_Low");
    //        content = new ContentData("Herb_Low");
    //        content.SetName("큰 약초밭", "하급 약초가 자라나는 약초밭을 설치합니다. 흔하긴 해도 마나를 지니고 있기 때문에 쓰임새는 많은 편이에요." +
    //            "\n적용 범위는 5 x 5 입니다.");
    //        content.SetCondition(100, 0, 1, Facility_Priority.Herb);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("Low");
    //        content.SetAction(() => SetBoundary(Define.Boundary_5x5, () => CreateAll<Herb>("Herb_Low")));

    //        Contents.Add(content);
    //    }


    //    {
    //        ContentData content = new ContentData("Herb_High");
    //        content = new ContentData("Herb_High");
    //        content.SetName("고급 약초밭", "고급 약초가 자라나는 약초밭을 설치합니다. 쉽게 구할 수 없는 귀한 약초입니다. 마나 효율이 높아요." +
    //            "\n적용 범위는 1 x 3 입니다.");
    //        content.SetCondition(125, 0, 1, Facility_Priority.Herb);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("High");
    //        content.SetAction(() => SetBoundary(Define.Boundary_1x3, () => CreateAll<Herb>("Herb_High")));

    //        Contents.Add(content);
    //    }


    //    {
    //        ContentData content = new ContentData("Mineral_small");
    //        content = new ContentData("Mineral");
    //        content.SetName("작은 광맥", "유용한 물질을 얻을 수 있는 광맥을 설치합니다. 여러 광물이 섞여있어요." +
    //            "\n적용 범위는 작은 십자 입니다.");
    //        content.SetCondition(50, 0, 1, Facility_Priority.Mineral);
    //        content.sprite = Managers.Sprite.GetSprite("Mineral");
    //        content.SetAction(() => SetBoundary(Define.Boundary_Cross_1, () =>
    //            CreateCustom(Create<Mineral>("Mineral_Stone", Define.Boundary_1x1),
    //                    Create<Mineral>("Mineral_Rock", Define.Boundary_Side_Cross))));

    //        Contents.Add(content);
    //    }

    //    {
    //        ContentData content = new ContentData("Mineral_large");
    //        content = new ContentData("Mineral");
    //        content.SetName("큰 광맥", "유용한 물질을 얻을 수 있는 광맥을 설치합니다. 여러 광물이 섞여있어요." +
    //            "\n적용 범위는 큰 마름모 입니다.");
    //        content.SetCondition(100, 0, 1, Facility_Priority.Mineral);
    //        content.sprite = Managers.Sprite.GetSprite("Mineral");
    //        content.SetAction(() => SetBoundary(Define.Boundary_Cross_3, () =>
    //                CreateCustom(Create<Mineral>("Mineral_Rock", Define.Boundary_Cross_1),
    //                    Create<Mineral>("Mineral_Stone", Define.Boundary_Side_X),
    //                    Create<Mineral>("Mineral_Sand", Define.Boundary_Side_Cross_2))));

    //        Contents.Add(content);
    //    }



    //    {
    //        ContentData content = new ContentData("Trap_Fallen_1");
    //        content.SetName("낙하 함정", "고전식 발밑 함정을 설치합니다. 발동되면 시간과 체력에 피해를 줄 수 있어요." +
    //            "\n적용 범위는 1 x 1 입니다.");
    //        content.SetCondition(0, 20, 1, Facility_Priority.Trap);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("Fallen_1");
    //        content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateAll<Trap>("Trap_Fallen_1")));

    //        Contents.Add(content);
    //    }

    //    {
    //        ContentData content = new ContentData("Sword");
    //        content = new ContentData("Sword");
    //        content.SetName("쓸만한 무기", "모험가들이 좋아할만한 무기를 설치합니다.\n" +
    //            "모험가에게 발견되면 던전의 인기도가 조금 올라요.");
    //        content.SetCondition(0, 30, 1, Facility_Priority.Treasure);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("Sword");
    //        content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateAll<Treasure>("Treasure_Sword")));

    //        Contents.Add(content);
    //    }
    //}

    //public void AddLevel2()
    //{
    //    {
    //        ContentData content = new ContentData("Herb_Pumpkin");
    //        content = new ContentData("Herb_Pumpkin");
    //        content.SetName("마나 호박밭", "마나를 지닌 호박입니다. 최고급 식재료로도 사용이 되며 기타 여러 합성에 재료로 사용됩니다." +
    //            "\n적용 범위는 작은 X 입니다.");
    //        content.SetCondition(85, 0, 2, Facility_Priority.Herb);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("Pumpkin");
    //        content.SetAction(() => SetBoundary(Define.Boundary_X_1, () => CreateAll<Herb>("Herb_Pumpkin")));

    //        Contents.Add(content);
    //    }

    //    {
    //        ContentData content = new ContentData("Entrance");
    //        content.SetName("입구", "입구의 위치를 지정합니다. 입구는 층 당 한개만 존재할 수 있어요. 만약 입구가 없으면 시작시 자동으로 생성됩니다.");
    //        content.SetCondition(50, 0, 2, Facility_Priority.System);
    //        content.sprite = Managers.Sprite.GetSprite("Entrance");

    //        content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateOnlyOne("Entrance")));

    //        Contents.Add(content);
    //    }

    //    {
    //        ContentData content = new ContentData("Exit");
    //        content.SetName("출구", "출구의 위치를 지정합니다. 출구는 층 당 한개만 존재할 수 있어요. 만약 출구가 없으면 시작시 자동으로 생성됩니다.");
    //        content.SetCondition(50, 0, 2, Facility_Priority.System);
    //        content.sprite = Managers.Sprite.GetSprite("Exit");

    //        content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateOnlyOne("Exit")));

    //        Contents.Add(content);
    //    }

    //    {
    //        ContentData content = new ContentData("Mineral_Lv2");
    //        content = new ContentData("Mineral_Lv2");
    //        content.SetName("단단한 광맥", "유용한 물질을 얻을 수 있는 광맥을 설치합니다. 마나를 많이 소모 할 수록 더 단단하고 귀한 물질로 되어있어요." +
    //            "\n적용 범위는 3 x 1 입니다.");
    //        content.SetCondition(100, 0, 2, Facility_Priority.Mineral);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("Stone");
    //        content.SetAction(() => SetBoundary(Define.Boundary_3x1, () => CreateAll<Mineral>("Mineral_Stone")));

    //        Contents.Add(content);
    //    }
    //    {
    //        ContentData content = new ContentData("Mineral_Lv2");
    //        content = new ContentData("Mineral_Lv2");
    //        content.SetName("석탄 광맥", "유용한 물질을 얻을 수 있는 광맥을 설치합니다. 마나를 많이 소모 할 수록 더 단단하고 귀한 물질로 되어있어요." +
    //            "\n적용 범위는 3 x 1 입니다.");
    //        content.SetCondition(150, 0, 2, Facility_Priority.Mineral);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("Coal");
    //        content.SetAction(() => SetBoundary(Define.Boundary_3x1, () => CreateAll<Mineral>("Mineral_Coal")));

    //        Contents.Add(content);
    //    }
    //    {
    //        ContentData content = new ContentData("Mineral_Lv2");
    //        content = new ContentData("Mineral_Lv2");
    //        content.SetName("마나 철광맥", "유용한 물질을 얻을 수 있는 광맥을 설치합니다. 마나를 많이 소모 할 수록 더 단단하고 귀한 물질로 되어있어요." +
    //            "\n적용 범위는 3 x 1 입니다.");
    //        content.SetCondition(200, 0, 2, Facility_Priority.Mineral);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("Iron");
    //        content.SetAction(() => SetBoundary(Define.Boundary_3x1, () => CreateAll<Mineral>("Mineral_Iron")));

    //        Contents.Add(content);
    //    }

    //    {
    //        ContentData content = new ContentData("Trap_Fallen_2");
    //        content.SetName("강화 낙화 함정", "고전식 발밑 함정을 설치합니다. 기존의 함정보다조금 더 효과가 강해졌어요." +
    //            "\n적용 범위는 1 x 1 입니다.");
    //        content.SetCondition(0, 45, 2, Facility_Priority.Trap);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("Fallen_2");
    //        content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateAll<Trap>("Trap_Fallen_2")));

    //        Contents.Add(content);
    //    }

    //    {
    //        ContentData content = new ContentData("Trap_Awl_1");
    //        content.SetName("송곳 함정", "바닥에서 송곳이 올라오는 무시무시한 함정을 설치합니다. 발동되면 큰 피해를 입힙니다." +
    //            "\n적용 범위는 1 x 1 입니다.");
    //        content.SetCondition(0, 125, 2, Facility_Priority.Trap);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("Awl_1");

    //        content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateAll<Trap>("Trap_Awl_1")));

    //        Contents.Add(content);
    //    }

    //    {
    //        ContentData content = new ContentData("Ring");
    //        content = new ContentData("Ring");
    //        content.SetName("매직 링", "던전의 마나를 사용해서 만들어낸 마법 반지입니다. 착용자의 마나를 조금 흡수하는 효과가 걸려있습니다.\n" +
    //            "모험가에게 발견되면 던전의 인기도가 조금 올라요.");
    //        content.SetCondition(0, 50, 1, Facility_Priority.Treasure);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("Ring");
    //        content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateAll<Treasure>("Treasure_Ring")));

    //        Contents.Add(content);
    //    }

    //    {
    //        ContentData content = new ContentData("Scroll");
    //        content = new ContentData("Scroll");
    //        content.SetName("마법 스크롤", "고대의 주문이 적힌 스크롤입니다. 고위 마법사들이 마법을 배우기 위해 필요한 아이템으로 인기가 많습니다.");
    //        content.SetCondition(0, 75, 1, Facility_Priority.Treasure);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("Scroll");
    //        content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateAll<Treasure>("Treasure_Scroll")));

    //        Contents.Add(content);
    //    }
    //}



    #region Clear 메서드
    void ClearAll()
    {
        if (Clear(Main.Instance.CurrentBoundary))
        {
            ClearOver();
            SoundManager.Instance.PlaySound("SFX/Action_Build");
        }
        else
        {
            Debug.Log("삭제할 수 없음");
            SoundManager.Instance.PlaySound("SFX/Action_Wrong");
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
                    else
                    {
                        return false;
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
    void CreateAll(string _keyName)
    {
        if (Create(_keyName, Main.Instance.CurrentBoundary))
        {
            CreateOver();
            SoundManager.Instance.PlaySound("SFX/Action_Build");
        }
        else
        {
            Debug.Log("배치할 수 없음");
            SoundManager.Instance.PlaySound("SFX/Action_Wrong");
        }
    }

    void CreateCustom(List<Func<bool>> _funcList)
    {
        bool all = true;
        foreach (var item in _funcList)
        {
            all &= item.Invoke();
        }

        if (all)
        {
            CreateOver();
            SoundManager.Instance.PlaySound("SFX/Action_Build");
        }
        else
        {
            Debug.Log("배치할 수 없음");
            SoundManager.Instance.PlaySound("SFX/Action_Wrong");
        }
    }


    void CreateOnlyOne(string prefab)
    {
        if (CreateUnique(prefab, Main.Instance.CurrentBoundary))
        {
            CreateOver();
            SoundManager.Instance.PlaySound("SFX/Action_Build");
        }
        else
        {
            Debug.Log("배치할 수 없음");
            SoundManager.Instance.PlaySound("SFX/Action_Wrong");
        }
    }

    bool Create(string _keyName, Vector2Int[] boundary)
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
                GameManager.Facility.CreateFacility(_keyName, info);
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
                GameManager.Facility.CreateFacility_OnlyOne(prefab, info);
            }
        }

        return true;
    }


    void CreateOver()
    {
        Main.Instance.PurchaseAction.Invoke();

        ResetAction();
    }

    #endregion


    #region Set Action

    void SetBoundary(Vector2Int[] vector2Ints, Action action, UI_Floor.BuildMode buildMode = UI_Floor.BuildMode.Build)
    {
        Managers.UI.PausePopUp();
        UserData.Instance.GamePlay();

        GameManager.Instance.StartCoroutine(ShowAllFloor(vector2Ints, action, buildMode));
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

    #endregion



}

public enum Facility_Priority // 상점에서 contents 정렬시 우선순위
{
    System = 1000,

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
//public class ContentComparer : IComparer<ContentData>
//{
//    public int Compare(ContentData x, ContentData y)
//    {
//        if (x.priority > y.priority)
//        {
//            return 1;
//        }
//        else if (x.priority < y.priority)
//        {
//            return -1;
//        }
//        else
//        {
//            var compare_lv = x.need_LV.CompareTo(y.need_LV);
//            if (compare_lv == 0)
//            {
//                var compare_mana = x.need_Mana.CompareTo(y.need_Mana);
//                if (compare_mana == 0)
//                {
//                    return x.need_Gold.CompareTo(y.need_Gold);
//                }
//                else
//                {
//                    return compare_mana;
//                }
//            }
//            else
//            {
//                return compare_lv;
//            }
//        }
//    }
//}

//public class ContentData
//{
//    public Facility_Priority priority;

//    public Sprite sprite;
//    public string name_Placement;
//    public string name_Detail;

//    public int need_Mana;
//    public int need_Gold;
//    public int need_LV;

//    public string contentName;
//    public string path;

//    public Action contentAction;

//    //public List<Option> boundaryOption;
//    public ContentData(string _contentName)
//    {
//        contentName = _contentName;
//        //boundaryOption = new List<Option>();
//    }

//    //public void AddOption(string _text, Vector2Int[] _option, Action _action, int mana = 0, int gold = 0)
//    //{
//    //    boundaryOption.Add(new Option(_text, _option, _action, mana, gold));
//    //}

//    public void SetName(string title, string box)
//    {
//        name_Placement = title;
//        name_Detail = box;
//    }
//    public void SetCondition(int mana, int gold, int lv, Facility_Priority _Priority)
//    {
//        need_Mana = mana;
//        need_Gold = gold;
//        need_LV = lv;
//        priority = _Priority;
//    }

//    public void SetAction(Action _action)
//    {
//        contentAction = _action;
//    }

//}

//public class Option
//{
//    public string optionText;
//    public Vector2Int[] boundary;

//    public int addMana;
//    public int addGold;
//    public Action Action { get; set; }

//    public Option(string option, Vector2Int[] _bounds, Action _action, int _mana, int _gold)
//    {
//        optionText = option;
//        boundary = _bounds;
//        Action = _action;
//        addMana = _mana;
//        addGold = _gold;
//    }
//}