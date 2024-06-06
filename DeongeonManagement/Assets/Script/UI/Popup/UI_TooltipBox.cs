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

    public void Init_Tooltip(string name, string contents)
    {
        SetCanvas();
        Init();

        ViewContents(contents);
    }


    public void SetCanvas()
    {
        Managers.UI.SetCanvas(gameObject, RenderMode.ScreenSpaceOverlay, true);
        panel = transform.GetChild(0).GetComponent<RectTransform>();
    }

    public override void Init()
    {
        Bind<GameObject>(typeof(Contents));

        //text_Name = GetObject((int)Contents.Title).GetComponent<TextMeshProUGUI>();
        text_Contents = GetObject((int)Contents.Contents).GetComponent<TextMeshProUGUI>();
    }




    private void LateUpdate()
    {
        //Debug.Log("###" + Input.mousePosition);
        //Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        //Vector3 centeredMousePosition = Input.mousePosition - screenCenter;
        //panel.localPosition = new Vector3(centeredMousePosition.x - 5, centeredMousePosition.y + 5, 0);


        //? ��ũ�������� �׽�Ʈ
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        Vector3 centeredMousePosition = Input.mousePosition - screenCenter;

        float offset = (float)Screen.width / 1280f;
        centeredMousePosition /= offset;

        //panel.localPosition = new Vector3(centeredMousePosition.x, centeredMousePosition.y);
        panel.localPosition = new Vector3(centeredMousePosition.x + (57600 / Screen.width), centeredMousePosition.y - (32400 / Screen.height), 0);
        //panel.localPosition = new Vector3(centeredMousePosition.x + 50, centeredMousePosition.y - 30, 0);

        // ��� ���
        //Debug.Log(centeredMousePosition);

        //? �����ڽ� ũ�⿡ ���缭 �̹��� ũ�� ���� -> �ǹ����������� ��ġ�޷����� �Ⱒ
        //GetObject((int)Contents.Contents).GetComponent<RectTransform>().sizeDelta =
        //    new Vector2(text_Contents.preferredWidth, text_Contents.preferredHeight);

        //GetObject((int)Contents.Panel).GetComponent<RectTransform>().sizeDelta =
        //    new Vector2(text_Contents.preferredWidth, text_Contents.preferredHeight);

        //? ���� ȭ�� �ٱ����� ����� ���ʿ� ǥ�õǰ� �����ؾ���
    }



    enum Contents
    {
        Panel,
        //Title,
        Contents,
        //Detail,
    }

    RectTransform panel;
    //TextMeshProUGUI text_Name;
    TextMeshProUGUI text_Contents;

    public void ViewContents(string contents)
    {
        text_Contents.text = contents;
    }

}
