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
        ProfilePanel,
        TraitPanel,

        FloorPanel,

        CommandPanel,
        fix,
        wan,
        atk,
    }

    enum Texts
    {
        Lv,
        Name,
        Status,
        //State,
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
        Command_Fixed,
        Command_Wander,
        Command_Attack,
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
    public UI_Type Type { get; set; }

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
            PlacePanelUpdate();
            //GetTMP((int)Texts.DetailInfo).text = $"{Main.Instance.CurrentFloor.LabelName}\n" +
            //    $"{UserData.Instance.LocaleText("배치된 몬스터")} : {Main.Instance.CurrentFloor.monsterList.Count}\n" +
            //    $"{UserData.Instance.LocaleText("배치가능 몬스터")} : {Main.Instance.CurrentFloor.MaxMonsterSize}";
        }
        else
        {
            GetImage(((int)Panels.FloorPanel)).gameObject.SetActive(false);
        }
    }

    void PlacePanelUpdate()
    {
        //? 마지막으로 배치된 몬스터(현재 선택되있는 몬스터)의 층 정보로 갱신하기
        string floorName = "";
        int placeMonsters = 0;
        int ableMonsters = 0;

        if (Current != null && Current.monster != null)
        {
            floorName = Current.monster.PlacementInfo.Place_Floor.LabelName;
            placeMonsters = Current.monster.PlacementInfo.Place_Floor.monsterList.Count;
            ableMonsters = Current.monster.PlacementInfo.Place_Floor.MaxMonsterSize;

            if (Current.monster.PlacementInfo.Place_Floor.FloorIndex == 3)
            {
                placeMonsters--;
            }
        }
        else
        {
            floorName = Main.Instance.CurrentFloor.LabelName;
            placeMonsters = Main.Instance.CurrentFloor.monsterList.Count;
            ableMonsters = Main.Instance.CurrentFloor.MaxMonsterSize;

            if (Main.Instance.CurrentFloor.FloorIndex == 3)
            {
                placeMonsters--;
            }
        }

        GetTMP((int)Texts.DetailInfo).text = $"{floorName}\n" +
            $"{UserData.Instance.LocaleText("배치된 몬스터")} : {placeMonsters}\n" +
            $"{UserData.Instance.LocaleText("배치가능 몬스터")} : {ableMonsters}";
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
        GetTMP((int)Texts.DetailInfo).text = $"{UserData.Instance.LocaleText("AP")} : {Main.Instance.Player_AP}";
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

        GetImage((int)Panels.ProfilePanel).gameObject.SetActive(true);

        GetTMP((int)Texts.Lv).text = $"Lv.{Current.monster.LV} / {Current.monster.Data.maxLv}";
        GetTMP((int)Texts.Name).text = Current.monster.Name_Color;

        GetTMP((int)Texts.Status).text = $"HP : {Current.monster.HP} / {Current.monster.HP_Max} " +
            $"{Util.SetTextColorTag($"(+{Current.monster.B_HP - Current.monster.HP})", Define.TextColor.npc_red)}\n";
        GetTMP((int)Texts.Status).text +=
            $"ATK : {Current.monster.ATK} {Util.SetTextColorTag($"(+{Current.monster.B_ATK - Current.monster.ATK})", Define.TextColor.npc_red)}" +
            $"\tDEF : {Current.monster.DEF} {Util.SetTextColorTag($"(+{Current.monster.B_DEF - Current.monster.DEF})", Define.TextColor.npc_red)}" +
            $"\nAGI : {Current.monster.AGI} {Util.SetTextColorTag($"(+{Current.monster.B_AGI - Current.monster.AGI})", Define.TextColor.npc_red)}" +
            $"\tLUK : {Current.monster.LUK} {Util.SetTextColorTag($"(+{Current.monster.B_LUK - Current.monster.LUK})", Define.TextColor.npc_red)}";


        //if (GameManager.Buff.CurrentBuff.Orb_red > 0)
        //{
        //    GetTMP(((int)Texts.Status)).text = $"HP : {Current.monster.HP} / {Current.monster.HP_Max}\n";
        //    GetTMP(((int)Texts.Status)).text += 
        //        $"ATK : {Current.monster.ATK} {"(+5)".SetTextColorTag(Define.TextColor.npc_red)} " +
        //        $"\tDEF : {Current.monster.DEF} {"(+5)".SetTextColorTag(Define.TextColor.npc_red)} " +
        //        $"\nAGI : {Current.monster.AGI} {"(+5)".SetTextColorTag(Define.TextColor.npc_red)} " +
        //        $"\tLUK : {Current.monster.LUK} {"(+5)".SetTextColorTag(Define.TextColor.npc_red)}";
        //}


        // GetTMP(((int)Texts.State)).text = $"{Current.monster.Data.evolutionHint}";
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
                GetImage(((int)Panels.fix)).color = Color.white;
                GetImage(((int)Panels.wan)).color = Color.clear;
                GetImage(((int)Panels.atk)).color = Color.clear;
                break;

            case Monster.MoveType.Wander:
                GetImage(((int)Panels.fix)).color = Color.clear;
                GetImage(((int)Panels.wan)).color = Color.white;
                GetImage(((int)Panels.atk)).color = Color.clear;
                break;

            case Monster.MoveType.Attack:
                GetImage(((int)Panels.fix)).color = Color.clear;
                GetImage(((int)Panels.wan)).color = Color.clear;
                GetImage(((int)Panels.atk)).color = Color.white;
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
        yield return new WaitForEndOfFrame();
        Debug.Log("창 새로고침");
        GetTMP((int)Texts.DetailInfo).text = $"{UserData.Instance.LocaleText("AP")} : {Main.Instance.Player_AP}";

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
                GetButton(((int)Buttons.Training)).GetComponentInChildren<TextMeshProUGUI>().text = 
                    $"{UserData.Instance.LocaleText("훈련")}(1)";

                GetButton(((int)Buttons.Release)).gameObject.
                    AddUIEvent((data) => GameManager.Monster.ReleaseMonster(Current.monster.MonsterID));

                GetTMP((int)Texts.DetailInfo).text = $"{UserData.Instance.LocaleText("대기중")}\n" +
                    $"{UserData.Instance.LocaleText("AP")} : {Main.Instance.Player_AP}";
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

                GetTMP((int)Texts.DetailInfo).text = $"{UserData.Instance.LocaleText("배치중")} : {Current.monster.PlacementInfo.Place_Floor.LabelName}\n" +
                    $"{UserData.Instance.LocaleText("AP")} : {Main.Instance.Player_AP}";
                break;



            case Monster.MonsterState.Injury:
                GetButton(((int)Buttons.Training)).gameObject.SetActive(false);
                GetButton(((int)Buttons.Placement)).gameObject.SetActive(false);
                GetButton(((int)Buttons.Retrieve)).gameObject.SetActive(false);

                GetButton(((int)Buttons.Release)).gameObject.
                    AddUIEvent((data) => GameManager.Monster.ReleaseMonster(Current.monster.MonsterID));

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

                GetButton(((int)Buttons.Recover)).gameObject.
                    AddUIEvent((data) => Current.monster.Recover(RecoverCost));
                GetButton(((int)Buttons.Recover)).GetComponentInChildren<TextMeshProUGUI>().text = $"{UserData.Instance.LocaleText("회복")}({RecoverCost})";


                GetTMP((int)Texts.DetailInfo).text = $"{UserData.Instance.LocaleText("회복")} {UserData.Instance.LocaleText("Mana")} : {RecoverCost}";
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

        if (Type == UI_Type.Placement)
        {
            PlacePanelUpdate();
        }
    }
    void ResetAction()
    {
        Main.Instance.ResetCurrentAction();

        if (Type == UI_Type.Management)
        {
            StartCoroutine(RefreshAll());
        }
        //else if (Type == UI_Type.Placement)
        //{
        //    for (int i = 0; i < childList.Count; i++)
        //    {
        //        childList[i].ShowContents();
        //    }
        //}

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
