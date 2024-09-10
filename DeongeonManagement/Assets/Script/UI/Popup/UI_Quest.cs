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

        //GetImage(((int)Images.Close)).gameObject.AddUIEvent((data) => Managers.UI.ClosePopupPick(this));
        Init_Contents();
    }


    void Init_Contents()
    {
        var pos = GetComponentInChildren<ContentSizeFitter>().transform;

        for (int i = 0; i < EventManager.Instance.CurrentQuestAction_forSave.Count; i++)
        {
            int id = EventManager.Instance.CurrentQuestAction_forSave[i];

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
