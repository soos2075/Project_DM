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

    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);
        Bind<GameObject>(typeof(Contents));

        PrintMessage();

        StartCoroutine(CloseDelay());
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

    IEnumerator CloseDelay()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        GetObject(((int)Contents.Panel)).AddUIEvent((data) => ClosePopUp(), Define.UIEvent.LeftClick);
    }


    //private void OnEnable()
    //{
    //    Time.timeScale = 0;
    //}
    //private void OnDestroy()
    //{
    //    Time.timeScale = 1;
    //}
}
