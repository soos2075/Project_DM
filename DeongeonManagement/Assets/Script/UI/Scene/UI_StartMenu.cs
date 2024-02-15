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
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));


        GetButton(((int)Buttons.NewGame)).gameObject.AddUIEvent(data => Button_NewGame());

        GetButton(((int)Buttons.Load)).gameObject.AddUIEvent(data => LoadGame());
    }


    void Button_NewGame()
    {
        StartCoroutine(OpeningSkip());
    }

    IEnumerator OpeningSkip()
    {
        var fade = Managers.UI.ShowPopUpAlone<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.Out, 2, false);

        yield return new WaitForSecondsRealtime(2);

        var openingConfirm = Managers.UI.ShowPopUp<UI_Confirm>();
        openingConfirm.SetText("�������� �����?");
        StartCoroutine(WaitForAnswer(openingConfirm));
    }

    IEnumerator WaitForAnswer(UI_Confirm confirm)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            Managers.Scene.AddLoadAction_OneTime(() => Opening());
            Managers.Scene.LoadSceneAsync("4_Direction", false);
        }
        else if (confirm.GetAnswer() == UI_Confirm.State.No)
        {
            Managers.Scene.AddLoadAction_OneTime(() => NewGame_Action());
            Managers.Scene.LoadSceneAsync("2_Management", false);
        }
    }


    void NewGame_Action()
    {
        Debug.Log($"�� ���� ����");
        Main.Instance.NewGame_Init();
    }
    void Opening()
    {
        Debug.Log($"������ ���");
        Director_Story.Instance.StartScene_1();
    }



    void LoadGame()
    {
        var ui = Managers.UI.ShowPopUpAlone<UI_SaveLoad>();
        ui.SetMode(UI_SaveLoad.Buttons.Load);
    }

}
