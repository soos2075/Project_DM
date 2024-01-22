using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Placement_Facility : UI_PopUp
{

    enum Buttons
    {
        Return,

        Clear,

        Entrance,
        Exit,

        Herb_Low_2x2,
        Herb_Low_3x3,

        Herb_High_1x3,
        Herb_High_3x1,

        Treasure_Ring,
        Rest_Campfire,

        Mineral_Low,


    }

    public override void Init()
    {
        base.Init();
        AddRightClickCloseAllEvent();

        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.Return).gameObject.AddUIEvent(data => CloseAll());

        GetButton((int)Buttons.Entrance).gameObject.AddUIEvent(data => SetBoundary(Define.Boundary_1x1, () => CreateAll("Entrance")));
        GetButton((int)Buttons.Exit).gameObject.AddUIEvent(data => SetBoundary(Define.Boundary_1x1, () => CreateAll("Exit")));


        GetButton((int)Buttons.Herb_Low_2x2).gameObject.AddUIEvent(data => SetBoundary(Define.Boundary_2x2, () => CreateAll("Herb_Low")));
        GetButton((int)Buttons.Herb_Low_3x3).gameObject.AddUIEvent(data => SetBoundary(Define.Boundary_3x3, () => CreateAll("Herb_Low")));

        GetButton((int)Buttons.Herb_High_1x3).gameObject.AddUIEvent(data => SetBoundary(Define.Boundary_1x3, () => CreateAll("Herb_High")));
        GetButton((int)Buttons.Herb_High_3x1).gameObject.AddUIEvent(data => SetBoundary(Define.Boundary_3x1, () => CreateAll("Herb_High")));

        GetButton((int)Buttons.Treasure_Ring).gameObject.AddUIEvent(data => SetBoundary(Define.Boundary_1x1, () => CreateAll("Treasure_Ring")));
        GetButton((int)Buttons.Rest_Campfire).gameObject.AddUIEvent(data => SetBoundary(Define.Boundary_1x1, () => CreateAll("Rest_Campfire")));

        GetButton((int)Buttons.Mineral_Low).gameObject.AddUIEvent(data => SetBoundary(Define.Boundary_Cross_1, () =>
        CreateTwo("Mineral_Diamond", "Mineral_Rock", Define.Boundary_1x1, Define.Boundary_Side_Cross)));

    }

    void Start()
    {
        Init();
    }


    public UI_Floor parents;




    void SetBoundary(Vector2Int[] vector2Ints, Action action)
    {
        Main.Instance.CurrentBoundary = vector2Ints;
        Main.Instance.CurrentAction = action;

        parents.ShowTile();

        Managers.UI.PausePopUp(this);
        //Managers.UI.ClosePopUp(this);
    }

    void ResetAction()
    {
        Main.Instance.CurrentBoundary = null;
        Main.Instance.CurrentAction = null;
        Main.Instance.CurrentTile = null;
    }



    void CreateOver()
    {
        Debug.Log("설치완료. 이제 돈받는처리같은거 하고 이벤트 취소하면 됨");
        Managers.UI.PauseClose();
        Managers.UI.ClosePopUp();
        ResetAction();
    }

    bool Create(string prefab, Vector2Int[] boundary)
    {
        if (Main.Instance.CurrentTile == null) return false;

        var tile = Main.Instance.CurrentTile;
        foreach (var item in boundary)
        {
            int _deltaX = tile.index.x + item.x;
            int _deltaY = tile.index.y + item.y;

            var content = Main.Instance.CurrentFloor.TileMap[_deltaX, _deltaY];
            var obj = Managers.Resource.Instantiate($"Facility/{prefab}").GetComponent<Facility>();
            obj.PlacementConfirm(Main.Instance.CurrentFloor, content);
        }

        return true;
    }

    void CreateAll(string prefab)
    {
        if (Create(prefab, Main.Instance.CurrentBoundary))
        {
            CreateOver();
        }
        else
        {
            Debug.Log("배치할 수 없음");
        }
    }

    void CreateTwo(string prefab1, string prefab2, Vector2Int[] boundary1, Vector2Int[] boundary2)
    {
        if (Create(prefab1, boundary1) && Create(prefab2, boundary2))
        {
            CreateOver();
        }
        else
        {
            Debug.Log("배치할 수 없음");
        }
    }
    void CreateThree(string prefab1, string prefab2, string prefab3, Vector2Int[] boundary1, Vector2Int[] boundary2, Vector2Int[] boundary3)
    {
        if (Create(prefab1, boundary1) && Create(prefab2, boundary2) && Create(prefab3, boundary3))
        {
            CreateOver();
        }
        else
        {
            Debug.Log("배치할 수 없음");
        }
    }



}
