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
        //AddEventContents();
    }

    //void AddEventContents()
    //{
    //    ContentData content = new ContentData("EggAppear");
    //    content.AddOption("��й� / ���ġ �Ұ���", Define.Boundary_1x1,
    //        (data) => CreateOnlyOne("EggEntrance"));


    //    EventContentsDic.Add(content.contentName, content);
    //}

    void AddContents()
    {
        {
            ContentData content = new ContentData("Clear");
            content.SetName("����", "����� ��ġ�� �ü��� ö���մϴ�. ������ ���� ȸ���� �� ������, ö�ź���� �ȹ޴°� ��𿡿�.");
            content.SetCondition(0, 0, 1);
            content.sprite = Managers.Sprite.GetSprite("Nothing");
            content.AddOption("\n���� ������ 1 x 1 �Դϴ�.", Define.Boundary_1x1,
                () => SetBoundary(Define.Boundary_1x1, () => ClearAll(), UI_Floor.BuildMode.Clear));

            content.AddOption("\n���� ������ 3 x 3 �Դϴ�.", Define.Boundary_3x3,
                () => SetBoundary(Define.Boundary_3x3, () => ClearAll(), UI_Floor.BuildMode.Clear));

            content.AddOption("\n���� ������ 5 x 5 �Դϴ�.", Define.Boundary_5x5,
                () => SetBoundary(Define.Boundary_5x5, () => ClearAll(), UI_Floor.BuildMode.Clear));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Entrance");
            content.SetName("�Ա�", "�÷��̾ ���� �Ա��� �����մϴ�. ���� �Ա��� ������ ������ġ�� �ڵ����� �����ſ�. �Ա��� �� �� �Ѱ��� ������ �� �־��.");
            content.SetCondition(50, 0, 1);
            content.sprite = Managers.Sprite.GetSprite("Entrance");
            content.AddOption("\n���� ������ 1 x 1 �Դϴ�.", Define.Boundary_1x1,
                () => SetBoundary(Define.Boundary_1x1, () => CreateOnlyOne("Entrance")));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Exit");
            content.SetName("�ⱸ", "�÷��̾ ���ư� �ⱸ�� �����մϴ�. ���� �ⱸ�� ������ ������ġ�� �ڵ����� �����ſ�. �ⱸ�� �� �� �Ѱ��� ������ �� �־��.");
            content.SetCondition(50, 0, 1);
            content.sprite = Managers.Sprite.GetSprite("Exit");
            content.AddOption("\n���� ������ 1 x 1 �Դϴ�.", Define.Boundary_1x1,
                () => SetBoundary(Define.Boundary_1x1, () => CreateOnlyOne("Exit")));

            Contents.Add(content);
        }


        {
            ContentData content = new ContentData("Herb_Low");
            content = new ContentData("Herb_Low");
            content.SetName("���� ���ʹ�", "���� ���ʹ��� ��ġ�մϴ�. ���ϱ� �ص� ������ ���ϰ� �ֱ� ������ ���ӻ��� ���� ���̿���. " +
                "�뷮���� ��ġ�ϸ� �� �� ȿ�����̿���.");
            content.SetCondition(25, 0, 1);
            content.sprite = Managers.Sprite.GetSprite("Herb_Low");

            content.AddOption("\n���� ������ 2 x 2 �Դϴ�.", Define.Boundary_2x2,
                () => SetBoundary(Define.Boundary_2x2, () => CreateAll("Herb_Low")));
            content.AddOption("\n���� ������ 3 x 3 �Դϴ�.", Define.Boundary_3x3,
                () => SetBoundary(Define.Boundary_3x3, () => CreateAll("Herb_Low")), mana: 20);
            content.AddOption("\n���� ������ 5 x 5 �Դϴ�.", Define.Boundary_5x5,
                () => SetBoundary(Define.Boundary_5x5, () => CreateAll("Herb_Low")), mana: 50, ap: 1);

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Herb_High");
            content = new ContentData("Herb_High");
            content.SetName("���� ���ʹ�", "���� ���ʹ��� ��ġ�մϴ�. ���� ���� �� ���� ���� �����Դϴ�. �������� �߰��ϸ� �� ì����� �� �ſ���.");
            content.SetCondition(40, 0, 1);
            content.sprite = Managers.Sprite.GetSprite("Nothing");
            content.AddOption("\n���� ������ 1 x 3 �Դϴ�.", Define.Boundary_1x3,
                () => SetBoundary(Define.Boundary_1x3, () => CreateAll("Herb_High")));
            content.AddOption("\n���� ������ 3 x 1 �Դϴ�.", Define.Boundary_3x1,
                () => SetBoundary(Define.Boundary_3x1, () => CreateAll("Herb_High")));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Mineral");
            content = new ContentData("Mineral");
            content.SetName("����", "������ ������ ���� �� �ִ� ������ ��ġ�մϴ�. ���� ������ ���� �����־��.");
            content.SetCondition(40, 0, 1);
            content.sprite = Managers.Sprite.GetSprite("Mineral");
            content.AddOption("\n���� ������ ���� ���ڸ�� �Դϴ�.", Define.Boundary_Cross_1,
                () => SetBoundary(Define.Boundary_Cross_1, () =>
                CreateTwo("Mineral_Diamond", "Mineral_Rock", Define.Boundary_1x1, Define.Boundary_Side_Cross)));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Trap_Fallen_1");
            content.SetName("�߹� ����", "���� ���̴� ������ �߹� ������ ��ġ�մϴ�. ������ �̷��ſ� �ɸ��� ����� �������? ���ÿ� �������� ��ġ�� ���� �־��.");
            content.SetCondition(0, 50, 1);
            content.sprite = Managers.Sprite.GetSprite("Nothing");

            content.AddOption("\n���� ������ 1 x 1 �Դϴ�.", Define.Boundary_1x1,
                () => SetBoundary(Define.Boundary_1x1, () => CreateAll("Trap_Fallen_1", true)));
            content.AddOption("\n���� ������ 1 x 3 �Դϴ�.", Define.Boundary_1x3,
                () => SetBoundary(Define.Boundary_1x3, () => CreateAll("Trap_Fallen_1", true)), mana: 0, gold: 50);
            content.AddOption("\n���� ������ 3 x 1 �Դϴ�.", Define.Boundary_3x1,
                () => SetBoundary(Define.Boundary_3x1, () => CreateAll("Trap_Fallen_1", true)), mana: 0, gold: 50);

            Contents.Add(content);
        }
    }

    public void AddLevel2()
    {
        {
            ContentData content = new ContentData("Trap_Fallen_2");
            content.SetName("��ȭ �߹� ����", "���� ���̴� ������ �߹� ������ ��ġ�մϴ�. ���� �� ȿ���� ���������. ������ �Ѹ���� ���� �޶����� ����̳׿�.");
            content.SetCondition(100, 50, 2);
            content.sprite = Managers.Sprite.GetSprite("Nothing");

            content.AddOption("\n���� ������ 1 x 1 �Դϴ�.", Define.Boundary_1x1,
                () => SetBoundary(Define.Boundary_1x1, () => CreateTrap("Trap_Fallen_2", Trap_Base.TrapType.Fallen_2)));
            content.AddOption("\n���� ������ 1 x 3 �Դϴ�.", Define.Boundary_1x3,
                () => SetBoundary(Define.Boundary_1x3, () => CreateTrap("Trap_Fallen_2", Trap_Base.TrapType.Fallen_2)), mana: 100, gold: 50);
            content.AddOption("\n���� ������ 3 x 1 �Դϴ�.", Define.Boundary_3x1,
                () => SetBoundary(Define.Boundary_3x1, () => CreateTrap("Trap_Fallen_2", Trap_Base.TrapType.Fallen_2)), mana: 100, gold: 50);

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Trap_Awl");
            content.SetName("�۰� ����", "�ٴڿ��� �۰��� �ö���� ���ù����� ������ ��ġ�մϴ�. �����ϸ� ���� �������� ���ϴ� ���谡�� �����ſ���.");
            content.SetCondition(50, 100, 2);
            content.sprite = Managers.Sprite.GetSprite("Nothing");

            content.AddOption("\n���� ������ 1 x 1 �Դϴ�.", Define.Boundary_1x1,
                () => SetBoundary(Define.Boundary_1x1, () => CreateTrap("Trap_Awl_1", Trap_Base.TrapType.Awl_1)));
            content.AddOption("\n���� ������ 1 x 3 �Դϴ�.", Define.Boundary_1x3,
                () => SetBoundary(Define.Boundary_1x3, () => CreateTrap("Trap_Awl_1", Trap_Base.TrapType.Awl_1)), mana: 50, gold: 100);
            content.AddOption("\n���� ������ 3 x 1 �Դϴ�.", Define.Boundary_3x1,
                () => SetBoundary(Define.Boundary_3x1, () => CreateTrap("Trap_Awl_1", Trap_Base.TrapType.Awl_1)), mana: 50, gold: 100);

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


    void CreateTwo(string prefab1, string prefab2, Vector2Int[] boundary1, Vector2Int[] boundary2)
    {
        if (Create(prefab1, boundary1) && Create(prefab2, boundary2))
        {
            CreateOver();
        }
        else
        {
            Debug.Log("��ġ�� �� ����");
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
            Debug.Log("��ġ�� �� ����");
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
    //        Debug.Log("��ġ�� �� ����");
    //    }
    //}

    #endregion UIEvent �Լ�


    #region ���� ���� �Լ�

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
        Debug.Log($"{Main.Instance.CurrentTile.index} : {Main.Instance.CurrentTile.placementable.Name_KR} ��ġ�Ϸ�");

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