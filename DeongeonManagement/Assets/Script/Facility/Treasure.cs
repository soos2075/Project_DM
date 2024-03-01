using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Treasure : Facility
{
    public override FacilityEventType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }
    public override int OptionIndex { get { return ((int)treasureType); } set { treasureType = (TreasureCategory)value; } }

    public enum TreasureCategory
    {
        Sword,
        Ring,
        Hat,
        Scroll,
        Coin,
        Crown,

    }
    public TreasureCategory treasureType;

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
        Name_prefab = name;
        InteractionOfTimes = 1;
        durationTime = 5;
        ap_value = 3;
        mp_value = 60;
        gold_value = 0;
        hp_value = 0;
        pop_value = 2;
        danger_value = 0;

        CategorySelect();
    }

    void CategorySelect()
    {
        var SLA = GetComponentInChildren<SpriteResolver>();
        SLA.SetCategoryAndLabel(treasureType.ToString(), "Entry");

        switch (treasureType)
        {
            case TreasureCategory.Sword:
                Name = "골든 소드";
                Detail_KR = "모험가들이 던전을 탐색하는 가장 큰 이유가 되는 보물입니다.";
                ap_value = 3;
                mp_value = 60;
                pop_value = 2;
                break;

            case TreasureCategory.Ring:
                Name = "매직 링";
                Detail_KR = "모험가들이 던전을 탐색하는 가장 큰 이유가 되는 보물입니다.";
                ap_value = 5;
                mp_value = 100;
                pop_value = 8;
                break;

            case TreasureCategory.Hat:
                Name = "마법사의 모자";
                Detail_KR = "모험가들이 던전을 탐색하는 가장 큰 이유가 되는 보물입니다.";
                ap_value = 5;
                mp_value = 200;
                pop_value = 5;
                break;

            case TreasureCategory.Scroll:
                Name = "마법 스크롤";
                Detail_KR = "모험가들이 던전을 탐색하는 가장 큰 이유가 되는 보물입니다.";
                ap_value = 5;
                mp_value = 200;
                pop_value = 10;
                danger_value = 5;
                break;

            case TreasureCategory.Coin:
                Name = "던전 금화";
                Detail_KR = "모험가들이 던전을 탐색하는 가장 큰 이유가 되는 보물입니다.";
                ap_value = 2;
                mp_value = 150;
                break;

            case TreasureCategory.Crown:
                Name = "황금 왕관";
                Detail_KR = "모험가들이 던전을 탐색하는 가장 큰 이유가 되는 보물입니다.";
                ap_value = 7;
                mp_value = 300;
                pop_value = 5;
                break;
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
