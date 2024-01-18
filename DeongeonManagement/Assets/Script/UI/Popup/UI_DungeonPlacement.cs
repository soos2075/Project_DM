using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DungeonPlacement : UI_PopUp
{
    enum Floor
    {
        LF_1,
        LF_2,
        LF_3,
        RF_1,
        RF_2,
        RF_3,
        F_4,
    }


    public override void Init()
    {
        base.Init();
        Bind<Image>(typeof(Floor));

        GetImage((int)Floor.LF_1).gameObject.AddUIEvent((data) =>
        {
            var popup = Managers.UI.ShowPopUpAlone<UI_Placement_TypeSelect>();
            popup.transform.GetChild(0).localPosition = new Vector3(500, 0, 0);
            popup.Place = "LF_1";
        });
        GetImage((int)Floor.RF_1).gameObject.AddUIEvent((data) =>
        {
            var popup = Managers.UI.ShowPopUpAlone<UI_Placement_TypeSelect>();
            popup.transform.GetChild(0).localPosition = new Vector3(-500, 0, 0);
            popup.Place = "RF_1";
        });
        GetImage((int)Floor.LF_2).gameObject.AddUIEvent((data) =>
        {
            var popup = Managers.UI.ShowPopUpAlone<UI_Placement_TypeSelect>();
            popup.transform.GetChild(0).localPosition = new Vector3(500, 0, 0);
            popup.Place = "LF_2";
        });
        GetImage((int)Floor.RF_2).gameObject.AddUIEvent((data) =>
        {
            var popup = Managers.UI.ShowPopUpAlone<UI_Placement_TypeSelect>();
            popup.transform.GetChild(0).localPosition = new Vector3(-500, 0, 0);
            popup.Place = "RF_2";
        });
        GetImage((int)Floor.LF_3).gameObject.AddUIEvent((data) =>
        {
            var popup = Managers.UI.ShowPopUpAlone<UI_Placement_TypeSelect>();
            popup.transform.GetChild(0).localPosition = new Vector3(500, 0, 0);
            popup.Place = "LF_3";
        });

        GetImage((int)Floor.RF_3).gameObject.AddUIEvent((data) =>
        {
            var popup = Managers.UI.ShowPopUpAlone<UI_Placement_TypeSelect>();
            popup.transform.GetChild(0).localPosition = new Vector3(-500, 0, 0);
            popup.Place = "RF_3";
        });

        GetImage((int)Floor.F_4).gameObject.AddUIEvent((data) =>
        {
            var popup = Managers.UI.ShowPopUpAlone<UI_Placement_TypeSelect>();
            popup.transform.GetChild(0).localPosition = new Vector3(0, 100, 0);
            popup.Place = "F_4";
        });
    }

    void Start()
    {
        Init();


    }




}
