using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Placement_TypeSelect : UI_PopUp, IWorldSpaceUI
{
    void Start()
    {
        Init();
    }
    enum Objects
    {
        //Panel,
        Place,
        Facility,
        Monster,
    }

    public void SetCanvasWorldSpace()
    {
        Managers.UI.SetCanvas_SubCamera(gameObject, RenderMode.WorldSpace);
    }

    //public UI_Floor parents { get; set; }


    public override void Init()
    {
        SetCanvasWorldSpace();
        AddRightClickCloseAllEvent();

        Bind<GameObject>(typeof(Objects));

        GetObject((int)Objects.Facility).AddUIEvent(data =>
        {
            ClosePopUp();
            var facility = Managers.UI.ShowPopUpAlone<UI_Placement_Facility>("Facility/UI_Placement_Facility");
            //facility.parents = this.parents;
            FindObjectOfType<UI_Management>().FloorPanelClear();
        });
        GetObject((int)Objects.Monster).AddUIEvent(data =>
        {
            ClosePopUp();
            var monster = Managers.UI.ShowPopUpAlone<UI_Monster_Management>("Monster/UI_Monster_Management");
            //monster.Mode = UI_Monster_Management.Unit_Mode.Placement;
            FindObjectOfType<UI_Management>().FloorPanelClear();

        });

        GetObject((int)Objects.Place).GetComponent<TextMeshProUGUI>().text = Main.Instance.CurrentFloor.LabelName;

        int nowMonster = Main.Instance.CurrentFloor.monsterList.Count;
        if (Main.Instance.CurrentFloor.FloorIndex == 0)
        {
            nowMonster--;
        }
        int maxMonster = nowMonster + Main.Instance.CurrentFloor.MaxMonsterSize;

        GetObject((int)Objects.Monster).GetComponentInChildren<TextMeshProUGUI>().text += $"\n{nowMonster}/{maxMonster}";


        if (Main.Instance.Turn < 2 && UserData.Instance.FileConfig.PlayRounds < 2)
        {
            GetObject((int)Objects.Monster).SetActive(false);
        }
    }



}
