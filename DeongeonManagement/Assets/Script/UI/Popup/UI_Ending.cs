using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Ending : UI_PopUp
{
    void Start()
    {
        Time.timeScale = 1;
        Init();
    }
    //private void Update()
    //{
    //    if (Input.anyKey)
    //    {
    //        Time.timeScale = 2.0f;
    //    }
    //    else
    //    {
    //        Time.timeScale = 1;
    //    }
    //}

    enum Background
    {
        BG,
    }
    enum TMP
    {
        Text_Clear,
        Text_Credit,
        Text_Record,
    }

    TextMeshProUGUI clear;
    TextMeshProUGUI credit;
    TextMeshProUGUI record;
    Image bg;

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(TMP));
        Bind<Image>(typeof(Background));

        clear = GetTMP((int)TMP.Text_Clear);
        credit = GetTMP((int)TMP.Text_Credit);
        record = GetTMP((int)TMP.Text_Record);
        bg = GetImage(((int)Background.BG));


        clear.text = "";
        credit.text = "";
        record.text = "";


        StartCoroutine(FadeBG());
    }



    IEnumerator FadeBG()
    {
        Color color = new Color32(255, 255, 255, 0);
        bg.color = color;

        float timer = 0;
        while (timer < 1)
        {
            yield return null;
            timer += Time.deltaTime;

            bg.color = Color.Lerp(color, Color.white, timer);
        }

        bg.color = Color.white;
        color = new Color32(70, 70, 70, 255);


        yield return new WaitForSeconds(1);

        timer = 0;
        while (timer < 1)
        {
            yield return null;
            timer += Time.deltaTime;

            bg.color = Color.Lerp(Color.white, color, timer);
        }
        bg.color = color;


        //? 메인으로 돌아가기
        Managers.Scene.LoadSceneAsync(SceneName._1_Start);

        if (UserData.Instance.CurrentPlayerData.GetClearCount() == 1)
        {
            Managers.Scene.AddLoadAction_OneTime(() => NGP_Info());
        }

        //? 만약 모든 엔딩을 다 봤다면 엔딩크레딧 연결하자
        if (UserData.Instance.CurrentPlayerData.EndingClearNumber() == System.Enum.GetNames(typeof(Endings)).Length)
        {
            Managers.Scene.AddLoadAction_OneTime(() => Managers.UI.ShowPopUp<UI_Credit>());
        }


        //if (CollectionManager.Instance.RoundClearData != null)
        //{
        //    var data = CollectionManager.Instance.RoundClearData;
        //    if (data.EndingClearCheck(Endings.Dog) && data.EndingClearCheck(Endings.Cat) && data.EndingClearCheck(Endings.Dragon) &&
        //        data.EndingClearCheck(Endings.Demon) && data.EndingClearCheck(Endings.Hero) && data.EndingClearCheck(Endings.Ravi))
        //    {
        //        Managers.Scene.AddLoadAction_OneTime(() => Managers.UI.ShowPopUp<UI_Credit>());
        //    }
        //}
        //StartCoroutine(Clear_Demo());


        //? 임시 테스트
        //StartCoroutine(SaveClearData());
    }

    void NGP_Info()
    {
        var ui = Managers.UI.ShowPopUp<UI_SystemMessage>();
        ui.Message = $"{UserData.Instance.LocaleText_NGP("First_Clear_MSG")}";
    }





    IEnumerator Clear_Demo()
    {
        clear.text = "Novice Dungeon Master\n\n" +
            "Demo Clear!";
        yield return StartCoroutine(TextFadeIn(clear, 1));
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(TextFadeOut(clear, 1));

        clear.text = "Made by LazyCnD \n\n" +
            "Special thanks\n" +
            "\nQU4RTER" +
            "\nyupi" +
            "\nREDRAY" +
            "\n다피네";
        yield return StartCoroutine(TextFadeIn(clear, 1));
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(TextFadeOut(clear, 1));

        clear.text = "And You\n\n\nThanks for playing!!";
        yield return StartCoroutine(TextFadeIn(clear, 1));
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(TextFadeOut(clear, 1));



        Managers.Scene.LoadSceneAsync(SceneName._1_Start);
        Managers.Scene.AddLoadAction_OneTime(() => DemoClearUI());
    }


    void DemoClearUI()
    {
        var ui = Managers.UI.ShowPopUp<UI_SystemMessage>();
        ui.Message = $"데모 클리어입니다.\n\n재밌게 즐겨주셨다면 스팀 찜하기를 눌러주세요!" +
            $"\n본편에서는 더 많은 이벤트와 엔딩이 기다리고 있습니다!";
    }





    IEnumerator SaveClearData()
    {
        var save = Managers.UI.ShowPopUp<UI_SaveLoad>();
        save.SetMode(UI_SaveLoad.DataState.Save);

        yield return new WaitUntil(() => save == null);

        // 오토세이브에 저장
        Managers.Data.SaveAndAddFile(EventManager.Instance.Temp_saveData, "AutoSave", 0);
        var autosaveData = Managers.Data.GetData($"AutoSave");

        EventManager.Instance.Temp_saveData = null;

    }




    IEnumerator TextFadeIn(TextMeshProUGUI _target, float _duration)
    {
        Color tempColor = Color.white;
        tempColor.a = 0;
        _target.color = tempColor;

        float timer = 0;
        while (timer < _duration)
        {
            yield return null;
            timer += Time.deltaTime;

            tempColor.a += Time.deltaTime / _duration;
            _target.color = tempColor;
        }

        _target.color = Color.white;
    }

    IEnumerator TextFadeOut(TextMeshProUGUI _target, float _duration)
    {
        Color tempColor = Color.white;
        _target.color = tempColor;

        float timer = 0;
        while (timer < _duration)
        {
            yield return null;
            timer += Time.deltaTime;

            tempColor.a -= Time.deltaTime / _duration;
            _target.color = tempColor;
        }

        _target.color = Color.clear;
    }
}
