using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_StartMenu : UI_Scene
{
    void Start()
    {
        Init();
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
        Collection,
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

        Init_CollectionButton();
    }


    void Init_CollectionButton()
    {
        //? 엔딩 도감 나중에 연결하면 됨


#if DEMO_BUILD
        GetButton((int)Buttons.Collection).gameObject.SetActive(false);
#endif
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


    void Button_NewGame()
    {
        StartCoroutine(OpeningSkip());

        UserData.Instance.SetData(PrefsKey.NewGameTimes, UserData.Instance.GetDataInt(PrefsKey.NewGameTimes) + 1);
    }

    IEnumerator OpeningSkip()
    {
        UserData.Instance.NewGameConfig();
        EventManager.Instance.NewGameReset();

        var fade = Managers.UI.ShowPopUpAlone<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.BlackOut, 2, false);

        yield return new WaitForSecondsRealtime(2);

        if (CollectionManager.Instance.RoundClearData != null)
        {
            yield return StartCoroutine(NewGamePlus());
        }




        //? 나중에 데모말고 다시 활성화 하면 댐
        //if (CollectionManager.Instance.RoundClearData != null)
        //{
        //    var dataConfirm = Managers.UI.ShowPopUp<UI_Confirm>();
        //    Debug.Log("로컬라이즈 해야댐 : 클리어 데이터");
        //    dataConfirm.SetText("클리어 데이터를 인계할까요?");// UserData.Instance.GetLocaleText("Confirm_Opening"));
        //    yield return StartCoroutine(WaitForAnswer_ClearData(dataConfirm));
        //}

        //Debug.Log("BIC 오프라인 데모버전 - 시작초기화지점");
        //var dataReset = Managers.UI.ShowPopUpAlone<UI_Confirm>();

        //string optionText = UserData.Instance.LocaleText("데이터초기화_First");
        //dataReset.SetText(optionText, () => DataReset_Action());

        //yield return new WaitUntil(() => dataReset == null);
        //yield return new WaitForSecondsRealtime(0.5f);

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




    void DataReset_Action()
    {
        // 플레이어 데이터 삭제
        PlayerPrefs.DeleteAll();
        // 클리어 데이터 삭제
        CollectionManager.Instance.RoundClearData = null;
        Managers.Data.DeleteSaveFile("ClearData");
        // 컬렉션 데이터 삭제
        Managers.Data.DeleteSaveFile("CollectionData");
        CollectionManager.Instance.DataResetAndNewGame();
        // 세이브 데이터 삭제
        Managers.Data.DeleteSaveFileAll();


    }

}
