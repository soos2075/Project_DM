using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_DialogueBubble : UI_PopUp, IWorldSpaceUI, IDialogue
{
    void Start()
    {
        Time.timeScale = 0;
        Init();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SkipText();
        }

    }
    private void LateUpdate()
    {
        BubbleSizeFitter();
    }
    public void SetCanvasWorldSpace()
    {
        Managers.UI.SetCanvas(gameObject, RenderMode.WorldSpace, true);

        if (Managers.Scene.GetSceneName() == "3_Guild")
        {
            GetComponent<RectTransform>().localScale = Vector3.one * 0.02f;
        }
    }


    enum Contents
    {
        Panel,
        Bubble,
        Text,
    }

    TextMeshProUGUI mainText;
    RectTransform bubbleImage;
    RectTransform textTransform;

    public override void Init()
    {
        SetCanvasWorldSpace();

        Bind<GameObject>(typeof(Contents));

        GetObject(((int)Contents.Panel)).AddUIEvent((data) => SkipText(), Define.UIEvent.LeftClick);

        bubbleImage = GetObject(((int)Contents.Bubble)).GetComponent<RectTransform>();
        mainText = GetObject(((int)Contents.Text)).GetComponent<TextMeshProUGUI>();
        textTransform = GetObject(((int)Contents.Text)).GetComponent<RectTransform>();


        Init_BubblePosition();
        Init_Conversation();
    }




    void BubbleSizeFitter()
    {
        bubbleImage.sizeDelta = textTransform.sizeDelta;
    }


    void ShowText(string _text)
    {
        mainText.text = _text;
    }

    void Init_BubblePosition()
    {
        //Camera.main.transform.position = new Vector3(bubble_Position.position.x, bubble_Position.position.y, -10);
        if (bubble_Position == null)
        {
            Debug.Log("타겟이 없음");
            return;
        }
        gameObject.transform.position = bubble_Position.position;
    }

    void BubbleReverse()
    {
        GetComponent<RectTransform>().rotation = Quaternion.Euler(180, 0, 0);
        GetObject(((int)Contents.Panel)).GetComponent<RectTransform>().localRotation = Quaternion.Euler(180, 0, 0);
        textTransform.localRotation = Quaternion.Euler(180, 0, 0);
        textTransform.localPosition = new Vector3(0, 10, 0);
    }
    void BubbleNormal()
    {
        GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
        GetObject(((int)Contents.Panel)).GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
        textTransform.localRotation = Quaternion.Euler(0, 0, 0);
        textTransform.localPosition = Vector3.zero;
    }



    public Transform bubble_Position;
    [field: SerializeField]
    public SO_DialogueData Data { get; set; }

    int textCount;
    public float delay;
    public int charCount = 1;

    WaitForSecondsRealtime seconds;



    void Init_Conversation()
    {
        seconds = new WaitForSecondsRealtime(delay);
        //GetObject(((int)Objects.TextBox)).AddUIEvent((data) => SkipText(), Define.UIEvent.LeftClick);
        //GetObject(((int)Objects.Panel)).AddUIEvent((data) => SkipText(), Define.UIEvent.LeftClick);

        TalkStart();
    }



    void TalkStart()
    {
        if (Data == null)
        {
            return;
        }

        StartCoroutine(ContentsRoofWithType(Data));
    }


    IEnumerator ContentsRoofWithType(SO_DialogueData textData)
    {
        yield return new WaitForEndOfFrame();

        while (textCount < textData.TextDataList.Count)
        {
            string option = textData.TextDataList[textCount].optionString;
            if (option == null)
            {
                option = string.Empty;
            }

            if (option.Contains("@Camera"))
            {
                string targetPos = option.Substring(option.IndexOf("@Camera::") + 9, option.IndexOf("::Camera") - (option.IndexOf("@Camera::") + 9));
                Transform pos = GameObject.Find(targetPos).transform;
                Camera.main.transform.position = new Vector3(pos.position.x, pos.position.y, -10);
            }

            if (option.Contains("@Target"))
            {
                string targetPos = option.Substring(option.IndexOf("@Target::") + 9, option.IndexOf("::Target") - (option.IndexOf("@Target::") + 9));
                Transform pos = GameObject.Find(targetPos).transform;
                transform.position = pos.position;
            }
            if (option.Contains("@Pos"))
            {
                string Pos = option.Substring(option.IndexOf("@Pos::") + 6, option.IndexOf("::Pos") - (option.IndexOf("@Pos::") + 6));
                //Debug.Log(Pos);
                if (Pos == "Down")
                {
                    BubbleReverse();
                }
                else if (Pos == "Up")
                {
                    BubbleNormal();
                }
            }


            if (option.Contains("@Action"))
            {
                string spritePath = option.Substring(option.IndexOf("@Action::") + 9, option.IndexOf("::Action") - (option.IndexOf("@Action::") + 9));
                EventManager.Instance.GetAction(int.Parse(spritePath))?.Invoke();
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
        Time.timeScale = 1;

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

            if (nowText.Length > 1 && nowText[nowText.Length - 1] == '<')
            {
                int startIndex = contents.IndexOf('<');
                int lastIndex = contents.IndexOf('>');

                int offset = lastIndex - startIndex;

                charIndexer += (offset + 1);
                //Debug.Log(contents.Substring(0, charIndexer));
                nowText = contents.Substring(0, charIndexer);
            }

            ShowText(nowText);
            yield return seconds;
            charIndexer += charCount;
        }
        isTyping = false;
        isSkip = false;

        ShowText(contents);

        if (action != null)
        {
            StopAllCoroutines();
            //ClosePopUp();
            //Time.timeScale = 1;
            action.Invoke();
        }

        Debug.Log("마우스 클릭 대기중");
        yield return new WaitUntil(() => isTyping == true);

        yield return new WaitForEndOfFrame();
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


    #region OptionBox

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
    }


    #endregion
}
