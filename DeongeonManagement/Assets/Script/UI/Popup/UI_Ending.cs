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
    private void Update()
    {
        if (Input.anyKey)
        {
            Time.timeScale = 2.0f;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

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





        //#if DEMO_BUILD
        //        // 데모 빌드 전용 코드
        //        StartCoroutine(Clear_Demo());
        //#else
        //        // 일반 빌드 전용 코드
        //        StartCoroutine(ClearText());
        //#endif

        StartCoroutine(Clear_Demo());
    }


    IEnumerator Clear_Demo()
    {
        clear.text = "Demo Clear";
        yield return StartCoroutine(TextFadeIn(clear, 1));
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(TextFadeOut(clear, 1));

        clear.text = "Thanks for playing";
        yield return StartCoroutine(TextFadeIn(clear, 1));
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(TextFadeOut(clear, 1));

        clear.text = "Made by LazyCnD & yupi";
        yield return StartCoroutine(TextFadeIn(clear, 1));
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(TextFadeOut(clear, 1));

        clear.text = "Novice Dungeon Master";
        yield return StartCoroutine(TextFadeIn(clear, 1));
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(TextFadeOut(clear, 1));


        Managers.Scene.LoadSceneAsync(SceneName._1_Start);
        Managers.Scene.AddLoadAction_OneTime(() => DemoClearUI());
    }


    void DemoClearUI()
    {
        var ui = Managers.UI.ShowPopUp<UI_SystemMessage>();
        ui.Message = $"데모 클리어입니다.\n\n재밌게 즐겨주셨다면 스팀 위시리스트도 추가해주세요!";
    }




    IEnumerator ClearText()
    {
        clear.text = "Thanks for playing";
        yield return StartCoroutine(TextFadeIn(clear, 1));
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(TextFadeOut(clear, 1));


        clear.text = "Made by LazyCnD";
        yield return StartCoroutine(TextFadeIn(clear, 1));
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(TextFadeOut(clear, 1));

        clear.text = "Novice Dungeon Master";
        yield return StartCoroutine(TextFadeIn(clear, 1));
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(TextFadeOut(clear, 1));


        yield return StartCoroutine(SaveClearData());

        //Managers.Scene.LoadSceneAsync(SceneName._1_Start);
    }





    IEnumerator SaveClearData()
    {
        UserData.Instance.isClear = true;

        //? 여기서 클리어한 데이터를 몽땅 보여주거나 나한테 보내거니 아무튼 처리하면 될듯
        // 만약 아래 게임클리어에서 해도 되긴하는데 음..


        var save = Managers.UI.ShowPopUp<UI_SaveLoad>();
        save.SetMode(UI_SaveLoad.Buttons.Save);

        yield return new WaitUntil(() => save == null);

        // 오토세이브에 저장
        Managers.Data.SaveAndAddFile(EventManager.Instance.Temp_saveData, "AutoSave", 0);
        var autosaveData = Managers.Data.GetData($"AutoSave");


        EventManager.Instance.Temp_saveData = null;

        switch (UserData.Instance.EndingState)
        {
            case Endings.Dog:
                UserData.Instance.SetData(PrefsKey.Clear_Dog, 1);
                break;

            case Endings.Dragon:
                UserData.Instance.SetData(PrefsKey.Clear_Dragon, 1);
                break;
        }


        UserData.Instance.GameClear(autosaveData);
    }




    IEnumerator RecordText()
    {
        //? 받아온 데이터 나열

        yield return new WaitForSeconds(1);
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
