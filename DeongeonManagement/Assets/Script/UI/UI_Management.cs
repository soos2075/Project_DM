using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Management : UI_Base
{
    public enum ButtonEvent
    {
        Summon,
        Training,
        Special,
        Guild,
        Placement,
    }


    public override void Init()
    {
        Bind<Button>(typeof(ButtonEvent));
        GetButton((int)ButtonEvent.Summon).gameObject.AddUIEvent((data) => Managers.UI.ClearAndShowPopUp<UI_Summon>());
        GetButton((int)ButtonEvent.Training).gameObject.AddUIEvent((data) => Managers.UI.ClearAndShowPopUp<UI_Training>());
        GetButton((int)ButtonEvent.Placement).gameObject.AddUIEvent((data) => Managers.UI.ClearAndShowPopUp<UI_DungeonPlacement>());
    }

    void Start()
    {
        Init();

    }

    void Update()
    {
        
    }


}
