using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Pause : UI_PopUp
{
    void Start()
    {
        UserData.Instance.SavePlayTime();
        Init();
    }



    public override void Init()
    {
        base.Init();
        Init_Button();
        Init_Components();
    }




    #region Button
    enum Buttons
    {
        Close,
        StartScene,
        Quit,
        Language,
        DataReset,

        Manual,

        Licenses,
        Licenses_Close,
    }

    enum Images
    {
        Licenses_Panel,
    }

    void Init_Button()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));


        GetButton(((int)Buttons.Close)).gameObject.AddUIEvent((data) => ClosePopUp());
        GetButton(((int)Buttons.StartScene)).gameObject.AddUIEvent((data) => GotoStartScene());
        GetButton(((int)Buttons.Quit)).gameObject.AddUIEvent((data) => QuitConfirm());
        GetButton(((int)Buttons.Language)).gameObject.AddUIEvent((data) => SetLanguage());
        GetButton(((int)Buttons.DataReset)).gameObject.AddUIEvent((data) => DataReset());

        GetButton(((int)Buttons.Manual)).gameObject.AddUIEvent((data) => Managers.UI.ShowPopUp<UI_Manual>());

        GetButton(((int)Buttons.Licenses)).gameObject.AddUIEvent((data) => GetImage(((int)Images.Licenses_Panel)).gameObject.SetActive(true));
        GetButton(((int)Buttons.Licenses_Close)).gameObject.AddUIEvent((data) => GetImage(((int)Images.Licenses_Panel)).gameObject.SetActive(false));
    }




    void DataReset()
    {
        var ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
        ui.SetText($"{UserData.Instance.LocaleText("�ʱ�ȭ_0_����")}", () => StartCoroutine(Delete_Confirm()));
    }

    IEnumerator Delete_Confirm()
    {
        UI_Confirm ui = null;
        Action DeleteAction = null;
        yield return new WaitForSecondsRealtime(0.5f);

        ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
        ui.SetText($"{UserData.Instance.LocaleText("�ʱ�ȭ_1_�����")}", () => DeleteAction += Delete_PlayerData);

        yield return new WaitUntil(() => ui == null);
        yield return new WaitForSecondsRealtime(0.5f);

        ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
        ui.SetText($"{UserData.Instance.LocaleText("�ʱ�ȭ_2_���̺�")}", () => DeleteAction += Delete_SaveData);

        yield return new WaitUntil(() => ui == null);
        yield return new WaitForSecondsRealtime(0.5f);

        ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
        ui.SetText($"{UserData.Instance.LocaleText("�ʱ�ȭ_3_Ŭ����")}", () => DeleteAction += Delete_ClearData);

        yield return new WaitUntil(() => ui == null);
        yield return new WaitForSecondsRealtime(0.5f);

        ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
        ui.SetText($"{UserData.Instance.LocaleText("�ʱ�ȭ_4_����")}", () => DeleteAction += Delete_CollectionData);


        yield return new WaitUntil(() => ui == null);
        yield return new WaitForSecondsRealtime(1.0f);


        ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
        ui.SetText($"{UserData.Instance.LocaleText("�ʱ�ȭ_5_����")}", () =>
        {
            DeleteAction.Invoke();
            GotoStartScene();
        });

    }

    void Delete_PlayerData()
    {
        // �÷��̾� ������ ����
        PlayerPrefs.DeleteAll();
    }
    void Delete_SaveData()
    {
        // ���̺� ������ ����
        Managers.Data.DeleteSaveFileAll();
    }

    void Delete_ClearData()
    {
        // Ŭ���� ������ ����
        CollectionManager.Instance.RoundClearData = null;
        Managers.Data.DeleteSaveFile("ClearData");
    }

    void Delete_CollectionData()
    {
        // �÷��� ������ ����
        Managers.Data.DeleteSaveFile("CollectionData");
    }



    void GotoStartScene()
    {
        // �� ���ε�
        Managers.Scene.LoadSceneAsync(SceneName._1_Start);
    }
    void SetLanguage()
    {
        var ui = Managers.UI.ShowPopUpAlone<UI_Language>();
    }
    void QuitConfirm()
    {
        var ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
        ui.SetText(UserData.Instance.LocaleText("Confirm_Quit"), () => Application.Quit());
    }




    #endregion


    enum Components
    {
        Slider_BGM,
        Slider_SFX,
        Slider_TextSpeed,
        Dropdown_Resolution,
        Toggle_FullScreen,

    }
    void Init_Components()
    {
        Bind<GameObject>(typeof(Components));

        //? �ʱⰪ ����
        GetObject(((int)Components.Dropdown_Resolution)).GetComponent<TMPro.TMP_Dropdown>().value = UserData.Instance.CurrentResolution;
        GetObject(((int)Components.Toggle_FullScreen)).GetComponent<Toggle>().isOn = UserData.Instance.FullScreen;

        GetObject(((int)Components.Slider_BGM)).GetComponent<Slider>().value = SoundManager.Instance.GetVolume(Define.AudioType.BGM);
        GetObject(((int)Components.Slider_SFX)).GetComponent<Slider>().value = SoundManager.Instance.GetVolume(Define.AudioType.Effect);
        GetObject(((int)Components.Slider_TextSpeed)).GetComponent<Slider>().value = Managers.Dialogue.CurrentTextSpeed;


        //? �� �ٲ�� Action
        GetObject(((int)Components.Slider_BGM)).GetComponent<Slider>().onValueChanged.AddListener(OnChanged_BGM);
        GetObject(((int)Components.Slider_SFX)).GetComponent<Slider>().onValueChanged.AddListener(OnChanged_SFX);
        GetObject(((int)Components.Slider_TextSpeed)).GetComponent<Slider>().onValueChanged.AddListener(OnChanged_TextSpeed);
        GetObject(((int)Components.Dropdown_Resolution)).GetComponent<TMPro.TMP_Dropdown>().onValueChanged.AddListener(OnChanged_Resolution);
        GetObject(((int)Components.Toggle_FullScreen)).GetComponent<Toggle>().onValueChanged.AddListener(OnChanged_FullScreen);
    }

    void OnChanged_BGM(float _volume)
    {
        //Debug.Log(_volume + "bgm");
        SoundManager.Instance.SetVolume(Define.AudioType.BGM, _volume);

    }
    void OnChanged_SFX(float _volume)
    {
        //Debug.Log(_volume + "sfx");
        SoundManager.Instance.SetVolume(Define.AudioType.Effect, _volume);
    }
    void OnChanged_TextSpeed(float _value)
    {
        //Debug.Log(_value);
        Managers.Dialogue.CurrentTextSpeed = (int)_value;
    }


    void OnChanged_Resolution(int _option)
    {
        //Debug.Log(GetObject(((int)Components.Dropdown_Resolution)).GetComponent<TMPro.TMP_Dropdown>().options[_option].text);
        //Debug.Log(_option);
        UserData.Instance.CurrentResolution = _option;
    }
    void OnChanged_FullScreen(bool _value)
    {
        //Debug.Log(_value);
        UserData.Instance.FullScreen = _value;
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
