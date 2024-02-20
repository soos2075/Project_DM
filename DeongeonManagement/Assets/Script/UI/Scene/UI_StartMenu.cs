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
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));


        GetButton(((int)Buttons.NewGame)).gameObject.AddUIEvent(data => Button_NewGame());

        GetButton(((int)Buttons.Load)).gameObject.AddUIEvent(data => LoadGame());

        GetButton(((int)Buttons.Quit)).gameObject.AddUIEvent(data => Button_Quit());

        LoadButtonActive();
    }


    void Button_Quit()
    {
        Debug.Log("게임종료");
        Application.Quit();
    }


    void LoadButtonActive()
    {
        for (int i = 0; i < 6; i++)
        {
            if (Managers.Data.GetData($"DM_Save_{i}") != null)
            {
                Debug.Log($"Data exist : DM_Save_{i}");
                return;
            }
        }
        if (Managers.Data.GetData($"AutoSave") != null)
        {
            Debug.Log($"Data exist : AutoSave");
            return;
        }

        GetButton(((int)Buttons.Load)).gameObject.SetActive(false);
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

        var openingConfirm = Managers.UI.ShowPopUp<UI_Confirm>();
        openingConfirm.SetText("오프닝을 볼까요?");
        StartCoroutine(WaitForAnswer(openingConfirm));
    }

    IEnumerator WaitForAnswer(UI_Confirm confirm)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            Managers.Scene.AddLoadAction_OneTime(() => Opening());
            Managers.Scene.LoadSceneAsync(SceneName._4_Direction, false);
        }
        else if (confirm.GetAnswer() == UI_Confirm.State.No)
        {
            Managers.Scene.AddLoadAction_OneTime(() => NewGame_Action());
            Managers.Scene.LoadSceneAsync(SceneName._2_Management, false);
        }
    }


    void NewGame_Action()
    {
        Debug.Log($"새 게임 시작");
        Main.Instance.NewGame_Init();


        //Main.Instance.Test_Init();
    }
    void Opening()
    {
        Debug.Log($"오프닝 재생");
        Director_Story.Instance.StartScene_1();
    }



    void LoadGame()
    {
        var ui = Managers.UI.ShowPopUpAlone<UI_SaveLoad>();
        ui.SetMode(UI_SaveLoad.Buttons.Load);
    }

}
