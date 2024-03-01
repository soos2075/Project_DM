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
        SoundManager.Instance.PlaySound("SFX/GameOver");
        SoundManager.Instance.StopMusic();
    }



    IEnumerator GameOver()
    {
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(3);

        Managers.Scene.LoadSceneAsync(SceneName._1_Start);
        //Time.timeScale = 1;

    }
}
