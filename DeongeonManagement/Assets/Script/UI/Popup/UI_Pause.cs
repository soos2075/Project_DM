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
        //Load,
        StartScene,
        Quit,
        Language,
        DataReset,

        Manual,
    }

    void Init_Button()
    {
        Bind<Button>(typeof(Buttons));

        GetButton(((int)Buttons.Close)).gameObject.AddUIEvent((data) => ClosePopUp());
        //GetButton(((int)Buttons.Load)).gameObject.AddUIEvent((data) => ShowLoadUI());
        GetButton(((int)Buttons.StartScene)).gameObject.AddUIEvent((data) => GotoStartScene());
        GetButton(((int)Buttons.Quit)).gameObject.AddUIEvent((data) => QuitConfirm());
        GetButton(((int)Buttons.Language)).gameObject.AddUIEvent((data) => SetLanguage());
        GetButton(((int)Buttons.DataReset)).gameObject.AddUIEvent((data) => DataReset());

        GetButton(((int)Buttons.Manual)).gameObject.AddUIEvent((data) => Managers.UI.ShowPopUp<UI_Tutorial>());
    }


    void DataReset()
    {
        var ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
        //ui.SetText("모든 데이터를 초기화 할까요?(복구 불가능)");
        ui.SetText($"{UserData.Instance.LocaleText("데이터초기화")}", () => ResetAction());
    }

    void ResetAction()
    {
        // 플레이어 데이터 삭제
        PlayerPrefs.DeleteAll();


        // 클리어 데이터 삭제
        CollectionManager.Instance.RoundClearData = null;
        Managers.Data.DeleteSaveFile("ClearData");


        // 컬렉션 데이터 삭제
        Managers.Data.DeleteSaveFile("CollectionData");


        // 세이브 데이터 삭제
        Managers.Data.DeleteSaveFileAll();


        // 씬 리로드
        GotoStartScene();
    }


    void ShowLoadUI()
    {
        var ui = Managers.UI.ShowPopUpAlone<UI_SaveLoad>();
        ui.SetMode(UI_SaveLoad.Buttons.Load);
    }

    void GotoStartScene()
    {
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

        //? 초기값 설정
        GetObject(((int)Components.Dropdown_Resolution)).GetComponent<TMPro.TMP_Dropdown>().value = UserData.Instance.CurrentResolution;
        GetObject(((int)Components.Toggle_FullScreen)).GetComponent<Toggle>().isOn = UserData.Instance.FullScreen;

        GetObject(((int)Components.Slider_BGM)).GetComponent<Slider>().value = SoundManager.Instance.GetVolume(Define.AudioType.BGM);
        GetObject(((int)Components.Slider_SFX)).GetComponent<Slider>().value = SoundManager.Instance.GetVolume(Define.AudioType.Effect);
        GetObject(((int)Components.Slider_TextSpeed)).GetComponent<Slider>().value = Managers.Dialogue.CurrentTextSpeed;


        //? 값 바뀌면 Action
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
