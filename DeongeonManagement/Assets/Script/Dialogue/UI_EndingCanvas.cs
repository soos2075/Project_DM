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


        float timer = 0;
        float durationTime = 2;


        //GetTMP((int)Texts.TempText).text = "Image 1 (����)";
        Color color = Color.clear;

        while (timer < durationTime)
        {
            color.a += (Time.deltaTime / durationTime);
            color.r += (Time.deltaTime / durationTime);
            color.g += (Time.deltaTime / durationTime);
            color.b += (Time.deltaTime / durationTime);

            mainImage.color = color;
            yield return null;
            timer += Time.deltaTime;
        }
        mainImage.color = Color.white;

        //Conversation(main_1);
        Data = new DialogueData();
        Data.TextDataList.Add(new DialogueData.TextData("", main_1));
        yield return StartCoroutine(ContentsRoofWithType(Data));

        yield return new WaitForSeconds(1);
        timer = 0;
        while (timer < durationTime)
        {
            color.a -= (Time.deltaTime / durationTime);
            color.r -= (Time.deltaTime / durationTime);
            color.g -= (Time.deltaTime / durationTime);
            color.b -= (Time.deltaTime / durationTime);

            mainImage.color = color;
            mainText.color = color;
            yield return null;
            timer += Time.deltaTime;
        }
        mainImage.color = Color.clear;
        mainText.color = Color.white;

        //GetTMP((int)Texts.TempText).text = "Image 2 (������ �̹���)";
        GetTMP((int)Texts.MainText).text = "";

        yield return new WaitForSeconds(1);
        timer = 0;
        while (timer < durationTime)
        {
            color.a += (Time.deltaTime / durationTime);
            color.r += (Time.deltaTime / durationTime);
            color.g += (Time.deltaTime / durationTime);
            color.b += (Time.deltaTime / durationTime);

            mainImage.color = color;
            yield return null;
            timer += Time.deltaTime;
        }
        mainImage.color = Color.white;



        Data = new DialogueData();
        Data.TextDataList.Add(new DialogueData.TextData("", "�װ����� ��� ���� ---- ����.\n\n�׸��� ���� 123123 �׷��� asdfasdf\n\n�׷��� ������ ���� ����."));
        yield return StartCoroutine(ContentsRoofWithType(Data));


        yield return new WaitForSeconds(1);
        timer = 0;
        while (timer < durationTime)
        {
            color.a -= (Time.deltaTime / durationTime);
            color.r -= (Time.deltaTime / durationTime);
            color.g -= (Time.deltaTime / durationTime);
            color.b -= (Time.deltaTime / durationTime);

            mainImage.color = color;
            mainText.color = color;
            yield return null;
            timer += Time.deltaTime;
        }
        mainImage.color = Color.clear;
        mainText.color = Color.white;


        //GetTMP((int)Texts.TempText).text = "End (���� �̹��� ������ ���� �����)";
        GetTMP((int)Texts.MainText).text = "";

        yield return new WaitForSeconds(1);
        var ui = Managers.UI.ShowPopUp<UI_Ending>();
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
        Debug.Log("��� ��");
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


        Debug.Log("���콺 Ŭ�� �����");
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
