using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Record : UI_PopUp
{
    void Start()
    {
        Init();
    }

    enum Contents
    {
        Panel,

        BG,
        Message,

        BG2,
        Message2,
    }


    public float DelayTime { get; set; } = 0.5f;

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);
        Bind<GameObject>(typeof(Contents));


        GetObject(((int)Contents.Message)).GetComponent<TextMeshProUGUI>().text = "";
        GetObject(((int)Contents.Message2)).GetComponent<TextMeshProUGUI>().text = "";

        StartCoroutine(CloseDelay(DelayTime));

        StartCoroutine(WaitFrame(GetObject(((int)Contents.Message)).GetComponent<TextMeshProUGUI>(), Msg1));
        StartCoroutine(WaitFrame(GetObject(((int)Contents.Message2)).GetComponent<TextMeshProUGUI>(), Msg2));
    }

    public string Msg1 { get; set; }

    public string Msg2 { get; set; }



    IEnumerator WaitFrame(TextMeshProUGUI target, string msg)
    {
        yield return new WaitForEndOfFrame();
        target.text = msg;
    }




    IEnumerator CloseDelay(float timer)
    {
        yield return new WaitForSecondsRealtime(timer);
        GetObject(((int)Contents.Panel)).AddUIEvent((data) => ClosePopUp(), Define.UIEvent.LeftClick);
    }

}
