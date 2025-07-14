using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Dialogue : UI_PopUp, IDialogue
{
    void Start()
    {
        Init();
        //Time.timeScale = 0;
        UserData.Instance.GameMode = Define.TimeMode.Stop;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SkipText();
        }
    }



    public override void Init()
    {
        Bind<GameObject>(typeof(Objects));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Sprite));

        Managers.UI.SetCanvas(gameObject);
        CloseDialogueEvent();

        Init_LogBox();
        Init_Conversation();
    }

    void CloseDialogueEvent()
    {
        var obj = Util.FindChild(gameObject, "Panel");
        if (obj)
        {
            obj.AddUIEvent((data) =>
            {
                Managers.UI.ClosePopUp();
                Managers.Dialogue.currentDialogue = null;

                UserData.Instance.GameMode = Define.TimeMode.Normal;

            }, Define.UIEvent.RightClick);
        }
    }

    enum Objects
    {
        LogButton,
        LogBox,

        LogTitle,
        LogText,

        TitleBox,
        OptionBox,

        TextBox,
        Panel,
    }

    #region Log
    string logText;

    void Init_LogBox()
    {


        GetObject(((int)Objects.LogButton)).gameObject.AddUIEvent((data) => Show_LogBox(), Define.UIEvent.LeftClick);

        GetObject(((int)Objects.LogBox)).AddUIEvent((data) => Close_Box(), Define.UIEvent.RightClick);
        Close_Box();

    }
    void Show_LogBox()
    {
        GetObject(((int)Objects.LogBox)).SetActive(true);
        GetObject(((int)Objects.LogText)).GetComponent<TextMeshProUGUI>().text = logText;

        StartCoroutine(Show_Log());
    }
    IEnumerator Show_Log()
    {
        yield return new WaitForEndOfFrame();
        var tmp = GetObject(((int)Objects.LogText)).GetComponent<TextMeshProUGUI>();

        var box = GetObject(((int)Objects.LogText)).GetComponent<RectTransform>();

        int linecount = tmp.textInfo.lineCount;

        //Debug.Log(linecount + "@@@@@@@");
        //Debug.Log(box.sizeDelta + "$$$$$$$$$");
        if (linecount > 15)
        {
            box.sizeDelta = new Vector2(box.sizeDelta.x, linecount * 35);
        }
    }
    void Close_Box()
    {
        GetObject(((int)Objects.LogBox)).SetActive(false);
        GetObject(((int)Objects.TitleBox)).gameObject.SetActive(false);
        GetObject(((int)Objects.OptionBox)).gameObject.SetActive(false);
    }

    #endregion


    #region OptionBox
    public void AddOption(GameObject button)
    {
        if (GetObject(((int)Objects.OptionBox)).gameObject.activeSelf == false)
        {
            GetObject(((int)Objects.OptionBox)).gameObject.SetActive(true);
        }

        button.transform.SetParent(GetObject(((int)Objects.OptionBox)).transform);
    }

    public void CloseOptionBox()
    {
        GetObject(((int)Objects.OptionBox)).gameObject.SetActive(false);
    }


    #endregion



    #region Conversation
    enum Texts
    {
        MainText,
        TitleName,
    }
    enum Sprite
    {
        SpeakerSprite,
    }

    [field:SerializeField]
    public DialogueData Data { get; set; }
    public float TextDelay { get; set; }

    int textCount;

    public int charCount = 1;

    WaitForSecondsRealtime seconds;


    void Init_Conversation()
    {


        seconds = new WaitForSecondsRealtime(TextDelay);
        GetObject(((int)Objects.TextBox)).AddUIEvent((data) => SkipText(), Define.UIEvent.LeftClick);
        //GetObject(((int)Objects.Panel)).AddUIEvent((data) => SkipText(), Define.UIEvent.LeftClick);

        Conversation_Test();
    }



    void Conversation_Test()
    {
        if (Data == null)
        {
            return;
        }

        StartCoroutine(ContentsRoofWithType(Data));
    }


    IEnumerator ContentsRoofWithType(DialogueData textData)
    {
        yield return new WaitForEndOfFrame();

        while (textCount < textData.TextDataList.Count)
        {
            //Debug.Log("새 대화 출력");
            //Debug.Log("새 대화 출력");

            string option = textData.TextDataList[textCount].optionString;
            if (option == null)
            {
                option = string.Empty;
            }

            if (option.Contains("@Name"))
            {
                string speakerName = option.Substring(option.IndexOf("@Name::") + 7, option.IndexOf("::Name") - (option.IndexOf("@Name::") + 7));
                if (string.IsNullOrEmpty(speakerName))
                {
                    GetTMP((int)Texts.TitleName).text = "";
                    GetObject(((int)Objects.TitleBox)).SetActive(false);
                }
                else
                {
                    GetObject(((int)Objects.TitleBox)).SetActive(true);
                    GetTMP((int)Texts.TitleName).text = speakerName;

                    logText += $"{speakerName.ToString().SetTextColorTag(Define.TextColor.yellow)}\n";
                }
            }

            //if (option.Contains("@Sprite"))
            //{
            //    string spritePath = option.Substring(option.IndexOf("@Sprite::") + 9, option.IndexOf("::Sprite") - (option.IndexOf("@Sprite::") + 9));
            //    if (string.IsNullOrEmpty(spritePath))
            //    {
            //        GetImage(0).sprite = null;
            //        GetImage(0).enabled = false;
            //    }
            //    else
            //    {
            //        GetImage(0).sprite = Managers.Sprite.GetSprite(spritePath);
            //        GetImage(0).enabled = true;
            //    }
            //}

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


            Action optionAction = null;
            if (option.Contains("@Option")) //? 옵션 타입은 두가지. 하나는 아이디를 받는거 / 하나는 Dia번호를 받는거. 두개 이름만 다르게하면됨
            {
                string npcID = option.Substring(option.IndexOf("@Option::") + 9, option.IndexOf("::Option") - (option.IndexOf("@Option::") + 9));
                var npc = GuildManager.Instance.GetInteraction(int.Parse(npcID));
                //var optionList = Managers.Dialogue.ShowOption(npcID);
                optionAction = () => npc.OneTimeOptionButton();
            }

            yield return StartCoroutine(TypingEffect(Data.TextDataList[textCount].mainText, optionAction));
            textCount++;
        }
        //Time.timeScale = 1;
        UserData.Instance.GameMode = Define.TimeMode.Normal;

        Managers.UI.ClosePopUp(this);
        Managers.Dialogue.currentDialogue = null;
        Debug.Log("대화창 닫음");
    }





    bool isSkip = false;
    bool isTyping = false;
    IEnumerator TypingEffect(string contents, Action action = null)
    {
        int charIndexer = 0;

        while (!isSkip && contents.Length >= charIndexer)
        {
            isTyping = true;
            
            string nowText = contents.Substring(0, charIndexer);
            SpeakSomething(nowText);
            yield return seconds;
            charIndexer += charCount;
        }
        isTyping = false;
        isSkip = false;

        SpeakSomething(contents);
        logText += $"{contents}\n\n\n";

        if (action != null)
        {
            StopAllCoroutines();
            action.Invoke();
        }

        Debug.Log("마우스 클릭 대기중");
        yield return new WaitUntil(() => isTyping == true);

        yield return new WaitForEndOfFrame();
    }

    void SpeakSomething(string contents)
    {
        GetTMP((int)Texts.MainText).text = contents;
        StartCoroutine(LineUp());
    }


    void SkipText()
    {
        if (isTyping)
        {
            isSkip = true;
        }
        else
        {
            isTyping = true;
        }
    }

    IEnumerator LineUp()
    {
        yield return new WaitForEndOfFrame();
        if (GetTMP((int)Texts.MainText).textInfo.lineCount > 6)
        {
            GetTMP((int)Texts.MainText).alignment = TextAlignmentOptions.BottomLeft;
        }
        else
        {
            GetTMP((int)Texts.MainText).alignment = TextAlignmentOptions.TopLeft;
        }
        
    }

    #endregion
}
