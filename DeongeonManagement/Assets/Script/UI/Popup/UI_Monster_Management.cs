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
        if (isQuickMode) return;

        Init();
    }

    public bool isQuickMode { get; set; }

    enum Panels
    {
        Panel,
        ClosePanel,

        SubPanel,
        GridPanel,
        ButtonPanel,
        SummonPanel,
        ProfilePanel,
        TraitPanel,
        FloorPanel,

        CommandPanel,

        PlacementPanel,
        ModePanel,

        EditPanel,
    }

    enum Texts
    {
        Lv,
        Name,

        DetailInfo_State,
        DetailInfo_AP,
        DetailInfo_Floor,
        DetailInfo_Unit,

        Status_HP,
        Status_ATK,
        Status_DEF,
        Status_AGI,
        Status_LUK,
    }
    enum Etc
    {
        Profile,
        Return,
        Icon_Face_State,
    }

    enum Buttons //? Training ~ Retrieve는 변하면 안됨. 고정int로 반복문 사용중이라 건들면 오류남
    {
        Training,
        Placement,
        Release,
        Recover,
        Retrieve,

        //? Summon
        Summon,

        //? 배치모드에서 회복
        Recover_Floor,

        //? Command
        Command_Fixed,
        Command_Wander,
        Command_Attack,

        //? Mode
        Mode_Managed,
        Mode_Placement,
        Mode_Edit,

        //? Floor
        Floor_Egg,
        Floor_01,
        Floor_02,
        Floor_03,
        Floor_04,

        //? Edit
        Change_Name,
        Change_Slot,
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
        Init_ModeButton();
        Init_FloorButton();
        Init_EditPanel();

        PanelClear();
    }


    public enum Unit_Mode
    {
        Management,
        Placement,
        Edit,
    }
    public Unit_Mode Mode { get; set; }

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
        GetImage(((int)Panels.SummonPanel)).gameObject.SetActive(false);
        GetImage(((int)Panels.PlacementPanel)).gameObject.SetActive(false);
        GetImage(((int)Panels.FloorPanel)).gameObject.SetActive(false);
        GetImage(((int)Panels.EditPanel)).gameObject.SetActive(false);

        GetImage(((int)Panels.ModePanel)).gameObject.SetActive(true);
        GetButton(((int)Buttons.Recover_Floor)).gameObject.SetActive(false);


        switch (Mode)
        {
            case Unit_Mode.Management:
                GetImage(((int)Panels.SummonPanel)).gameObject.SetActive(true);
                GetButton((int)Buttons.Summon).gameObject.AddUIEvent((data) => Show_SummonUI());

                GetButton(((int)Buttons.Mode_Managed)).transform.parent.GetComponent<Image>().color = Color.white;
                GetButton(((int)Buttons.Mode_Placement)).transform.parent.GetComponent<Image>().color = Color.clear;
                GetButton(((int)Buttons.Mode_Edit)).transform.parent.GetComponent<Image>().color = Color.clear;
                break;

            case Unit_Mode.Placement:
                GetImage(((int)Panels.PlacementPanel)).gameObject.SetActive(true);
                GetImage(((int)Panels.FloorPanel)).gameObject.SetActive(true);
                PlacePanelUpdate();

                GetButton(((int)Buttons.Mode_Managed)).transform.parent.GetComponent<Image>().color = Color.clear;
                GetButton(((int)Buttons.Mode_Placement)).transform.parent.GetComponent<Image>().color = Color.white;
                GetButton(((int)Buttons.Mode_Edit)).transform.parent.GetComponent<Image>().color = Color.clear;
                break;

            case Unit_Mode.Edit:
                Init_EditPanel();
                GetImage(((int)Panels.EditPanel)).gameObject.SetActive(true);

                GetButton(((int)Buttons.Mode_Managed)).transform.parent.GetComponent<Image>().color = Color.clear;
                GetButton(((int)Buttons.Mode_Placement)).transform.parent.GetComponent<Image>().color = Color.clear;
                GetButton(((int)Buttons.Mode_Edit)).transform.parent.GetComponent<Image>().color = Color.white;
                break;
        }
    }

    void Show_SummonUI()
    {
        Hide_This();
        var summon = Managers.UI.ShowPopUpAlone<UI_Summon_Monster>("Monster/UI_Summon_Monster");
        summon.OnPopupCloseEvent += () => Show_This();
        //SetNotice(OverlayImages.OverlayImage_Summon, false);
    }

    void Hide_This()
    {
        Managers.UI.ClosePopUp(this);
    }
    void Show_This()
    {
        Managers.UI.ClearAndShowPopUp<UI_Monster_Management>("Monster/UI_Monster_Management");
        //SetNotice(OverlayImages.OverlayImage_Monster, false);
    }



    void PlacePanelUpdate()
    {
        //? 마지막으로 배치된 몬스터(현재 선택되있는 몬스터)의 층 정보로 갱신하기
        string floorName = "";
        int placeMonsters = 0;
        int ableMonsters = 0;

        if (Current != null && Current.monster != null && Current.monster.State == Monster.MonsterState.Placement)
        {
            floorName = Current.monster.PlacementInfo.Place_Floor.LabelName;
            placeMonsters = Current.monster.PlacementInfo.Place_Floor.monsterList.Count;
            ableMonsters = Current.monster.PlacementInfo.Place_Floor.MaxMonsterSize;

            if (Current.monster.PlacementInfo.Place_Floor.FloorIndex == 3)
            {
                placeMonsters--;
            }
        }
        else if (Main.Instance.CurrentFloor != null)
        {
            floorName = Main.Instance.CurrentFloor.LabelName;
            placeMonsters = Main.Instance.CurrentFloor.monsterList.Count;
            ableMonsters = Main.Instance.CurrentFloor.MaxMonsterSize;

            if (Main.Instance.CurrentFloor.FloorIndex == 3)
            {
                placeMonsters--;
            }
        }

        GetTMP((int)Texts.DetailInfo_Floor).text = $"{floorName}";

        if (string.IsNullOrEmpty(floorName))
        {
            GetTMP((int)Texts.DetailInfo_Unit).text = "";
        }
        else
        {
            GetTMP((int)Texts.DetailInfo_Unit).text = $"{UserData.Instance.LocaleText("배치된 몬스터")} : {placeMonsters}\n" +
                $"{UserData.Instance.LocaleText("배치가능 몬스터")} : {ableMonsters}";
        }

        GetTMP((int)Texts.DetailInfo_AP).text = "";
        GetTMP((int)Texts.DetailInfo_State).text = "";
    }

    void PlacePanelUpdate(int floorIndex)
    {
        Main.Instance.CurrentFloor = Main.Instance.Floor[floorIndex];

        string floorName = Main.Instance.CurrentFloor.LabelName;
        int placeMonsters = Main.Instance.CurrentFloor.monsterList.Count;
        int ableMonsters = Main.Instance.CurrentFloor.MaxMonsterSize;
        if (Main.Instance.CurrentFloor.FloorIndex == 3)
        {
            placeMonsters--;
        }

        GetTMP((int)Texts.DetailInfo_Floor).text = $"{floorName}";

        GetTMP((int)Texts.DetailInfo_Unit).text = $"{UserData.Instance.LocaleText("배치된 몬스터")} : {placeMonsters}\n" +
            $"{UserData.Instance.LocaleText("배치가능 몬스터")} : {ableMonsters}";

        GetTMP((int)Texts.DetailInfo_AP).text = "";
        GetTMP((int)Texts.DetailInfo_State).text = "";
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


    public void MonsterBox_ClickEvent(UI_MonsterBox selected)
    {
        if (Cor_SlotChangeWait != null)
        {
            isChanging = true;
            Current = selected;
            return;
        }

        if (Current != null && Current == selected && selected.monster != null && Mode == Unit_Mode.Management)
        {
            switch (selected.monster.State)
            {
                case Monster.MonsterState.Standby:
                    PlacementEvent(Define.Boundary_1x1, () => CreateAll(Current.monster.MonsterID));
                    break;

                case Monster.MonsterState.Placement:
                    selected.monster.MonsterOutFloor();
                    StartCoroutine(RefreshAll());
                    break;

                case Monster.MonsterState.Injury:
                    break;
            }
        }
        else
        {
            ShowDetail(selected);
        }
    }


    public void ShowDetail(UI_MonsterBox selected)
    {
        SelectedPanel(selected);
        if (selected.monster == null)
        {
            //TextClear();
            PanelClear();
            return;
        }

        switch (Mode)
        {
            case Unit_Mode.Management:
                AddFloorInfo();
                AddButtonEvent();
                AddCommandPanel();
                AddTextContents();
                break;

            case Unit_Mode.Placement:
                if (selected.monster.State != Monster.MonsterState.Injury)
                {
                    PanelClear();
                    PlacementEvent(Define.Boundary_1x1, () => CreateAll(Current.monster.MonsterID));
                }
                else
                {
                    GetButton(((int)Buttons.Recover_Floor)).gameObject.SetActive(true);
                    GetButton((int)Buttons.Recover_Floor).gameObject.RemoveUIEventAll();

                    AddEvent_Recover(GetButton(((int)Buttons.Recover_Floor)).gameObject);
                    GetButton(((int)Buttons.Recover_Floor)).gameObject.AddUIEvent(data =>
                    {
                        Reset_Selected();
                        PanelClear();
                        for (int i = 0; i < childList.Count; i++)
                        {
                            childList[i].ShowContents();
                        }
                    });
                }
                break;

            case Unit_Mode.Edit:
                AddEditButtonEvent();
                break;
        }

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
        GetTMP((int)Texts.DetailInfo_AP).text = $"{UserData.Instance.LocaleText("AP")} : {Main.Instance.Player_AP}";
        GetTMP((int)Texts.DetailInfo_Floor).text = "";
        GetTMP((int)Texts.DetailInfo_Unit).text = "";
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
        selected.ChangePanelColor(Color.white);

        Current = selected;
        //Debug.Log(Current.monster.MonsterID);
        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].ChangePanelColor(Color.white);
        }
    }

    void Reset_Selected()
    {
        Current = null;
        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].ChangePanelColor(Color.white);
        }

        GetTMP((int)Texts.DetailInfo_State).text = "";
        GetTMP((int)Texts.DetailInfo_AP).text = "";
        GetTMP((int)Texts.DetailInfo_Floor).text = "";
        GetTMP((int)Texts.DetailInfo_Unit).text = "";

        if (Cor_SlotChangeWait != null)
        {
            StopCoroutine(Cor_SlotChangeWait);
            Cor_SlotChangeWait = null;
        }
    }


    void AddTextContents()
    {
        if (Current == null || Current.monster == null) return;

        GetImage((int)Panels.ProfilePanel).gameObject.SetActive(true);

        GetTMP((int)Texts.Lv).text = $"Lv.{Current.monster.LV} / {Current.monster.Data.maxLv}";
        GetTMP((int)Texts.Name).text = Current.monster.CallName;

        GetTMP((int)Texts.Status_HP).text = $"{Current.monster.HP}/{Current.monster.HP_Max} " +
            $"{Util.SetTextColorTag($"(+{Current.monster.B_HP - Current.monster.HP})", Define.TextColor.npc_red)}";

        GetTMP((int)Texts.Status_ATK).text = $"{Current.monster.ATK} " +
            $"{Util.SetTextColorTag($"(+{Current.monster.B_ATK - Current.monster.ATK})", Define.TextColor.npc_red)}";

        GetTMP((int)Texts.Status_DEF).text = $"{Current.monster.DEF} " +
            $"{Util.SetTextColorTag($"(+{Current.monster.B_DEF - Current.monster.DEF})", Define.TextColor.npc_red)}";

        GetTMP((int)Texts.Status_AGI).text = $"{Current.monster.AGI} " +
            $"{Util.SetTextColorTag($"(+{Current.monster.B_AGI - Current.monster.AGI})", Define.TextColor.npc_red)}";

        GetTMP((int)Texts.Status_LUK).text = $"{Current.monster.LUK} " +
            $"{Util.SetTextColorTag($"(+{Current.monster.B_LUK - Current.monster.LUK})", Define.TextColor.npc_red)}";


        var TraitPanel = GetImage((int)Panels.TraitPanel).transform;
        //? 특성 초기화
        for (int i = TraitPanel.childCount - 1; i >= 0; i--)
        {
            Managers.Resource.Destroy(TraitPanel.GetChild(i).gameObject);
        }
        //? 특성 새로추가
        for (int i = 0; i < Current.monster.TraitList.Count; i++)
        {
            GameManager.Trait.CreateTraitBar(Current.monster.TraitList[i].ID, TraitPanel);
        }
        
        GetObject(((int)Etc.Profile)).GetComponent<Image>().sprite = Managers.Sprite.GetSprite(Current.monster.Data.spritePath);
    }

    #endregion


    #region PlacementPanel
    void Init_FloorButton()
    {
        GetButton(((int)Buttons.Floor_Egg)).gameObject.AddUIEvent(data => PlacePanelUpdate(3));
        GetButton(((int)Buttons.Floor_01)).gameObject.AddUIEvent(data => PlacePanelUpdate(0));
        GetButton(((int)Buttons.Floor_02)).gameObject.AddUIEvent(data => PlacePanelUpdate(1));
        GetButton(((int)Buttons.Floor_03)).gameObject.AddUIEvent(data => PlacePanelUpdate(2));
        GetButton(((int)Buttons.Floor_04)).gameObject.AddUIEvent(data => PlacePanelUpdate(4));

        GetButton(((int)Buttons.Floor_Egg)).GetComponentInChildren<TextMeshProUGUI>().text =
    $"{UserData.Instance.LocaleText("숨겨진곳")}";

        GetButton(((int)Buttons.Floor_01)).GetComponentInChildren<TextMeshProUGUI>().text = 
    $"{UserData.Instance.LocaleText("지하")} {1} {UserData.Instance.LocaleText("층")}";
        GetButton(((int)Buttons.Floor_02)).GetComponentInChildren<TextMeshProUGUI>().text =
    $"{UserData.Instance.LocaleText("지하")} {2} {UserData.Instance.LocaleText("층")}";
        GetButton(((int)Buttons.Floor_03)).GetComponentInChildren<TextMeshProUGUI>().text =
    $"{UserData.Instance.LocaleText("지하")} {3} {UserData.Instance.LocaleText("층")}";
        GetButton(((int)Buttons.Floor_04)).GetComponentInChildren<TextMeshProUGUI>().text =
    $"{UserData.Instance.LocaleText("지하")} {4} {UserData.Instance.LocaleText("층")}";
    }

    #endregion

    #region ModePanel
    void Init_ModeButton()
    {
        GetButton(((int)Buttons.Mode_Managed)).gameObject.AddUIEvent(data => ChangeMode(Unit_Mode.Management));
        GetButton(((int)Buttons.Mode_Placement)).gameObject.AddUIEvent(data => ChangeMode(Unit_Mode.Placement));
        GetButton(((int)Buttons.Mode_Edit)).gameObject.AddUIEvent(data => ChangeMode(Unit_Mode.Edit));
    }

    void ChangeMode(Unit_Mode _mode)
    {
        Reset_Selected();
        Mode = _mode;
        PanelClear();
    }

    #endregion


    #region CommandPanel
    void Init_CommandButton()
    {
        GetButton(((int)Buttons.Command_Fixed)).gameObject.AddUIEvent(data => ChangeMoveMode(Monster.MoveType.Fixed));
        GetButton(((int)Buttons.Command_Wander)).gameObject.AddUIEvent(data => ChangeMoveMode(Monster.MoveType.Wander));
        GetButton(((int)Buttons.Command_Attack)).gameObject.AddUIEvent(data => ChangeMoveMode(Monster.MoveType.Attack));

        var fix = GetButton(((int)Buttons.Command_Fixed)).gameObject.GetOrAddComponent<UI_Tooltip>();
        fix.SetTooltipContents("", UserData.Instance.LocaleText_Tooltip("Command_Fixed"), UI_TooltipBox.ShowPosition.LeftUp);

        var wander = GetButton(((int)Buttons.Command_Wander)).gameObject.GetOrAddComponent<UI_Tooltip>();
        wander.SetTooltipContents("", UserData.Instance.LocaleText_Tooltip("Command_Wander"), UI_TooltipBox.ShowPosition.LeftUp);

        var attack = GetButton(((int)Buttons.Command_Attack)).gameObject.GetOrAddComponent<UI_Tooltip>();
        attack.SetTooltipContents("", UserData.Instance.LocaleText_Tooltip("Command_Attack"), UI_TooltipBox.ShowPosition.LeftUp);
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
                GetButton((int)Buttons.Command_Fixed).transform.parent.GetComponent<Image>().color = Color.white;
                GetButton((int)Buttons.Command_Wander).transform.parent.GetComponent<Image>().color = Color.clear;
                GetButton((int)Buttons.Command_Attack).transform.parent.GetComponent<Image>().color = Color.clear;
                break;

            case Monster.MoveType.Wander:
                GetButton((int)Buttons.Command_Fixed).transform.parent.GetComponent<Image>().color = Color.clear;
                GetButton((int)Buttons.Command_Wander).transform.parent.GetComponent<Image>().color = Color.white;
                GetButton((int)Buttons.Command_Attack).transform.parent.GetComponent<Image>().color = Color.clear;

                break;

            case Monster.MoveType.Attack:
                GetButton((int)Buttons.Command_Fixed).transform.parent.GetComponent<Image>().color = Color.clear;
                GetButton((int)Buttons.Command_Wander).transform.parent.GetComponent<Image>().color = Color.clear;
                GetButton((int)Buttons.Command_Attack).transform.parent.GetComponent<Image>().color = Color.white;
                break;
        }
    }
    #endregion


    #region EditPanel

    void Init_EditPanel()
    {
        GetButton((int)Buttons.Change_Name).gameObject.SetActive(false);
        GetButton((int)Buttons.Change_Slot).gameObject.SetActive(false);
        GetImage((int)Panels.EditPanel).gameObject.SetActive(false);
    }


    void AddEditButtonEvent()
    {
        GetImage((int)Panels.EditPanel).gameObject.SetActive(true);

        GetButton((int)Buttons.Change_Name).gameObject.SetActive(true);
        GetButton((int)Buttons.Change_Name).gameObject.RemoveUIEventAll();
        GetButton((int)Buttons.Change_Name).gameObject.AddUIEvent(data => ChangeNameEvent());

        GetButton((int)Buttons.Change_Slot).gameObject.SetActive(true);
        GetButton((int)Buttons.Change_Slot).gameObject.RemoveUIEventAll();
        GetButton((int)Buttons.Change_Slot).gameObject.AddUIEvent(data => ChangeSlotEvent());
    }



    void ChangeNameEvent()
    {
        var ui = Managers.UI.ShowPopUp<UI_InputField>();
        ui.SetAction(_value => ChangeUnitName(_value), Current.monster.CallName, Current.monster.CallName);
    }

    void ChangeUnitName(string _value)
    {
        Current.monster.CustomName = _value;
        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].ShowContents();
        }
    }

    void ChangeSlotEvent()
    {
        //? 다른 슬롯 클릭을 기다렸다가
        Cor_SlotChangeWait = StartCoroutine(SlotChange());

        //? 자기와 다른 슬롯클릭이 감지되면

        //? 작업을 종료하고 초기화
    }

    bool isChanging { get; set; } = false;


    Coroutine Cor_SlotChangeWait;
    IEnumerator SlotChange()
    {
        int first = childList.IndexOf(Current);
        yield return new WaitUntil(() => isChanging);
        int second = childList.IndexOf(Current);

        var mon = GameManager.Monster.Monsters[first];
        GameManager.Monster.Monsters[first] = GameManager.Monster.Monsters[second];
        GameManager.Monster.Monsters[second] = mon;

        foreach (var item in childList)
        {
            Managers.Resource.Destroy(item.gameObject);
        }

        CreateMonsterBox();
        Init_EditPanel();
        isChanging = false;
        Cor_SlotChangeWait = null;
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
        GetButton(((int)Buttons.Recover_Floor)).gameObject.SetActive(false);
    }
    IEnumerator RefreshAll()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Debug.Log("창 새로고침");
        GetTMP((int)Texts.DetailInfo_AP).text = $"{UserData.Instance.LocaleText("AP")} : {Main.Instance.Player_AP}";

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

        if (Mode != Unit_Mode.Management)
        {
            return;
        }

        GetImage(((int)Panels.ButtonPanel)).gameObject.SetActive(true);
        GetImage(((int)Panels.SummonPanel)).gameObject.SetActive(false);
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
                GetButton(((int)Buttons.Training)).GetComponentInChildren<TextMeshProUGUI>().text = 
                    $"{UserData.Instance.LocaleText("훈련")}(1)";

                GetButton(((int)Buttons.Release)).gameObject.
                    AddUIEvent((data) => GameManager.Monster.ReleaseMonster(Current.monster.MonsterID));

                GetTMP((int)Texts.DetailInfo_State).text = $"{UserData.Instance.LocaleText("대기중")}\n";
                GetObject((int)Etc.Icon_Face_State).GetComponent<Image>().sprite = Managers.Sprite.GetSprite_SLA("Element_State", "Perfect");
                GetTMP((int)Texts.DetailInfo_AP).text = $"{UserData.Instance.LocaleText("AP")} : {Main.Instance.Player_AP}";
                GetTMP((int)Texts.DetailInfo_Floor).text = "";
                GetTMP((int)Texts.DetailInfo_Unit).text = "";
                break;



            case Monster.MonsterState.Placement:
                GetButton(((int)Buttons.Placement)).gameObject.SetActive(false);
                GetButton(((int)Buttons.Release)).gameObject.SetActive(false);
                GetButton(((int)Buttons.Recover)).gameObject.SetActive(false);

                GetButton(((int)Buttons.Retrieve)).gameObject.
                    AddUIEvent((data) => Current.monster.MonsterOutFloor());

                GetButton(((int)Buttons.Training)).gameObject.
                    AddUIEvent((data) => Current.monster.Training());
                GetButton(((int)Buttons.Training)).GetComponentInChildren<TextMeshProUGUI>().text = 
                    $"{UserData.Instance.LocaleText("훈련")}(1)";

                GetTMP((int)Texts.DetailInfo_State).text = $"{UserData.Instance.LocaleText("배치중")}";
                GetTMP((int)Texts.DetailInfo_Floor).text = $"{Current.monster.PlacementInfo.Place_Floor.LabelName}";
                GetTMP((int)Texts.DetailInfo_Unit).text = "";
                GetObject((int)Etc.Icon_Face_State).GetComponent<Image>().sprite = Managers.Sprite.GetSprite_SLA("Element_State", "Good");
                GetTMP((int)Texts.DetailInfo_AP).text = $"{UserData.Instance.LocaleText("AP")} : {Main.Instance.Player_AP}";
                
                break;



            case Monster.MonsterState.Injury:
                GetButton(((int)Buttons.Training)).gameObject.SetActive(false);
                GetButton(((int)Buttons.Placement)).gameObject.SetActive(false);
                GetButton(((int)Buttons.Retrieve)).gameObject.SetActive(false);

                GetButton(((int)Buttons.Release)).gameObject.
                    AddUIEvent((data) => GameManager.Monster.ReleaseMonster(Current.monster.MonsterID));

                AddEvent_Recover(GetButton(((int)Buttons.Recover)).gameObject);
                break;
        }
    }



    void AddEvent_Recover(GameObject targetButton)
    {
        int RecoverCost = (int)(((Current.monster.LV * 0.08f) + 0.3f) * Current.monster.Data.manaCost);

        if (Current.monster.TraitCheck(TraitGroup.ShirkingC))
        {
            RecoverCost = Mathf.RoundToInt(RecoverCost * 0.9f);
        }
        if (Current.monster.TraitCheck(TraitGroup.ShirkingB))
        {
            RecoverCost = Mathf.RoundToInt(RecoverCost * 0.8f);
        }
        if (Current.monster.TraitCheck(TraitGroup.ShirkingA))
        {
            RecoverCost = Mathf.RoundToInt(RecoverCost * 0.7f);
        }

            
        targetButton.AddUIEvent((data) => Current.monster.Recover(RecoverCost));
        targetButton.GetComponentInChildren<TextMeshProUGUI>().text = $"{UserData.Instance.LocaleText("회복")}({RecoverCost})";
        GetObject((int)Etc.Icon_Face_State).GetComponent<Image>().sprite = Managers.Sprite.GetSprite_SLA("Element_State", "Bad");
        GetTMP((int)Texts.DetailInfo_State).text = $"{UserData.Instance.LocaleText("회복")} {UserData.Instance.LocaleText("Mana")} : {RecoverCost}";
        GetTMP((int)Texts.DetailInfo_Floor).text = "";
        GetTMP((int)Texts.DetailInfo_Unit).text = "";
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
        var dp = Managers.UI.ShowPopUpAlone<UI_DungeonPlacement>();

        //? 테스트!
        var letter = Managers.UI.ShowPopUpNonPush<UI_LetterBox>();
        letter.SetBoxOption(UI_LetterBox.BoxOption.Monster, dp);

        yield return new WaitForEndOfFrame();
        yield return null;
        PanelDisable();

        UserData.Instance.GamePlay();

        Main.Instance.CurrentBoundary = vector2Ints;
        Main.Instance.CurrentAction = action;
        for (int i = 0; i < Main.Instance.ActiveFloor_Basement; i++)
        {
            if (Main.Instance.Floor[i].MaxMonsterSize > 0)
            {
                Main.Instance.Floor[i].UI_Floor.ShowTile();
            }
            else
            {
                Main.Instance.Floor[i].UI_Floor.ShowTile_IgnoreEvent(Define.Color_Red);
            }
        }

        FindAnyObjectByType<UI_Management>().Hide_MainCanvas();
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
            SoundManager.Instance.PlaySound("SFX/Action_Build");
        }
        else
        {
            Debug.Log("배치할 수 없음");
            SoundManager.Instance.PlaySound("SFX/Action_Wrong");
        }
    }

    void CreateOver()
    {
        Debug.Log($"{Current.monster.Name_Color}(이)가 {Main.Instance.CurrentTile.floor.LabelName}에 배치되었습니다");
        ResetAction();
    }
    void ResetAction()
    {
        Main.Instance.ResetCurrentAction();

        switch (Mode)
        {
            case Unit_Mode.Management:
                StartCoroutine(RefreshAll());
                break;

            case Unit_Mode.Placement:
                PlacePanelUpdate();
                break;

            case Unit_Mode.Edit:
                break;
        }
    }



    public void QuickPlacement(int MonsterID)
    {
        StartCoroutine(ShowAllFloor(Define.Boundary_1x1, () => CreateQuick(MonsterID)));
    }

    void CreateQuick(int monsterID)
    {
        if (Create(monsterID, Main.Instance.CurrentBoundary))
        {
            SoundManager.Instance.PlaySound("SFX/Action_Build");
            Main.Instance.ResetCurrentAction();
            Managers.UI.ClosePopUp();
        }
        else
        {
            Debug.Log("배치할 수 없음");
            SoundManager.Instance.PlaySound("SFX/Action_Wrong");
        }

    }


    #endregion



    public override void PauseRefresh()
    {
        if (isQuickMode)
        {
            Managers.UI.ClosePopUp(this);
        }
    }



    public override bool EscapeKeyAction()
    {
        return true;
    }


    private void OnEnable()
    {
        Time.timeScale = 0;
    }
    private void OnDestroy()
    {
        PopupUI_OnDestroy();
    }
}
