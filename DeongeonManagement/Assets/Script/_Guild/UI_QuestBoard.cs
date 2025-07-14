using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestBoard : UI_PopUp, IDialogue
{
    private void Start()
    {
        Init();
    }



    enum TMP
    {
        Title,
        Content,
    }
    enum Images
    {
        Panel,
        NoTouch,
        CloseButton,
    }

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(TMP));
        Bind<Image>(typeof(Images));

        Init_Event();
        Init_Text();
    }

    void Init_Event()
    {
        //GetImage(((int)Images.NoTouch)).gameObject.AddUIEvent((data) => CloseAndAddAction(), Define.UIEvent.LeftClick);
        GetImage(((int)Images.NoTouch)).gameObject.AddUIEvent((data) => CloseAndAddAction(), Define.UIEvent.RightClick);
        //GetImage(((int)Images.Panel)).gameObject.AddUIEvent((data) => CloseAndAddAction(), Define.UIEvent.LeftClick);
        GetImage(((int)Images.Panel)).gameObject.AddUIEvent((data) => CloseAndAddAction(), Define.UIEvent.RightClick);

        GetImage(((int)Images.CloseButton)).gameObject.AddUIEvent((data) => CloseAndAddAction(), Define.UIEvent.LeftClick);
    }

    void CloseAndAddAction()
    {
        Debug.Log("중복체크");

        string option = Data.TextDataList[0].optionString;

        if (option.Contains("@Action"))
        {
            string actionName = option.Substring(option.IndexOf("@Action::") + 9, option.IndexOf("::Action") - (option.IndexOf("@Action::") + 9));
            int id = 0;
            if (int.TryParse(actionName, out id))
            {
                EventManager.Instance.GetAction(id)?.Invoke();
            }
            else
            {
                EventManager.Instance.GetAction(actionName)?.Invoke();
            }
        }


        Managers.UI.ClosePopupPick(this);
        Managers.Dialogue.currentDialogue = null;
        UserData.Instance.GameMode = Define.TimeMode.Normal;
    }



    public void Init_Text()
    {
        if (Data == null)
        {
            Debug.Log("퀘스트 데이터가 없음");
            return;
        }

        GetTMP(((int)TMP.Title)).text = Data.dialogueName;
        GetTMP(((int)TMP.Content)).text = "";

        foreach (var item in Data.TextDataList)
        {
            GetTMP(((int)TMP.Content)).text += item.mainText;
            GetTMP(((int)TMP.Content)).text += "\n";
        }
    }


    #region IDialogue /  OptionBox
    public DialogueData Data { get; set; }
    public float TextDelay { get; set; }


    UI_OptionBox optionBox;
    public void AddOption(GameObject button)
    {
        if (optionBox == null)
        {
            optionBox = Managers.UI.ShowPopUpAlone<UI_OptionBox>();
        }
        button.transform.SetParent(optionBox.GetTransform());
    }

    public void CloseOptionBox()
    {
        Managers.UI.ClosePopUp(optionBox);
        optionBox = null;
        Managers.Dialogue.currentDialogue = null;
        UserData.Instance.GameMode = Define.TimeMode.Normal;
    }


    #endregion
}
