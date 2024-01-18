using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Placement_Facility : UI_PopUp
{

    public string Place { get; set; }
    enum Buttons
    {
        Return,
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.Return).gameObject.AddUIEvent(data => ClosePopUp());
    }

    void Start()
    {
        Init();
    }

}
