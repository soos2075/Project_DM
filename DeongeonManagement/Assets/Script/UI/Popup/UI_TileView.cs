using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_TileView : UI_PopUp, IWorldSpaceUI
{
    void Awake()
    {
        Init();
        SetCanvasWorldSpace();
    }
    void Start()
    {
        if (Main.Instance.CurrentAction != null)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetCanvasWorldSpace()
    {
        //Managers.UI.SetCanvas(gameObject, RenderMode.WorldSpace);

        //GetComponent<Canvas>().worldCamera = Camera.main;

        Managers.UI.SetCanvas_SubCamera(gameObject, RenderMode.ScreenSpaceCamera, false);

        panel = transform.GetChild(0).GetComponent<RectTransform>();
    }


    RectTransform panel;


    private void LateUpdate()
    {
        //? 기존
        //Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        //Vector3 centeredMousePosition = Input.mousePosition - screenCenter;
        //panel.localPosition = new Vector3(centeredMousePosition.x - 5, centeredMousePosition.y + 5, 0);

        //? 스크린사이즈 테스트
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        Vector3 centeredMousePosition = Input.mousePosition - screenCenter;

        float offset = (float)Screen.width / 1280f;
        centeredMousePosition /= offset;

        panel.localPosition = new Vector3(centeredMousePosition.x + (57600 / Screen.width), centeredMousePosition.y - (32400 / Screen.height), 0);
    }


    enum Contents
    {
        Panel,
        Name,
        Contents,
        Detail,
    }

    TextMeshProUGUI text_Name;
    TextMeshProUGUI text_Contents;
    TextMeshProUGUI text_Detail;


    public override void Init()
    {
        Bind<GameObject>(typeof(Contents));

        text_Name = GetObject((int)Contents.Name).GetComponent<TextMeshProUGUI>();
        text_Contents = GetObject((int)Contents.Contents).GetComponent<TextMeshProUGUI>();
        text_Detail = GetObject((int)Contents.Detail).GetComponent<TextMeshProUGUI>();

        text_Detail.text = "";
    }



    public void ViewContents(string name, string contents)
    {
        text_Name.text = name;
        text_Contents.text = contents;
    }


    public void ViewDetail(string addContents)
    {
        text_Detail.text = addContents;
    }

}
