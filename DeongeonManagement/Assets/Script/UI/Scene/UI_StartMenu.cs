using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_StartMenu : UI_Scene
{
    void Start()
    {
        Init();
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
        //? 조건 1 = 한번이라도 클리어했는지 -> UserData.Instance.GetDataInt(PrefsKey.FirstClear, 0) == 1
        //? 조건 2 = 걍 세이브 데이터가 있는지

        if (UserData.Instance.GetDataInt(PrefsKey.FirstClear, 0) == 1)
        {
            GetButton((int)Buttons.Collection).gameObject.SetActive(true);
            GetButton((int)Buttons.Collection).gameObject.AddUIEvent(data => Managers.UI.ShowPopUp<UI_Collection>());
        }
        else
        {
            GetButton((int)Buttons.Collection).gameObject.SetActive(false);
        }


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
    }

    IEnumerator OpeningSkip()
    {
        var fade = Managers.UI.ShowPopUpAlone<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.BlackOut, 2, false);

        yield return new WaitForSecondsRealtime(2);

        if (CollectionManager.Instance.PlayData != null)
        {
            var dataConfirm = Managers.UI.ShowPopUp<UI_Confirm>();
            Debug.Log("로컬라이즈 해야댐 : 클리어 데이터");
            dataConfirm.SetText("클리어 데이터를 인계할까요?");// UserData.Instance.GetLocaleText("Confirm_Opening"));
            yield return StartCoroutine(WaitForAnswer_ClearData(dataConfirm));
        }


        var openingConfirm = Managers.UI.ShowPopUp<UI_Confirm>();
        openingConfirm.SetText(UserData.Instance.GetLocaleText("Confirm_Opening"));
        StartCoroutine(WaitForAnswer(openingConfirm));
    }

    IEnumerator WaitForAnswer(UI_Confirm confirm)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            Managers.Scene.AddLoadAction_OneTime(() => Opening());
            Managers.Scene.LoadSceneAsync(SceneName._6_NewOpening, false);
        }
        else if (confirm.GetAnswer() == UI_Confirm.State.No)
        {
            Managers.Scene.AddLoadAction_OneTime(() => NewGame_Action());
            Managers.Scene.LoadSceneAsync(SceneName._2_Management, false);
        }
    }


    IEnumerator WaitForAnswer_ClearData(UI_Confirm confirm)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            CollectionManager.Instance.PlayData.dataApply = true;
            confirm.ClosePopUp();
        }
        else if (confirm.GetAnswer() == UI_Confirm.State.No)
        {
            CollectionManager.Instance.PlayData.dataApply = false;
            confirm.ClosePopUp();
        }
    }




    void NewGame_Action()
    {
        Debug.Log($"새 게임 시작");
        Main.Instance.NewGame_Init();
        UserData.Instance.NewGameConfig();

        //Main.Instance.Test_Init();
    }
    void Opening()
    {
        Debug.Log($"오프닝 재생");
        //Director_Story.Instance.StartScene_1();
    }



    void LoadGame()
    {
        var ui = Managers.UI.ShowPopUpAlone<UI_SaveLoad>();
        ui.SetMode(UI_SaveLoad.Buttons.Load);
    }

}
