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

    public Sprite Command_Select;
    public Sprite Command_NonSelect;

    public Sprite Button_Normal;
    public Sprite Button_Push;

    enum Images
    {
        Profile,

        Tooltip_Fix,
        Tooltip_Wander,
        Tooltip_Attack,

        Image_HP,
        Image_ATK,
        Image_DEF,
        Image_AGI,
        Image_LUK,
    }

    enum Texts
    {
        Lv,
        Name,

        DetailInfo_State,
        DetailInfo_Floor,
        //DetailInfo_AP,
        //DetailInfo_Unit,

        Status_HP,
        Status_ATK,
        Status_DEF,
        Status_AGI,
        Status_LUK,

        Value_trainingAP,
        Value_recoverMana,
        Value_eventAP,
    }
    enum Etc
    {
        UnitCount,

        Return,
        //Icon_Face_State,

        Panel,
        ClosePanel,

        SubPanel,
        GridPanel,

        ProfilePanel,
        ButtonPanel,
        StatusPanel,
        TraitPanel,

        UnitActionGroup,
        CommandGroup,

        Summon,
    }

    enum Buttons
    {
        Training,
        Placement,
        Release,
        Recover,
        Retrieve,

        UnitEvent,


        //? Edit
        Change_Name,
        Change_Slot,


        //? Command
        Command_Fixed,
        Command_Wander,
        Command_Attack,


        //? 일괄 행동
        All_Placement,
        All_Retrieve,
        All_Fix,
        All_Wander,
        All_Attack,
    }

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        Bind<Image>(typeof(Images));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(Etc));
        Bind<Button>(typeof(Buttons));


        Init_Panels();
        CreateMonsterBox();
        Init_CommandButton();
        //Init_ModeButton();
        //Init_FloorButton();
        //Init_EditPanel();

        PanelClear();
        Init_DefaultSetting();

        Init_All_Button();
    }


    void Init_All_Button()
    {
        GetButton(((int)Buttons.All_Placement)).gameObject.AddUIEvent(data => All_Placement());
        GetButton(((int)Buttons.All_Retrieve)).gameObject.AddUIEvent(data => All_Retrieve());
        GetButton(((int)Buttons.All_Fix)).gameObject.AddUIEvent(data => All_Fix());
        GetButton(((int)Buttons.All_Wander)).gameObject.AddUIEvent(data => All_Wander());
        GetButton(((int)Buttons.All_Attack)).gameObject.AddUIEvent(data => All_Attack());
    }


    void All_Placement()
    {
        var mon = GameManager.Monster.GetMonsterAll(); //? player는 포함 안되는거 확인
        var standbyList = new Queue<Monster>(); //? 배치할 수 있는 리스트
        foreach (var item in mon)
        {
            if (item.State == Monster.MonsterState.Standby)
            {
                standbyList.Enqueue(item);
            }
        }

        var currentFloor = Main.Instance.ActiveFloor_Basement - 1;

        for (int i = currentFloor; i > 0; currentFloor--)
        {
            var floor = Main.Instance.Floor[currentFloor];
            while (floor.MaxMonsterSize > 0 && standbyList.Count > 0)
            {
                var tile = floor.GetRandomTile();
                if (tile != null)
                {
                    var tempMon = standbyList.Dequeue();
                    GameManager.Placement.PlacementConfirm(tempMon, new PlacementInfo(floor, tile));
                    floor.MaxMonsterSize--;
                    tempMon.State = Monster.MonsterState.Placement;
                }
                else
                {
                    break;
                }
            }

            if (standbyList.Count == 0)
            {
                break;
            }
        }


        StartCoroutine(RefreshAll_NoSelected());
    }
    void All_Retrieve()
    {
        var mon = GameManager.Monster.GetMonsterAll(); //? player는 포함 안되는거 확인

        for (int i = 0; i < mon.Count; i++)
        {
            //Debug.Log($"{mon[i].name}");
            if (mon[i].State == Monster.MonsterState.Placement)
            {
                mon[i].MonsterOutFloor();
            }
        }
        StartCoroutine(RefreshAll_NoSelected());
    }
    void All_Fix()
    {
        var mon = GameManager.Monster.GetMonsterAll(); //? player는 포함 안되는거 확인

        for (int i = 0; i < mon.Count; i++)
        {
            mon[i].Mode = Monster.MoveType.Fixed;
        }
        StartCoroutine(RefreshAll_NoSelected());
    }
    void All_Wander()
    {
        var mon = GameManager.Monster.GetMonsterAll(); //? player는 포함 안되는거 확인

        for (int i = 0; i < mon.Count; i++)
        {
            mon[i].Mode = Monster.MoveType.Wander;
        }
        StartCoroutine(RefreshAll_NoSelected());
    }
    void All_Attack()
    {
        var mon = GameManager.Monster.GetMonsterAll(); //? player는 포함 안되는거 확인

        for (int i = 0; i < mon.Count; i++)
        {
            mon[i].Mode = Monster.MoveType.Attack;
        }
        StartCoroutine(RefreshAll_NoSelected());
    }




    void Init_DefaultSetting()
    {
        if (UserData.Instance.FileConfig.Notice_Summon)
        {
            AddNotice_UI("New_Small", this, "Summon", "Notice_Summon");
        }

        GetObject((int)Etc.Summon).AddUIEvent((data) => Show_SummonUI());
        GetObject((int)Etc.UnitCount).GetComponent<TextMeshProUGUI>().text = $"{GameManager.Monster.GetCurrentMonsterSize()}/{GameManager.Monster.GetSlotSize()}";

        var hp = GetImage(((int)Images.Image_HP)).gameObject.GetOrAddComponent<UI_Tooltip>();
        hp.SetTooltipContents("HP", UserData.Instance.LocaleText_Tooltip("Stat_HP"), UI_TooltipBox.ShowPosition.LeftUp);
        var atk = GetImage(((int)Images.Image_ATK)).gameObject.GetOrAddComponent<UI_Tooltip>();
        atk.SetTooltipContents("ATK", UserData.Instance.LocaleText_Tooltip("Stat_ATK"), UI_TooltipBox.ShowPosition.LeftUp);
        var def = GetImage(((int)Images.Image_DEF)).gameObject.GetOrAddComponent<UI_Tooltip>();
        def.SetTooltipContents("DEF", UserData.Instance.LocaleText_Tooltip("Stat_DEF"), UI_TooltipBox.ShowPosition.LeftUp);
        var agi = GetImage(((int)Images.Image_AGI)).gameObject.GetOrAddComponent<UI_Tooltip>();
        agi.SetTooltipContents("AGI", UserData.Instance.LocaleText_Tooltip("Stat_AGI"), UI_TooltipBox.ShowPosition.LeftUp);
        var luk = GetImage(((int)Images.Image_LUK)).gameObject.GetOrAddComponent<UI_Tooltip>();
        luk.SetTooltipContents("LUK", UserData.Instance.LocaleText_Tooltip("Stat_LUK"), UI_TooltipBox.ShowPosition.LeftUp);

    }



    void Init_Panels()
    {
        GetObject(((int)Etc.ClosePanel)).AddUIEvent((data) => ClosePopUp(), Define.UIEvent.LeftClick);
        GetObject(((int)Etc.ClosePanel)).AddUIEvent((data) => ClosePopUp(), Define.UIEvent.RightClick);
        GetObject(((int)Etc.Panel)).AddUIEvent((data) => ClosePopUp(), Define.UIEvent.RightClick);

        GetObject((int)Etc.Return).AddUIEvent(data => CloseAll());
    }


    void PanelClear()
    {
        GetImage(((int)Images.Profile)).sprite = Managers.Sprite.GetClear();
        for (int i = 0; i < Enum.GetNames(typeof(Texts)).Length; i++)
        {
            GetTMP(i).text = "";
        }

        Init_ButtonEvent();

        GetObject((int)Etc.ButtonPanel).SetActive(false);
        GetObject((int)Etc.TraitPanel).SetActive(false);
        GetObject((int)Etc.CommandGroup).SetActive(false);
        GetObject((int)Etc.UnitActionGroup).SetActive(false);
    }

    void PanelShow()
    {
        GetObject((int)Etc.ButtonPanel).SetActive(true);
        GetObject((int)Etc.TraitPanel).SetActive(true);
        GetObject((int)Etc.CommandGroup).SetActive(true);
        GetObject((int)Etc.UnitActionGroup).SetActive(true);
    }



    void Show_SummonUI()
    {
        Hide_This();
        var summon = Managers.UI.ShowPopUpAlone<UI_Summon_Monster>("Monster/UI_Summon_Monster");
        summon.OnPopupCloseEvent += () => Show_This();

        UserData.Instance.FileConfig.SetBoolValue("Notice_Summon", false);
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



    //void PlacePanelUpdate()
    //{
    //    //? 마지막으로 배치된 몬스터(현재 선택되있는 몬스터)의 층 정보로 갱신하기
    //    string floorName = "";

    //    if (Current != null && Current.monster != null && Current.monster.State == Monster.MonsterState.Placement)
    //    {
    //        floorName = Current.monster.PlacementInfo.Place_Floor.LabelName;
    //    }

    //    GetTMP((int)Texts.DetailInfo_Floor).text = $"{floorName}";
    //    GetTMP((int)Texts.DetailInfo_State).text = "";
    //}

    //void PlacePanelUpdate(int floorIndex)
    //{
    //    Main.Instance.CurrentFloor = Main.Instance.Floor[floorIndex];

    //    string floorName = Main.Instance.CurrentFloor.LabelName;
    //    int placeMonsters = Main.Instance.CurrentFloor.monsterList.Count;
    //    int ableMonsters = Main.Instance.CurrentFloor.MaxMonsterSize;
    //    if (Main.Instance.CurrentFloor.FloorIndex == 0)
    //    {
    //        placeMonsters--;
    //    }

    //    GetTMP((int)Texts.DetailInfo_Floor).text = $"{floorName}";

    //    GetTMP((int)Texts.DetailInfo_Unit).text = $"{UserData.Instance.LocaleText("배치된 몬스터")} : {placeMonsters}\n" +
    //        $"{UserData.Instance.LocaleText("배치가능 몬스터")} : {ableMonsters}";

    //    GetTMP((int)Texts.DetailInfo_AP).text = "";
    //    GetTMP((int)Texts.DetailInfo_State).text = "";
    //}



    void CreateMonsterBox()
    {
        childList = new List<UI_MonsterBox>();

        for (int i = 0; i < GameManager.Monster.GetSlotSize(); i++)
        {
            var obj = Managers.Resource.Instantiate("UI/PopUp/Monster/MonsterBox", GetObject(((int)Etc.GridPanel)).transform);

            var box = obj.GetComponent<UI_MonsterBox>();
            box.monster = GameManager.Monster.GetMonster(i);
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

        if (Current != null && Current == selected && selected.monster != null)
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


        PanelShow();
        AddButtonEvent();
        AddCommandPanel();
        AddTextContents();
        AddEditButtonEvent();

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

    //void Reset_Selected()
    //{
    //    Current = null;
    //    for (int i = 0; i < childList.Count; i++)
    //    {
    //        childList[i].ChangePanelColor(Color.white);
    //    }

    //    GetTMP((int)Texts.DetailInfo_State).text = "";
    //    GetTMP((int)Texts.DetailInfo_AP).text = "";
    //    GetTMP((int)Texts.DetailInfo_Floor).text = "";
    //    GetTMP((int)Texts.DetailInfo_Unit).text = "";

    //    if (Cor_SlotChangeWait != null)
    //    {
    //        StopCoroutine(Cor_SlotChangeWait);
    //        Cor_SlotChangeWait = null;
    //    }
    //}


    void AddTextContents()
    {
        if (Current == null || Current.monster == null) return;

        //GetImage((int)Images.ProfilePanel).gameObject.SetActive(true);

        GetTMP((int)Texts.Lv).text = $"Lv.{Current.monster.LV} / {Current.monster.Data.maxLv}";
        GetTMP((int)Texts.Name).text = Current.monster.CallName;

        int currentHP = Mathf.Clamp(Current.monster.B_HP, 0, Current.monster.B_HP);
        GetTMP((int)Texts.Status_HP).text = $"{currentHP}/{Current.monster.HP_Max} " +
            $"{Util.SetTextColorTag($"(+{Current.monster.HP_Final - Current.monster.HP})", Define.TextColor.LightYellow)}";

        GetTMP((int)Texts.Status_ATK).text = $"{Current.monster.ATK} " +
            $"{Util.SetTextColorTag($"(+{Current.monster.ATK_Final - Current.monster.ATK})", Define.TextColor.LightYellow)}";

        GetTMP((int)Texts.Status_DEF).text = $"{Current.monster.DEF} " +
            $"{Util.SetTextColorTag($"(+{Current.monster.DEF_Final - Current.monster.DEF})", Define.TextColor.LightYellow)}";

        GetTMP((int)Texts.Status_AGI).text = $"{Current.monster.AGI} " +
            $"{Util.SetTextColorTag($"(+{Current.monster.AGI_Final - Current.monster.AGI})", Define.TextColor.LightYellow)}";

        GetTMP((int)Texts.Status_LUK).text = $"{Current.monster.LUK} " +
            $"{Util.SetTextColorTag($"(+{Current.monster.LUK_Final - Current.monster.LUK})", Define.TextColor.LightYellow)}";


        var TraitPanel = GetObject((int)Etc.TraitPanel).transform;
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
        
        GetImage(((int)Images.Profile)).sprite = 
            Managers.Sprite.Get_SLA(SpriteManager.Library.Monster, Current.monster.Data.SLA_category, Current.monster.Data.SLA_label);
    }

    #endregion



    #region CommandPanel
    void Init_CommandButton()
    {
        Add_CommandChangeEvent();

        var fix = GetImage(((int)Images.Tooltip_Fix)).gameObject.GetOrAddComponent<UI_Tooltip>();
        fix.SetTooltipContents("", UserData.Instance.LocaleText_Tooltip("Command_Fixed"), UI_TooltipBox.ShowPosition.LeftDown);

        var wander = GetImage(((int)Images.Tooltip_Wander)).gameObject.GetOrAddComponent<UI_Tooltip>();
        wander.SetTooltipContents("", UserData.Instance.LocaleText_Tooltip("Command_Wander"), UI_TooltipBox.ShowPosition.LeftDown);

        var attack = GetImage(((int)Images.Tooltip_Attack)).gameObject.GetOrAddComponent<UI_Tooltip>();
        attack.SetTooltipContents("", UserData.Instance.LocaleText_Tooltip("Command_Attack"), UI_TooltipBox.ShowPosition.LeftDown);
    }

    void Add_CommandChangeEvent()
    {
        GetButton(((int)Buttons.Command_Fixed)).gameObject.AddUIEvent(data => ChangeMoveMode(Monster.MoveType.Fixed));
        GetButton(((int)Buttons.Command_Wander)).gameObject.AddUIEvent(data => ChangeMoveMode(Monster.MoveType.Wander));
        GetButton(((int)Buttons.Command_Attack)).gameObject.AddUIEvent(data => ChangeMoveMode(Monster.MoveType.Attack));
    }

    void ChangeMoveMode(Monster.MoveType _mode)
    {
        if (Current == null || Current.monster == null) return;

        Current.monster.SetMoveType(_mode);
        //Debug.Log("change");
        SelectedImage();

        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].ShowContents();
        }
    }

    void AddCommandPanel()
    {
        if (Current == null || Current.monster == null) return;

        //GetImage(((int)Images.CommandPanel)).gameObject.SetActive(true);
        SelectedImage();
        Add_CommandChangeEvent();
    }
    void SelectedImage()
    {
        switch (Current.monster.Mode)
        {
            case Monster.MoveType.Fixed:
                GetButton((int)Buttons.Command_Fixed).image.sprite = Command_Select;
                GetButton((int)Buttons.Command_Wander).image.sprite = Command_NonSelect;
                GetButton((int)Buttons.Command_Attack).image.sprite = Command_NonSelect;
                break;

            case Monster.MoveType.Wander:
                GetButton((int)Buttons.Command_Fixed).image.sprite = Command_NonSelect;
                GetButton((int)Buttons.Command_Wander).image.sprite = Command_Select;
                GetButton((int)Buttons.Command_Attack).image.sprite = Command_NonSelect;

                break;

            case Monster.MoveType.Attack:
                GetButton((int)Buttons.Command_Fixed).image.sprite = Command_NonSelect;
                GetButton((int)Buttons.Command_Wander).image.sprite = Command_NonSelect;
                GetButton((int)Buttons.Command_Attack).image.sprite = Command_Select;
                break;
        }
    }
    #endregion


    #region EditPanel
    void AddEditButtonEvent()
    {
        //GetImage((int)Images.EditPanel).gameObject.SetActive(true);

        //GetButton((int)Buttons.Change_Name).gameObject.SetActive(true);
        GetButton((int)Buttons.Change_Name).gameObject.RemoveUIEventAll();
        GetButton((int)Buttons.Change_Name).gameObject.AddUIEvent(data => ChangeNameEvent());

        //GetButton((int)Buttons.Change_Slot).gameObject.SetActive(true);
        GetButton((int)Buttons.Change_Slot).image.sprite = Button_Normal;
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
        GetButton((int)Buttons.Change_Slot).image.sprite = Button_Push;
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

        GameManager.Monster.ChangeMonsterSlot(first, second);

        foreach (var item in childList)
        {
            Managers.Resource.Destroy(item.gameObject);
        }

        CreateMonsterBox();
        ChangeOver();
        isChanging = false;
        Cor_SlotChangeWait = null;
    }


    void ChangeOver()
    {
        //ShowDetail(Current);
        Current = null;
        PanelClear();

    }


    #endregion


    #region ButtonPanel

    void Init_ButtonEvent()
    {
        for (int i = 0; i < Enum.GetNames(typeof(Buttons)).Length; i++)
        {
            GetButton(i).gameObject.SetActive(true);
            GetButton(i).gameObject.RemoveUIEventAll();
            //GetButton(i).gameObject.AddUIEvent((data) => StartCoroutine(RefreshAll()));
        }


        GetButton((int)Buttons.Placement).gameObject.AddUIEvent((data) => StartCoroutine(RefreshAll()));
        GetButton((int)Buttons.Training).gameObject.AddUIEvent((data) => StartCoroutine(RefreshAll()));
        GetButton((int)Buttons.Release).gameObject.AddUIEvent((data) => StartCoroutine(RefreshAll()));
        GetButton((int)Buttons.Recover).gameObject.AddUIEvent((data) => StartCoroutine(RefreshAll()));
        GetButton((int)Buttons.Retrieve).gameObject.AddUIEvent((data) => StartCoroutine(RefreshAll()));
        GetButton((int)Buttons.UnitEvent).gameObject.AddUIEvent((data) => StartCoroutine(RefreshAll()));

        Init_All_Button();
    }


    IEnumerator RefreshAll()
    {
        yield return new WaitForEndOfFrame();
        yield return UserData.Instance.Wait_GamePlay;
        yield return new WaitForEndOfFrame();
        Debug.Log("창 새로고침");

        ShowDetail(Current);
        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].ShowContents();
        }
        //AddButtonEvent();
        //Add_CommandChangeEvent();
        //AddEditButtonEvent();
    }

    IEnumerator RefreshAll_NoSelected()
    {
        yield return new WaitForEndOfFrame();
        yield return UserData.Instance.Wait_GamePlay;
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < childList.Count; i++)
        {
            childList[i].ShowContents();
        }
    }


    void AddButtonEvent()
    {
        if (Current == null || Current.monster == null) return;

        Init_ButtonEvent();

        if (!Current.monster.UnitDialogueEvent.ExistCurrentEvent())
        {
            GetButton(((int)Buttons.UnitEvent)).gameObject.SetActive(false);
        }
        else
        {
            GetTMP((int)Texts.Value_eventAP).text = "1";
            GetButton(((int)Buttons.UnitEvent)).gameObject.AddUIEvent(data =>
            {
                if (Main.Instance.Player_AP <= 0)
                {
                    var ui = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
                    ui.Message = UserData.Instance.LocaleText("Message_No_AP");
                    return;
                }
                else
                {
                    Main.Instance.Player_AP--;
                }

                //Debug.Log($"{Current.monster.UnitDialogueEvent.GetDialogue(false)}");
                GameManager.Monster.StartUnitEventAction(Current.monster.UnitDialogueEvent.GetDialogue(true), Current.monster);
            });
        }

        switch (Current.monster.State)
        {
            case Monster.MonsterState.Standby:
                GetButton(((int)Buttons.Retrieve)).gameObject.SetActive(false);
                GetButton(((int)Buttons.Recover)).gameObject.SetActive(false);

                GetButton(((int)Buttons.Placement)).gameObject.
                    AddUIEvent((data) => PlacementEvent(Define.Boundary_1x1, () => CreateAll(Current.monster.MonsterID)));

                GetButton(((int)Buttons.Training)).gameObject.AddUIEvent((data) => Current.monster.Training());
                GetTMP((int)Texts.Value_trainingAP).text = "1";

                GetButton(((int)Buttons.Release)).gameObject.AddUIEvent((data) => GameManager.Monster.ReleaseMonster(Current.monster.MonsterID));

                GetTMP((int)Texts.DetailInfo_State).text = $"{UserData.Instance.LocaleText("대기중")}\n";
                GetTMP((int)Texts.DetailInfo_Floor).text = "";
                break;



            case Monster.MonsterState.Placement:
                GetButton(((int)Buttons.Placement)).gameObject.SetActive(false);
                GetButton(((int)Buttons.Release)).gameObject.SetActive(false);
                GetButton(((int)Buttons.Recover)).gameObject.SetActive(false);

                GetButton(((int)Buttons.Retrieve)).gameObject.
                    AddUIEvent((data) => Current.monster.MonsterOutFloor());

                GetButton(((int)Buttons.Training)).gameObject.AddUIEvent((data) => Current.monster.Training());
                GetTMP((int)Texts.Value_trainingAP).text = "1";

                GetTMP((int)Texts.DetailInfo_State).text = $"{UserData.Instance.LocaleText("배치중")}";
                GetTMP((int)Texts.DetailInfo_Floor).text = $"{Current.monster.PlacementInfo.Place_Floor.LabelName}";
                break;



            case Monster.MonsterState.Injury:
                GetButton(((int)Buttons.Training)).gameObject.SetActive(false);
                GetButton(((int)Buttons.Placement)).gameObject.SetActive(false);
                GetButton(((int)Buttons.Retrieve)).gameObject.SetActive(false);
                GetButton(((int)Buttons.UnitEvent)).gameObject.SetActive(false);

                GetButton(((int)Buttons.Release)).gameObject.
                    AddUIEvent((data) => GameManager.Monster.ReleaseMonster(Current.monster.MonsterID));

                AddEvent_Recover(GetButton(((int)Buttons.Recover)).gameObject);
                break;
        }
    }



    void AddEvent_Recover(GameObject targetButton)
    {
        int RecoverCost = (int)(((Current.monster.LV * 0.08f) + 0.2f) * Current.monster.Data.manaCost);

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

        if (GameManager.Technical.Get_Technical<Hospital>() != null)
        {
            RecoverCost = Mathf.RoundToInt(RecoverCost * 0.5f);
        }
            
        targetButton.AddUIEvent((data) => Current.monster.Recover(RecoverCost));
        GetTMP((int)Texts.Value_recoverMana).text = $"{RecoverCost}";
        GetTMP((int)Texts.DetailInfo_Floor).text = "";
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
                var obj = GameManager.Monster.GetMonster(monsterID);
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

        StartCoroutine(RefreshAll());
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
