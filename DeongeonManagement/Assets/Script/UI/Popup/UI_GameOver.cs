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


    enum Btn
    {
        Btn_Main,
        Btn_Load,
    }

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        Bind<Button>(typeof(Btn));

        SoundManager.Instance.StopMusic();
        SoundManager.Instance.PlaySound("SFX/GameOver");

        //UserData.Instance.SetData(PrefsKey.GameOverTimes, UserData.Instance.GetDataInt(PrefsKey.GameOverTimes) + 1);
        UserData.Instance.CurrentPlayerData.config.GameOverCount++;

        StartCoroutine(GameOver());
    }



    IEnumerator GameOver()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(1);

        GetButton((int)Btn.Btn_Main).gameObject.AddUIEvent(data => ToMain());
        GetButton((int)Btn.Btn_Load).gameObject.AddUIEvent(data => Load());
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
