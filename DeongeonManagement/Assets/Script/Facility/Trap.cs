using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Trap : Facility
{

    public override void Init_Personal()
    {
        trapType = (TrapType)OptionIndex;

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
            Cor_Facility = StartCoroutine(FacilityEvent(npc, durationTime, "함정에 빠짐...", ap: ap_value, mp: mp_value, hp: hp_value));
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
        npc.State = npc.StateRefresh();

        base.OverCor(npc, isRemove);
    }


}
