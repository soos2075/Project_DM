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
        Batch,
    }


    public override void Init()
    {
        Bind<Button>(typeof(ButtonEvent));
    }

    void Start()
    {
        Init();
        GetButton((int)ButtonEvent.Summon).gameObject.AddUIEvent((data) => Managers.UI.ShowPopUp<UI_Summon>());
        GetButton((int)ButtonEvent.Training).gameObject.AddUIEvent((data) => Managers.UI.ShowPopUp<UI_Training>());
    }

    void Update()
    {
        
    }


}
