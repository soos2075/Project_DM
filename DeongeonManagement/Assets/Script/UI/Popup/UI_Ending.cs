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
            Time.timeScale = 1.5f;
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

        StartCoroutine(ClearText());

    }



    IEnumerator ClearText()
    {

        clear.text = "Ending(Demo)";
        yield return StartCoroutine(TextFadeIn(clear, 1));
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(TextFadeOut(clear, 1));

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


        //yield return StartCoroutine(RecordText());


        Managers.Scene.LoadSceneAsync(SceneName._1_Start);
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
