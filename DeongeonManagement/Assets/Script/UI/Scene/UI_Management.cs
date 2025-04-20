using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Management : UI_Base
{
    void Start()
    {

    }
    public void Start_Main()
    {
        Init();
    }
    private void LateUpdate()
    {
        Texts_Refresh();

        if (Main.Instance.Management)
        {
            TurnOver_HotKey();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (Managers.UI._popupStack.Count > 0 && Managers.UI._popupStack.Peek().GetType() != typeof(UI_TileView))
                {
                    return;
                }

                if (FindAnyObjectByType<UI_Stop>() == null)
                {
                    Managers.Resource.Instantiate("UI/PopUp/UI_Stop");
                    //Managers.UI.ShowPopUp<UI_Stop>();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (eventBox != null)
            {
                eventBox.BoxActive();
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.timeScale != 0 && UserData.Instance.GameMode == Define.GameMode.Normal)
        {
            GameSpeedChange();
        }
    }

    private float spaceStartTime = -1f; // 스페이스바를 누르기 시작한 시간
    private const float HOLD_DURATION = 1.0f; // 필요한 홀드 시간

    void TurnOver_HotKey()
    {
        // 스페이스바를 누르기 시작할 때
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spaceStartTime = Time.time;
        }

        // 스페이스바를 떼었을 때
        if (Input.GetKeyUp(KeyCode.Space))
        {
            spaceStartTime = -1f; // 타이머 리셋
        }

        // 스페이스바를 계속 누르고 있고, 0.5초가 지났다면
        if (Input.GetKey(KeyCode.Space) && spaceStartTime > 0f)
        {
            if (Time.time - spaceStartTime >= HOLD_DURATION)
            {
                TurnStartAction();
                spaceStartTime = -1f; // 액션 실행 후 타이머 리셋
            }
        }
    }



    public enum ButtonEvent
    {
        _1_Facility,
        //_2_Summon,
        _3_Management,
        _4_Guild,
        _5_Quest,
        _6_DungeonEdit,

        DungeonTitle,

        DayChange,

        Save,
        Pause,
        Pedia,
        DayLog,

        Speed1x,
        Speed2x,
        Speed3x,
    }

    enum Texts
    {
        TMP_Mana,
        TMP_Gold,
        TMP_Pop,
        TMP_Danger,
    }
    enum Images
    {
        MainUI,

        GameSpeed,

        Image_Day_NX,
        Image_Day_XN,
        Image_Rank,

        DifficultyLevel,
    }

    enum Objects
    {
        AP,

        mana_Tooltip,
        gold_Tooltip,
        pop_Tooltip,
        danger_Tooltip,
        rank_Tooltip,
    }



    Canvas canvas;
    UI_EventBox eventBox;

    public override void Init()
    {
        //Managers.UI.SetCanvas(gameObject, RenderMode.ScreenSpaceCamera, false);
        canvas = GetComponent<Canvas>();

        eventBox = FindAnyObjectByType<UI_EventBox>();
        //eventBox = Managers.UI.ShowSceneUI<UI_EventBox>("UI_EventBox");

        GenerateSceneFloorUI();

        Bind<Button>(typeof(ButtonEvent));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(Objects));

        Init_Tooltip();
        Texts_Refresh();
        Init_Button();

        StartCoroutine(DayInit());

        AP_Refresh();
        SpeedButtonImage();

        StartCoroutine(WaitAndNoticeUpdate());
    }




    void Init_Tooltip()
    {
        var gold = GetObject((int)Objects.gold_Tooltip).GetOrAddComponent<UI_Tooltip>();
        gold.SetTooltipContents(UserData.Instance.LocaleText_Tooltip("Gold"), UserData.Instance.LocaleText_Tooltip("Gold_Detail"));

        var mana = GetObject((int)Objects.mana_Tooltip).GetOrAddComponent<UI_Tooltip>();
        mana.SetTooltipContents(UserData.Instance.LocaleText_Tooltip("Mana"), UserData.Instance.LocaleText_Tooltip("Mana_Detail"));

        var ap = GetObject((int)Objects.AP).GetOrAddComponent<UI_Tooltip>();
        ap.SetTooltipContents(UserData.Instance.LocaleText_Tooltip("AP"), UserData.Instance.LocaleText_Tooltip("AP_Detail"));

        var pop = GetObject((int)Objects.pop_Tooltip).GetOrAddComponent<UI_Tooltip>();
        pop.SetTooltipContents(UserData.Instance.LocaleText_Tooltip("Popularity"), UserData.Instance.LocaleText_Tooltip("Pop_Detail"));

        var danger = GetObject((int)Objects.danger_Tooltip).GetOrAddComponent<UI_Tooltip>();
        danger.SetTooltipContents(UserData.Instance.LocaleText_Tooltip("Danger"), UserData.Instance.LocaleText_Tooltip("Danger_Detail"));

        var rank = GetObject((int)Objects.rank_Tooltip).GetOrAddComponent<UI_Tooltip>();
        rank.SetTooltipContents(UserData.Instance.LocaleText_Tooltip("Rank"), UserData.Instance.LocaleText_Tooltip("Rank_Detail"));
    }



    void Init_Difficulty()
    {
        if (UserData.Instance.FileConfig.Difficulty == Define.DifficultyLevel.Easy)
        {
            GetImage((int)Images.DifficultyLevel).gameObject.SetActive(false);
        }
        else
        {
            GetImage((int)Images.DifficultyLevel).GetComponentInChildren<TextMeshProUGUI>().text = $"★ x {(int)UserData.Instance.FileConfig.Difficulty + 1}";
        }
    }



    public void AP_Refresh()
    {
        if (GetObject(((int)Objects.AP)) == null) return;

        var pos = GetObject(((int)Objects.AP)).transform;

        for (int i = pos.childCount - 1; i >= 0; i--)
        {
            Destroy(pos.GetChild(i).gameObject);
        }

        for (int i = 0; i < Main.Instance.Player_AP; i++)
        {
            Managers.Resource.Instantiate("UI/PopUp/Element/behaviour", pos);
        }
    }


    public void Texts_Refresh()
    {
        int tens = Main.Instance.Turn / 10;
        int ones = Main.Instance.Turn % 10;

        GetImage((int)Images.Image_Day_NX).sprite = Managers.Sprite.Get_SLA(SpriteManager.Library.UI, "Number", $"{tens}");
        GetImage((int)Images.Image_Day_XN).sprite = Managers.Sprite.Get_SLA(SpriteManager.Library.UI, "Number", $"{ones}");

        GetImage((int)Images.Image_Rank).sprite = Managers.Sprite.Get_SLA(SpriteManager.Library.UI, "Rank", 
            $"{(Define.DungeonRank)Main.Instance.DungeonRank}");


        GetTMP(((int)Texts.TMP_Mana)).text = $"{Main.Instance.Player_Mana}";
        GetTMP(((int)Texts.TMP_Gold)).text = $"{Main.Instance.Player_Gold}";
        GetTMP(((int)Texts.TMP_Pop)).text = $"{Main.Instance.PopularityOfDungeon}";
        GetTMP(((int)Texts.TMP_Danger)).text = $"{Main.Instance.DangerOfDungeon}";
    }



    public void OverlayImageReset()
    {
        if (UserData.Instance.FileConfig.Notice_Facility)
        {
            AddNotice_UI("New", this, ButtonEvent._1_Facility.ToString(), "Notice_Facility");
        }

        if (UserData.Instance.FileConfig.Notice_Monster)
        {
            AddNotice_UI("New", this, ButtonEvent._3_Management.ToString(), "Notice_Monster");
        }

        if (UserData.Instance.FileConfig.Notice_Guild)
        {   //? 노클리어버전으로
            AddNotice_NoClear("Notice", this, ButtonEvent._4_Guild.ToString(), "Notice_Guild");
        }
        else //? 만약 이번턴에 업데이트했는데 없으면 이전에 남아있던 overlay라도 삭제해야함
        {
            RemoveNotice_UI(GetButton((int)ButtonEvent._4_Guild).gameObject);
        }


        if (UserData.Instance.FileConfig.Notice_Quest)
        {
            AddNotice_UI("Notice", this, ButtonEvent._5_Quest.ToString(), "Notice_Quest");
        }
        else //? 만약 이번턴에 업데이트했는데 없으면 이전에 남아있던 overlay라도 삭제해야함
        {
            RemoveNotice_UI(GetButton((int)ButtonEvent._5_Quest).gameObject);
        }


        if (UserData.Instance.FileConfig.Notice_DungeonEdit)
        {
            AddNotice_UI("Notice", this, ButtonEvent._6_DungeonEdit.ToString(), "Notice_DungeonEdit");
        }
    }



    public void GuildButtonNotice()
    {
        if (EventManager.Instance.CheckGuildNotice())
        {
            if (UserData.Instance.FileConfig.Notice_Guild == false)
            {
                UserData.Instance.FileConfig.Notice_Guild = true;
            }
        }
        else
        {
            UserData.Instance.FileConfig.Notice_Guild = false;
        }
    }


    IEnumerator WaitAndNoticeUpdate()
    {
        yield return null;

        //Init_Difficulty();
        GuildButtonNotice();
        OverlayImageReset();
    }






    void Init_Button()
    {
        GetButton((int)ButtonEvent._1_Facility).gameObject.AddUIEvent((data) => Button_Facility());
        //GetButton((int)ButtonEvent._2_Summon).gameObject.AddUIEvent((data) => Button_Summon());
        GetButton((int)ButtonEvent._3_Management).gameObject.AddUIEvent((data) => Button_MonsterManage());
        GetButton((int)ButtonEvent._4_Guild).gameObject.AddUIEvent((data) => Visit_Guild());
        GetButton((int)ButtonEvent._5_Quest).gameObject.AddUIEvent((data) => Button_Quest());
        GetButton((int)ButtonEvent._6_DungeonEdit).gameObject.AddUIEvent((data) => Button_DungeonEdit());


        GetButton((int)ButtonEvent.DungeonTitle).gameObject.AddUIEvent((data) => Button_DungeonTitle());


        GetButton((int)ButtonEvent.DayChange).gameObject.AddUIEvent((data) => DayStart());

        GetButton((int)ButtonEvent.Save).gameObject.AddUIEvent((data) => Button_Save());
        GetButton((int)ButtonEvent.Pause).gameObject.AddUIEvent((data) => Button_Pause());

        GetButton((int)ButtonEvent.Pedia)?.gameObject.AddUIEvent((data) => Button_Pedia());
        GetButton((int)ButtonEvent.DayLog)?.gameObject.AddUIEvent((data) => Button_DayLog());


        GetButton((int)ButtonEvent.Speed1x).gameObject.AddUIEvent((data) => GameSpeedUp(1));
        GetButton((int)ButtonEvent.Speed2x).gameObject.AddUIEvent((data) => GameSpeedUp(2));
        GetButton((int)ButtonEvent.Speed3x).gameObject.AddUIEvent((data) => GameSpeedUp(3));
    }



    public void Hide_MainCanvas()
    {
        if (GetComponent<Canvas>().enabled == false)
        {
            return;
        }

        GetComponent<Canvas>().enabled = false;
    }
    public void Show_MainCanvas()
    {
        if (UserData.Instance.GameMode == Define.GameMode.Stop)
        {
            return;
        }

        GetComponent<Canvas>().enabled = true;
    }





    #region UI_ButtonEvent
    public void Button_DayLog()
    {
        var ui = Managers.UI.ShowPopUp<UI_DayLog>();
        //ui.TextContents(Main.Instance.DayList[Main.Instance.Turn - 1]);
    }
    public void Button_Pedia()
    {
        Managers.UI.ShowPopUp<UI_Collection>("Collection/UI_Collection");
    }
    void Button_Pause()
    {
        Managers.UI.ShowPopUp<UI_Pause>();
    }
    public void Button_Save()
    {
        //? 턴진행중은 아예 클릭이 안되게 하거나 클릭했을 때 로드모드로만 열리게 하는 두가지 방법이 있는데 음..
        //if (!Main.Instance.Management) return;

        var save = Managers.UI.ShowPopUp<UI_SaveLoad>();
        if (!Main.Instance.Management)
        {
            save.SetMode(UI_SaveLoad.DataState.Load);
        }
    }


    public void Button_Facility()
    {
        if (!Main.Instance.Management) return;

        if (UserData.Instance.FileConfig.PlayRounds == 1 && Main.Instance.Turn < 1)
        {
            return;
        }

        var facility = Managers.UI.ClearAndShowPopUp<UI_Placement_Facility>("Facility/UI_Placement_Facility");

        FloorPanelClear();
    }

    void Button_Summon()
    {
        if (!Main.Instance.Management) return;

        Managers.UI.ClearAndShowPopUp<UI_Summon_Monster>("Monster/UI_Summon_Monster");
    }
    public void Button_MonsterManage()
    {
        if (!Main.Instance.Management) return;

        if (UserData.Instance.FileConfig.PlayRounds == 1 && Main.Instance.Turn < 2)
        {
            return;
        }

        var monster = Managers.UI.ClearAndShowPopUp<UI_Monster_Management>("Monster/UI_Monster_Management");
        //if (UserData.Instance.FileConfig.Notice_Summon)
        //{
        //    AddNotice_UI("New_Small", monster, "Summon", "Notice_Summon");
        //}
    }



    UI_Quest questUI;
    public void Button_Quest()
    {
        if (!Main.Instance.Management) return;
        if (UserData.Instance.FileConfig.PlayRounds == 1 && Main.Instance.Turn < 6)
        {
            return;
        }


        if (questUI == null)
        {
            questUI = Managers.UI.ClearAndShowPopUp<UI_Quest>();
        }
        else
        {
            Managers.UI.ClosePopupPick(questUI);
        }
    }

    UI_DungeonEdit dungeonEdit;
    public void Button_DungeonEdit()
    {
        if (!Main.Instance.Management) return;
        if (UserData.Instance.FileConfig.PlayRounds == 1 && Main.Instance.Turn < 1)
        {
            return;
        }

        if (dungeonEdit == null)
        {
            dungeonEdit = Managers.UI.ClearAndShowPopUp<UI_DungeonEdit>();
            //if (UserData.Instance.FileConfig.Notice_Ex4)
            //{
            //    AddNotice_UI("New_Small", dungeonEdit, "Floor_4", "Notice_Ex4");
            //}
            //if (UserData.Instance.FileConfig.Notice_Ex5)
            //{
            //    AddNotice_UI("New_Small", dungeonEdit, "Floor_5", "Notice_Ex5");
            //}
        }
        else
        {
            Managers.UI.ClosePopupPick(dungeonEdit);
            dungeonEdit = null;
        }
    }

    public void DungeonEdit_Refresh()
    {
        if (dungeonEdit != null)
        {
            Managers.UI.ClosePopupPick(dungeonEdit);
            dungeonEdit = null;
            Button_DungeonEdit();
        }
    }

    UI_DungeonTitle dungeontitle;
    public void Button_DungeonTitle()
    {
        if (dungeontitle == null)
        {
            dungeontitle = Managers.UI.ClearAndShowPopUp<UI_DungeonTitle>("Title/UI_DungeonTitle");
        }
        else
        {
            Managers.UI.ClosePopupPick(dungeontitle);
            dungeontitle = null;
        }
    }



    public int GuildVisit_AP { get; set; } = 1;
    public void Visit_Guild()
    {
        if (!Main.Instance.Management) return;
        if (UserData.Instance.FileConfig.PlayRounds == 1 && Main.Instance.Turn < 6)
        {
            return;
        }

        if (Main.Instance.Player_AP <= 0)
        {
            var ui = Managers.UI.ShowPopUp<UI_SystemMessage>();
            ui.Message = UserData.Instance.LocaleText("Message_No_AP");
        }
        else
        {
            var ui = Managers.UI.ClearAndShowPopUp<UI_Confirm>();
            ui.SetText($"{UserData.Instance.LocaleText("Confirm_Guild")}\n" +
                $"<size=25>({GuildVisit_AP} {UserData.Instance.LocaleText("AP")} {UserData.Instance.LocaleText("필요")})",
                () => Visit_Action());
        }
    }

    void Visit_Action()
    {
        Main.Instance.Player_AP -= GuildVisit_AP;
        UserData.Instance.GameSpeed_GuildReturn = UserData.Instance.GameSpeed;

        Managers.Data.SaveAndAddFile("Temp_GuildSave", -2);
        //Managers.Data.SaveAndAddFile("AutoSave", 0);

        Managers.Scene.LoadSceneAsync(SceneName._3_Guild);
    }


    //public void AddNotice_UI(string label, UI_Base parent, string findName, string setBoolName)
    //{
    //    UserData.Instance.FileConfig.SetBoolValue(setBoolName, true);
    //    StartCoroutine(WaitFrame(label, parent, findName, setBoolName));
    //}
    //IEnumerator WaitFrame(string label, UI_Base parent, string findName, string setBoolName)
    //{
    //    yield return null;

    //    var obj = Util.FindChild(parent.gameObject, findName, true);

    //    var overlay = Managers.Resource.Instantiate("UI/PopUp/Element/OverlayImage", obj.transform);
    //    var ui = overlay.GetComponent<UI_Overlay>();
    //    ui.SetOverlay(Managers.Sprite.Get_SLA(SpriteManager.Library.UI, "Overlay_Icon", label), obj, setBoolName);
    //}

    //void AddNotice_NoClear(string label, UI_Base parent, string findName, string setBoolName)
    //{
    //    UserData.Instance.FileConfig.SetBoolValue(setBoolName, true);
    //    StartCoroutine(WaitFrame_NoClear(label, parent, findName));
    //}
    //IEnumerator WaitFrame_NoClear(string label, UI_Base parent, string findName)
    //{
    //    yield return null;

    //    var obj = Util.FindChild(parent.gameObject, findName, true);

    //    var overlay = Managers.Resource.Instantiate("UI/PopUp/Element/OverlayImage", obj.transform);
    //    var ui = overlay.GetComponent<UI_Overlay>();
    //    ui.SetOverlay_DontDest(Managers.Sprite.Get_SLA(SpriteManager.Library.UI, "Overlay_Icon", label), obj);
    //}

    //void RemoveNotice_UI(GameObject btn)
    //{
    //    var overlay = btn.GetComponentInChildren<UI_Overlay>();
    //    if (overlay != null)
    //    {
    //        overlay.SelfDestroy();
    //    }
    //}




    #endregion

    void GameSpeedChange()
    {
        switch (UserData.Instance.GameSpeed)
        {
            case 1:
                GameSpeedUp(2);
                break;

            case 2:
                GameSpeedUp(3);
                break;

            case 3:
                GameSpeedUp(1);
                break;
        }
    }


    void GameSpeedUp(int speed = 1)
    {
        UserData.Instance.GameSpeed = speed;
        Time.timeScale = speed;
        SpeedButtonImage();
    }

    public void SpeedButtonImage()
    {
        GetImage((int)Images.GameSpeed).sprite = Managers.Sprite.Get_SLA(SpriteManager.Library.UI, "GameSpeed", $"{UserData.Instance.GameSpeed}");
    }




    [System.Obsolete]
    public void ButtonAllActive()
    {
        StartCoroutine(ObsAct());
    }
    IEnumerator ObsAct()
    {
        yield return new WaitForSecondsRealtime(1);
        GetButton((int)ButtonEvent._1_Facility).gameObject.SetActive(true);
        //GetButton((int)ButtonEvent._2_Summon).gameObject.SetActive(true);
        GetButton((int)ButtonEvent._3_Management).gameObject.SetActive(true);
        GetButton((int)ButtonEvent._4_Guild).gameObject.SetActive(true);
        GetButton((int)ButtonEvent._5_Quest).gameObject.SetActive(true);

        Active_Floor();
    }


    void EventBoxClose()
    {
        eventBox.BoxActive(false);
    }
    void EventBoxOpen()
    {
        eventBox.BoxActive(true);
        //eventBox.TextClear();
    }


    public void TurnOverEvent()
    {
        Texts_Refresh();
        GuildButtonNotice();
        //EventBoxClose();
        OverlayImageReset();
    }


    void TurnStartAction()
    {
        if (!Main.Instance.Management) return;

        Camera.main.GetComponent<CameraControl>().AutoChasing = true;
        //EventBoxOpen();
        eventBox.TextClear();
        Main.Instance.DayChange_Start();
        Texts_Refresh();
        SoundManager.Instance.PlaySound("SFX/TurnChange");
    }


    void DayStart()
    {
        if (!Main.Instance.Management) return;

        if (Main.Instance.Player_AP > 0)
        {
            var ui = Managers.UI.ShowPopUp<UI_Confirm>();
            //ui.SetText($"행동력을 쓰지않고 턴을 종료할까요?");
            ui.SetText($"{UserData.Instance.LocaleText("턴종료_행동력")}", () => TurnStartAction());
        }
        else
        {
            TurnStartAction();
        }
    }


    void DayZero()
    {
        GetButton((int)ButtonEvent._1_Facility).gameObject.SetActive(false);
        //GetButton((int)ButtonEvent._2_Summon).gameObject.SetActive(false);
        GetButton((int)ButtonEvent._3_Management).gameObject.SetActive(false);
        GetButton((int)ButtonEvent._4_Guild).gameObject.SetActive(false);
        GetButton((int)ButtonEvent._5_Quest).gameObject.SetActive(false);
        GetButton((int)ButtonEvent._6_DungeonEdit).gameObject.SetActive(false);

        InActive_Floor();
    }

    IEnumerator DayInit()
    {
        yield return null;

        if (UserData.Instance.FileConfig.PlayRounds > 1)
        {
            yield break;
        }

        switch (Main.Instance.Turn)
        {
            case 0:
                DayZero();
                break;

            case 1:
                //GetButton((int)ButtonEvent._2_Summon).gameObject.SetActive(false);
                GetButton((int)ButtonEvent._3_Management).gameObject.SetActive(false);
                GetButton((int)ButtonEvent._4_Guild).gameObject.SetActive(false);
                GetButton((int)ButtonEvent._5_Quest).gameObject.SetActive(false);
                //GetButton((int)ButtonEvent._6_DungeonEdit).gameObject.SetActive(false);
                break;

            case 2:
            case 3:
            case 4:
            case 5:
                GetButton((int)ButtonEvent._4_Guild).gameObject.SetActive(false);
                GetButton((int)ButtonEvent._5_Quest).gameObject.SetActive(false);
                break;


            default:
                break;
        }
    }
    public void Active_Button(ButtonEvent button) //? 메인에서 하나씩 풀면 됨
    {
        GetButton((int)button).gameObject.SetActive(true);

        switch (button)
        {
            case ButtonEvent._1_Facility:
                UserData.Instance.FileConfig.Notice_Facility = true;
                break;
            case ButtonEvent._3_Management:
                UserData.Instance.FileConfig.Notice_Monster = true;
                UserData.Instance.FileConfig.Notice_Summon = true;
                break;
            case ButtonEvent._4_Guild:
                UserData.Instance.FileConfig.Notice_Guild = true;
                break;
            case ButtonEvent._5_Quest:
                UserData.Instance.FileConfig.Notice_Quest = true;
                break;
            case ButtonEvent._6_DungeonEdit:
                UserData.Instance.FileConfig.Notice_DungeonEdit = true;
                break;
        }
        OverlayImageReset();
    }
    public void Active_Floor()
    {
        foreach (var item in sceneFloorList)
        {
            item.gameObject.SetActive(true);
        }
    }
    public void InActive_Floor()
    {
        foreach (var item in sceneFloorList)
        {
            item.gameObject.SetActive(false);
        }
    }



    






    List<UI_TileView_Floor> sceneFloorList;
    public void GenerateSceneFloorUI()
    {
        sceneFloorList = new List<UI_TileView_Floor>();

        for (int i = 0; i < Main.Instance.ActiveFloor_Basement; i++)
        {
            var content = Managers.UI.ShowSceneUI<UI_TileView_Floor>("TileView_Floor");
            content.FloorID = i;
            sceneFloorList.Add(content);
        }
    }

    public void DungeonExpansion()
    {
        for (int i = 0; i < sceneFloorList.Count; i++)
        {
            Managers.Resource.Destroy(sceneFloorList[i].gameObject);
        }
        Managers.UI.SceneUI_Clear();
        GenerateSceneFloorUI();
    }


    public void FloorPanelActive()
    {
        for (int i = 0; i < sceneFloorList.Count; i++)
        {
            sceneFloorList[i].ChildColorChange(Define.Color_White);
        }
    }
    public void FloorPanelClear()
    {
        for (int i = 0; i < sceneFloorList.Count; i++)
        {
            sceneFloorList[i].Refresh();
        }
    }





}
