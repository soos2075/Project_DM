using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DungeonPlacement : UI_PopUp, IWorldSpaceUI
{


    public void SetCanvasWorldSpace()
    {
        Managers.UI.SetCanvas(gameObject, RenderMode.WorldSpace);
    }

    public override void Init()
    {
        SetCanvasWorldSpace();

        GenerateFloorUI();
    }


    void Awake()
    {
        //Init();
    }
    void Start()
    {
        Init();
    }


    public List<UI_Floor> uI_Floors = new List<UI_Floor>();

    void GenerateFloorUI()
    {

        for (int i = 0; i < Main.Instance.Floor.Length; i++)
        {
            UI_Floor content = Managers.Resource.Instantiate("UI/PopUp/Element/Floor", transform).
                GetComponent<UI_Floor>();

            content.SetFloorSize(Main.Instance.Floor[i].transform.position, Main.Instance.Floor[i].BoxCollider.bounds.size);
            content.FloorID = i;

            uI_Floors.Add(content);
        }
    }




}
