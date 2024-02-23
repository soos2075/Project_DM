using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        switch (trapType)
        {
            case TrapType.Fallen_1:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 2;
                }

                Name = "���� ����";
                Detail_KR = "���� ���̴� �����Դϸ�, �ɸ��ٸ� �ϳ� ȿ���� �����ſ���.";
                durationTime = 5;
                ap_value = 2;
                mp_value = 0;
                hp_value = 5;
                break;

            case TrapType.Fallen_2:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 5;
                }

                Name = "��ȭ ���� ����";
                Detail_KR = "���� ���̴� �����Դϸ�, �ɸ��ٸ� �ϳ� ȿ���� �����ſ���. �� �� ȿ���� �����߾��.";
                durationTime = 5;
                ap_value = 4;
                mp_value = 0;
                hp_value = 10;
                break;

            case TrapType.Awl_1:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 5;
                }

                Name = "�۰� ����";
                Detail_KR = "���ù����� �۰��� �ö���� �����̿���. �𸣰� ���� �������ٸ� ũ�� ��ĥ�ſ���.";
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
            Cor_Facility = StartCoroutine(FacilityEvent(npc, durationTime, "������ ����...", ap: ap_value, mp: mp_value, hp: hp_value));
            GetComponentInChildren<Animator>().Play(Define.ANIM_interaction);
            npc.GetComponent<SpriteRenderer>().color = Define.Color_White;
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}�� �̺�Ʈ Ƚ������");
            return null;
        }
    }

    protected override void OverCor(NPC npc)
    {
        GetComponentInChildren<Animator>().Play(Define.ANIM_idle);
        npc.GetComponent<SpriteRenderer>().color = Color.white;
    }


}
