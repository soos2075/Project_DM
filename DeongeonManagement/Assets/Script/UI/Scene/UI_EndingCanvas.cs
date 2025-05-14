using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EndingCanvas : UI_Scene, IDialogue
{

    void Start()
    {
        Time.timeScale = 1;
        Init();
    }


    private void LateUpdate()
    {
        if (Input.anyKey)
        {
            //seconds = 0.03f;
            Time.timeScale = 2.0f;
        }
        else
        {
            //seconds = 0.06f;
            Time.timeScale = 1.0f;
        }


        TurnOver_HotKey();
    }

    private float spaceStartTime = -1f; // 스페이스바를 누르기 시작한 시간
    private const float HOLD_DURATION = 1.0f; // 필요한 홀드 시간

    void TurnOver_HotKey()
    {
        // 스페이스바를 누르기 시작할 때
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            spaceStartTime = Time.time;
        }

        // 스페이스바를 떼었을 때
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            spaceStartTime = -1f; // 타이머 리셋
        }

        if (Input.GetKey(KeyCode.Escape) && spaceStartTime > 0f)
        {
            if (Time.time - spaceStartTime >= HOLD_DURATION)
            {
                Skip_Ending();
                spaceStartTime = -1f; // 액션 실행 후 타이머 리셋
            }
        }
    }


    void Skip_Ending()
    {
        StopAllCoroutines();
        var ui = Managers.UI.ShowPopUp<UI_Ending>();
    }



    enum Images
    {
        BackGround,
        //MainImage,
    }

    public enum Preset
    {
        Preset_Center,
        Preset_Left,
        Preset_Right,
        Preset_TextOnly,
    }

    Image mainImage;
    TextMeshProUGUI mainText;


    public override void Init()
    {
        //Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(Preset));

        //gameObject.AddUIEvent(data => SkipText(), Define.UIEvent.LeftClick);
        Select_Preset(Preset.Preset_Center);

        SelectEnding();
        Init_Conversation();
    }


    SO_Ending EndingData;

    public void SelectEnding()
    {
        EndingData = CollectionManager.Instance.GetData_Ending(UserData.Instance.EndingState);
    }


    void Select_Preset(Preset preset)
    {
        for (int i = 0; i < Enum.GetNames(typeof(Preset)).Length; i++)
        {
            GetObject(i).SetActive(false);
        }

        var obj = GetObject((int)preset);
        mainImage = obj.GetComponentInChildren<Image>(true);
        mainText = obj.GetComponentInChildren<TextMeshProUGUI>(true);
        mainImage.sprite = Managers.Sprite.GetClear();
        mainText.text = "";
        obj.SetActive(true);
    }



    [field: SerializeField]
    public DialogueData Data { get; set; }
    public float TextDelay { get; set; }

    int textCount;
    public int charCount = 1;
    public float seconds;

    void Init_Conversation()
    {
        seconds = 0.06f;

        if (EndingData == null)
        {
            Debug.Log("SO_Ending Data Not Exist");
            return;
        }
        else
        {
            StartCoroutine(Ending(EndingData));
        }
    }


    IEnumerator Ending(SO_Ending data)
    {
        for (int i = 0; i < data.cutSceneList.Count; i++)
        {
            Select_Preset(data.cutSceneList[i].preset);

            mainImage.sprite = data.cutSceneList[i].sprite;
            //mainText.text = "";

            var textData = Managers.Dialogue.GetDialogue(data.cutSceneList[i].dialogueName);
            yield return StartCoroutine(SceneFadeAndShowText(mainImage, mainText, textData));

            mainText.text = "";
            mainText.color = Color.white;

            yield return new WaitForSeconds(1);
        }

        var ui = Managers.UI.ShowPopUp<UI_Ending>();
    }


    //IEnumerator SceneFadeOnce(Image targetImage, TextMeshProUGUI targetText, string main_1)
    //{
    //    yield return StartCoroutine(Fade(FadeMode.ToAlpha, 2, targetImage));

    //    //Conversation(main_1);
    //    Data = new DialogueData();
    //    Data.TextDataList.Add(new DialogueData.TextData("", main_1));
    //    yield return StartCoroutine(ContentsRoofWithType(Data));

    //    yield return new WaitForSeconds(1);

    //    yield return StartCoroutine(Fade(FadeMode.ToClear, 2, targetImage, targetText));
    //}

    IEnumerator SceneFadeAndShowText(Image targetImage, TextMeshProUGUI targetText, DialogueData dialogueData)
    {
        yield return StartCoroutine(Fade(FadeMode.ToAlpha, 2, targetImage));

        Data = dialogueData;
        yield return StartCoroutine(ContentsRoofWithType(Data));

        yield return new WaitForSeconds(1);
        var startTime = Time.time;
        yield return new WaitUntil(() => Input.anyKeyDown || Input.GetMouseButtonDown(0) || Time.time >= startTime + 5f);

        yield return StartCoroutine(Fade(FadeMode.ToClear, 2, targetImage, targetText));
    }


    enum FadeMode
    {
        ToClear,
        ToAlpha,
        ToWhite,
        ToBlack,
    }


    IEnumerator Fade(FadeMode mode, float durationTime, Image targetImage, TextMeshProUGUI targetText = null)
    {
        float timer = 0;
        Color color;

        switch (mode)
        {
            case FadeMode.ToClear:
                color = Color.white;
                while (timer < durationTime)
                {
                    color.a -= (Time.deltaTime / durationTime);
                    color.r -= (Time.deltaTime / durationTime);
                    color.g -= (Time.deltaTime / durationTime);
                    color.b -= (Time.deltaTime / durationTime);

                    targetImage.color = color;
                    if (targetText != null)
                    {
                        targetText.color = color;
                    }
                    yield return null;
                    timer += Time.deltaTime;
                }
                targetImage.color = Color.clear;
                if (targetText != null)
                {
                    targetText.color = Color.clear;
                }
                break;


            case FadeMode.ToAlpha:
                color = Color.clear;
                while (timer < durationTime)
                {
                    color.a += (Time.deltaTime / durationTime);
                    color.r += (Time.deltaTime / durationTime);
                    color.g += (Time.deltaTime / durationTime);
                    color.b += (Time.deltaTime / durationTime);

                    targetImage.color = color;
                    if (targetText != null)
                    {
                        targetText.color = color;
                    }

                    yield return null;
                    timer += Time.deltaTime;
                }
                targetImage.color = Color.white;
                if (targetText != null)
                {
                    targetText.color = Color.white;
                }
                break;


            case FadeMode.ToWhite:
                break;


            case FadeMode.ToBlack:
                break;
        }
    }



    //void Conversation(string _mainText)
    //{
    //    Data = new DialogueData();
    //    Data.TextDataList.Add(new DialogueData.TextData("", _mainText));
    //    StartCoroutine(ContentsRoofWithType(Data));
    //}


    IEnumerator ContentsRoofWithType(DialogueData textData)
    {
        textCount = 0;
        yield return new WaitForEndOfFrame();

        string TextAddText = "";
        int prevCharCount = 0;

        while (textCount < textData.TextDataList.Count)
        {
            if (!string.IsNullOrEmpty(TextAddText))
            {
                TextAddText += "\n\n";
                prevCharCount = TextAddText.Length;
            }
            TextAddText += Data.TextDataList[textCount].mainText;

            yield return StartCoroutine(TypingEffect(TextAddText, prevCharCount));
            textCount++;
        }
        Debug.Log("출력 끝");
    }


    bool isSkip = false;
    bool isTyping = false;
    IEnumerator TypingEffect(string contents, int prevCount)
    {
        int charIndexer = prevCount;

        while (!isSkip && contents.Length >= charIndexer)
        {
            isTyping = true;

            string nowText = contents.Substring(0, charIndexer);
            SpeakSomething(nowText);
            yield return new WaitForSeconds(seconds);
            charIndexer += charCount;
        }
        isTyping = false;
        isSkip = false;

        SpeakSomething(contents);

        yield return new WaitForEndOfFrame();
    }

    void SpeakSomething(string contents)
    {
        mainText.text = contents;
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



    public void AddOption(GameObject button)
    {
        throw new System.NotImplementedException();
    }

    public void CloseOptionBox()
    {
        throw new NotImplementedException();
    }
}
