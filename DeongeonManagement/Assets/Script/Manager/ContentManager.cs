using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContentManager
{

    public ContentData[] Contents { get; set; }

    public void Init()
    {
        Contents = new ContentData[7];

        Contents[0] = new ContentData("Clear");
        Contents[0].SetName("����", "����� ��ġ�� �ü��� ö���մϴ�. ������ ���� ȸ���� �� ������, ö�ź���� �ȹ޴°� ��𿡿�.");
        Contents[0].SetCondition(0, 0, 1);
        Contents[0].sprite = Managers.Sprite.GetSprite("Nothing");
        Contents[0].AddOption("\n���� ������ 1 x 1 �Դϴ�.", Define.Boundary_1x1, 
            (data) => SetBoundary(Define.Boundary_1x1, () => ClearAll(), UI_Floor.BuildMode.Clear));

        Contents[0].AddOption("\n���� ������ 3 x 3 �Դϴ�.", Define.Boundary_3x3,
            (data) => SetBoundary(Define.Boundary_3x3, () => ClearAll(), UI_Floor.BuildMode.Clear));

        Contents[0].AddOption("\n���� ������ 5 x 5 �Դϴ�.", Define.Boundary_5x5,
            (data) => SetBoundary(Define.Boundary_5x5, () => ClearAll(), UI_Floor.BuildMode.Clear));



        Contents[1] = new ContentData("Entrance");
        Contents[1].SetName("�Ա�", "�÷��̾ ���� �Ա��� �����մϴ�. ���� �Ա��� ������ ������ġ�� �ڵ����� �����ſ�. �Ա��� �� �� �Ѱ��� ������ �� �ֽ��ϴ�.");
        Contents[1].SetCondition(10, 0, 1);
        Contents[1].sprite = Managers.Sprite.GetSprite("Nothing");
        Contents[1].AddOption("\n���� ������ 1 x 1 �Դϴ�.", Define.Boundary_1x1,
            data => SetBoundary(Define.Boundary_1x1, () => CreateOnlyOne("Entrance")));



        Contents[2] = new ContentData("Exit");
        Contents[2].SetName("�ⱸ", "�÷��̾ ���ư� �ⱸ�� �����մϴ�. ���� �ⱸ�� ������ ������ġ�� �ڵ����� �����ſ�. �ⱸ�� �� �� �Ѱ��� ������ �� �ֽ��ϴ�.");
        Contents[2].SetCondition(10, 0, 1);
        Contents[2].sprite = Managers.Sprite.GetSprite("Nothing");
        Contents[2].AddOption("\n���� ������ 1 x 1 �Դϴ�.", Define.Boundary_1x1,
            data => SetBoundary(Define.Boundary_1x1, () => CreateOnlyOne("Exit")));



        Contents[3] = new ContentData("Herb_Low_Small");
        Contents[3].SetName("���� ���ʹ�(��)", "���� ���ʹ��� ��ġ�մϴ�. ���ϱ� �ص� ������ ���ϰ� �ֱ� ������ ���ӻ��� ���� ���̿���.");
        Contents[3].SetCondition(25, 0, 1);
        Contents[3].sprite = Managers.Sprite.GetSprite("Nothing");
        Contents[3].AddOption("\n���� ������ 2 x 2 �Դϴ�.", Define.Boundary_2x2,
            data => SetBoundary(Define.Boundary_2x2, () => CreateAll("Herb_Low")));



        Contents[4] = new ContentData("Herb_Low_Middle");
        Contents[4].SetName("���� ���ʹ�(��)", "���� ���ʹ��� ��ġ�մϴ�. �뷮���� ��ġ�ؼ� �� �� ȿ�����̰� �ƾ��.");
        Contents[4].SetCondition(40, 0, 2);
        Contents[4].sprite = Managers.Sprite.GetSprite("Nothing");
        Contents[4].AddOption("\n���� ������ 3 x 3 �Դϴ�.", Define.Boundary_3x3,
            data => SetBoundary(Define.Boundary_3x3, () => CreateAll("Herb_Low")));



        Contents[5] = new ContentData("Herb_High");
        Contents[5].SetName("���� ���ʹ�(��)", "���� ���ʹ��� ��ġ�մϴ�. ���� ���� �� ���� ���� �����Դϴ�. ���ʲ۵��� ���� ���� Ű�� ã���ſ���.");
        Contents[5].SetCondition(40, 0, 2);
        Contents[5].sprite = Managers.Sprite.GetSprite("Nothing");
        Contents[5].AddOption("\n���� ������ 1 x 3 �Դϴ�.", Define.Boundary_1x3,
            data => SetBoundary(Define.Boundary_1x3, () => CreateAll("Herb_High")));
        Contents[5].AddOption("\n���� ������ 3 x 1 �Դϴ�.", Define.Boundary_3x1,
            data => SetBoundary(Define.Boundary_3x1, () => CreateAll("Herb_High")));



        Contents[6] = new ContentData("Mineral");
        Contents[6].SetName("����", "������ ������ ���� �� �ִ� ������ ��ġ�մϴ�. �� ������ �ٰ��� �Ǵ� �����Դϴ�. ������ �ִ� ��������.");
        Contents[6].SetCondition(40, 0, 2);
        Contents[6].sprite = Managers.Sprite.GetSprite("Nothing");
        Contents[6].AddOption("\n���� ������ ���� ���ڸ�� �Դϴ�.", Define.Boundary_Cross_1,
            data => SetBoundary(Define.Boundary_Cross_1, () =>
            CreateTwo("Mineral_Diamond", "Mineral_Rock", Define.Boundary_1x1, Define.Boundary_Side_Cross)));

    }




    #region ClearButton
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


    #region AddUIEvent �Լ�
    void CreateAll(string prefab)
    {
        if (Create(prefab, Main.Instance.CurrentBoundary))
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
    void CreateThree(string prefab1, string prefab2, string prefab3, Vector2Int[] boundary1, Vector2Int[] boundary2, Vector2Int[] boundary3)
    {
        if (Create(prefab1, boundary1) && Create(prefab2, boundary2) && Create(prefab3, boundary3))
        {
            CreateOver();
        }
        else
        {
            Debug.Log("��ġ�� �� ����");
        }
    }

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

    void CreateOver()
    {
        Debug.Log($"{Main.Instance.CurrentTile.index} : {Main.Instance.CurrentTile.placementable.Name_KR} ��ġ�Ϸ�");
        //Managers.UI.PauseClose();
        //Managers.UI.ClosePopUp();
        ResetAction();
        Managers.UI.CloseAll();
    }

    bool Create(string prefab, Vector2Int[] boundary)
    {
        if (Main.Instance.CurrentTile == null) return false;

        var tile = Main.Instance.CurrentTile;
        foreach (var item in boundary)
        {
            int _deltaX = tile.index.x + item.x;
            int _deltaY = tile.index.y + item.y;

            var content = Main.Instance.CurrentFloor.TileMap[_deltaX, _deltaY];

            var newObj = Managers.Placement.CreatePlacementObject($"Facility/{prefab}", null, Define.PlacementType.Facility);
            Managers.Placement.PlacementConfirm(newObj, new PlacementInfo(Main.Instance.CurrentFloor, content));
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

            var newObj = Managers.Placement.CreateOnlyOne($"Facility/{prefab}", null, Define.PlacementType.Facility);
            Managers.Placement.PlacementConfirm(newObj, new PlacementInfo(Main.Instance.CurrentFloor, content), true);
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

    public void AddOption(string _text, Vector2Int[] _option, Action<PointerEventData> _action)
    {
        boundaryOption.Add(new Option(_text, _option, _action));
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
    public Action<PointerEventData> Action { get; set; }

    public Option(string option, Vector2Int[] _bounds, Action<PointerEventData> _action)
    {
        optionText = option;
        boundary = _bounds;
        Action = _action;
    }
}