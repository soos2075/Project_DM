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
        Close,
    }

    public override void Init()
    {
        base.Init();

        Bind<Image>(typeof(Images));

        GetImage(((int)Images.Close)).gameObject.AddUIEvent((data) => Managers.UI.ClosePopupPick(this));


        Init_Contents();
    }






    void Init_Contents()
    {
        var pos = GetComponentInChildren<ContentSizeFitter>().transform;

        for (int i = 0; i < EventManager.Instance.CurrentQuestEvent_ForSave.Count; i++)
        {
            var content = Managers.Resource.Instantiate("UI/PopUp/Element/QuestBox", pos).GetComponent<UI_QuestBox>();

            int id = EventManager.Instance.CurrentQuestEvent_ForSave[i];

            string title = Managers.Dialogue.GetDialogue($"Guild_{id}").dialogueName;
            string detail = Managers.Dialogue.GetDialogue($"Guild_{id}").TextDataList[0].mainText;

            content.SetText(title, detail);
        }
    }
}
