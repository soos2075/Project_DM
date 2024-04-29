using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EndingCanvas : UI_Scene, IDialogue
{

    void Start()
    {
        Init();
    }


    enum Texts
    {
        MainText,
        //TempText,
    }

    enum Images
    {
        BackGround,
        MainImage,
    }



    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));


        //gameObject.AddUIEvent(data => SkipText(), Define.UIEvent.LeftClick);
        Init_Conversation();
    }


    


    public Endings EndingState;

    public void SelectEnding(Endings ending)
    {
        EndingState = ending;
    }






    [field: SerializeField]
    public DialogueData Data { get; set; }
    public float TextDelay { get; set; }

    int textCount;
    public int charCount = 1;
    WaitForSeconds seconds;

    void Init_Conversation()
    {
        seconds = new WaitForSeconds(0.06f);
        StartCoroutine(Ending());
    }




    IEnumerator Ending()
    {
        Image mainImage = GetImage((int)Images.MainImage);
        var mainText = GetTMP((int)Texts.MainText);

        string main_1 = GetTMP((int)Texts.MainText).text;
        mainText.text = "";

        //? Image 1
        //GetTMP((int)Texts.TempText).text = "Image 1 (공통)";
        yield return StartCoroutine(SceneFadeOnce(mainImage, mainText, main_1));


        GetTMP((int)Texts.MainText).text = "";
        mainText.color = Color.white;

        yield return new WaitForSeconds(1);

        //? Image 2
        //GetTMP((int)Texts.TempText).text = "Image 2 (엔딩별 이미지)";
        yield return StartCoroutine(SceneFadeOnce(mainImage, mainText, "그곳에서 깨어난 것은 ---- 였다.\n\n그리고 나는 123123 그래서 asdfasdf\n\n그렇게 모험은 끝이 났다."));


        GetTMP((int)Texts.MainText).text = "";
        mainText.color = Color.white;
        yield return new WaitForSeconds(1);

        //? Image 3
        //GetTMP((int)Texts.TempText).text = "End (여긴 이미지 넣을지 말지 고민중)";


        //? ending
        var ui = Managers.UI.ShowPopUp<UI_Ending>();
    }


    IEnumerator SceneFadeOnce(Image targetImage, TextMeshProUGUI targetText, string main_1)
    {
        yield return StartCoroutine(Fade(FadeMode.ToAlpha, 2, targetImage));

        //Conversation(main_1);
        Data = new DialogueData();
        Data.TextDataList.Add(new DialogueData.TextData("", main_1));
        yield return StartCoroutine(ContentsRoofWithType(Data));

        yield return new WaitForSeconds(1);

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







    void Conversation(string _mainText)
    {
        //if (Data == null)
        //{
        //    return;
        //}
        Data = new DialogueData();
        Data.TextDataList.Add(new DialogueData.TextData("", _mainText));
        StartCoroutine(ContentsRoofWithType(Data));
    }


    IEnumerator ContentsRoofWithType(DialogueData textData)
    {
        textCount = 0;
        yield return new WaitForEndOfFrame();

        while (textCount < textData.TextDataList.Count)
        {
            yield return StartCoroutine(TypingEffect(Data.TextDataList[textCount].mainText));
            textCount++;
        }
        Debug.Log("출력 끝");
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


        Debug.Log("마우스 클릭 대기중");
        //yield return new WaitUntil(() => isTyping == true);
        yield return new WaitForEndOfFrame();
    }

    void SpeakSomething(string contents)
    {
        GetTMP((int)Texts.MainText).text = contents;
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
}
