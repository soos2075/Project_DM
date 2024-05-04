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
    //        content.SetName("ö�� (1x1)", "����� ��ġ�� �ü��� ö���մϴ�. ������ ���� ȸ���� �� ������, ö�ź���� �ȹ޴°� ��𿡿�." +
    //            "\n���� ������ 1 x 1 �Դϴ�.");
    //        content.SetCondition(0, 0, 1, Facility_Priority.System);
    //        content.sprite = Managers.Sprite.GetSprite("Nothing");
    //        content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => ClearAll(), UI_Floor.BuildMode.Clear));

    //        Contents.Add(content);
    //    }
    //    {
    //        ContentData content = new ContentData("Clear");
    //        content.SetName("ö�� (3x3)", "����� ��ġ�� �ü��� ö���մϴ�. ������ ���� ȸ���� �� ������, ö�ź���� �ȹ޴°� ��𿡿�." +
    //            "\n���� ������ 3 x 3 �Դϴ�.");
    //        content.SetCondition(0, 0, 1, Facility_Priority.System);
    //        content.sprite = Managers.Sprite.GetSprite("Nothing");
    //        content.SetAction(() => SetBoundary(Define.Boundary_3x3, () => ClearAll(), UI_Floor.BuildMode.Clear));
    //        Contents.Add(content);
    //    }


    //    //{
    //    //    ContentData content = new ContentData("Herb_Low");
    //    //    content = new ContentData("Herb");
    //    //    content.SetName("���� ���ʹ�(��)", "�ϱ� ���ʰ� �ڶ󳪴� ���ʹ��� ��ġ�մϴ�. ���ϱ� �ص� ������ ���ϰ� �ֱ� ������ ���ӻ��� ���� ���̿���." +
    //    //        "\n���� ������ 2 x 2 �Դϴ�.");
    //    //    content.SetCondition(25, 0, 1, Facility_Priority.Herb);
    //    //    content.sprite = Managers.Sprite.GetSprite_SLA("Low");
    //    //    content.SetAction(() => SetBoundary(Define.Boundary_2x2, () => CreateAll<Herb>("Herb", (int)Herb.HerbType.Low)));

    //    //    Contents.Add(content);
    //    //}
    //    {
    //        ContentData content = new ContentData("Herb_Low");
    //        content = new ContentData("Herb");
    //        content.SetName("���� ���ʹ�", "�ϱ� ���ʰ� �ڶ󳪴� ���ʹ��� ��ġ�մϴ�. ���ϱ� �ص� ������ ���ϰ� �ֱ� ������ ���ӻ��� ���� ���̿���." +
    //            "\n���� ������ 3 x 3 �Դϴ�.");
    //        content.SetCondition(50, 0, 1, Facility_Priority.Herb);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("Low");
    //        content.SetAction(() => SetBoundary(Define.Boundary_3x3, () => CreateAll<Herb>("Herb_Low")));

    //        Contents.Add(content);
    //    }

    //    {
    //        ContentData content = new ContentData("Herb_Low");
    //        content = new ContentData("Herb_Low");
    //        content.SetName("ū ���ʹ�", "�ϱ� ���ʰ� �ڶ󳪴� ���ʹ��� ��ġ�մϴ�. ���ϱ� �ص� ������ ���ϰ� �ֱ� ������ ���ӻ��� ���� ���̿���." +
    //            "\n���� ������ 5 x 5 �Դϴ�.");
    //        content.SetCondition(100, 0, 1, Facility_Priority.Herb);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("Low");
    //        content.SetAction(() => SetBoundary(Define.Boundary_5x5, () => CreateAll<Herb>("Herb_Low")));

    //        Contents.Add(content);
    //    }


    //    {
    //        ContentData content = new ContentData("Herb_High");
    //        content = new ContentData("Herb_High");
    //        content.SetName("��� ���ʹ�", "��� ���ʰ� �ڶ󳪴� ���ʹ��� ��ġ�մϴ�. ���� ���� �� ���� ���� �����Դϴ�. ���� ȿ���� ���ƿ�." +
    //            "\n���� ������ 1 x 3 �Դϴ�.");
    //        content.SetCondition(125, 0, 1, Facility_Priority.Herb);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("High");
    //        content.SetAction(() => SetBoundary(Define.Boundary_1x3, () => CreateAll<Herb>("Herb_High")));

    //        Contents.Add(content);
    //    }


    //    {
    //        ContentData content = new ContentData("Mineral_small");
    //        content = new ContentData("Mineral");
    //        content.SetName("���� ����", "������ ������ ���� �� �ִ� ������ ��ġ�մϴ�. ���� ������ �����־��." +
    //            "\n���� ������ ���� ���� �Դϴ�.");
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
    //        content.SetName("ū ����", "������ ������ ���� �� �ִ� ������ ��ġ�մϴ�. ���� ������ �����־��." +
    //            "\n���� ������ ū ������ �Դϴ�.");
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
    //        content.SetName("���� ����", "������ �߹� ������ ��ġ�մϴ�. �ߵ��Ǹ� �ð��� ü�¿� ���ظ� �� �� �־��." +
    //            "\n���� ������ 1 x 1 �Դϴ�.");
    //        content.SetCondition(0, 20, 1, Facility_Priority.Trap);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("Fallen_1");
    //        content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateAll<Trap>("Trap_Fallen_1")));

    //        Contents.Add(content);
    //    }

    //    {
    //        ContentData content = new ContentData("Sword");
    //        content = new ContentData("Sword");
    //        content.SetName("������ ����", "���谡���� �����Ҹ��� ���⸦ ��ġ�մϴ�.\n" +
    //            "���谡���� �߰ߵǸ� ������ �α⵵�� ���� �ö��.");
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
    //        content.SetName("���� ȣ�ڹ�", "������ ���� ȣ���Դϴ�. �ְ�� �����ε� ����� �Ǹ� ��Ÿ ���� �ռ��� ���� ���˴ϴ�." +
    //            "\n���� ������ ���� X �Դϴ�.");
    //        content.SetCondition(85, 0, 2, Facility_Priority.Herb);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("Pumpkin");
    //        content.SetAction(() => SetBoundary(Define.Boundary_X_1, () => CreateAll<Herb>("Herb_Pumpkin")));

    //        Contents.Add(content);
    //    }

    //    {
    //        ContentData content = new ContentData("Entrance");
    //        content.SetName("�Ա�", "�Ա��� ��ġ�� �����մϴ�. �Ա��� �� �� �Ѱ��� ������ �� �־��. ���� �Ա��� ������ ���۽� �ڵ����� �����˴ϴ�.");
    //        content.SetCondition(50, 0, 2, Facility_Priority.System);
    //        content.sprite = Managers.Sprite.GetSprite("Entrance");

    //        content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateOnlyOne("Entrance")));

    //        Contents.Add(content);
    //    }

    //    {
    //        ContentData content = new ContentData("Exit");
    //        content.SetName("�ⱸ", "�ⱸ�� ��ġ�� �����մϴ�. �ⱸ�� �� �� �Ѱ��� ������ �� �־��. ���� �ⱸ�� ������ ���۽� �ڵ����� �����˴ϴ�.");
    //        content.SetCondition(50, 0, 2, Facility_Priority.System);
    //        content.sprite = Managers.Sprite.GetSprite("Exit");

    //        content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateOnlyOne("Exit")));

    //        Contents.Add(content);
    //    }

    //    {
    //        ContentData content = new ContentData("Mineral_Lv2");
    //        content = new ContentData("Mineral_Lv2");
    //        content.SetName("�ܴ��� ����", "������ ������ ���� �� �ִ� ������ ��ġ�մϴ�. ������ ���� �Ҹ� �� ���� �� �ܴ��ϰ� ���� ������ �Ǿ��־��." +
    //            "\n���� ������ 3 x 1 �Դϴ�.");
    //        content.SetCondition(100, 0, 2, Facility_Priority.Mineral);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("Stone");
    //        content.SetAction(() => SetBoundary(Define.Boundary_3x1, () => CreateAll<Mineral>("Mineral_Stone")));

    //        Contents.Add(content);
    //    }
    //    {
    //        ContentData content = new ContentData("Mineral_Lv2");
    //        content = new ContentData("Mineral_Lv2");
    //        content.SetName("��ź ����", "������ ������ ���� �� �ִ� ������ ��ġ�մϴ�. ������ ���� �Ҹ� �� ���� �� �ܴ��ϰ� ���� ������ �Ǿ��־��." +
    //            "\n���� ������ 3 x 1 �Դϴ�.");
    //        content.SetCondition(150, 0, 2, Facility_Priority.Mineral);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("Coal");
    //        content.SetAction(() => SetBoundary(Define.Boundary_3x1, () => CreateAll<Mineral>("Mineral_Coal")));

    //        Contents.Add(content);
    //    }
    //    {
    //        ContentData content = new ContentData("Mineral_Lv2");
    //        content = new ContentData("Mineral_Lv2");
    //        content.SetName("���� ö����", "������ ������ ���� �� �ִ� ������ ��ġ�մϴ�. ������ ���� �Ҹ� �� ���� �� �ܴ��ϰ� ���� ������ �Ǿ��־��." +
    //            "\n���� ������ 3 x 1 �Դϴ�.");
    //        content.SetCondition(200, 0, 2, Facility_Priority.Mineral);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("Iron");
    //        content.SetAction(() => SetBoundary(Define.Boundary_3x1, () => CreateAll<Mineral>("Mineral_Iron")));

    //        Contents.Add(content);
    //    }

    //    {
    //        ContentData content = new ContentData("Trap_Fallen_2");
    //        content.SetName("��ȭ ��ȭ ����", "������ �߹� ������ ��ġ�մϴ�. ������ ������������ �� ȿ���� ���������." +
    //            "\n���� ������ 1 x 1 �Դϴ�.");
    //        content.SetCondition(0, 45, 2, Facility_Priority.Trap);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("Fallen_2");
    //        content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateAll<Trap>("Trap_Fallen_2")));

    //        Contents.Add(content);
    //    }

    //    {
    //        ContentData content = new ContentData("Trap_Awl_1");
    //        content.SetName("�۰� ����", "�ٴڿ��� �۰��� �ö���� ���ù����� ������ ��ġ�մϴ�. �ߵ��Ǹ� ū ���ظ� �����ϴ�." +
    //            "\n���� ������ 1 x 1 �Դϴ�.");
    //        content.SetCondition(0, 125, 2, Facility_Priority.Trap);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("Awl_1");

    //        content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateAll<Trap>("Trap_Awl_1")));

    //        Contents.Add(content);
    //    }

    //    {
    //        ContentData content = new ContentData("Ring");
    //        content = new ContentData("Ring");
    //        content.SetName("���� ��", "������ ������ ����ؼ� ���� ���� �����Դϴ�. �������� ������ ���� ����ϴ� ȿ���� �ɷ��ֽ��ϴ�.\n" +
    //            "���谡���� �߰ߵǸ� ������ �α⵵�� ���� �ö��.");
    //        content.SetCondition(0, 50, 1, Facility_Priority.Treasure);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("Ring");
    //        content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateAll<Treasure>("Treasure_Ring")));

    //        Contents.Add(content);
    //    }

    //    {
    //        ContentData content = new ContentData("Scroll");
    //        content = new ContentData("Scroll");
    //        content.SetName("���� ��ũ��", "����� �ֹ��� ���� ��ũ���Դϴ�. ���� ��������� ������ ���� ���� �ʿ��� ���������� �αⰡ �����ϴ�.");
    //        content.SetCondition(0, 75, 1, Facility_Priority.Treasure);
    //        content.sprite = Managers.Sprite.GetSprite_SLA("Scroll");
    //        content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateAll<Treasure>("Treasure_Scroll")));

    //        Contents.Add(content);
    //    }
    //}



    #region Clear �޼���
    void ClearAll()
    {
        if (Clear(Main.Instance.CurrentBoundary))
        {
            ClearOver();
            SoundManager.Instance.PlaySound("SFX/Action_Build");
        }
        else
        {
            Debug.Log("������ �� ����");
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
        Debug.Log($"{Main.Instance.CurrentTile.index} Ÿ�� ����");
        //Managers.UI.PauseClose();
        //Managers.UI.ClosePopUp();
        ResetAction();
        Managers.UI.CloseAll();
    }

    #endregion


    #region Create �޼���
    void CreateAll(string _keyName)
    {
        if (Create(_keyName, Main.Instance.CurrentBoundary))
        {
            CreateOver();
            SoundManager.Instance.PlaySound("SFX/Action_Build");
        }
        else
        {
            Debug.Log("��ġ�� �� ����");
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
            Debug.Log("��ġ�� �� ����");
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
            Debug.Log("��ġ�� �� ����");
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

public enum Facility_Priority // �������� contents ���Ľ� �켱����
{
    System = 1000,

    Herb = 100,

    Mineral = 200,

    Trap = 300,



    Treasure = 400,

    Artifact = 500,

    Etc = 900,
}
// ��ǰ ������ ����, ���ڰ� �������� �켱������ ���� 
// 1������ - Facility_Priority
// 2���� - �䱸����
// 3���� - �ʿ� ����
// 4���� - �ʿ� ���
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