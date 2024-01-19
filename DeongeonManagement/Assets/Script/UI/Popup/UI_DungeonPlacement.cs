using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_DungeonPlacement : UI_PopUp, UI_Interface.IWorldSpace
{
    enum Floor
    {
        BasementFloor_1,
        BasementFloor_2,
        BasementFloor_3,

    }

    public void SetCanvasWorldSpace()
    {
        Managers.UI.SetCanvasWorld(gameObject);
    }

    public override void Init()
    {
        SetCanvasWorldSpace();

        Bind<Image>(typeof(Floor));

        GetImage((int)Floor.BasementFloor_1).gameObject.AddUIEvent((data) => OpenPlacementType(data, Main.Instance.Floor[0]));
        GetImage((int)Floor.BasementFloor_2).gameObject.AddUIEvent((data) => OpenPlacementType(data, Main.Instance.Floor[1]));
        GetImage((int)Floor.BasementFloor_3).gameObject.AddUIEvent((data) => OpenPlacementType(data, Main.Instance.Floor[2]));
    }

    void Start()
    {
        Init();


    }


    void OpenPlacementType(PointerEventData data, BasementFloor floor)
    {
        Main.Instance.CurrentFloor = floor;

        var popup = Managers.UI.ShowPopUpAlone<UI_Placement_TypeSelect>();
        var pos = Camera.main.ScreenToWorldPoint(data.position);
        popup.transform.localPosition = new Vector3(pos.x, pos.y, 0);
    }


}
