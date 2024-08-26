using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DialogueBubble : UI_PopUp, IWorldSpaceUI, IDialogue
{
    void Start()
    {
        //Time.timeScale = 0;
        UserData.Instance.GameMode = Define.GameMode.Stop;
        Init();

        mainCam = Camera.main.GetComponent<CameraControl>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            SkipText();
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (Managers.Scene.GetCurrentScene() == SceneName._2_Management)
            {
                PerfectSkip();
            }
        }
    }
    private void LateUpdate()
    {
        BubbleSizeFitter();
    }
    public void SetCanvasWorldSpace()
    {
        Managers.UI.SetCanvas(gameObject, RenderMode.WorldSpace, true);

        if (Managers.Scene.GetCurrentScene() != SceneName._2_Management)
        {
            GetComponent<RectTransform>().localScale = Vector3.one * 0.02f;
            OffsetSize = 0.02f;
        }
    }

    float OffsetSize = 0.01f;

    enum Contents
    {
        Panel,
        Bubble,
        Text,
        Bubble_tail,
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



        //? 테스트
        if (Managers.Scene.GetCurrentScene() == SceneName._2_Management)
        {
            var letterBox = Managers.UI.ShowPopUpNonPush<UI_LetterBox>();
            letterBox.SetBoxOption(UI_LetterBox.BoxOption.Dialogue, this);
        }
        else if (Managers.Scene.GetCurrentScene() == SceneName._6_NewOpening)
        {
            var letterBox = Managers.UI.ShowPopUpNonPush<UI_LetterBox>();
            letterBox.SetBoxOption(UI_LetterBox.BoxOption.LetterBox, this);
        }
    }




    void BubbleSizeFitter()
    {
        bubbleImage.sizeDelta = textTransform.sizeDelta;
        if (bubbleImage.sizeDelta == Vector2.zero)
        {
            bubbleImage.GetComponent<Image>().enabled = false;
        }
        else if (bubbleImage.GetComponent<Image>().isActiveAndEnabled == false)
        {
            bubbleImage.GetComponent<Image>().enabled = true;
        }
    }

    void BubbleReset()
    {
        textTransform.sizeDelta = Vector2.zero;
        bubbleImage.sizeDelta = Vector2.zero;
        //bubbleImage.GetComponent<Image>().enabled = false;
        GetObject(((int)Contents.Bubble)).SetActive(false);
    }

    void ShowText(string _text)
    {
        if (_text.Length <= 1)
        {
            //Debug.Log("1글자임" + _text.Length);
            //Debug.Log(string.IsNullOrEmpty(_text));
            //Debug.Break();
            mainText.text = string.Empty;
        }
        else
        {
            mainText.text = _text;
            SoundManager.Instance.PlaySound("SFX/Speech1");
            if (GetObject(((int)Contents.Bubble)).activeSelf == false)
            {
                GetObject(((int)Contents.Bubble)).SetActive(true);
            }
        }
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
        GetComponent<RectTransform>().localScale = Vector3.one * -OffsetSize;
        textTransform.localScale = Vector3.one * -1;
        textTransform.localPosition = new Vector3(0, 10, 0);

        //GetComponent<RectTransform>().rotation = Quaternion.Euler(180, 0, 0);
        //GetObject(((int)Contents.Panel)).GetComponent<RectTransform>().localRotation = Quaternion.Euler(180, 0, 0);
        //textTransform.localRotation = Quaternion.Euler(180, 0, 0);
        //textTransform.localPosition = new Vector3(0, 10, 0);
    }
    void BubbleNormal()
    {
        GetComponent<RectTransform>().localScale = Vector3.one * OffsetSize;
        textTransform.localScale = Vector3.one * 1;
        textTransform.localPosition = new Vector3(0, textTransform.sizeDelta.y / 2, 0);

        //GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
        //GetObject(((int)Contents.Panel)).GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
        //textTransform.localRotation = Quaternion.Euler(0, 0, 0);
        //textTransform.localPosition = new Vector3(0, textTransform.sizeDelta.y / 2, 0);
    }








    public Transform bubble_Position;
    [field: SerializeField]
    public DialogueData Data { get; set; }
    public float TextDelay { get; set; }

    int textCount;

    public int charCount = 1;

    WaitForSecondsRealtime seconds;



    void Init_Conversation()
    {
        seconds = new WaitForSecondsRealtime(TextDelay);
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


    IEnumerator ContentsRoofWithType(DialogueData textData)
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
                //Camera.main.transform.position = new Vector3(pos.position.x, pos.position.y, -10);
                Camera.main.GetComponent<CameraControl>().ChasingTarget(pos, 2);
            }

            if (option.Contains("@Fade"))
            {
                string targetPos = option.Substring(option.IndexOf("@Fade::") + 7, option.IndexOf("::Fade") - (option.IndexOf("@Fade::") + 7));
                int fadeOption = int.Parse(targetPos);

                var fade = Managers.UI.ShowPopUp<UI_Fade>();
                fade.SetFadeOption((UI_Fade.FadeMode)fadeOption, 1, true);
            }

            if (option.Contains("@Target"))
            {
                string targetPos = option.Substring(option.IndexOf("@Target::") + 9, option.IndexOf("::Target") - (option.IndexOf("@Target::") + 9));
                Transform pos;
                if (Managers.Scene.GetCurrentScene() == SceneName._2_Management)
                {
                    pos = GameManager.Placement.Find_Placement(targetPos);
                }
                else
                {
                    pos = GameObject.Find(targetPos).transform;
                }

                BubbleReset();
                transform.position = pos.position;
                yield return null;
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

            if (option.Contains("@Skip"))
            {
                string timer = option.Substring(option.IndexOf("@Skip::") + 7, option.IndexOf("::Skip") - (option.IndexOf("@Skip::") + 7));
                StartCoroutine(AutoSkip(float.Parse(timer)));
            }


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


            if (option.Contains("@Reserve"))
            {
                string actionName = option.Substring(option.IndexOf("@Reserve::") + 10, option.IndexOf("::Reserve") - (option.IndexOf("@Reserve::") + 10));

                var split = actionName.Split(':');
                int days = int.Parse(split[0]);
                int id = int.Parse(split[1]);

                EventManager.Instance.ReservationToQuest(days, id);
            }




            Action optionAction = null;
            if (option.Contains("@Option")) //? ID를 받아서 퀘스트만큼의 선택지를 제공
            {
                string npcID = option.Substring(option.IndexOf("@Option::") + 9, option.IndexOf("::Option") - (option.IndexOf("@Option::") + 9));
                var npc = GuildManager.Instance.GetInteraction(int.Parse(npcID));
                optionAction = () => npc.OneTimeOptionButton();
            }


            //? 위의 Option은 길드에서만 가능. 만약 게임씬에서 선택지를 쓰고싶으면 새로 다른이름으로 새로 만들어야함.


            textCount++;
            yield return StartCoroutine(TypingEffect(Data.TextDataList[textCount - 1].mainText, optionAction));
        }

        //Time.timeScale = 1;
        UserData.Instance.GameMode = Define.GameMode.Normal;
        Managers.UI.ClosePopupPick(this);
        Managers.Dialogue.currentDialogue = null;
        Debug.Log("대화 종료");
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

        //Debug.Log("마우스 클릭 대기중");
        yield return new WaitUntil(() => isTyping == true);
        SoundManager.Instance.PlaySound("SFX/Speech2");
        yield return new WaitForEndOfFrame();
    }


    CameraControl mainCam;

    void SkipText()
    {
        if (Managers.UI._popupStack.Peek() != this) return;
        if (mainCam.Cor_CameraChasing != null) return;

        if (isAutoMode)
        {
            return;
        }

        if (isTyping)
        {
            isSkip = true;
        }
        else
        {
            isTyping = true;
        }
    }

    bool isAutoMode;
    IEnumerator AutoSkip(float time)
    {
        isAutoMode = true;
        yield return new WaitForEndOfFrame();

        yield return new WaitUntil(() => isTyping == false);

        yield return new WaitForSecondsRealtime(time);
        isAutoMode = false;
        SkipText();
    }



    void PerfectSkip()
    {
        if (Data == null)
        {
            return;
        }

        if (Managers.Dialogue.AllowPerfectSkip == false)
        {
            return;
        }

        DialogueData textData = Data;

        while (textCount < textData.TextDataList.Count)
        {
            string option = textData.TextDataList[textCount].optionString;
            if (option == null)
            {
                option = string.Empty;
            }

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

            if (option.Contains("@Reserve"))
            {
                string actionName = option.Substring(option.IndexOf("@Reserve::") + 10, option.IndexOf("::Reserve") - (option.IndexOf("@Reserve::") + 10));

                var split = actionName.Split(':');
                int days = int.Parse(split[0]);
                int id = int.Parse(split[1]);

                EventManager.Instance.ReservationToQuest(days, id);
            }

            //if (option.Contains("@Option")) //? ID를 받아서 퀘스트만큼의 선택지를 제공
            //{
            //    //Debug.Log("선택지 제공 - 스킵할 때 여기서 멈춰야함");
            //    string npcID = option.Substring(option.IndexOf("@Option::") + 9, option.IndexOf("::Option") - (option.IndexOf("@Option::") + 9));
            //    var npc = GuildManager.Instance.GetInteraction(int.Parse(npcID));
            //    npc.OneTimeOptionButton();
            //}
            ////? 위의 Option은 길드에서만 가능. 만약 게임씬에서 선택지를 쓰고싶으면 새로 다른이름으로 새로 만들어야함.

            textCount++;
        }

        UserData.Instance.GameMode = Define.GameMode.Normal;
        Managers.UI.ClosePopupPick(this);
        Managers.Dialogue.currentDialogue = null;
        Debug.Log("대화 스킵");
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
