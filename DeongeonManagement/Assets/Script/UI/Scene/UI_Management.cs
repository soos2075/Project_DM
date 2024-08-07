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
    void Update()
    {
        //if (Managers.UI._popupStack.Count > 0)
        //{
        //    canvas.sortingOrder = 5;
        //}
        //else
        //{
        //    canvas.sortingOrder = 9;
        //}
    }
    private void LateUpdate()
    {
        Texts_Refresh();
    }


    public enum ButtonEvent
    {
        _1_Facility,
        _2_Summon,
        _3_Management,
        _4_Guild,
        _5_Quest,

        DayChange,

        Save,
        Pause,
        Pedia,


        Speed1x,
        Speed2x,
        Speed3x,
    }

    enum Texts
    {
        Default,
        Value,

        Day,
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

    public enum OverlayImages
    {
        OverlayImage_Facility,
        OverlayImage_Summon,
        OverlayImage_Monster,
        OverlayImage_Guild,
        OverlayImage_Quest,
    }



    Canvas canvas;
    UI_EventBox eventBox;

    public override void Init()
    {
        //Managers.UI.SetCanvas(gameObject, RenderMode.ScreenSpaceCamera, false);
        canvas = GetComponent<Canvas>();
        eventBox = Managers.UI.ShowSceneUI<UI_EventBox>("UI_EventBox");

        GenerateSceneFloorUI();

        Bind<Button>(typeof(ButtonEvent));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(Objects));
        Bind<Image>(typeof(OverlayImages));

        Init_Tooltip();
        Texts_Refresh();
        Init_Button();
        StartCoroutine(DayInit());
        AP_Refresh();
        SpeedButtonImage();

        OverlayImageReset();

        StartCoroutine(WaitAndUpdate_GuildButton());
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

        if (Main.Instance.Player_AP <= 2)
        {
            GetObject(((int)Objects.AP)).GetComponent<ContentSizeFitter>().enabled = false;
            GetObject(((int)Objects.AP)).GetComponent<RectTransform>().sizeDelta = new Vector2(110, 70);
        }
        else
        {
            GetObject(((int)Objects.AP)).GetComponent<ContentSizeFitter>().enabled = true;
        }
    }


    public void Texts_Refresh()
    {
        GetTMP(((int)Texts.Day)).text = $"{Main.Instance.Turn} {UserData.Instance.LocaleText("Day")}";
        GetTMP(((int)Texts.Value)).text = $"{Main.Instance.Player_Mana}";
        GetTMP(((int)Texts.Value)).text += $"\n{Main.Instance.Player_Gold}";
        GetTMP(((int)Texts.Value)).text += $"\n{Main.Instance.PopularityOfDungeon}";
        GetTMP(((int)Texts.Value)).text += $"\n{Main.Instance.DangerOfDungeon}";
        GetTMP(((int)Texts.Value)).text += $"\n{(Define.DungeonRank)Main.Instance.DungeonRank}";
    }



    void OverlayImageReset()
    {
        GetImage((int)OverlayImages.OverlayImage_Facility).enabled = false;
        GetImage((int)OverlayImages.OverlayImage_Summon).enabled = false;
        GetImage((int)OverlayImages.OverlayImage_Monster).enabled = false;
        GetImage((int)OverlayImages.OverlayImage_Guild).enabled = false;
        GetImage((int)OverlayImages.OverlayImage_Quest).enabled = false;
    }

    public void GuildButtonNotice()
    {
        if (EventManager.Instance.CheckGuildNotice())// || EventManager.Instance.CheckGuildNotice_Wating())
        {
            //Debug.Log("길드 알림!!");
            GetImage((int)OverlayImages.OverlayImage_Guild).enabled = true;
        }
        else
        {
            //Debug.Log("길드 알림 없음!!");
            GetImage((int)OverlayImages.OverlayImage_Guild).enabled = false;
        }
    }

    public void SetNotice(OverlayImages notice, bool onoff)
    {
        GetImage((int)notice).enabled = onoff;
    }

    public void QuestNotice()
    {
        if (EventManager.Instance.CurrentQuestAction_forSave.Count > 0)
        {
            SetNotice(OverlayImages.OverlayImage_Quest, true);
        }
    }


    IEnumerator WaitAndUpdate_GuildButton()
    {
        yield return null;
        GuildButtonNotice();
    }



    void Init_Button()
    {
        GetButton((int)ButtonEvent._1_Facility).gameObject.AddUIEvent((data) => Button_Facility());
        GetButton((int)ButtonEvent._2_Summon).gameObject.AddUIEvent((data) => Button_Summon());
        GetButton((int)ButtonEvent._3_Management).gameObject.AddUIEvent((data) => Button_MonsterManage());
        GetButton((int)ButtonEvent._4_Guild).gameObject.AddUIEvent((data) => Visit_Guild());
        GetButton((int)ButtonEvent._5_Quest).gameObject.AddUIEvent((data) => Button_Quest());

        GetButton((int)ButtonEvent.DayChange).gameObject.AddUIEvent((data) => DayStart());

        GetButton((int)ButtonEvent.Save).gameObject.AddUIEvent((data) => Button_Save());
        GetButton((int)ButtonEvent.Pause).gameObject.AddUIEvent((data) => Button_Pause());


        GetButton((int)ButtonEvent.Pedia)?.gameObject.AddUIEvent((data) => Button_Pedia());


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

        //if (Managers.UI.CurrentCanvasList != null)
        //{
        //    Canvas main = GetComponent<Canvas>();

        //    foreach (var item in Managers.UI.CurrentCanvasList)
        //    {
        //        if (item == main)
        //        {
        //            return;
        //        }
        //    }
        //}


        GetComponent<Canvas>().enabled = true;
    }





    #region UI_ButtonEvent
    void Button_Pedia()
    {
        Managers.UI.ShowPopUp<UI_Collection>();
    }
    void Button_Pause()
    {
        Managers.UI.ShowPopUp<UI_Pause>();
    }
    void Button_Save()
    {
        if (!Main.Instance.Management) return;

        var save = Managers.UI.ShowPopUp<UI_SaveLoad>();
    }






    public void Button_Facility()
    {
        if (!Main.Instance.Management) return;

        var facility = Managers.UI.ClearAndShowPopUp<UI_Placement_Facility>("Facility/UI_Placement_Facility");
        SetNotice(OverlayImages.OverlayImage_Facility, false);

        FloorPanelClear();
    }

    void Button_Summon()
    {
        if (!Main.Instance.Management) return;

        Managers.UI.ClearAndShowPopUp<UI_Summon_Monster>("Monster/UI_Summon_Monster");
        SetNotice(OverlayImages.OverlayImage_Summon, false);
    }
    void Button_MonsterManage()
    {
        if (!Main.Instance.Management) return;

        Managers.UI.ClearAndShowPopUp<UI_Monster_Management>("Monster/UI_Monster_Management");
        SetNotice(OverlayImages.OverlayImage_Monster, false);
    }
    void Button_Quest()
    {
        if (!Main.Instance.Management) return;

        Managers.UI.ShowPopUpAlone<UI_Quest>();
        SetNotice(OverlayImages.OverlayImage_Quest, false);
    }



    public int GuildVisit_AP { get; set; } = 1;
    void Visit_Guild()
    {
        if (!Main.Instance.Management) return;

        if (Main.Instance.Player_AP <= 0)
        {
            var ui = Managers.UI.ShowPopUp<UI_SystemMessage>();
            ui.Message = UserData.Instance.LocaleText("Message_No_AP");
            //Debug.Log("훈련횟수 없음");
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


    #endregion


    void GameSpeedUp(int speed = 1)
    {
        UserData.Instance.GameSpeed = speed;
        Time.timeScale = speed;
        SpeedButtonImage();
    }

    public void SpeedButtonImage()
    {
        switch (UserData.Instance.GameSpeed)
        {
            case 1:
                GetButton((int)ButtonEvent.Speed1x).GetComponent<Image>().enabled = true;
                GetButton((int)ButtonEvent.Speed2x).GetComponent<Image>().enabled = false;
                GetButton((int)ButtonEvent.Speed3x).GetComponent<Image>().enabled = false;
                break;

            case 2:
                GetButton((int)ButtonEvent.Speed1x).GetComponent<Image>().enabled = false;
                GetButton((int)ButtonEvent.Speed2x).GetComponent<Image>().enabled = true;
                GetButton((int)ButtonEvent.Speed3x).GetComponent<Image>().enabled = false;
                break;

            case 3:
                GetButton((int)ButtonEvent.Speed1x).GetComponent<Image>().enabled = false;
                GetButton((int)ButtonEvent.Speed2x).GetComponent<Image>().enabled = false;
                GetButton((int)ButtonEvent.Speed3x).GetComponent<Image>().enabled = true;
                break;
        }
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
        GetButton((int)ButtonEvent._2_Summon).gameObject.SetActive(true);
        GetButton((int)ButtonEvent._3_Management).gameObject.SetActive(true);
        GetButton((int)ButtonEvent._4_Guild).gameObject.SetActive(true);
        GetButton((int)ButtonEvent._5_Quest).gameObject.SetActive(true);

        Active_Floor();
    }


    public void EventBoxClose()
    {
        eventBox.BoxActive(false);
    }



    void TurnOverAction()
    {
        if (!Main.Instance.Management) return;

        eventBox.BoxActive(true);
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
            ui.SetText($"{UserData.Instance.LocaleText("턴종료_행동력")}", () => TurnOverAction());
        }
        else
        {
            TurnOverAction();
        }
    }


    void DayZero()
    {
        GetButton((int)ButtonEvent._1_Facility).gameObject.SetActive(false);
        GetButton((int)ButtonEvent._2_Summon).gameObject.SetActive(false);
        GetButton((int)ButtonEvent._3_Management).gameObject.SetActive(false);
        GetButton((int)ButtonEvent._4_Guild).gameObject.SetActive(false);
        GetButton((int)ButtonEvent._5_Quest).gameObject.SetActive(false);

        InActive_Floor();
    }

    IEnumerator DayInit()
    {
        yield return null;
        switch (Main.Instance.Turn)
        {
            case 0:
                DayZero();
                break;

            case 1:
                GetButton((int)ButtonEvent._2_Summon).gameObject.SetActive(false);
                GetButton((int)ButtonEvent._3_Management).gameObject.SetActive(false);
                GetButton((int)ButtonEvent._4_Guild).gameObject.SetActive(false);
                GetButton((int)ButtonEvent._5_Quest).gameObject.SetActive(false);
                break;

            case 2:
            case 3:
            case 4:
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
