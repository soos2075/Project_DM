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
                (data) => SetBoundary(Define.Boundary_1x1, () => ClearAll(), UI_Floor.BuildMode.Clear));

            content.AddOption("\n���� ������ 3 x 3 �Դϴ�.", Define.Boundary_3x3,
                (data) => SetBoundary(Define.Boundary_3x3, () => ClearAll(), UI_Floor.BuildMode.Clear));

            content.AddOption("\n���� ������ 5 x 5 �Դϴ�.", Define.Boundary_5x5,
                (data) => SetBoundary(Define.Boundary_5x5, () => ClearAll(), UI_Floor.BuildMode.Clear));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Entrance");
            content.SetName("�Ա�", "�÷��̾ ���� �Ա��� �����մϴ�. ���� �Ա��� ������ ������ġ�� �ڵ����� �����ſ�. �Ա��� �� �� �Ѱ��� ������ �� �־��.");
            content.SetCondition(10, 0, 1);
            content.sprite = Managers.Sprite.GetSprite("Entrance");
            content.AddOption("\n���� ������ 1 x 1 �Դϴ�.", Define.Boundary_1x1,
                data => SetBoundary(Define.Boundary_1x1, () => CreateOnlyOne("Entrance", useMana: 10)));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Exit");
            content.SetName("�ⱸ", "�÷��̾ ���ư� �ⱸ�� �����մϴ�. ���� �ⱸ�� ������ ������ġ�� �ڵ����� �����ſ�. �ⱸ�� �� �� �Ѱ��� ������ �� �־��.");
            content.SetCondition(10, 0, 1);
            content.sprite = Managers.Sprite.GetSprite("Exit");
            content.AddOption("\n���� ������ 1 x 1 �Դϴ�.", Define.Boundary_1x1,
                data => SetBoundary(Define.Boundary_1x1, () => CreateOnlyOne("Exit", useMana: 10)));

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
                data => SetBoundary(Define.Boundary_2x2, () => CreateAll("Herb_Low", useMana: 25)));
            content.AddOption("\n���� ������ 3 x 3 �Դϴ�.", Define.Boundary_3x3,
                data => SetBoundary(Define.Boundary_3x3, () => CreateAll("Herb_Low", useMana: 45)),
                mana: 20);
            content.AddOption("\n���� ������ 5 x 5 �Դϴ�.", Define.Boundary_5x5,
                data => SetBoundary(Define.Boundary_5x5, () => CreateAll("Herb_Low", useMana: 100)),
                mana:75);

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Herb_High");
            content = new ContentData("Herb_High");
            content.SetName("���� ���ʹ�", "���� ���ʹ��� ��ġ�մϴ�. ���� ���� �� ���� ���� �����Դϴ�. �������� �߰��ϸ� �� ì����� �� �ſ���.");
            content.SetCondition(40, 0, 2);
            content.sprite = Managers.Sprite.GetSprite("Nothing");
            content.AddOption("\n���� ������ 1 x 3 �Դϴ�.", Define.Boundary_1x3,
                data => SetBoundary(Define.Boundary_1x3, () => CreateAll("Herb_High")));
            content.AddOption("\n���� ������ 3 x 1 �Դϴ�.", Define.Boundary_3x1,
                data => SetBoundary(Define.Boundary_3x1, () => CreateAll("Herb_High")));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Mineral");
            content = new ContentData("Mineral");
            content.SetName("����", "������ ������ ���� �� �ִ� ������ ��ġ�մϴ�. ���� ������ ���� �����־��.");
            content.SetCondition(40, 0, 2);
            content.sprite = Managers.Sprite.GetSprite("Mineral");
            content.AddOption("\n���� ������ ���� ���ڸ�� �Դϴ�.", Define.Boundary_Cross_1,
                data => SetBoundary(Define.Boundary_Cross_1, () =>
                CreateTwo("Mineral_Diamond", "Mineral_Rock", Define.Boundary_1x1, Define.Boundary_Side_Cross, useMana: 40)));

            Contents.Add(content);
        }

        {
            ContentData content = new ContentData("Trap");
            content.SetName("����", "���� ���̴� ������ �߹� ����. �̷��ſ� �ɸ��� ����� �������?");
            content.SetCondition(0, 50, 2);
            content.sprite = Managers.Sprite.GetSprite("Nothing");

            content.AddOption("\n���� ������ 1 x 1 �Դϴ�.", Define.Boundary_1x1,
                data => SetBoundary(Define.Boundary_1x1, () => CreateAll("Trap", true, useGold: 50)));
            content.AddOption("\n���� ������ 1 x 3 �Դϴ�.", Define.Boundary_1x3,
                data => SetBoundary(Define.Boundary_1x3, () => CreateAll("Trap", true, useGold: 100)),
                mana: 0, gold: 50);
            content.AddOption("\n���� ������ 3 x 1 �Դϴ�.", Define.Boundary_3x1,
                data => SetBoundary(Define.Boundary_3x1, () => CreateAll("Trap", true, useGold: 100)),
                mana: 0, gold: 50);

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
        Debug.Log($"{Main.Instance.CurrentTile.index} Ÿ�� ����");
        //Managers.UI.PauseClose();
        //Managers.UI.ClosePopUp();
        ResetAction();
        Managers.UI.CloseAll();
    }

    #endregion


    #region Create �޼���
    void CreateAll(string prefab, bool isUnchangeable = false, int useMana = 0, int useGold = 0)
    {
        if (Create(prefab, Main.Instance.CurrentBoundary, isUnchangeable))
        {
            CreateOver(useMana, useGold);
        }
        else
        {
            Debug.Log("��ġ�� �� ����");
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
            Debug.Log("��ġ�� �� ����");
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
        Debug.Log($"{Main.Instance.CurrentTile.index} : {Main.Instance.CurrentTile.placementable.Name_KR} ��ġ�Ϸ�");


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