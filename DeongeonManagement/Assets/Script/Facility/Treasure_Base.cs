using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure_Base : Facility
{
    public override FacilityType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }
    public override int OptionIndex { get { return ((int)treasureType); } set { treasureType = (TreasureType)value; } }



    public enum TreasureType
    {
        sword,
        ring,
        coin,
        scroll,
    }
    public TreasureType treasureType;


    public override void FacilityInit()
    {
        Type = FacilityType.Treasure;
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
            Cor_Facility = StartCoroutine(FacilityEvent(npc, 5, "보물 발견중...", ap: 1, mp: 0, hp: 0));
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}의 이벤트 횟수없음");
            return null;
        }
    }
}
