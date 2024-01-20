using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Placement_TypeSelect : UI_PopUp, Interface.IWorldSpaceUI
{

    enum Objects
    {
        Panel,
        Place,
        Facility,
        Monster,
    }

    public void SetCanvasWorldSpace()
    {
        Managers.UI.SetCanvasWorld(gameObject);
    }

    public override void Init()
    {
        SetCanvasWorldSpace();


        Bind<GameObject>(typeof(Objects));

        GetObject((int)Objects.Facility).AddUIEvent(data =>
        {
            ClosePopUp();
            var facility = Managers.UI.ShowPopUp<UI_Placement_Facility>();
        });
        GetObject((int)Objects.Monster).AddUIEvent(data =>
        {
            ClosePopUp();
            var monster = Managers.UI.ShowPopUp<UI_Placement_Monster>();
        });

        GetObject((int)Objects.Place).GetComponent<TextMeshProUGUI>().text = Main.Instance.CurrentFloor.Name_KR;
    }


    void Start()
    {
        Init();
    }


}
