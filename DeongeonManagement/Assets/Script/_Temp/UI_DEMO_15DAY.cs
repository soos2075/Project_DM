using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DEMO_15DAY : UI_Base
{
    void Start()
    {
        Init();
    }

    enum Objects
    {
        Time,
        ToMain,
        Fade,
    }



    public override void Init()
    {
        Bind<GameObject>(typeof(Objects));

        SoundManager.Instance.StopMusic();
        UserData.Instance.FileConfig.PlayTimeApply();
        float playtime = UserData.Instance.FileConfig.PlayTimes;


        GetObject((int)Objects.Time).GetComponent<TMPro.TextMeshProUGUI>().text = $"PlayTime {(int)playtime / 60} : {(int)playtime % 60}";


        fade = GetObject((int)Objects.Fade).GetComponent<Image>();
        StartCoroutine(WaitDelay());
    }


    Image fade;


    IEnumerator WaitDelay()
    {
        float time = 2;
        float timer = 0;
        Color color = Color.white;

        while (timer < time)
        {
            color.a -= (Time.unscaledDeltaTime / time);
            fade.color = color;
            yield return null;
            timer += Time.unscaledDeltaTime;
        }

        GetObject((int)Objects.ToMain).AddUIEvent((data) => LoadToMain());
    }


    public bool isClick;
    void LoadToMain()
    {
        if (isClick) return;

        isClick = true;
        Managers.Scene.LoadSceneAsync(SceneName._1_Start, false);
    }

}
