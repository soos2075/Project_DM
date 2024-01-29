using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Dialogue : UI_PopUp
{
    void Start()
    {
        Init();
        Time.timeScale = 0;
    }

    void Update()
    {
        
    }


    public override void Init()
    {
        base.Init();

        Init_LogBox();
        Init_Conversation();
    }


    #region Log
    //? 로그오프셋 LogText의 y크기 = 라인 * 50;

    enum Objects
    {
        LogButton,
        LogBox,

        LogTitle,
        LogText,
    }

    string logText;

    void Init_LogBox()
    {
        Bind<GameObject>(typeof(Objects));

        GetObject(((int)Objects.LogButton)).gameObject.AddUIEvent((data) => Show_LogBox(), Define.UIEvent.LeftClick);

        GetObject(((int)Objects.LogBox)).AddUIEvent((data) => Close_LogBox(), Define.UIEvent.RightClick);
        Close_LogBox();

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
            box.sizeDelta = new Vector2(box.sizeDelta.x, linecount * 50);
        }
    }
    void Close_LogBox()
    {
        GetObject(((int)Objects.LogBox)).SetActive(false);
    }

    #endregion


    #region Conversation
    enum Texts
    {
        main,
        name,
    }

    public SO_DialogueData data;

    int textCount;
    public float delay;
    public int charCount = 1;

    WaitForSecondsRealtime seconds;


    void Init_Conversation()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));

        seconds = new WaitForSecondsRealtime(delay);

        GetTMP(((int)Texts.main)).gameObject.AddUIEvent((data) => SkipText(), Define.UIEvent.LeftClick);

        Conversation_Test();
    }


    void Conversation_Test()
    {
        if (data == null)
        {
            return;
        }

        StartCoroutine(ContentsRoof(data));
    }

    IEnumerator ContentsRoof(SO_DialogueData textData)
    {
        while (textCount < textData.mainTextList.Count)
        {
            Debug.Log("새 대화 출력");
            yield return StartCoroutine(TypingEffect(data.mainTextList[textCount]));
            textCount++;
        }
        Time.timeScale = 1;

        Managers.UI.ClosePopUp(this);
        Debug.Log("대화창 닫음");
    }


    bool isSkip = false;
    bool isTyping = false;
    IEnumerator TypingEffect(string contents)
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
        logText += $"{contents}\n\n";

        Debug.Log("마우스 클릭 대기중");
        yield return new WaitUntil(() => isTyping == true);

        yield return new WaitForEndOfFrame();
    }

    void SpeakSomething(string contents)
    {
        GetTMP((int)Texts.main).text = contents;
        //GetText((int)ConversationTexts.contentsText).text = contents;
        LineUp();
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

    void LineUp()
    {
        if (GetTMP((int)Texts.main).textInfo.lineCount > 6)
        {
            GetTMP((int)Texts.main).alignment = TextAlignmentOptions.BottomLeft;
        }
        else
        {
            GetTMP((int)Texts.main).alignment = TextAlignmentOptions.TopLeft;
        }
        
    }

    #endregion
}
