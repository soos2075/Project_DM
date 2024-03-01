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
        if (Managers.UI._popupStack.Count > 0)
        {
            canvas.sortingOrder = 5;
        }
        else
        {
            canvas.sortingOrder = 9;
        }
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

        Speed2x,
        Speed3x,
        
        //DayChange_Temp,
    }

    enum Texts
    {
        Default,
        Value,

        Day,
    }

    enum ActionPoint
    {
        AP,
    }



    Canvas canvas;
    UI_EventBox eventBox;

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject, RenderMode.ScreenSpaceCamera, false);
        canvas = GetComponent<Canvas>();
        eventBox = Managers.UI.ShowSceneUI<UI_EventBox>("UI_EventBox");

        GenerateSceneFloorUI();

        Bind<Button>(typeof(ButtonEvent));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<GameObject>(typeof(ActionPoint));

        Texts_Refresh();
        Init_Button();
        StartCoroutine(DayInit());
        AP_Refresh();
    }

    public void AP_Refresh()
    {
        if (GetObject(((int)ActionPoint.AP)) == null) return;

        var pos = GetObject(((int)ActionPoint.AP)).transform;

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
        GetTMP(((int)Texts.Day)).text = $"{Main.Instance.Turn}일차";
        GetTMP(((int)Texts.Value)).text = $"{Main.Instance.Player_Mana}";
        GetTMP(((int)Texts.Value)).text += $"\n{Main.Instance.Player_Gold}";
        GetTMP(((int)Texts.Value)).text += $"\n{Main.Instance.PopularityOfDungeon}";
        GetTMP(((int)Texts.Value)).text += $"\n{Main.Instance.DangerOfDungeon}";
        GetTMP(((int)Texts.Value)).text += $"\n{Main.Instance.DungeonRank}";
    }

    void Init_Button()
    {
        GetButton((int)ButtonEvent._1_Facility).gameObject.AddUIEvent((data) => FacilityButton());
        GetButton((int)ButtonEvent._2_Summon).gameObject.AddUIEvent((data) => Managers.UI.ClearAndShowPopUp<UI_Summon_Monster>("Monster/UI_Summon_Monster"));
        GetButton((int)ButtonEvent._3_Management).gameObject.AddUIEvent((data) => Managers.UI.ClearAndShowPopUp<UI_Monster_Management>("Monster/UI_Monster_Management"));
        GetButton((int)ButtonEvent._4_Guild).gameObject.AddUIEvent((data) => Visit_Guild());
        GetButton((int)ButtonEvent._5_Quest).gameObject.AddUIEvent((data) => Managers.UI.ShowPopUpAlone<UI_Quest>());

        GetButton((int)ButtonEvent.DayChange).gameObject.AddUIEvent((data) => DayStart());

        GetButton((int)ButtonEvent.Save).gameObject.AddUIEvent((data) => { 
            var save = Managers.UI.ShowPopUp<UI_SaveLoad>();
            save.SetMode(UI_SaveLoad.Buttons.Save);
        });

        GetButton((int)ButtonEvent.Pause).gameObject.AddUIEvent((data) => Managers.UI.ShowPopUp<UI_Pause>());

        GetButton((int)ButtonEvent.Speed2x).gameObject.AddUIEvent((data) => Time.timeScale = 1f);
        GetButton((int)ButtonEvent.Speed3x).gameObject.AddUIEvent((data) => Time.timeScale = 2f);


        //GetButton((int)ButtonEvent.DayChange_Temp).gameObject.AddUIEvent((data) => DayChange_Temp());
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


    [System.Obsolete]
    void DayChange_Temp()
    {
        eventBox.BoxActive(true);
        eventBox.TextClear();
        Main.Instance.DayChange();
        Texts_Refresh();
    }



    void DayStart()
    {
        if (Main.Instance.Management)
        {
            eventBox.BoxActive(true);
            eventBox.TextClear();

            Main.Instance.DayChange();
            Texts_Refresh();
            SoundManager.Instance.PlaySound("SFX/TurnChange");
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



    void Visit_Guild()
    {
        if (Main.Instance.Player_AP <= 0)
        {
            var ui = Managers.UI.ShowPopUp<UI_SystemMessage>();
            ui.Message = "최소 1 행동력이 필요합니다.";
            Debug.Log("훈련횟수 없음");
        }
        else
        {
            var ui = Managers.UI.ClearAndShowPopUp<UI_Confirm>();
            ui.SetText($"길드에 방문할까요?\n(남은 행동력이 모두 소모됩니다)");
            StartCoroutine(WaitForAnswer(ui));
        }

    }


    IEnumerator WaitForAnswer(UI_Confirm confirm)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            Managers.Data.SaveToJson("AutoSave", 0);
            Managers.Scene.LoadSceneAsync(SceneName._3_Guild);
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



    public void FacilityButton()
    {
        var facility = Managers.UI.ShowPopUpAlone<UI_Placement_Facility>("Facility/UI_Placement_Facility");
        FloorPanelClear();
    }
}
