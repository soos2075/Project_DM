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
        Managers.UI.SetCanvas(gameObject, RenderMode.WorldSpace);
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
