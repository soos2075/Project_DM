using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manual : UI_PopUp
{
    void Start()
    {
        Init();
    }


    public Sprite Btn_Active;
    public Sprite Btn_Inactive;


    enum Buttons
    {
        Close,
        Next,
        Prev,

        Btn_Main,
        Btn_Dungeon,
        Btn_Hotkey,
    }

    enum TMP_Texts
    {
        Text_Page,
    }

    enum Images
    {
        Panel,

        GuideBox_Main,
        GuideBox_Dungeon,
        GuideBox_HotKey,
    }


    enum GuideContents
    {
        Main_1,
        Main_2,
        Main_3,
        Main_4,
        Main_5,

        Dungeon_1,
        Dungeon_2,
        Dungeon_3,
        Dungeon_4,

        HotKey_1,
    }


    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GuideContents));
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(TMP_Texts));
        Bind<Image>(typeof(Images));


        GetButton((int)Buttons.Close).gameObject.AddUIEvent(data => ClosePopUp());

        Init_Box();
    }

    int current_Num;
    List<GameObject> CurrentBox;
    //GameObject CurrentContent;

    void Init_Box()
    {
        GetButton((int)Buttons.Next).gameObject.AddUIEvent(data => Set_CG(1));
        GetButton((int)Buttons.Prev).gameObject.AddUIEvent(data => Set_CG(-1));

        GetButton((int)Buttons.Btn_Main).gameObject.AddUIEvent(data => Set_Number(0));
        GetButton((int)Buttons.Btn_Dungeon).gameObject.AddUIEvent(data => Set_Number(5));
        GetButton((int)Buttons.Btn_Hotkey).gameObject.AddUIEvent(data => Set_Number(9));


        CurrentBox = new List<GameObject>();
        for (int i = 0; i < System.Enum.GetNames(typeof(GuideContents)).Length; i++)
        {
            CurrentBox.Add(GetObject(i));
        }

        Set_CG(0);
    }

    void Set_Number(int changeValue)
    {
        CurrentBox[current_Num].SetActive(false);
        current_Num = changeValue;
        Set_CG(0);
    }


    void Set_CG(int changeValue)
    {
        if (CurrentBox == null) return;

        current_Num += changeValue;
        if (current_Num >= CurrentBox.Count)
        {
            current_Num = CurrentBox.Count - 1;
        }
        if (current_Num < 0)
        {
            current_Num = 0;
        }

        if (changeValue != 0)
        {
            CurrentBox[current_Num - changeValue].SetActive(false);
        }

        CurrentBox[current_Num].SetActive(true);

        //Debug.Log(current_Num);

        if (0 <= current_Num && current_Num <= 4)
        {
            Btn_Manual(Buttons.Btn_Main);
        }
        else if (5 <= current_Num && current_Num <= 8)
        {
            Btn_Manual(Buttons.Btn_Dungeon);
        }
        else
        {
            Btn_Manual(Buttons.Btn_Hotkey);
        }


        //GetImage((int)Images.GuideBox).sprite = CurrentBox[current_Num];
        GetTMP((int)TMP_Texts.Text_Page).text = $"{current_Num + 1} / {CurrentBox.Count}";
    }

    void Btn_Manual(Buttons btn)
    {
        Btn_Init();

        GetButton((int)btn).image.sprite = Btn_Inactive;
    }

    void Btn_Init()
    {
        GetButton((int)Buttons.Btn_Dungeon).image.sprite = Btn_Active;
        GetButton((int)Buttons.Btn_Hotkey).image.sprite = Btn_Active;
        GetButton((int)Buttons.Btn_Main).image.sprite = Btn_Active;
    }




    public override bool EscapeKeyAction()
    {
        return true;
    }
    private void OnEnable()
    {
        Time.timeScale = 0;
    }
    private void OnDestroy()
    {
        PopupUI_OnDestroy();
    }

}
