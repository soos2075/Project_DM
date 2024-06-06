using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_SystemMessage : UI_PopUp
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
    }


    public float DelayTime { get; set; } = 0.5f;


    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);
        Bind<GameObject>(typeof(Contents));

        PrintMessage();

        Wait_Delay = StartCoroutine(CloseDelay(DelayTime));
    }

    //[TextArea(2,10)]
    public string Message { get; set; }
    void PrintMessage()
    {
        if (string.IsNullOrEmpty(Message))
        {
            Managers.UI.ClosePopUp(this);
            return;
        }

        GetObject(((int)Contents.Message)).GetComponent<TextMeshProUGUI>().text = Message;
        StartCoroutine(BackgroundSize());
    }

    IEnumerator BackgroundSize()
    {
        yield return new WaitForEndOfFrame();
        var size = GetObject(((int)Contents.Message)).GetComponent<RectTransform>().sizeDelta;

        GetObject(((int)Contents.BG)).GetComponent<RectTransform>().sizeDelta = size + new Vector2(60, 40);
    }


    Coroutine Wait_Delay;

    IEnumerator CloseDelay(float timer)
    {
        yield return new WaitForSecondsRealtime(timer);
        GetObject(((int)Contents.Panel)).AddUIEvent((data) => ClosePopUp(), Define.UIEvent.LeftClick);

        Wait_Delay = null;
    }


    //private void OnEnable()
    //{
    //    Time.timeScale = 0;
    //}
    //private void OnDestroy()
    //{
    //    Time.timeScale = 1;
    //}


    public override bool EscapeKeyAction()
    {
        if (Wait_Delay == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


}
