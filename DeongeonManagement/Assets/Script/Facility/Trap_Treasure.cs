using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Trap_Treasure : Treasure
{


    public enum Mimic
    {
        Treasure_Silver = 3260,
        Treasure_Gold = 3261,
    }

    public Mimic MimicType { get; set; } = Mimic.Treasure_Silver;

    Animator trap_Anim;
    SpriteResolver SLA;

    public override void Init_Personal()
    {
        MimicType = (Mimic)CategoryIndex;

        trap_Anim = GetComponentInChildren<Animator>();
        SLA = GetComponentInChildren<SpriteResolver>();
        SLA.SetCategoryAndLabel(MimicType.ToString(), "Entry");

        trap_Anim.Play($"{MimicType}");
    }


    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            InteractionOfTimes--;
            Main.Instance.CurrentStatistics.Interaction_Trap++;

            Cor_Facility = StartCoroutine(TreasureTrap(npc));

            trap_Anim.Play($"{MimicType}_Action");

            npc.GetComponentInChildren<Animator>().Play("Trap");
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}의 이벤트 횟수없음");
            return null;
        }
    }

    IEnumerator TreasureTrap(NPC npc)
    {
        UI_EventBox.AddEventText($"●{npc.Name_Color} {UserData.Instance.LocaleText("Event_Trap")}");
        PlacementState = PlacementState.Busy;

        bool isLastInteraction = false;
        if (InteractionOfTimes <= 0)
        {
            isLastInteraction = true;
        }

        yield return new WaitForSeconds(durationTime);

        ap_value += Mathf.RoundToInt(ap_value * (GameManager.Buff.EffectUp_Trap * 0.01f));
        hp_value += Mathf.RoundToInt(hp_value * (GameManager.Buff.EffectUp_Trap * 0.01f));

        npc.Change_ActionPoint(-ap_value);
        npc.Change_HP(-hp_value);


        OverCor(npc, isLastInteraction);
    }

    protected override void OverCor(NPC npc, bool isRemove)
    {
        base.OverCor(npc, isRemove);

        trap_Anim.Play($"{MimicType}");

        Main.Instance.ShowDM(-hp_value, Main.TextType.hp, npc.transform);
        npc.State = npc.StateRefresh();
    }



    public override void MouseClickEvent()
    {
        if (Main.Instance.Management == false) return;

        var ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
        ui.SetText($"[{Name}] {UserData.Instance.LocaleText("Confirm_Remove")}", () => GameManager.Facility.RemoveFacility(this));
    }



}
