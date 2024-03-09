using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Treasure : Facility
{
    public override void Init_Personal()
    {
        treasureType = (TreasureCategory)OptionIndex;

        CategorySelect();
    }

    public enum TreasureCategory
    {
        Sword = 2200,
        Ring = 2201,
        Hat = 2202,
        Scroll = 2203,
        Coin = 2204,
        Crown = 2205,

    }
    public TreasureCategory treasureType;

    void CategorySelect()
    {
        var SLA = GetComponentInChildren<SpriteResolver>();
        SLA.SetCategoryAndLabel(treasureType.ToString(), "Entry");
    }




    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(TreasureEvent(npc));
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}의 이벤트 횟수없음");
            return null;
        }
    }

    protected IEnumerator TreasureEvent(NPC npc)
    {
        UI_EventBox.AddEventText($"●{npc.Name_Color} {UserData.Instance.GetLocaleText("Event_Treasure")}");
        PlacementState = PlacementState.Busy;

        bool isLastInteraction = false;
        if (InteractionOfTimes <= 0)
        {
            isLastInteraction = true;
        }

        yield return new WaitForSeconds(durationTime);

        int changeMP = mp_value;
        int changeGold = gold_value;
        int changePop = pop_value;
        int changeDanger = danger_value;

        if (npc.GetType() != typeof(Adventurer))
        {
            changeMP = (int)(mp_value * 0.3f);
            changeGold = (int)(gold_value * 0.3f);
            changePop = (int)(pop_value * 0.3f);
            changeDanger = (int)(danger_value * 0.3f);
        }


        int applyMana = Mathf.Clamp(changeMP, 0, npc.Mana); //? 높은 마나회수여도 npc가 가진 마나 이상으로 얻진 못함. - 앵벌이 방지용

        npc.ActionPoint -= ap_value;
        npc.Mana -= applyMana;
        npc.HP -= hp_value;


        if (applyMana > 0)
        {
            Main.Instance.CurrentDay.AddMana(applyMana);
            var dm = Main.Instance.dm_small.Spawn(transform.position, $"+{applyMana} mana");
            dm.SetColor(Color.blue);
        }

        Main.Instance.CurrentDay.AddGold(changeGold);
        Main.Instance.CurrentDay.AddPop(changePop);
        Main.Instance.CurrentDay.AddDanger(changeDanger);

        OverCor(npc, isLastInteraction);
    }
}
