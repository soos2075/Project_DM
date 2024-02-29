using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Trap : Facility
{
    public override FacilityEventType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }
    public override int OptionIndex { get { return ((int)trapType); } set { trapType = (TrapType)value; } }

    public override void FacilityInit()
    {
        Type = FacilityEventType.NPC_Event;
        Name_prefab = name;

        trap_Anim = GetComponentInChildren<Animator>();
        TrapInit();
    }


    public enum TrapType
    {
        Fallen_1 = 0,
        Fallen_2 = 1,
        Awl_1 = 2,
    }

    public TrapType trapType { get; set; } = TrapType.Fallen_1;
    float durationTime;
    int ap_value;
    int mp_value;
    int hp_value;
    void TrapInit()
    {
        var SLA = GetComponentInChildren<SpriteResolver>();
        SLA.SetCategoryAndLabel(trapType.ToString(), "Ready");

        switch (trapType)
        {
            case TrapType.Fallen_1:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 2;
                }

                Name = "낙하 함정";
                Detail_KR = "뻔히 보이는 함정입니만, 걸린다면 꽤나 효과가 있을거에요.";
                durationTime = 5;
                ap_value = 2;
                mp_value = 0;
                hp_value = 10;
                break;

            case TrapType.Fallen_2:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 5;
                }

                Name = "강화 낙하 함정";
                Detail_KR = "뻔히 보이는 함정입니만, 걸린다면 꽤나 효과가 있을거에요. 좀 더 여러번 사용 가능합니다.";
                durationTime = 5;
                ap_value = 4;
                mp_value = 0;
                hp_value = 12;
                break;

            case TrapType.Awl_1:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 3;
                }

                Name = "송곳 함정";
                Detail_KR = "무시무시한 송곳이 올라오는 함정이에요. 모르고 위를 지나간다면 크게 다칠거에요.";
                durationTime = 5;
                ap_value = 3;
                mp_value = 0;
                hp_value = 25;
                break;
        }
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

    protected override void OverCor(NPC npc)
    {
        base.OverCor(npc);

        trap_Anim.enabled = false;
        //npc.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        npc.State = npc.StateRefresh();
    }


}
