using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Trap : Facility
{

    public override void Init_Personal()
    {
        trapType = (TrapType)CategoryIndex;

        trap_Anim = GetComponentInChildren<Animator>();
        TrapInit();
    }


    public enum TrapType
    {
        Fallen_1 = 3200,
        Fallen_2 = 3201,
        Awl_1 = 3202,
    }

    public TrapType trapType { get; set; } = TrapType.Fallen_1;
    void TrapInit()
    {
        var SLA = GetComponentInChildren<SpriteResolver>();
        SLA.SetCategoryAndLabel(trapType.ToString(), "Ready");
    }

    Animator trap_Anim;

    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(FacilityEvent(npc, durationTime, UserData.Instance.LocaleText("Event_Trap"), 
                ap: ap_value, mp: mp_value, hp: hp_value));
            trap_Anim.enabled = true;
            trap_Anim.Play(trapType.ToString());

            npc.GetComponentInChildren<Animator>().Play("Trap");
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}의 이벤트 횟수없음");
            npc.State = npc.StateRefresh();
            return null;
        }
    }

    protected override void OverCor(NPC npc, bool isRemove)
    {
        trap_Anim.enabled = false;

        //npc.GetComponentInChildren<Animator>().Play("Running");

        Main.Instance.ShowDM(-hp_value, Main.TextType.hp, npc.transform);
        npc.State = npc.StateRefresh();

        base.OverCor(npc, isRemove);
    }





    #region 퍼실리티 클릭이벤트 (제거)
    //? 함정 가격 : 40 / 110 / 260
    int ReturnGold(TrapType _type)
    {
        switch (_type)
        {
            case TrapType.Fallen_1:
                return 20;

            case TrapType.Fallen_2:
                return 55;

            case TrapType.Awl_1:
                return 130;
        }
        return 0;
    }


    public override void MouseClickEvent()
    {
        if (Main.Instance.Management == false) return;

        var ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
        ui.SetText($"[{Name}] {UserData.Instance.LocaleText("Confirm_Remove")}" +
            $"\n(+{ReturnGold(trapType)} {UserData.Instance.LocaleText("Gold")})",
            () => YesAction());
    }

    void YesAction()
    {
        Main.Instance.CurrentDay.AddGold(ReturnGold(trapType), Main.DayResult.EventType.Facility);
        GameManager.Facility.RemoveFacility(this);
    }

    #endregion
}
