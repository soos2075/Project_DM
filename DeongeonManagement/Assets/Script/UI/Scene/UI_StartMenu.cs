using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_StartMenu : UI_Scene
{
    void Start()
    {
        Init();
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (NewGameCor != null)
            {
                StopCoroutine(NewGameCor);

                Managers.UI.ClosePopupPickType(typeof(UI_Fade));
                Managers.UI.ClosePopupPickType(typeof(UI_NewGamePlus));
                Managers.UI.ClosePopupPickType(typeof(UI_Confirm));
            }
        }
    }

    enum TMP_Texts
    {
        VersionText,
    }
    enum Buttons
    {
        NewGame,
        Load,
        Quit,
        Pause,
        Elbum,
        Credit,
    }

    public override void Init()
    {
        base.Init();

        Bind<TMPro.TextMeshProUGUI>(typeof(TMP_Texts));
        GetTMP((int)TMP_Texts.VersionText).text = $"v_{Application.version}";

        Bind<Button>(typeof(Buttons));

        GetButton(((int)Buttons.NewGame)).gameObject.AddUIEvent(data => Button_NewGame());
        GetButton(((int)Buttons.Load)).gameObject.AddUIEvent(data => LoadGame());
        GetButton(((int)Buttons.Quit)).gameObject.AddUIEvent(data => Button_Quit());
        GetButton((int)Buttons.Pause).gameObject.AddUIEvent((data) => Managers.UI.ShowPopUp<UI_Pause>());

        LoadButtonActive();

        StartCoroutine(WaitFrame());
    }


    IEnumerator WaitFrame()
    {
        yield return null;
        Init_CollectionButton();
    }


    void Init_CollectionButton()
    {
        GetButton((int)Buttons.Elbum).gameObject.SetActive(false);
        GetButton((int)Buttons.Credit).gameObject.SetActive(false);

        StartCoroutine(Add_ElbumBtn());


        if (UserData.Instance.CurrentPlayerData.EndingClearNumber() == System.Enum.GetNames(typeof(Endings)).Length)
        {
            GetButton((int)Buttons.Credit).gameObject.SetActive(true);
            GetButton(((int)Buttons.Credit)).gameObject.AddUIEvent(data => Managers.UI.ShowPopUp<UI_Credit>());
        }

        //if (CollectionManager.Instance.RoundClearData != null)
        //{
        //    var data = CollectionManager.Instance.RoundClearData;
        //    if (data.EndingClearCheck(Endings.Dog) && data.EndingClearCheck(Endings.Cat) && data.EndingClearCheck(Endings.Dragon) &&
        //        data.EndingClearCheck(Endings.Demon) && data.EndingClearCheck(Endings.Hero) && data.EndingClearCheck(Endings.Ravi))
        //    {
        //        GetButton((int)Buttons.Credit).gameObject.SetActive(true);
        //        GetButton(((int)Buttons.Credit)).gameObject.AddUIEvent(data => Managers.UI.ShowPopUp<UI_Credit>());
        //    }
        //}
    }

    IEnumerator Add_ElbumBtn()
    {
        yield return null;


        if (UserData.Instance.CurrentPlayerData.GetClearCount() > 0)
        {
            GetButton((int)Buttons.Elbum).gameObject.SetActive(true);
            GetButton(((int)Buttons.Elbum)).gameObject.AddUIEvent(data => Managers.UI.ShowPopUp<UI_Elbum>());
            Title_Clear();
        }

        //if (CollectionManager.Instance.RoundClearData != null)
        //{
        //    GetButton((int)Buttons.Elbum).gameObject.SetActive(true);
        //    GetButton(((int)Buttons.Elbum)).gameObject.AddUIEvent(data => Managers.UI.ShowPopUp<UI_Elbum>());
        //    Title_Clear();
        //}
    }

    void Title_Clear()
    {
        var back = GameObject.Find("Background");

        if (UserData.Instance.CurrentPlayerData.EndingClearCheck(Endings.Dog))
        {
            back.transform.Find("Title_Dog").gameObject.SetActive(true);
        }
        if (UserData.Instance.CurrentPlayerData.EndingClearCheck(Endings.Dragon))
        {
            back.transform.Find("Title_Dragon").gameObject.SetActive(true);
        }
        if (UserData.Instance.CurrentPlayerData.EndingClearCheck(Endings.Ravi))
        {
            back.transform.Find("Title_Ravi").gameObject.SetActive(true);
        }
        if (UserData.Instance.CurrentPlayerData.EndingClearCheck(Endings.Cat))
        {
            back.transform.Find("Title_Heroine").gameObject.SetActive(true);
        }
        if (UserData.Instance.CurrentPlayerData.EndingClearCheck(Endings.Demon))
        {
            back.transform.Find("Title_Evil").gameObject.SetActive(true);
        }
        if (UserData.Instance.CurrentPlayerData.EndingClearCheck(Endings.Hero))
        {
            back.transform.Find("Title_Deer").gameObject.SetActive(true);
        }
    }



    void Button_Quit()
    {
        //Managers.Data.SaveCollectionData();

        Debug.Log("게임종료");
        Application.Quit();
    }



    void LoadButtonActive()
    {
        if (Managers.Data.SaveFileExistCheck() == false)
        {
            GetButton(((int)Buttons.Load)).gameObject.SetActive(false);
        }
    }


    Coroutine NewGameCor;

    void Button_NewGame()
    {
        NewGameCor = StartCoroutine(OpeningSkip());

        //UserData.Instance.SetData(PrefsKey.NewGameTimes, UserData.Instance.GetDataInt(PrefsKey.NewGameTimes) + 1);
        UserData.Instance.CurrentPlayerData.config.NewGameCount++;
    }

    IEnumerator OpeningSkip()
    {
        UserData.Instance.NewGameConfig();
        EventManager.Instance.NewGameReset();
        GuildManager.Instance.NewGameReset();
        JournalManager.Instance.NewGame_Init();

        var fade = Managers.UI.ShowPopUpAlone<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.BlackOut, 2, false);

        yield return new WaitForSecondsRealtime(2);


        //? 2회차 이상 - 뉴게임플러스 팝업
        if (UserData.Instance.CurrentPlayerData.GetClearCount() > 0)
        {
            yield return StartCoroutine(NewGamePlus());
        }
        //if (CollectionManager.Instance.RoundClearData != null)
        //{
        //    yield return StartCoroutine(NewGamePlus());
        //}



        //? 새로운 회차에 새시작하고 싶은 사람이 있을까? 데이터인계없이 플레이할지 물어보는 항목임.
        //if (CollectionManager.Instance.RoundClearData != null)
        //{
        //    var dataConfirm = Managers.UI.ShowPopUp<UI_Confirm>();
        //    Debug.Log("로컬라이즈 해야댐 : 클리어 데이터");
        //    dataConfirm.SetText("클리어 데이터를 인계할까요?");// UserData.Instance.GetLocaleText("Confirm_Opening"));
        //    yield return StartCoroutine(WaitForAnswer_ClearData(dataConfirm));
        //}


        var openingConfirm = Managers.UI.ShowPopUp<UI_Confirm>();
        openingConfirm.SetText(UserData.Instance.LocaleText("Confirm_Opening"),
            () => Go_Opening(), 
            () => Go_Game());
    }




    IEnumerator NewGamePlus()
    {
        var ui = Managers.UI.ShowPopUp<UI_NewGamePlus>();

        yield return new WaitUntil(() => ui == null);

        yield return new WaitForSecondsRealtime(1);
    }





    void Go_Opening()
    {
        Managers.Scene.AddLoadAction_OneTime(() => Opening());
        Managers.Scene.LoadSceneAsync(SceneName._6_NewOpening, false);
    }
    void Go_Game()
    {
        Managers.Scene.AddLoadAction_OneTime(() => NewGame_Action());
        Managers.Scene.LoadSceneAsync(SceneName._2_Management, false);
    }





    void NewGame_Action()
    {
        Debug.Log($"새 게임 시작");
        Main.Instance.NewGame_Init();

    }
    void Opening()
    {
        Debug.Log($"오프닝 재생");
    }



    void LoadGame()
    {
        var ui = Managers.UI.ShowPopUpAlone<UI_SaveLoad>();
        ui.SetMode(UI_SaveLoad.DataState.Load);
    }




    //void DataReset_Action()
    //{
    //    // 플레이어 데이터 삭제
    //    PlayerPrefs.DeleteAll();
    //    // 클리어 데이터 삭제
    //    CollectionManager.Instance.RoundClearData = null;
    //    Managers.Data.DeleteSaveFile("ClearData");
    //    // 컬렉션 데이터 삭제
    //    Managers.Data.DeleteSaveFile("CollectionData");
    //    CollectionManager.Instance.DataResetAndNewGame();
    //    // 세이브 데이터 삭제
    //    Managers.Data.DeleteSaveFileAll();


    //}

}
