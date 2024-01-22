using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Placement_Monster : UI_PopUp
{

    enum Objects
    {
        Return,
        Content,
        ResumeCount,
    }


    public override void Init()
    {
        base.Init();
        AddRightClickCloseAllEvent();

        Bind<GameObject>(typeof(Objects));

        GetObject((int)Objects.Return).gameObject.AddUIEvent(data => CloseAll());
    }
    void Start()
    {
        Init();
        GenerateContents();
        ResumeCountUpdate();
    }


    public UI_Floor parents;



    public void ResumeCountUpdate()
    {
        GetObject((int)Objects.ResumeCount).GetComponent<TextMeshProUGUI>().text = $"배치 가능 횟수 : {Main.Instance.CurrentFloor.MaxMonsterSize}";
    }


    void GenerateContents()
    {
        for (int i = 0; i < Main.Instance.Monsters.Length; i++)
        {
            UI_Placement_Content content = Managers.Resource.Instantiate("UI/PopUp/Element/Placement_Content", GetObject((int)Objects.Content).transform).
                GetComponent<UI_Placement_Content>();

            if (Main.Instance.Monsters[i] != null)
            {
                content.MonsterID = i;
            }
            else
            {
                content.MonsterID = -1;
            }
        }
    }





    public void SetBoundary(Vector2Int[] vector2Ints, Action action)
    {
        Main.Instance.CurrentBoundary = vector2Ints;
        Main.Instance.CurrentAction = action;

        parents.ShowTile();

        Managers.UI.PausePopUp(this);
    }

    void ResetAction()
    {
        Main.Instance.CurrentBoundary = null;
        Main.Instance.CurrentAction = null;
        Main.Instance.CurrentTile = null;
    }

    void CreateOver()
    {
        Debug.Log("배치완료. 이제 돈받는처리같은거 하면 됨");
        Managers.UI.PauseClose();
        Managers.UI.ClosePopUp();
        ResetAction();
    }

    bool Create(int monsterID, Vector2Int[] boundary)
    {
        if (Main.Instance.CurrentTile == null) return false;

        var tile = Main.Instance.CurrentTile;
        foreach (var item in boundary)
        {
            int _deltaX = tile.index.x + item.x;
            int _deltaY = tile.index.y + item.y;

            var content = Main.Instance.CurrentFloor.TileMap[_deltaX, _deltaY];


            var obj = Main.Instance.Monsters[monsterID];
            obj.PlacementConfirm(Main.Instance.CurrentFloor, content);
        }

        return true;
    }

    public void CreateAll(int monsterID)
    {
        if (Create(monsterID, Main.Instance.CurrentBoundary))
        {
            CreateOver();
        }
        else
        {
            Debug.Log("배치할 수 없음");
        }
    }

}
