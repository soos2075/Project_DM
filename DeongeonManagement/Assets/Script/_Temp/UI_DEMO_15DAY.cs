using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DEMO_15DAY : UI_PopUp
{
    void Start()
    {
        //Time.timeScale = 0;
        Init();
    }

    enum Objects
    {
        Time,
        ToMain,
        Fade,

        SteamLink,

        //ContinueButton,
    }



    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);

        Bind<GameObject>(typeof(Objects));

        SoundManager.Instance.StopMusic();
        UserData.Instance.FileConfig.PlayTimeApply();
        float playtime = UserData.Instance.FileConfig.PlayTimes;


        GetObject((int)Objects.Time).GetComponent<TMPro.TextMeshProUGUI>().text = $"PlayTime {(int)playtime / 60} : {(int)playtime % 60}";


        fade = GetObject((int)Objects.Fade).GetComponent<Image>();
        StartCoroutine(WaitDelay());

        GetObject((int)Objects.SteamLink).AddUIEvent(data => OpenWebPageButton());
    }


    public string webpageURL = "https://store.steampowered.com/app/2886090/Novice_Dungeon_Master/";
    public void OpenWebPageButton()
    {
        Application.OpenURL(webpageURL);
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

        //GetObject((int)Objects.ContinueButton).AddUIEvent((data) => ContinueGame());
    }


    public bool isClick;
    void LoadToMain()
    {
        if (isClick) return;

        isClick = true;
        Managers.UI.Stop_Reservation();
        Managers.Scene.LoadSceneAsync(SceneName._1_Start, false);
    }

    void ContinueGame()
    {
        ClosePopUp();
        SoundManager.Instance.ReStartMusic();
    }

}
