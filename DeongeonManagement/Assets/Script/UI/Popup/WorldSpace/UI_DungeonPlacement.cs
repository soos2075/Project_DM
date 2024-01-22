using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DungeonPlacement : UI_PopUp, Interface.IWorldSpaceUI
{


    public void SetCanvasWorldSpace()
    {
        Managers.UI.SetCanvasWorld(gameObject);
    }

    public override void Init()
    {
        SetCanvasWorldSpace();

        GenerateFloorUI();
    }


    void Start()
    {
        Init();
    }



    void GenerateFloorUI()
    {

        for (int i = 0; i < Main.Instance.Floor.Length; i++)
        {
            UI_Floor content = Managers.Resource.Instantiate("UI/PopUp/Element/Floor", transform).
                GetComponent<UI_Floor>();

            content.SetFloorSize(Main.Instance.Floor[i].transform.position, Main.Instance.Floor[i].boxCollider.bounds.size);
            content.FloorID = i;
        }
    }




}
