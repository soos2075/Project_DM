using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_QuestBox : UI_Base
{
    void Start()
    {
        Init();
    }

    public override void Init()
    {
        Bind<TextMeshProUGUI>(typeof(TMP));
        rect = GetComponent<RectTransform>();
    }

    enum TMP
    {
        Title,
        Detali,
        Day,
    }

    //public void SetText(string _title, string _main, int _day)
    //{
    //    StartCoroutine(WaitFrame(_title, _main, _day));
    //}

    //IEnumerator WaitFrame(string _title, string _main, int _day)
    //{
    //    yield return new WaitForEndOfFrame();
    //    GetTMP(((int)TMP.Title)).text = _title;
    //    GetTMP(((int)TMP.Detali)).text = _main;
    //    if (_day > 0)
    //    {
    //        GetTMP(((int)TMP.Day)).text = $"{_day}{UserData.Instance.LocaleText("Day")}";
    //    }
    //    else
    //    {
    //        GetTMP(((int)TMP.Day)).text = "";
    //    }

    //}

    RectTransform rect;

    public void SetText(string _title, string _main, string _day)
    {
        StartCoroutine(WaitFrame(_title, _main, _day));
    }
    IEnumerator WaitFrame(string _title, string _main, string _day)
    {
        yield return new WaitForEndOfFrame();
        GetTMP(((int)TMP.Title)).text = _title;
        GetTMP(((int)TMP.Detali)).text = _main;
        GetTMP(((int)TMP.Day)).text = _day;

        yield return new WaitForEndOfFrame();
        float h = GetTMP(((int)TMP.Detali)).rectTransform.sizeDelta.y;
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, h + 42);
    }


}
