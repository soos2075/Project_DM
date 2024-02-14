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
                Name = "�߹� ����";
                Detail_KR = "���� ���̴� �����Դϸ�, �ɸ��ٸ� �ϳ� ȿ���� �����ſ���.";
                durationTime = 5;
                ap_value = 2;
                mp_value = 0;
                hp_value = 5;
                break;

            case TrapType.Fallen_2:
                InteractionOfTimes = 5;
                Name = "��ȭ �߹� ����";
                Detail_KR = "���� ���̴� �����Դϸ�, �ɸ��ٸ� �ϳ� ȿ���� �����ſ���. �� �� ȿ���� �����߾��.";
                durationTime = 5;
                ap_value = 4;
                mp_value = 0;
                hp_value = 10;
                break;

            case TrapType.Awl_1:
                InteractionOfTimes = 5;
                Name = "�۰� ����";
                Detail_KR = "���ù����� �۰��� �ö���� �����̿���. �𸣰� ���� �������ٸ� ū�ڴ�ĥ�ſ���.";
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
