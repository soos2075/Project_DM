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

    }

    public void SetCanvasWorldSpace()
    {
        Managers.UI.SetCanvasWorld(gameObject);
    }


    enum Contents
    {
        Panel,
        Name,
        Contents,
    }

    TextMeshProUGUI text_Name;
    TextMeshProUGUI text_Contents;



    public override void Init()
    {
        Bind<GameObject>(typeof(Contents));

        text_Name = GetObject((int)Contents.Name).GetComponent<TextMeshProUGUI>();
        text_Contents = GetObject((int)Contents.Contents).GetComponent<TextMeshProUGUI>();
    }



    public void ViewContents(string name, string contents)
    {
        text_Name.text = name;
        text_Contents.text = contents;
    }


}
