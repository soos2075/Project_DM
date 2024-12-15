using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EventBox : UI_Scene
{
    void Start()
    {
        Init();
    }
    void Update()
    {
        if (Current) return;

        ShowEventBox(textOrigin);

    }
    private void LateUpdate() //? GUI가 그려지고 난 다음에 textInfo가 업데이트 되기때문에 Late에서 해줘야함
    {
        if (Current) return;

        int prevPage = maxPage;

        maxPage = (mainText.textInfo.lineCount / pageLine);

        if (mainText.textInfo.lineCount % pageLine > 0) //? 페이지수로 나누고나서 한줄이 더 추가되면 그 때 페이지 넘기기
        {
            maxPage += 1;
        }


        if (prevPage != maxPage)
        {
            mainText.pageToDisplay = maxPage;
        }

        if (maxPage > 1)
        {
            pageText.text = $"{mainText.pageToDisplay}/{maxPage}";
        }
        else
        {
            pageText.text = "";
        }
    }

    static string textOrigin;
    public static void AddEventText(string contents)
    {
        if (string.IsNullOrEmpty(textOrigin))
        {
            textOrigin += $"{contents}";
        }
        else
        {
            textOrigin += $"\n{contents}";
        }
    }


    enum Contents
    {
        Panel_Active,
        Panel_Inactive,

        MainText,
        Next,
        Previous,
        Page,

        Maximize,
        Minimize,
    }

    TextMeshProUGUI mainText;
    TextMeshProUGUI pageText;

    int maxPage;
    readonly int pageLine = 6;
    

    public override void Init()
    {
        //Managers.UI.SetCanvas(gameObject, true);
        //Managers.UI.SetCanvas(gameObject, RenderMode.ScreenSpaceCamera);
        Managers.UI.SetCanvas_SubCamera(gameObject, RenderMode.ScreenSpaceCamera, false);


        Bind<GameObject>(typeof(Contents));

        mainText = GetObject((int)Contents.MainText).GetComponent<TextMeshProUGUI>();
        pageText = GetObject((int)Contents.Page).GetComponent<TextMeshProUGUI>();


        GetObject((int)Contents.Next).gameObject.AddUIEvent((data) => NextButton());
        GetObject((int)Contents.Previous).gameObject.AddUIEvent((data) => PreviousButton());

        GetObject((int)Contents.Maximize).gameObject.AddUIEvent((data) => BoxActive(Current));
        GetObject((int)Contents.Minimize).gameObject.AddUIEvent((data) => BoxActive(Current));

        //GetObject((int)Contents.Minimize).GetComponent<Image>().sprite = Managers.Sprite.SmallButtons(SpriteManager.UI_Small_Buttons.Plus_Normal);


        mainText.text = "";
        BoxActive(true);
        BoxActive(false);
    }





    void ShowEventBox(string contents)
    {
        if (mainText.text == contents)
        {
            return;
        }

        mainText.text = contents;
        //? 자동페이지 넘기기기능 = on off로 제공해도될듯?
        //mainText.pageToDisplay = maxPage;
    }


    void NextButton()
    {
        if (mainText.isTextOverflowing == false) return;
        if (maxPage <= mainText.pageToDisplay) return;

        mainText.pageToDisplay++;
    }

    void PreviousButton()
    {
        if (mainText.isTextOverflowing == false) return;
        if (1 >= mainText.pageToDisplay) return;

        mainText.pageToDisplay--;
    }



    bool Current { get; set; }


    public void BoxActive()
    {
        if (Current)
        {
            Current = false;
            GetObject((int)Contents.Panel_Active).SetActive(true);
            GetObject((int)Contents.Panel_Inactive).SetActive(false);
        }
        else
        {
            Current = true;
            GetObject((int)Contents.Panel_Active).SetActive(false);
            GetObject((int)Contents.Panel_Inactive).SetActive(true);
        }
    }

    public void BoxActive(bool _active)
    {
        if (_active)
        {
            Current = false;
            GetObject((int)Contents.Panel_Active).SetActive(true);
            GetObject((int)Contents.Panel_Inactive).SetActive(false);
        }
        else
        {
            Current = true;
            GetObject((int)Contents.Panel_Active).SetActive(false);
            GetObject((int)Contents.Panel_Inactive).SetActive(true);
        }
    }


    public void TextClear()
    {
        textOrigin = "";
    }

}
