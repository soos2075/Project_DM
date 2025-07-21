using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameOver : UI_PopUp
{
    void Start()
    {
        Init();
    }

    [Header("텍스트 이미지")]
    public Sprite txt_gameOver;
    public Sprite txt_victory;
    public Sprite txt_result;

    [Header("메인 이미지")]
    public Sprite main_defeat;
    public Sprite main_win30;
    public Sprite main_win50;
    public Sprite main_win100;


    enum Btn
    {
        Btn_Result,
        Btn_Main,
        Btn_Load,
    }

    enum Images
    {
        Image_Text,
        Image_Main,
    }

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        Bind<Button>(typeof(Btn));
        Bind<Image>(typeof(Images));

        SoundManager.Instance.StopMusic();
        SoundManager.Instance.PlaySound("SFX/GameOver");

        //UserData.Instance.SetData(PrefsKey.GameOverTimes, UserData.Instance.GetDataInt(PrefsKey.GameOverTimes) + 1);
        UserData.Instance.CurrentPlayerData.config.GameOverCount++;
        UserData.Instance.GameOver(Managers.Data.SaveCurrentData("Result_Temp", allow_day: true));

        StartCoroutine(GameOver());

        UserData.Instance.FileConfig.PlayTimeApply();
        CurrentImage();
    }


    void CurrentImage()
    {
        int turn = Main.Instance.Turn;

        if (turn <= 30)
        {
            GetImage((int)Images.Image_Main).sprite = main_defeat;
            GetImage((int)Images.Image_Text).sprite = txt_gameOver;
        }
        else if (turn > 30 && turn <= 50)
        {
            GetImage((int)Images.Image_Main).sprite = main_win30;
            GetImage((int)Images.Image_Text).sprite = txt_result;
        }
        else if (turn > 50 && turn <= 100)
        {
            GetImage((int)Images.Image_Main).sprite = main_win50;
            GetImage((int)Images.Image_Text).sprite = txt_result;
        }
        else if (turn > 100)
        {
            GetImage((int)Images.Image_Main).sprite = main_win100;
            GetImage((int)Images.Image_Text).sprite = txt_result;
        }
    }




    IEnumerator GameOver()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(1);

        GetButton((int)Btn.Btn_Result).gameObject.AddUIEvent(data => ShowResult());
        GetButton((int)Btn.Btn_Main).gameObject.AddUIEvent(data => ToMain());
        GetButton((int)Btn.Btn_Load).gameObject.AddUIEvent(data => Load());
    }


    void ShowResult()
    {
        Managers.UI.ShowPopUp<UI_FinalResult>();
    }


    bool OnceCheck;
    void ToMain()
    {
        if (OnceCheck) return;

        OnceCheck = true;
        Managers.Scene.LoadSceneAsync(SceneName._1_Start);
    }

    void Load()
    {
        var save = Managers.UI.ShowPopUp<UI_SaveLoad>();
        save.SetMode(UI_SaveLoad.DataState.Load);
    }
}
