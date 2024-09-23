using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            string[] datas = Managers.Data.GetTextData_Object(item.id);

            if (datas == null)
            {
                Debug.Log($"{item.id} : CSV Data Not Exist");
                continue;
            }

            item.labelName = datas[0];
            item.detail = datas[1];


            {
                List<Func<bool>> customFuncList = new List<Func<bool>>();

                //int OptionIndex = 0;
                if (item.Options.Count > 0)
                {
                    if (item.isOnlyOne)
                    {
                        if (item.Options[0].FacilityKeyName == "Exit" || item.Options[0].FacilityKeyName == "Entrance")
                        {
                            Vector2Int[] boundary = Util.GetBoundary(Define.Boundary.Boundary_1x1);
                            customFuncList.Add(() => CreatePortal(item.Options[0].FacilityKeyName, boundary));
                        }
                        else
                        {
                            Vector2Int[] boundary = Util.GetBoundary(Define.Boundary.Boundary_1x1);
                            customFuncList.Add(() => CreateUnique(item.Options[0].FacilityKeyName, boundary));
                        }
                    }
                    else
                    {
                        if (item.Options[0].createType == Contents_CreateType.CreateAll)
                        {
                            Vector2Int[] boundary = Util.GetBoundary(item.Options[0].Boundary);
                            customFuncList.Add(() => Create(item.Options[0].FacilityKeyName, boundary));
                        }
                        else if (item.Options[0].createType == Contents_CreateType.CreateOne)
                        {
                            Vector2Int[] boundary = Util.GetBoundary(item.Options[0].Boundary);
                            customFuncList.Add(() => CreateBigOne(item.Options[0].FacilityKeyName, boundary));
                        }
                    }
                }
                if (item.Options.Count > 1)
                {
                    if (item.Options[1].createType == Contents_CreateType.CreateAll)
                    {
                        Vector2Int[] boundary = Util.GetBoundary(item.Options[1].Boundary);
                        customFuncList.Add(() => Create(item.Options[1].FacilityKeyName, boundary));
                    }
                    else if (item.Options[1].createType == Contents_CreateType.CreateOne)
                    {
                        Vector2Int[] boundary = Util.GetBoundary(item.Options[1].Boundary);
                        customFuncList.Add(() => CreateBigOne(item.Options[1].FacilityKeyName, boundary));
                    }
                }
                if (item.Options.Count > 2)
                {
                    if (item.Options[2].createType == Contents_CreateType.CreateAll)
                    {
                        Vector2Int[] boundary = Util.GetBoundary(item.Options[2].Boundary);
                        customFuncList.Add(() => Create(item.Options[2].FacilityKeyName, boundary));
                    }
                    else if (item.Options[2].createType == Contents_CreateType.CreateOne)
                    {
                        Vector2Int[] boundary = Util.GetBoundary(item.Options[2].Boundary);
                        customFuncList.Add(() => CreateBigOne(item.Options[2].FacilityKeyName, boundary));
                    }
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
        //list.Sort((a, b) => a.id.CompareTo(b.id));
        return SortByOption(list);
    }

    public List<SO_Contents> SortByOption(List<SO_Contents> target, ContentsSortOption sortOption = ContentsSortOption.Basic, 
        bool keep = true, bool ascend = true)
    {
        ContentsComparer comparer = new ContentsComparer(sortOption, keep, ascend);
        target.Sort(comparer);
        return target;
    }
    //public List<UI_Facility_Content> SortByOption(List<UI_Facility_Content> target, ContentsSortOption sortOption = ContentsSortOption.Basic)
    //{
    //    //List<SO_Contents> newList = new List<SO_Contents>();
    //    List<SO_Contents> newList = target.Select(item => item.Content).ToList();
    //    SortByOption(newList);

    //    Dictionary<SO_Contents, UI_Facility_Content> contentMap = target.ToDictionary(item => item.Content);

    //    List<UI_Facility_Content> sortedTarget = newList.Select(content => contentMap[content]).ToList();
    //    return sortedTarget;
    //}



    public class ContentsComparer : IComparer<SO_Contents>
    {
        bool PriorityKeep;
        bool Ascending;
        ContentsSortOption SortOption;


        public ContentsComparer(ContentsSortOption option , bool priority = true, bool ascend = true)
        {
            SortOption = option;
            PriorityKeep = priority;
            Ascending = ascend;
        }

        public int Compare(SO_Contents x, SO_Contents y)
        {
            int result = 0;
            if (PriorityKeep) //? 기본 정렬상태 유지(타입)
            {
                int temp = x.priority.CompareTo(y.priority);

                switch (SortOption)
                {
                    case ContentsSortOption.Basic: //? 1차 = 카테고리, 2차 = 랭크, 3차 = 마나와 골드 합계
                        var temp2 = temp == 0 ? x.UnlockRank.CompareTo(y.UnlockRank) : temp;
                        result = temp2 == 0 ? (x.Mana + x.Gold).CompareTo(y.Mana + y.Gold) : temp2;
                        break;

                    case ContentsSortOption.Rank:
                        result = temp == 0 ? x.UnlockRank.CompareTo(y.UnlockRank) : temp;
                        break;

                    case ContentsSortOption.Mana:
                        result = temp == 0 ? x.Mana.CompareTo(y.Mana) : temp;
                        break;

                    case ContentsSortOption.Gold:
                        result = temp == 0 ? x.Gold.CompareTo(y.Gold) : temp;
                        break;

                    case ContentsSortOption.AP:
                        result = temp == 0 ? x.Ap.CompareTo(y.Ap) : temp;
                        break;

                    case ContentsSortOption.Name:
                        result = temp == 0 ? x.labelName.CompareTo(y.labelName) : temp;
                        break;

                    default:
                        result = temp;
                        break;
                }
            }
            else
            {
                switch (SortOption)
                {
                    case ContentsSortOption.Rank:
                        result = x.UnlockRank.CompareTo(y.UnlockRank);
                        break;

                    case ContentsSortOption.Mana:
                        result = x.Mana.CompareTo(y.Mana);
                        break;

                    case ContentsSortOption.Gold:
                        result = x.Gold.CompareTo(y.Gold);
                        break;

                    case ContentsSortOption.AP:
                        result = x.Ap.CompareTo(y.Ap);
                        break;

                    case ContentsSortOption.Name:
                        result = x.labelName.CompareTo(y.labelName);
                        break;

                    default:
                        result = x.priority.CompareTo(y.priority);
                        break;
                }
            }

            return Ascending ? result : result *= -1;
        }
    }

    public enum ContentsSortOption
    {
        Basic,
        Mana,
        Gold,
        AP,
        Name,
        Rank,
        Priority,
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
        Main.Instance.ResetCurrentAction();
        Managers.UI.CloseAll();
    }

    #endregion


    #region Create 메서드
    //void CreateAll(string _keyName)
    //{
    //    if (Create(_keyName, Main.Instance.CurrentBoundary))
    //    {
    //        CreateOver();
    //        SoundManager.Instance.PlaySound("SFX/Action_Build");
    //    }
    //    else
    //    {
    //        Debug.Log("배치할 수 없음");
    //        SoundManager.Instance.PlaySound("SFX/Action_Wrong");
    //    }
    //}

    //void CreateOnlyOne(string prefab)
    //{
    //    if (CreateUnique(prefab, Main.Instance.CurrentBoundary))
    //    {
    //        CreateOver();
    //        SoundManager.Instance.PlaySound("SFX/Action_Build");
    //    }
    //    else
    //    {
    //        Debug.Log("배치할 수 없음");
    //        SoundManager.Instance.PlaySound("SFX/Action_Wrong");
    //    }
    //}

    void CreateCustom(List<Func<bool>> _funcList)
    {
        if (Main.Instance.CurrentPurchase.PurchaseCheck() == false)
        {
            CreateOver();
            return;
        }

        bool all = true;
        foreach (var item in _funcList)
        {
            all &= item.Invoke();
        }

        if (all)
        {
            SoundManager.Instance.PlaySound("SFX/Action_Build");
            Main.Instance.CurrentPurchase.PurchaseConfirm();
            if (Main.Instance.CurrentPurchase.isContinuous == false)
            {
                CreateOver();
            }
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

    bool CreatePortal(string prefab, Vector2Int[] boundary)
    {
        if (Main.Instance.CurrentTile == null) return false;
        if (Main.Instance.CurrentTile.floor.FloorIndex == (int)Define.DungeonFloor.Egg) return false;

        return CreateUnique(prefab, boundary);
    }


    bool CreateBigOne(string _keyName, Vector2Int[] boundary)
    {
        if (Main.Instance.CurrentTile == null) return false;

        var tile = Main.Instance.CurrentTile;

        Facility original = null;

        for (int i = 0; i < boundary.Length; i++)
        {
            Vector2Int delta = tile.index + boundary[i];
            BasementTile temp = null;
            if (Main.Instance.CurrentTile.floor.TileMap.TryGetValue(delta, out temp))
            {
                var info = new PlacementInfo(Main.Instance.CurrentTile.floor, temp);
                if (i == 0)
                {
                    original = GameManager.Facility.CreateFacility(_keyName, info) as Facility;
                }
                else
                {
                    var clone = GameManager.Facility.CreateFacility("Clone_Facility_Wall", info);
                    clone.GetObject().GetComponent<Clone_Facility_Wall>().OriginalTarget = original;
                }
            }
        }

        return true;
    }





    void CreateOver()
    {
        //Main.Instance.PurchaseAction.Invoke();
        //Main.Instance.CurrentPurchase.PurchaseConfirm();

        Main.Instance.ResetCurrentAction();
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
        var dp = Managers.UI.ShowPopUpAlone<UI_DungeonPlacement>();

        //? 테스트!
        var letter = Managers.UI.ShowPopUpNonPush<UI_LetterBox>();
        letter.SetBoxOption(UI_LetterBox.BoxOption.Build, dp);
        letter.ShowCalculationPanel(Main.Instance.CurrentPurchase);

        yield return new WaitForEndOfFrame();

        Main.Instance.CurrentBoundary = vector2Ints;
        Main.Instance.CurrentAction += action;

        for (int i = 0; i < Main.Instance.ActiveFloor_Basement; i++)
        {
            Main.Instance.Floor[i].UI_Floor.ShowTile();
            Main.Instance.Floor[i].UI_Floor.Mode = buildMode;
        }
    }

    IEnumerator ShowFloor_NoEggRoom(Vector2Int[] vector2Ints, Action action, UI_Floor.BuildMode buildMode = UI_Floor.BuildMode.Build)
    {
        Managers.UI.ShowPopUpAlone<UI_DungeonPlacement>();
        yield return new WaitForEndOfFrame();

        Main.Instance.CurrentBoundary = vector2Ints;
        Main.Instance.CurrentAction += action;

        for (int i = 0; i < Main.Instance.ActiveFloor_Basement; i++)
        {
            if (i == 3) continue;

            Main.Instance.Floor[i].UI_Floor.ShowTile();
            Main.Instance.Floor[i].UI_Floor.Mode = buildMode;
        }
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