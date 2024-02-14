using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Base : Facility
{
    public override FacilityType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }


    public enum TrapType
    {
        Fallen_1,
        Fallen_2,
        Awl_1,
    }

    public TrapType trapType { get; set; } = TrapType.Fallen_1;
    float durationTime;
    int ap_value;
    int mp_value;
    int hp_value;
    public override void FacilityInit()
    {
        Type = FacilityType.Trap;

        switch (trapType)
        {
            case TrapType.Fallen_1:
                InteractionOfTimes = 2;
                Name = "발밑 함정";
                Detail_KR = "뻔히 보이는 함정입니만, 걸린다면 꽤나 효과가 있을거에요.";
                durationTime = 5;
                ap_value = 2;
                mp_value = 0;
                hp_value = 5;
                break;

            case TrapType.Fallen_2:
                InteractionOfTimes = 5;
                Name = "강화 발밑 함정";
                Detail_KR = "뻔히 보이는 함정입니만, 걸린다면 꽤나 효과가 있을거에요. 좀 더 효과를 개량했어요.";
                durationTime = 5;
                ap_value = 4;
                mp_value = 0;
                hp_value = 10;
                break;

            case TrapType.Awl_1:
                InteractionOfTimes = 5;
                Name = "송곳 함정";
                Detail_KR = "무시무시한 송곳이 올라오는 함정이에요. 모르고 위를 지나간다면 큰코다칠거에요.";
                durationTime = 5;
                ap_value = 3;
                mp_value = 0;
                hp_value = 15;
                break;
        }
    }


    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(FacilityEvent(npc, durationTime, "함정에 빠짐...", ap: ap_value, mp: mp_value, hp: hp_value));
            GetComponentInChildren<Animator>().Play(Define.ANIM_interaction);
            npc.GetComponent<SpriteRenderer>().color = Define.Color_White;
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}의 이벤트 횟수없음");
            return null;
        }
    }

    protected override void OverCor(NPC npc)
    {
        GetComponentInChildren<Animator>().Play(Define.ANIM_idle);
        npc.GetComponent<SpriteRenderer>().color = Color.white;
    }





}
