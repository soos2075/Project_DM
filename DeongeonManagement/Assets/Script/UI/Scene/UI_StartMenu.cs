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
        //? ���� 1 = �ѹ��̶� Ŭ�����ߴ��� -> UserData.Instance.GetDataInt(PrefsKey.FirstClear, 0) == 1
        //? ���� 2 = �� ���̺� �����Ͱ� �ִ���

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

        Debug.Log("��������");
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

        //? ���߿� ���𸻰� �ٽ� Ȱ��ȭ �ϸ� ��
        //if (CollectionManager.Instance.RoundClearData != null)
        //{
        //    var dataConfirm = Managers.UI.ShowPopUp<UI_Confirm>();
        //    Debug.Log("���ö����� �ؾߴ� : Ŭ���� ������");
        //    dataConfirm.SetText("Ŭ���� �����͸� �ΰ��ұ��?");// UserData.Instance.GetLocaleText("Confirm_Opening"));
        //    yield return StartCoroutine(WaitForAnswer_ClearData(dataConfirm));
        //}

        //Debug.Log("BIC �������� ������� - �����ʱ�ȭ����");
        //var dataReset = Managers.UI.ShowPopUpAlone<UI_Confirm>();

        //string optionText = UserData.Instance.LocaleText("�������ʱ�ȭ_First");
        //dataReset.SetText(optionText, () => DataReset_Action());

        //yield return new WaitUntil(() => dataReset == null);
        //yield return new WaitForSecondsRealtime(0.5f);

        var openingConfirm = Managers.UI.ShowPopUp<UI_Confirm>();
        openingConfirm.SetText(UserData.Instance.LocaleText("Confirm_Opening"),
            () => Go_Opening(), 
            () => Go_Game());
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
        Debug.Log($"�� ���� ����");
        Main.Instance.NewGame_Init();


        //Main.Instance.Test_Init();
    }
    void Opening()
    {
        Debug.Log($"������ ���");
        //Director_Story.Instance.StartScene_1();
    }



    void LoadGame()
    {
        var ui = Managers.UI.ShowPopUpAlone<UI_SaveLoad>();
        ui.SetMode(UI_SaveLoad.DataState.Load);
    }




    void DataReset_Action()
    {
        // �÷��̾� ������ ����
        PlayerPrefs.DeleteAll();
        // Ŭ���� ������ ����
        CollectionManager.Instance.RoundClearData = null;
        Managers.Data.DeleteSaveFile("ClearData");
        // �÷��� ������ ����
        Managers.Data.DeleteSaveFile("CollectionData");
        CollectionManager.Instance.DataResetAndNewGame();
        // ���̺� ������ ����
        Managers.Data.DeleteSaveFileAll();


    }

}
