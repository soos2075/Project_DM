using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContentManager
{
    public Dictionary<string, ContentData> EventContentsDic { get; set; }


    public List<ContentData> Contents { get; set; } //? ��ųʸ��� �����ص� �����Ű����ѵ� �ϴ��� ��������

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
            content.SetName("ö�� (1x1)", "����� ��ġ�� �ü��� ö���մϴ�. ������ ���� ȸ���� �� ������, ö�ź���� �ȹ޴°� ��𿡿�." +
                "\n���� ������ 1 x 1 �Դϴ�.");
            content.SetCondition(0, 0, 1, Facility_Priority.System);
            content.sprite = Managers.Sprite.GetSprite("Nothing");
            content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => ClearAll(), UI_Floor.BuildMode.Clear));

            Contents.Add(content);
        }
        {
            ContentData content = new ContentData("Clear");
            content.SetName("ö�� (3x3)", "����� ��ġ�� �ü��� ö���մϴ�. ������ ���� ȸ���� �� ������, ö�ź���� �ȹ޴°� ��𿡿�." +
                "\n���� ������ 3 x 3 �Դϴ�.");
            content.SetCondition(0, 0, 1, Facility_Priority.System);
            content.sprite = Managers.Sprite.GetSprite("Nothing");
            content.SetAction(() => SetBoundary(Define.Boundary_3x3, () => ClearAll(), UI_Floor.BuildMode.Clear));
            Contents.Add(content);
        }


        {
            ContentData content = new ContentData("Herb_Low");
            content = new ContentData("Herb");
            content.SetName("�ϱ� ���ʹ�(��)", "�ϱ� ���ʰ� �ڶ󳪴� ���ʹ��� ��ġ�մϴ�. ���ϱ� �ص� ������ ���ϰ� �ֱ� ������ ���ӻ��� ���� ���̿���." +
                "\n���� ������ 2 x 2 �Դϴ�.");
            content.SetCondition(25, 0, 1, Facility_Priority.Herb);
            content.sprite = Managers.Sprite.GetSprite_SLA("Low");
            content.SetAction(() => SetBoundary(Define.Boundary_2x2, () => CreateAll<Herb>("Herb", (int)Herb.HerbType.Low)));

            Contents.Add(content);
        }
        {
            ContentData content = new ContentData("Herb_Low");
            content = new ContentData("Herb");
            content.SetName("���� ���ʹ�(��)", "�ϱ� ���ʰ� �ڶ󳪴� ���ʹ��� ��ġ�մϴ�. ���ϱ� �ص� ������ ���ϰ� �ֱ� ������ ���ӻ��� ���� ���̿���." +
                "\n���� ������ 3 x 3 �Դϴ�.");
            content.SetCondition(50, 0, 1, Facility_Priority.Herb);
            content.sprite = Managers.Sprite.GetSprite_SLA("Low");
            content.SetAction(() => SetBoundary(Define.Boundary_3x3, () => CreateAll<Herb>("Herb", (int)Herb.HerbType.Low)));

            Contents.Add(content);
        }
        {
            ContentData content = new ContentData("Herb_Low");
            content = new ContentData("Herb_Low");
            content.SetName("���� ���ʹ�(��)", "�ϱ� ���ʰ� �ڶ󳪴� ���ʹ��� ��ġ�մϴ�. ���ϱ� �ص� ������ ���ϰ� �ֱ� ������ ���ӻ��� ���� ���̿���." +
                "\n���� ������ 5 x 5 �Դϴ�.");
            content.SetCondition(100, 0, 1, Facility_Priority.Herb);
            content.sprite = Managers.Sprite.GetSprite_SLA("Low");
            content.SetAction(() => SetBoundary(Define.Boundary_5x5, () => CreateAll<Herb>("Herb", (int)Herb.HerbType.Low)));

            Contents.Add(content);
        }


        {
            ContentData content = new ContentData("Herb_High");
            content = new ContentData("Herb_High");
            content.SetName("��� ���ʹ�", "��� ���ʰ� �ڶ󳪴� ���ʹ��� ��ġ�մϴ�. ���� ���� �� ���� ���� �����Դϴ�. ���� ȿ���� ���ƿ�." +
                "\n���� ������ 1 x 3 �Դϴ�.");
            content.SetCondition(75, 0, 1, Facility_Priority.Herb);
            content.sprite = Managers.Sprite.GetSprite_SLA("High");
            content.SetAction(() => SetBoundary(Define.Boundary_1x3, () => CreateAll<Herb>("Herb", (int)Herb.HerbType.High)));

            Contents.Add(content);
        }


        {
            ContentData content = new ContentData("Mineral");
            content = new ContentData("Mineral");
            content.SetName("���� ����", "������ ������ ���� �� �ִ� ������ ��ġ�մϴ�. ���� ������ �����־��." +
                "\n���� ������ ���� ���� �Դϴ�.");
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
            content.SetName("ū ����", "������ ������ ���� �� �ִ� ������ ��ġ�մϴ�. ���� ������ �����־��." +
                "\n���� ������ ū ������ �Դϴ�.");
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
            content.SetName("���� ����", "������ �߹� ������ ��ġ�մϴ�. �ߵ��Ǹ� �ð��� ü�¿� ���ظ� �� �� �־��." +
                "\n���� ������ 1 x 1 �Դϴ�.");
            content.SetCondition(0, 50, 1, Facility_Priority.Trap);
            content.sprite = Managers.Sprite.GetSprite_SLA("Fallen_1");
            content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateAll<Trap>("Trap", (int)Trap.TrapType.Fallen_1)));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Sword");
            content = new ContentData("Sword");
            content.SetName("������ ����", "���谡���� �����Ҹ��� ���⸦ ��ġ�մϴ�.\n" +
                "���谡���� �߰ߵǸ� ������ �α⵵�� ���� �ö��.");
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
            content.SetName("���� ȣ�ڹ�", "������ ���� ȣ���Դϴ�. �ְ�� �����ε� ����� �Ǹ� ��Ÿ ���� �ռ��� ���� ���˴ϴ�." +
                "\n���� ������ ���� X �Դϴ�.");
            content.SetCondition(125, 0, 1, Facility_Priority.Herb);
            content.sprite = Managers.Sprite.GetSprite_SLA("Pumpkin");
            content.SetAction(() => SetBoundary(Define.Boundary_X_1, () => CreateAll<Herb>("Herb", (int)Herb.HerbType.Pumpkin)));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Entrance");
            content.SetName("�Ա�", "�÷��̾ ���� �Ա��� �����մϴ�. ���� �Ա��� ������ ������ġ�� �ڵ����� �����˴ϴ�. �Ա��� �� �� �Ѱ��� ������ �� �־��.");
            content.SetCondition(50, 0, 2, Facility_Priority.System);
            content.sprite = Managers.Sprite.GetSprite("Entrance");

            content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateOnlyOne("Entrance")));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Exit");
            content.SetName("�ⱸ", "�÷��̾ ���� �ⱸ�� �����մϴ�. ���� �ⱸ�� ������ ������ġ�� �ڵ����� �����˴ϴ�. �ⱸ�� �� �� �Ѱ��� ������ �� �־��.");
            content.SetCondition(50, 0, 2, Facility_Priority.System);
            content.sprite = Managers.Sprite.GetSprite("Exit");

            content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateOnlyOne("Exit")));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Mineral_Lv2");
            content = new ContentData("Mineral_Lv2");
            content.SetName("�ܴ��� ����", "������ ������ ���� �� �ִ� ������ ��ġ�մϴ�. ������ ���� �Ҹ� �� ���� �� �ܴ��ϰ� ���� ������ �Ǿ��־��." +
                "\n���� ������ 3 x 1 �Դϴ�.");
            content.SetCondition(100, 0, 2, Facility_Priority.Mineral);
            content.sprite = Managers.Sprite.GetSprite_SLA("Stone");
            content.SetAction(() => SetBoundary(Define.Boundary_3x1, () => CreateAll<Mineral>("Mineral", (int)Mineral.MienralCategory.Stone)));

            Contents.Add(content);
        }
        {
            ContentData content = new ContentData("Mineral_Lv2");
            content = new ContentData("Mineral_Lv2");
            content.SetName("ö ����", "������ ������ ���� �� �ִ� ������ ��ġ�մϴ�. ������ ���� �Ҹ� �� ���� �� �ܴ��ϰ� ���� ������ �Ǿ��־��." +
                "\n���� ������ 3 x 1 �Դϴ�.");
            content.SetCondition(150, 0, 2, Facility_Priority.Mineral);
            content.sprite = Managers.Sprite.GetSprite_SLA("Iron");
            content.SetAction(() => SetBoundary(Define.Boundary_3x1, () => CreateAll<Mineral>("Mineral", (int)Mineral.MienralCategory.Iron)));

            Contents.Add(content);
        }
        {
            ContentData content = new ContentData("Mineral_Lv2");
            content = new ContentData("Mineral_Lv2");
            content.SetName("��ź ����", "������ ������ ���� �� �ִ� ������ ��ġ�մϴ�. ������ ���� �Ҹ� �� ���� �� �ܴ��ϰ� ���� ������ �Ǿ��־��." +
                "\n���� ������ 3 x 1 �Դϴ�.");
            content.SetCondition(200, 0, 2, Facility_Priority.Mineral);
            content.sprite = Managers.Sprite.GetSprite_SLA("Coal");
            content.SetAction(() => SetBoundary(Define.Boundary_3x1, () => CreateAll<Mineral>("Mineral", (int)Mineral.MienralCategory.Coal)));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Trap_Fallen_2");
            content.SetName("��ȭ ��ȭ ����", "������ �߹� ������ ��ġ�մϴ�. ������ ������������ �� ȿ���� ���������." +
                "\n���� ������ 1 x 1 �Դϴ�.");
            content.SetCondition(0, 100, 2, Facility_Priority.Trap);
            content.sprite = Managers.Sprite.GetSprite_SLA("Fallen_2");
            content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateAll<Trap>("Trap", (int)Trap.TrapType.Fallen_2)));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Trap_Awl");
            content.SetName("�۰� ����", "�ٴڿ��� �۰��� �ö���� ���ù����� ������ ��ġ�մϴ�. �ߵ��Ǹ� ū ���ظ� �����ϴ�." +
                "\n���� ������ 1 x 1 �Դϴ�.");
            content.SetCondition(0, 150, 2, Facility_Priority.Trap);
            content.sprite = Managers.Sprite.GetSprite_SLA("Awl_1");

            content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateAll<Trap>("Trap", (int)Trap.TrapType.Awl_1)));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Ring");
            content = new ContentData("Ring");
            content.SetName("���� ��", "������ ������ ����ؼ� ���� ���� �����Դϴ�. �������� ������ ���� ����ϴ� ȿ���� �ɷ��ֽ��ϴ�.\n" +
                "���谡���� �߰ߵǸ� ������ �α⵵�� ���� �ö��.");
            content.SetCondition(0, 50, 1, Facility_Priority.Treasure);
            content.sprite = Managers.Sprite.GetSprite_SLA("Ring");
            content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateAll<Treasure>("Treasure", (int)Treasure.TreasureCategory.Ring)));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Scroll");
            content = new ContentData("Scroll");
            content.SetName("���� ��ũ��", "����� �ֹ��� ���� ��ũ���Դϴ�. ���� ��������� ������ ���� ���� �ʿ��� ���������� �αⰡ �����ϴ�.");
            content.SetCondition(0, 75, 1, Facility_Priority.Treasure);
            content.sprite = Managers.Sprite.GetSprite_SLA("Scroll");
            content.SetAction(() => SetBoundary(Define.Boundary_1x1, () => CreateAll<Treasure>("Treasure", (int)Treasure.TreasureCategory.Scroll)));

            Contents.Add(content);
        }
    }



    #region Clear �޼���
    void ClearAll()
    {
        if (Clear(Main.Instance.CurrentBoundary))
        {
            ClearOver();
        }
        else
        {
            Debug.Log("������ �� ����");
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
        Debug.Log($"{Main.Instance.CurrentTile.index} Ÿ�� ����");
        //Managers.UI.PauseClose();
        //Managers.UI.ClosePopUp();
        ResetAction();
        Managers.UI.CloseAll();
    }

    #endregion


    #region Create �޼���
    void CreateAll(string prefab, bool isUnchangeable = false)
    {
        if (Create(prefab, Main.Instance.CurrentBoundary, isUnchangeable))
        {
            CreateOver();
        }
        else
        {
            Debug.Log("��ġ�� �� ����");
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
            Debug.Log("��ġ�� �� ����");
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
            Debug.Log("��ġ�� �� ����");
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
            Debug.Log("��ġ�� �� ����");
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
    //        Debug.Log("��ġ�� �� ����");
    //    }
    //}


    #endregion UIEvent �Լ�


    #region ���� ���� �Լ�

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

public enum Facility_Priority // �������� contents ���Ľ� �켱����
{
    System = 0,

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