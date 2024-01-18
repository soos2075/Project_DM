using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Placement_TypeSelect : UI_PopUp
{
    public string Place { get; set; }

    enum Objects
    {
        Panel,
        Facility,
        Monster,
    }



    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(Objects));

        GetObject((int)Objects.Facility).AddUIEvent(data =>
        {
            var facility = Managers.UI.ShowPopUp<UI_Placement_Facility>();
            facility.Place = Place;
        });
        GetObject((int)Objects.Monster).AddUIEvent(data =>
        {
            var monster = Managers.UI.ShowPopUp<UI_Placement_Monster>();
            monster.Place = Place;
        });
    }


    void Start()
    {
        Init();
    }


}
