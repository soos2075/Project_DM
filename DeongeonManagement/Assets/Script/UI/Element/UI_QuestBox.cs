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
    }

    enum TMP
    {
        Title,
        Detali,
    }

    public void SetText(string _title, string _main)
    {
        StartCoroutine(WaitFrame(_title, _main));
    }

    IEnumerator WaitFrame(string _title, string _main)
    {
        yield return new WaitForEndOfFrame();
        GetTMP(((int)TMP.Title)).text = _title;
        GetTMP(((int)TMP.Detali)).text = _main;
    }


}
