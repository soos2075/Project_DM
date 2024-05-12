using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GameOver : UI_PopUp
{
    void Start()
    {
        Init();
    }

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        StartCoroutine(GameOver());
        SoundManager.Instance.StopMusic();
        SoundManager.Instance.PlaySound("SFX/GameOver");

        UserData.Instance.SetData(PrefsKey.GameOverTimes, UserData.Instance.GetDataInt(PrefsKey.GameOverTimes) + 1);
    }



    IEnumerator GameOver()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(3);
        Managers.Scene.LoadSceneAsync(SceneName._1_Start);
    }
}
