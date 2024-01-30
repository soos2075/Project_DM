using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Technical_Select : UI_PopUp, IWorldSpaceUI
{
    void Start()
    {
        Init();
    }
    enum Objects
    {
        //Panel,
        Place,
        Technical,
    }

    public void SetCanvasWorldSpace()
    {
        Managers.UI.SetCanvas(gameObject, RenderMode.WorldSpace);
    }

    public override void Init()
    {
        SetCanvasWorldSpace();
        AddRightClickCloseAllEvent();

        Bind<GameObject>(typeof(Objects));

        GetObject((int)Objects.Technical).AddUIEvent(data =>
        {
            ClosePopUp();
            var tech = Managers.UI.ShowPopUp<UI_Placement_Technical>("Technical/UI_Placement_Technical");
            tech.parents = this.parents;
            //parents.PanelDisable();
        });

        GetObject((int)Objects.Place).GetComponent<TextMeshProUGUI>().text = parents.Name_KR;
    }




    public UI_Technical parents;



}
