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

        GetObject(((int)Contents.Panel)).AddUIEvent((data) => ClosePopUp(), Define.UIEvent.LeftClick);

        PrintMessage();
    }

    [TextArea(2,10)]
    public string Message;
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
}
