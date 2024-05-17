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
        GetComponent<Canvas>().worldCamera = Camera.main;
        panel = transform.GetChild(0).GetComponent<RectTransform>();
    }


    RectTransform panel;

    //private void Update()
    //{
    //    Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    panel.position = new Vector3(mouse.x + 50, mouse.y + 50, 0);
    //}
    private void LateUpdate()
    {
        //Vector3 mouse = Camera.main.WorldToViewportPoint(Input.mousePosition);
        //panel.localPosition = new Vector3(mouse.x, mouse.y, 0);
        //Debug.Log(mouse);

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        Vector3 centeredMousePosition = Input.mousePosition - screenCenter;
        panel.localPosition = new Vector3(centeredMousePosition.x, centeredMousePosition.y, 0);
        // 결과 출력
        //Debug.Log(centeredMousePosition);
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
