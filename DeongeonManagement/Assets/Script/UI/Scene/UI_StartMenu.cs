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
        Elbum,
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
        GetButton((int)Buttons.Elbum).gameObject.SetActive(false);

        StartCoroutine(Add_ElbumBtn());
    }

    IEnumerator Add_ElbumBtn()
    {
        yield return null;

        if (CollectionManager.Instance.RoundClearData != null)
        {
            GetButton((int)Buttons.Elbum).gameObject.SetActive(true);
            GetButton(((int)Buttons.Elbum)).gameObject.AddUIEvent(data => Managers.UI.ShowPopUp<UI_Elbum>());
            Title_Clear();
        }
    }

    void Title_Clear()
    {
        var back = GameObject.Find("Background");

        if (CollectionManager.Instance.RoundClearData.EndingClearCheck(Endings.Dog))
        {
            back.transform.Find("Title_Dog").gameObject.SetActive(true);
        }
        if (CollectionManager.Instance.RoundClearData.EndingClearCheck(Endings.Dragon))
        {
            back.transform.Find("Title_Dragon").gameObject.SetActive(true);
        }
        if (CollectionManager.Instance.RoundClearData.EndingClearCheck(Endings.Ravi))
        {
            back.transform.Find("Title_Ravi").gameObject.SetActive(true);
        }
        if (CollectionManager.Instance.RoundClearData.EndingClearCheck(Endings.Cat))
        {
            back.transform.Find("Title_Heroine").gameObject.SetActive(true);
        }
        if (CollectionManager.Instance.RoundClearData.EndingClearCheck(Endings.Demon))
        {
            back.transform.Find("Title_Evil").gameObject.SetActive(true);
        }
        if (CollectionManager.Instance.RoundClearData.EndingClearCheck(Endings.Hero))
        {
            back.transform.Find("Title_Deer").gameObject.SetActive(true);
        }
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

        UserData.Instance.SetData(PrefsKey.NewGameTimes, UserData.Instance.GetDataInt(PrefsKey.NewGameTimes) + 1);
    }

    IEnumerator OpeningSkip()
    {
        UserData.Instance.NewGameConfig();
        EventManager.Instance.NewGameReset();
        GuildManager.Instance.NewGameReset();

        var fade = Managers.UI.ShowPopUpAlone<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.BlackOut, 2, false);

        yield return new WaitForSecondsRealtime(2);


        //? 2ȸ�� �̻� - �������÷��� �˾�
        if (CollectionManager.Instance.RoundClearData != null)
        {
            yield return StartCoroutine(NewGamePlus());
        }



        //? ���ο� ȸ���� �������ϰ� ���� ����� ������? �������ΰ���� �÷������� ����� �׸���.
        //if (CollectionManager.Instance.RoundClearData != null)
        //{
        //    var dataConfirm = Managers.UI.ShowPopUp<UI_Confirm>();
        //    Debug.Log("���ö����� �ؾߴ� : Ŭ���� ������");
        //    dataConfirm.SetText("Ŭ���� �����͸� �ΰ��ұ��?");// UserData.Instance.GetLocaleText("Confirm_Opening"));
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
        Debug.Log($"�� ���� ����");
        Main.Instance.NewGame_Init();

    }
    void Opening()
    {
        Debug.Log($"������ ���");
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
