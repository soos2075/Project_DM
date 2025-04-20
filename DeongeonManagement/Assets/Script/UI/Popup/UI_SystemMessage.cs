using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        TopImage,
        Contents,
    }


    public float DelayTime { get; set; } = 0.5f;


    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject);
        Bind<GameObject>(typeof(Contents));

        PrintMessage();

        Wait_Delay = StartCoroutine(CloseDelay(DelayTime));

        if (mainSprite == null)
        {
            GetObject((int)Contents.TopImage).SetActive(false);
        }
        else
        {
            GetObject((int)Contents.Contents).GetComponent<Image>().sprite = mainSprite;
        }
    }


    Sprite mainSprite;

    public void Set_Image(Sprite sprite)
    {
        mainSprite = sprite;
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



    private void LateUpdate()
    {
        if (Wait_Delay == null)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                ClosePopUp();
            }
        }
    }

    private void OnEnable()
    {
        Time.timeScale = 0;
    }
    private void OnDestroy()
    {
        PopupUI_OnDestroy();
    }


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
