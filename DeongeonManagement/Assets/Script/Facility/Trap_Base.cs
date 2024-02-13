using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Base : Facility
{
    public override FacilityType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }

    public override void FacilityInit()
    {
        Type = FacilityType.Trap;
        InteractionOfTimes = 1;
        Name = "발밑 함정";
        Detail_KR = "뻔히 보이는 함정입니만, 피해갈 수 없다면 효과가 있을거에요.";
    }


    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(FacilityEvent(npc, 5, "함정에 빠짐...", ap: 1, mp: 0, hp: 5));
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}의 이벤트 횟수없음");
            return null;
        }
    }






}
