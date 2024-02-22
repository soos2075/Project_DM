using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Monster_Management : UI_PopUp
{
    void Start()
    {
        Init();
    }


    enum Panels
    {
        Panel,
        ClosePanel,

        SubPanel,
        GridPanel,
        ButtonPanel,
        ProfilePanel,
        FloorPanel,

        CommandPanel,
        atk,
        def,
        wan,
    }

    enum Texts
    {
        Lv,
        Name,
        Status,
        State,
        DetailInfo,
    }
    enum Etc
    {
        Profile,
        Return,
    }

    enum Buttons
    {
        Training,
        Placement,
        Release,
        Recover,
        Retrieve,

        //? Command
        Command_Attack,
        Command_Defend,
        Command_Wander,
    }

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        Bind<Image>(typeof(Panels));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(Etc));
        Bind<Button>(typeof(Buttons));

        Init_Panels();

        CreateMonsterBox();
        Init_CommandButton();
        PanelClear();
    }


    public enum UI_Type
    {
        Management,
        Placement,
    }
    public UI_Type Type;


    void Init_Panels()
    {
        GetImage(((int)Panels.ClosePanel)).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.LeftClick);
        GetImage(((int)Panels.ClosePanel)).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.RightClick);
        GetImage(((int)Panels.Panel)).gameObject.AddUIEvent((data) => ClosePopUp(), Define.UIEvent.RightClick);

        GetObject((int)Etc.Return).gameObject.AddUIEvent(data => CloseAll());
    }

    void PanelClear()
    {
        GetImage(((int)Panels.ProfilePanel)).gameObject.SetActive(false);
        GetImage(((int)Panels.ButtonPanel)).gameObject.SetActive(false);
        GetImage(((int)Panels.CommandPanel)).gameObject.SetActive(false);

        if (Type == UI_Type.Placement)
        {
            GetImage(((int)Panels.FloorPanel)).gameObject.SetActive(true);
            GetTMP((int)Texts.DetailInfo).text = $"{Main.Instance.CurrentFloor.Name_KR}\n배치된 몬스터 : {Main.Instance.CurrentFloor.monsterList.Count}\n" +
                $"추가 배치가능 몬스터 : {Main.Instance.CurrentFloor.MaxMonsterSize}";
        }
        else
        {
            GetImage(((int)Panels.FloorPanel)).gameObject.SetActive(false);
        }
    }




    void CreateMonsterBox()
    {
        childList = new List<UI_MonsterBox>();

        for (int i = 0; i < GameManager.Monster.Monsters.Length; i++)
        {
            var obj = Managers.Resource.Instantiate("UI/PopUp/Monster/MonsterBox", GetImage(((int)Panels.GridPanel)).transform);

            var box = obj.GetComponent<UI_MonsterBox>();
            box.monster = GameManager.Monster.Monsters[i];
            box.parent = this;
            childList.Add(box);
        }
    }

    public UI_MonsterBox Current { get; private set; }
    List<UI_MonsterBox> childList;

    public void ShowDetail(UI_MonsterBox selected)
    {
        SelectedPanel(selected);
        if (selected.monster == null)
        {
            //TextClear();
            PanelClear();
            return;
        }

        if (Type == UI_Type.Placement && selected.monster.State != Monster.MonsterState.Injury)
        {
            //PlacementOne(Define.Boundary_1x1, () => CreateAll(Current.monster.MonsterID));
            PlacementEvent(Define.Boundary_1x1, () => CreateAll(Current.monster.MonsterID));
            return;
        }


        AddFloorInfo();
        AddButtonEvent();
        AddCommandPanel();
        AddTextContents();
    }

    public void ShowDetail(Monster _monster)
    {
        foreach (var item in childList)
        {
            if (item.monster == _monster)
            {
                ShowDetail(item);
            }
        }
    }


    #region FloorPanel

    void AddFloorInfo()
    {
        GetImage(((int)Panels.FloorPanel)).gameObject.SetActive(true);
        GetTMP((int)Texts.DetailInfo).text = $"남은 행동력 : {Main.Instance.Player_AP}";
    }

    #endregion


    #region ProfilePanel
    //void TextClear()
    //{
    //    GetTMP(((int)Texts.Lv)).text = "";
    //    GetTMP(((int)Texts.Name)).text = "";
    //    GetTMP(((int)Texts.Status)).text = "";
    //    GetTMP(((int)Texts.State)).text = "";
    //    GetObject(((int)Etc.Profile)).GetComponent<Image>().sprite = Managers.Sprite.GetClear();
    //}

    void SelectedPanel(UI_MonsterBox selected)
    {
        selected.ChangePanelColor(Define.Color_Alpha_2);

        Current = selected;
        //Debug.Log(Current.monster.MonsterID);
        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].ChangePanelColor(Define.Color_Alpha_6);
        }
    }


    void AddTextContents()
    {
        if (Current == null || Current.monster == null) return;

        GetImage(((int)Panels.ProfilePanel)).gameObject.SetActive(true);

        GetTMP(((int)Texts.Lv)).text = $"Lv.{Current.monster.LV}";
        GetTMP(((int)Texts.Name)).text = Current.monster.Name_KR;

        GetTMP(((int)Texts.Status)).text = $"HP : {Current.monster.HP} / {Current.monster.HP_Max} \n";
        GetTMP(((int)Texts.Status)).text += $"ATK : {Current.monster.ATK} \tDEF : {Current.monster.DEF} \n" +
            $"AGI : {Current.monster.AGI} \tLUK : {Current.monster.LUK}";

        GetTMP(((int)Texts.State)).text = "진화조건 : ???";

        GetObject(((int)Etc.Profile)).GetComponent<Image>().sprite = Current.monster.Data.sprite;
    }

    #endregion

    #region CommandPanel
    void Init_CommandButton()
    {
        GetButton(((int)Buttons.Command_Attack)).gameObject.AddUIEvent(data => ChangeMoveMode(Monster.MoveType.Move_Hunting));
        GetButton(((int)Buttons.Command_Defend)).gameObject.AddUIEvent(data => ChangeMoveMode(Monster.MoveType.Fixed));
        GetButton(((int)Buttons.Command_Wander)).gameObject.AddUIEvent(data => ChangeMoveMode(Monster.MoveType.Move_Wandering));
    }

    void ChangeMoveMode(Monster.MoveType _mode)
    {
        if (Current == null || Current.monster == null) return;

        Current.monster.SetMoveType(_mode);
        //Debug.Log("change");
        SelectedImage();
    }

    void AddCommandPanel()
    {
        if (Current == null || Current.monster == null) return;

        GetImage(((int)Panels.CommandPanel)).gameObject.SetActive(true);
        SelectedImage();
    }
    void SelectedImage()
    {
        switch (Current.monster.Mode)
        {
            case Monster.MoveType.Fixed:
                GetImage(((int)Panels.atk)).color = Color.clear;
                GetImage(((int)Panels.def)).color = Color.white;
                GetImage(((int)Panels.wan)).color = Color.clear;
                break;

            case Monster.MoveType.Move_Wandering:
                GetImage(((int)Panels.atk)).color = Color.clear;
                GetImage(((int)Panels.def)).color = Color.clear;
                GetImage(((int)Panels.wan)).color = Color.white;
                break;

            case Monster.MoveType.Move_Hunting:
                GetImage(((int)Panels.atk)).color = Color.white;
                GetImage(((int)Panels.def)).color = Color.clear;
                GetImage(((int)Panels.wan)).color = Color.clear;
                break;
        }
    }
    #endregion

    #region ButtonPanel

    void Init_ButtonEvent()
    {
        for (int i = 0; i < 5; i++)
        {
            GetButton(i).gameObject.SetActive(true);
            GetButton(i).gameObject.RemoveUIEventAll();
            GetButton(i).gameObject.AddUIEvent((data) => StartCoroutine(RefreshAll()));
        }
    }
    IEnumerator RefreshAll()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("창 새로고침");
        GetTMP((int)Texts.DetailInfo).text = $"남은 행동력 : {Main.Instance.Player_AP}";
        ShowDetail(Current);
        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].ShowContents();
        }
        AddButtonEvent();
    }

    void AddButtonEvent()
    {
        if (Current == null || Current.monster == null) return;

        GetImage(((int)Panels.ButtonPanel)).gameObject.SetActive(true);
        Init_ButtonEvent();

        switch (Current.monster.State)
        {
            case Monster.MonsterState.Standby:
                GetButton(((int)Buttons.Retrieve)).gameObject.SetActive(false);
                GetButton(((int)Buttons.Recover)).gameObject.SetActive(false);

                GetButton(((int)Buttons.Placement)).gameObject.
                    AddUIEvent((data) => PlacementEvent(Define.Boundary_1x1, () => CreateAll(Current.monster.MonsterID)));

                GetButton(((int)Buttons.Training)).gameObject.
                    AddUIEvent((data) => Current.monster.Training());

                GetButton(((int)Buttons.Release)).gameObject.
                    AddUIEvent((data) => GameManager.Monster.ReleaseMonster(Current.monster.MonsterID));

                GetTMP((int)Texts.DetailInfo).text = $"대기중\n남은 행동력 : {Main.Instance.Player_AP}";
                break;



            case Monster.MonsterState.Placement:
                GetButton(((int)Buttons.Placement)).gameObject.SetActive(false);
                GetButton(((int)Buttons.Release)).gameObject.SetActive(false);
                GetButton(((int)Buttons.Recover)).gameObject.SetActive(false);

                GetButton(((int)Buttons.Retrieve)).gameObject.
                    AddUIEvent((data) => Current.monster.MonsterOutFloor());

                GetButton(((int)Buttons.Training)).gameObject.
                    AddUIEvent((data) => Current.monster.Training());

                GetTMP((int)Texts.DetailInfo).text = $"배치중 : {Current.monster.PlacementInfo.Place_Floor.Name_KR}\n남은 행동력 : {Main.Instance.Player_AP}";
                break;



            case Monster.MonsterState.Injury:
                GetButton(((int)Buttons.Training)).gameObject.SetActive(false);
                GetButton(((int)Buttons.Placement)).gameObject.SetActive(false);
                GetButton(((int)Buttons.Retrieve)).gameObject.SetActive(false);

                GetButton(((int)Buttons.Release)).gameObject.
                    AddUIEvent((data) => GameManager.Monster.ReleaseMonster(Current.monster.MonsterID));

                int RecoverCost = (int)(((Current.monster.LV * 0.15f) + 0.25f) * Current.monster.Data.ManaCost);
                GetButton(((int)Buttons.Recover)).gameObject.
                    AddUIEvent((data) => Current.monster.Recover(RecoverCost));


                GetTMP((int)Texts.DetailInfo).text = $"부상 회복 필요 마나 : {RecoverCost}";
                break;
        }
    }





    //void PlacementOne(Vector2Int[] vector2Ints, Action action)
    //{
    //    if (Current.monster.State == Monster.MonsterState.Placement)
    //    {
    //        Current.monster.MonsterOutFloor();
    //    }
    //    if (Main.Instance.CurrentFloor.MaxMonsterSize <= 0)
    //    {
    //        return;
    //    }

    //    Managers.UI.PausePopUp(this);
    //    Time.timeScale = 1;
    //    Main.Instance.CurrentBoundary = vector2Ints;
    //    Main.Instance.CurrentAction = action;
    //    Main.Instance.CurrentFloor.UI_Floor.ShowTile();
    //}


    void PlacementEvent(Vector2Int[] vector2Ints, Action action)
    {
        if (Current == null || Current.monster == null) return;

        if (Current.monster.State == Monster.MonsterState.Placement)
        {
            Current.monster.MonsterOutFloor();
        }

        StartCoroutine(ShowAllFloor(vector2Ints, action));
    }
    IEnumerator ShowAllFloor(Vector2Int[] vector2Ints, Action action)
    {
        Managers.UI.PausePopUp(this);
        Managers.UI.ShowPopUpAlone<UI_DungeonPlacement>();

        yield return new WaitForEndOfFrame();
        PanelDisable();

        Time.timeScale = 1;
        Main.Instance.CurrentBoundary = vector2Ints;
        Main.Instance.CurrentAction = action;
        for (int i = 0; i < Main.Instance.ActiveFloor_Basement; i++)
        {
            if (Main.Instance.Floor[i].MaxMonsterSize > 0)
            {
                Main.Instance.Floor[i].UI_Floor.ShowTile();
            }
        }
    }
    public void PanelDisable()
    {
        var floorList = FindObjectsOfType<UI_Floor>();
        foreach (var item in floorList)
        {
            item.GetComponent<Image>().enabled = false;
        }
    }



    bool Create(int monsterID, Vector2Int[] boundary)
    {
        if (Main.Instance.CurrentTile == null) return false;

        var tile = Main.Instance.CurrentTile;
        //Main.Instance.CurrentFloor = tile.floor;

        foreach (var item in boundary)
        {
            Vector2Int delta = tile.index + item;
            BasementTile temp = null;
            if (tile.floor.TileMap.TryGetValue(delta, out temp))
            {
                var obj = GameManager.Monster.Monsters[monsterID];
                GameManager.Placement.PlacementConfirm(obj, new PlacementInfo(tile.floor, temp));

                obj.PlacementInfo.Place_Floor.MaxMonsterSize--;
                obj.State = Monster.MonsterState.Placement;
            }
        }
        return true;
    }

    void CreateAll(int monsterID)
    {
        if (Create(monsterID, Main.Instance.CurrentBoundary))
        {
            CreateOver();
        }
        else
        {
            Debug.Log("배치할 수 없음");
        }
    }

    void CreateOver()
    {
        Debug.Log($"{Current.monster.Name_KR}(이)가 {Main.Instance.CurrentTile.floor.Name_KR}에 배치되었습니다");
        ResetAction();
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
        StartCoroutine(RefreshAll());
    }


    #endregion



    private void OnEnable()
    {
        Time.timeScale = 0;
    }
    private void OnDestroy()
    {
        Time.timeScale = 1;
    }
}
