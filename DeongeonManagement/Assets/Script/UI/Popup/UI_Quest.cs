using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Quest : UI_PopUp
{
    void Start()
    {
        Init();
    }

    enum Images
    {
        Panel,
        //Close,
    }

    public override void Init()
    {
        base.Init();
        Bind<Image>(typeof(Images));
        pos = GetComponentInChildren<ContentSizeFitter>().transform;



        //GetImage(((int)Images.Close)).gameObject.AddUIEvent((data) => Managers.UI.ClosePopupPick(this));

        //? 랜덤이벤트 예측
        Soothsayer_Orb();
        Init_CurrentJournal();
        //Init_Contents();
    }


    Transform pos;



    void Init_CurrentJournal()
    {
        foreach (var item in JournalManager.Instance.CurrentJournalList)
        {
            var content = Managers.Resource.Instantiate("UI/PopUp/Element/QuestBox", pos).GetComponent<UI_QuestBox>();

            var textData = JournalManager.Instance.GetData(item.ID);

            string dayInfo = "";
            if (item.startDay != 0 && item.endDay != 0)
            {
                if (item.startDay == item.endDay)
                {
                    //dayInfo = $"{item.startDay}{UserData.Instance.LocaleText("Day")}";
                    dayInfo = $"<b>D-{item.startDay - Main.Instance.Turn}</b>";
                }
                else
                {
                    //dayInfo = $"~{item.endDay}{UserData.Instance.LocaleText("Day")}";
                    //dayInfo = $"D-{item.endDay}";
                    for (int i = 0; i < Main.Instance.Turn - (item.startDay - 1); i++)
                    {
                        dayInfo += "●";
                    }
                    for (int i = 0; i < item.endDay - Main.Instance.Turn; i++)
                    {
                        dayInfo += "○";
                    }
                }
            }

            //? 고정날짜 이벤트
            if (textData.fixedEvent)
            {
                dayInfo = $"<b>D-{textData.dayInfo - Main.Instance.Turn}</b>";
            }

            content.SetText(textData.title, textData.description, dayInfo);

            item.noticeCheck = true;
        }
    }



    void Init_Contents()
    {
        //var pos = GetComponentInChildren<ContentSizeFitter>().transform;

        foreach (var item in EventManager.Instance.CurrentQuestAction_forSave)
        {
            int id = item;

            string title = Managers.Dialogue.GetDialogue((DialogueName)id).dialogueName;
            string detail = Managers.Dialogue.GetDialogue((DialogueName)id).TextDataList[0].mainText;
            string day = Managers.Dialogue.GetDialogue((DialogueName)id).TextDataList[0].optionString;


            if (day.Contains("@NoView"))
            {
                continue;
            }

            var content = Managers.Resource.Instantiate("UI/PopUp/Element/QuestBox", pos).GetComponent<UI_QuestBox>();

            int dayOption = 0;
            if (day.Contains("@Day"))
            {
                string numbersOnly = System.Text.RegularExpressions.Regex.Replace(day, "[^0-9]", "");
                if (string.IsNullOrEmpty(numbersOnly) == false)
                {
                    dayOption = int.Parse(numbersOnly);
                }
            }

            content.SetText(title, detail, dayOption);
        }


        //for (int i = 0; i < EventManager.Instance.CurrentQuestAction_forSave.Count; i++)
        //{
        //    int id = EventManager.Instance.CurrentQuestAction_forSave[i];

        //    string title = Managers.Dialogue.GetDialogue((DialogueName)id).dialogueName;
        //    string detail = Managers.Dialogue.GetDialogue((DialogueName)id).TextDataList[0].mainText;
        //    string day = Managers.Dialogue.GetDialogue((DialogueName)id).TextDataList[0].optionString;

        //    if (day.Contains("@NoView"))
        //    {
        //        continue;
        //    }


        //    var content = Managers.Resource.Instantiate("UI/PopUp/Element/QuestBox", pos).GetComponent<UI_QuestBox>();

        //    int dayOption = 0;
        //    if (day.Contains("@Day"))
        //    {
        //        string numbersOnly = System.Text.RegularExpressions.Regex.Replace(day, "[^0-9]", "");
        //        if (string.IsNullOrEmpty(numbersOnly) == false)
        //        {
        //            dayOption = int.Parse(numbersOnly);
        //        }
        //    }

        //    content.SetText(title, detail, dayOption);
        //}
    }


    void Soothsayer_Orb()
    {
        if (GameManager.Artifact.Check_Artifact_Exist(ArtifactLabel.SoothsayerOrb) == false && UserData.Instance.FileConfig.Next_RE_Info == false)
        {
            return;
        }

        int turn = Main.Instance.Turn;
        var data = RandomEventManager.Instance.Get_NextRandomEventID(turn, RandomEventManager.Instance.CurrentEventList);
        var content = Managers.Resource.Instantiate("UI/PopUp/Element/QuestBox", pos).GetComponent<UI_QuestBox>();
        string msg = "";
        string day = "";

        if (data._id < 0)
        {
            msg = UserData.Instance.LocaleText("아무일없음");
        }
        else
        {
            //day = $"{data._startDay - turn}{UserData.Instance.LocaleText("~일 후")}";
            day = $"<b>D-{data._startDay - turn}</b>";

            msg = $"{RandomEventManager.Instance.GetData(data._id).description} ";

        }

        content.SetText(GameManager.Artifact.GetData("SoothsayerOrb").labelName, msg, day);
    }





    public override bool EscapeKeyAction()
    {
        return true;
    }

    //private void OnDestroy()
    //{
    //    PopupUI_OnDestroy();
    //}
}
