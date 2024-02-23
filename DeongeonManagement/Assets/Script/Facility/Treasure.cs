using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : Facility
{
    public override FacilityEventType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }
    public override int OptionIndex { get { return ((int)treasureType); } set { treasureType = (TreasureType)value; } }


    public Sprite[] treasureSprites;
    public enum TreasureType
    {
        sword,
        ring,
        coin,
        scroll,
    }
    public TreasureType treasureType;

    float durationTime;
    int ap_value;
    int mp_value;
    int gold_value;
    int hp_value;
    int pop_value;
    int danger_value;


    public override void FacilityInit()
    {
        Type = FacilityEventType.NPC_Interaction;
        InteractionOfTimes = 1;
        Name = "보물";
        Detail_KR = "모험가들이 던전을 탐색하는 가장 큰 이유가 되는 보물입니다.";

        if (InteractionOfTimes <= 0)
        {
            InteractionOfTimes = 1;
        }
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
        UI_EventBox.AddEventText($"●{npc.Name_KR} (이)가 {PlacementInfo.Place_Floor.Name_KR}에서 {"보물 탐색중..."}");
        PlacementState = PlacementState.Busy;
        yield return new WaitForSeconds(durationTime);

        int changeMP = mp_value;
        int changePop = pop_value;

        if (npc.GetType() != typeof(Adventurer))
        {
            changeMP = (int)(mp_value * 0.3f);
            changePop = (int)(pop_value * 0.3f);
        }


        int applyMana = Mathf.Clamp(changeMP, 0, npc.Mana); //? 높은 마나회수여도 npc가 가진 마나 이상으로 얻진 못함. - 앵벌이 방지용

        npc.ActionPoint -= ap_value;
        npc.Mana -= applyMana;
        npc.HP -= hp_value;


        if (applyMana > 0)
        {
            Main.Instance.CurrentDay.AddMana(applyMana);
            var dm = Main.Instance.dmMesh_dungeon.Spawn(transform.position, $"+{applyMana} mana");
            dm.SetColor(Color.blue);
        }

        Main.Instance.CurrentDay.AddGold(gold_value);
        Main.Instance.CurrentDay.AddPop(pop_value);
        Main.Instance.CurrentDay.AddDanger(danger_value);

        OverCor(npc);
    }
}
