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

            var tool = GetButton(i).gameObject.GetOrAddComponent<UI_Tooltip>();
            tool.SetTooltipContents("", UserData.Instance.LocaleText_Tooltip($"FloorEffect_{i}"), UI_TooltipBox.ShowPosition.LeftUp);
        }


        FloorBtn_Update();
    }


    void FloorBtn_Update()
    {
        GetButton(0).GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"{UserData.Instance.LocaleText("숨겨진곳")}";
        for (int i = 1; i < System.Enum.GetNames(typeof(Floor)).Length; i++)
        {
            GetButton(i).GetComponentInChildren<TMPro.TextMeshProUGUI>().text =
                $"{UserData.Instance.LocaleText("지하")} {i} {UserData.Instance.LocaleText("층")}";
            GetButton(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < Mathf.Max(Main.Instance.ActiveFloor_Basement, Main.Instance.DungeonRank + 3); i++)
        {
            if (i == 6) //? 지금은 5층까지만 되니까 6이상은 바로 브레이크
            {
                break;
            }

            //? New Overlay
            if (i == Main.Instance.ActiveFloor_Basement)
            {
                StartCoroutine(WaitFrame(GetButton(Main.Instance.ActiveFloor_Basement).gameObject));
            }

            //? C랭크인데 확장을 안해서 4층만 보여야할 땐 이걸로 (5층생략)
            if (i > Main.Instance.ActiveFloor_Basement)
            {
                break;
            }

            GetButton(i).gameObject.SetActive(true);
        }
    }


    IEnumerator WaitFrame(GameObject floor)
    {
        yield return null;

        var overlay = Managers.Resource.Instantiate("UI/PopUp/Element/OverlayImage", floor.transform);
        var ui = overlay.GetComponent<UI_Overlay>();
        ui.SetOverlay_DontDest(Managers.Sprite.Get_SLA(SpriteManager.Library.UI, "Overlay_Icon", "New_Small"), floor);
    }

    //void AddFloorTooltip(Floor floor, string _detail)
    //{
    //    var tool = GetButton((int)Floor.Floor_0).gameObject.GetOrAddComponent<UI_Tooltip>();
    //    tool.SetTooltipContents("", Data.detail, UI_TooltipBox.ShowPosition.LeftDown);
    //}





    void Floor_ClickEvent(int floorIndex)
    {
        Managers.UI.ClosePopupPickType(typeof(UI_TooltipBox));

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

        if (UserData.Instance.FileConfig.PlayRounds == 1 && Main.Instance.Turn < 4)
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
                    GuildManager.Instance.AddInstanceGuildNPC(GuildNPC_LabelName.Lightning);
                    break;

                case Define.DungeonFloor.Floor_5:
                    EventManager.Instance.EntranceMove_4to5();
                    GuildManager.Instance.AddInstanceGuildNPC(GuildNPC_LabelName.Lightning);
                    EventManager.Instance.Add_GuildQuest_Special(12001, false);
                    break;

                case Define.DungeonFloor.Floor_6:
                    break;

                case Define.DungeonFloor.Floor_7:
                    break;
            }

            yield return new WaitUntil(() => Managers.Dialogue.GetState() == DialogueManager.DialogueState.None);
            FindObjectOfType<UI_Management>().DungeonEdit_Refresh();
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
