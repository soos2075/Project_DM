using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_TooltipBox : UI_PopUp
{

    //void Start()
    //{
    //    Init();
    //}

    public void Init_Tooltip(string name, string contents, ShowPosition position = ShowPosition.RightDown)
    {
        SetCanvas();
        Init();

        ViewContents(name, contents);

        BoxPosition = position;
    }


    public void SetCanvas()
    {
        Managers.UI.SetCanvas(gameObject, RenderMode.ScreenSpaceOverlay, true);
        panel = transform.GetChild(0).GetComponent<RectTransform>();
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(Contents));

        text_Title = GetObject((int)Contents.Title).GetComponent<TextMeshProUGUI>();
        text_Contents = GetObject((int)Contents.Contents).GetComponent<TextMeshProUGUI>();
    }



    public enum ShowPosition
    {
        RightDown,
        RightUp,

        LeftDown,
        LeftUp,
    }
    ShowPosition BoxPosition;


    private void LateUpdate()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        Vector3 centeredMousePosition = Input.mousePosition - screenCenter;

        float offset = (float)Screen.width / 1280f;
        centeredMousePosition /= offset;

        switch (BoxPosition)
        {
            case ShowPosition.RightDown:
                panel.localPosition = new Vector3(centeredMousePosition.x + (57600 / Screen.width), centeredMousePosition.y - (32400 / Screen.height), 0);
                break;

            case ShowPosition.RightUp:
                panel.pivot = Vector2.zero;
                panel.localPosition = new Vector3(centeredMousePosition.x, centeredMousePosition.y, 0);
                break;

            case ShowPosition.LeftDown:
                panel.pivot = Vector2.one;
                panel.localPosition = new Vector3(centeredMousePosition.x, centeredMousePosition.y, 0);
                break;

            case ShowPosition.LeftUp:
                panel.pivot = Vector2.right;
                panel.localPosition = new Vector3(centeredMousePosition.x, centeredMousePosition.y, 0);
                break;
        }
    }



    enum Contents
    {
        Panel,
        Title,
        Contents,
    }

    RectTransform panel;
    TextMeshProUGUI text_Title;
    TextMeshProUGUI text_Contents;

    public void ViewContents(string title, string contents)
    {
        text_Title.text = title;
        if (string.IsNullOrEmpty(title))
        {
            GetObject((int)Contents.Title).SetActive(false);
        }
        text_Contents.text = contents;
    }

}
