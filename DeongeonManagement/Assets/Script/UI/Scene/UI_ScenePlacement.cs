using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ScenePlacement : UI_Scene, IWorldSpaceUI
{
    public void SetCanvasWorldSpace()
    {
        Managers.UI.SetCanvas(gameObject, RenderMode.WorldSpace, false);
        GetComponent<Canvas>().sortingOrder = 4;
    }

    public override void Init()
    {
        SetCanvasWorldSpace();

        GenerateFloorUI();
    }

    //public override void Refresh()
    //{
    //    base.Refresh();
    //    GenerateFloorUI();
    //}


    void Start()
    {
        Init();
    }



    void GenerateFloorUI()
    {

        for (int i = 0; i < Main.Instance.Floor.Length; i++)
        {
            UI_TileView_Floor content = Managers.Resource.Instantiate("UI/PopUp/Element/TileView_Floor", transform).
                GetComponent<UI_TileView_Floor>();

            content.SetFloorSize(Main.Instance.Floor[i].transform.position, Main.Instance.Floor[i].boxCollider.bounds.size);
            content.FloorID = i;
        }
    }

}
