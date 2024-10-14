using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DungeonEdit : UI_PopUp
{
    void Start()
    {
        Init();
    }


    enum Objects
    {
        //NoTouch,
        Panel,
        //Close,

        Content,
 
    }

    enum Floor
    {
        Floor_0,
        Floor_1,
        Floor_2,
        Floor_3,
        Floor_4,
        Floor_5,
    }

    ScrollRect scroll;

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(Objects));
        Bind<Button>(typeof(Floor));

        scroll = GetComponentInChildren<ScrollRect>(true);

        //GetObject((int)Objects.Close).AddUIEvent(data => ClosePopUp());


        for (int i = 0; i < System.Enum.GetNames(typeof(Floor)).Length; i++)
        {
            GetButton(i).gameObject.AddUIEvent(data => Floor_ClickEvent(data.selectedObject.transform.GetSiblingIndex()), Define.UIEvent.LeftClick);

            //? 스크롤렉트의 드래그이벤트 연결
            GetButton(i).gameObject.AddUIEvent((data) => scroll.OnDrag(data), Define.UIEvent.Drag);
            GetButton(i).gameObject.AddUIEvent((data) => scroll.OnBeginDrag(data), Define.UIEvent.BeginDrag);
            GetButton(i).gameObject.AddUIEvent((data) => scroll.OnEndDrag(data), Define.UIEvent.EndDrag);
        }

        GetButton(0).GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"{UserData.Instance.LocaleText("숨겨진곳")}";
        for (int i = 1; i < System.Enum.GetNames(typeof(Floor)).Length; i++)
        {
            GetButton(i).GetComponentInChildren<TMPro.TextMeshProUGUI>().text = 
                $"{UserData.Instance.LocaleText("지하")} {i} {UserData.Instance.LocaleText("층")}";
            GetButton(i).gameObject.SetActive(false);
        }


        for (int i = 0; i < Mathf.Max(Main.Instance.ActiveFloor_Basement, Main.Instance.DungeonRank + 3); i++)
        {
            if (i == 6)
            {
                break;
            }
            GetButton(i).gameObject.SetActive(true);
        }
    }



    void Floor_ClickEvent(int floorIndex)
    {
        if (Main.Instance.ActiveFloor_Basement > floorIndex)
        {
            MoveCam_Floor(floorIndex);
            return;
        }

        if (Main.Instance.ActiveFloor_Basement == 4 && floorIndex == 4)
        {
            ExpansionCheck(Define.DungeonFloor.Floor_4, (int)Define.DungeonRank.D, 500, 250, 2);
            return;
        }

        if (Main.Instance.ActiveFloor_Basement == 5 && floorIndex == 5)
        {
            ExpansionCheck(Define.DungeonFloor.Floor_5, (int)Define.DungeonRank.C, 1000, 500, 3);
            return;
        }
    }


    void MoveCam_Floor(int floorIndex)
    {
        var cam = Camera.main.GetComponent<CameraControl>();

        Vector3 pos = Main.Instance.Floor[floorIndex].transform.position;
        cam.View_Target(pos);
    }



    void ExpansionCheck(Define.DungeonFloor targetFloor, int rank, int mana, int gold, int ap)
    {
        if (!Main.Instance.Management)
        {
            return;
        }

        var ui = Managers.UI.ShowPopUp<UI_Confirm>();
        ui.SetText(UserData.Instance.LocaleText("Confirm_Expansion"), () => StartCoroutine(WaitForAnswer(mana, gold, ap, rank, targetFloor)));
        ui.SetMode_Calculation((Define.DungeonRank)rank, mana.ToString(), gold.ToString(), ap.ToString());
    }


    bool ConfirmCheck(int mana, int gold, int ap, int rank)
    {
        if (Main.Instance.DungeonRank < rank)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = UserData.Instance.LocaleText("Message_No_Rank");
            return false;
        }
        if (Main.Instance.Player_AP < ap)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = UserData.Instance.LocaleText("Message_No_AP");
            return false;
        }
        if (Main.Instance.Player_Mana < mana)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = UserData.Instance.LocaleText("Message_No_Mana");
            return false;
        }
        if (Main.Instance.Player_Gold < gold)
        {
            var msg = Managers.UI.ShowPopUpAlone<UI_SystemMessage>();
            msg.Message = UserData.Instance.LocaleText("Message_No_Gold");
            return false;
        }

        return true;
    }



    IEnumerator WaitForAnswer(int mana, int gold, int ap, int rank, Define.DungeonFloor floor)
    {
        if (ConfirmCheck(mana, gold, ap, rank))
        {
            Main.Instance.CurrentDay.SubtractMana(mana, Main.DayResult.EventType.Etc);
            Main.Instance.CurrentDay.SubtractGold(gold, Main.DayResult.EventType.Etc);
            Main.Instance.Player_AP -= ap;

            Main.Instance.Basement_Expansion();
            FindObjectOfType<UI_Management>().DungeonExpansion();

            yield return null;
            yield return null;

            switch (floor)
            {
                case Define.DungeonFloor.Floor_4:
                    //? 4층 확장시 4층으로 전이진이 옮겨지는 이벤트
                    Managers.Dialogue.ShowDialogueUI(DialogueName.Expansion_4, Main.Instance.Player);
                    break;

                case Define.DungeonFloor.Floor_5:
                    EventManager.Instance.EntranceMove_4to5();
                    break;

                case Define.DungeonFloor.Floor_6:
                    break;

                case Define.DungeonFloor.Floor_7:
                    break;
            }
        }
    }






    public override bool EscapeKeyAction()
    {
        return true;
    }
    //private void OnEnable()
    //{
    //    Time.timeScale = 0;
    //}
    //private void OnDestroy()
    //{
    //    PopupUI_OnDestroy();
    //}
}
