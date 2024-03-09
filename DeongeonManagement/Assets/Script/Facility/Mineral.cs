using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Mineral : Facility
{
    public override void Init_Personal()
    {
        mineralType = (MineralCategory)OptionIndex;
        Init_Mineral();
    }

    public enum MineralCategory
    {
        Rock = 2100,
        Sand = 2101,
        Stone = 2102,
        Iron = 2103,
        Coal = 2104,
        Diamond = 2105,
    }
    public MineralCategory mineralType { get; set; } = MineralCategory.Rock;


    void Init_Mineral()
    {
        var SLA = GetComponentInChildren<SpriteResolver>();
        SLA.SetCategoryAndLabel(mineralType.ToString(), "Entry");
    }


    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            int changeMP = mp_value;

            if (npc.GetType() == typeof(Miner))
            {
                changeMP = mp_value;
            }
            else if (npc.GetType() == typeof(Herbalist))
            {
                changeMP = (int)(mp_value * 0.5f);
            }
            else if (npc.GetType() == typeof(Adventurer))
            {
                changeMP = (int)(mp_value * 0.3f);
            }
            else
            {
                changeMP = (int)(mp_value * 0.3f);
            }

            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(FacilityEvent(npc, durationTime, UserData.Instance.GetLocaleText("Event_Mineral"), 
                ap: ap_value, mp: changeMP, hp: hp_value));
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}의 이벤트 횟수없음");
            return null;
        }
    }



    public override void MouseClickEvent()
    {
        if (Main.Instance.Management == false) return;

        var ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
        ui.SetText($"[{Name}] {UserData.Instance.GetLocaleText("Confirm_Remove")}");
        StartCoroutine(WaitForAnswer(ui));
    }


    IEnumerator WaitForAnswer(UI_Confirm confirm)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            GameManager.Facility.RemoveFacility(this);
        }
    }

}
