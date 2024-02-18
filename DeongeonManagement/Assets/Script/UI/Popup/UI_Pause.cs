using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Pause : UI_PopUp
{
    void Start()
    {
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
        Load,
        StartScene,
        Quit,
    }

    void Init_Button()
    {
        Bind<Button>(typeof(Buttons));

        GetButton(((int)Buttons.Close)).gameObject.AddUIEvent((data) => ClosePopUp());
        GetButton(((int)Buttons.Load)).gameObject.AddUIEvent((data) => ShowLoadUI());
        GetButton(((int)Buttons.StartScene)).gameObject.AddUIEvent((data) => GotoStartScene());
        GetButton(((int)Buttons.Quit)).gameObject.AddUIEvent((data) => QuitConfirm());
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
    void QuitConfirm()
    {
        var ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
        ui.SetText("게임을 종료할까요?");
        StartCoroutine(WaitForAnswer(ui));
    }

    IEnumerator WaitForAnswer(UI_Confirm confirm)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            Debug.Log("게임종료");
            Application.Quit();
        }
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



    private void OnEnable()
    {
        Time.timeScale = 0;
    }
    private void OnDestroy()
    {
        Time.timeScale = 1;
    }
}
