using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Mineral : Facility
{
    public override FacilityEventType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }
    public override int OptionIndex { get { return ((int)mineralType); } set { mineralType = (MienralCategory)value; } }

    public override void FacilityInit()
    {
        Type = FacilityEventType.NPC_Interaction;
        Name_prefab = name;

        Init_Mineral();
    }

    public enum MienralCategory
    {
        Rock = 0,
        Sand = 1,
        Stone = 2,
        Iron = 3,
        Coal = 4,
        Diamond = 5,
    }
    public MienralCategory mineralType { get; set; } = MienralCategory.Rock;

    float durationTime;
    int ap_value;
    int mp_value;
    int hp_value;

    void Init_Mineral()
    {
        var SLA = GetComponentInChildren<SpriteResolver>();
        SLA.SetCategoryAndLabel(mineralType.ToString(), "Entry");

        switch (mineralType)
        {
            case MienralCategory.Rock:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 3;
                }

                Name = "���� ����";
                Detail_KR = "���� ������ �����Դϴ�. �׷����� ������ �����ʹ� ������ �ٸ� �����̿���.";
                durationTime = 2;
                ap_value = 1;
                mp_value = 6;
                hp_value = 0;
                break;

            case MienralCategory.Sand:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 4;
                }

                Name = "�𷡹���";
                Detail_KR = "���� �μ��� �𷡰� �Ǵ� ��������. ������ ������ �־� ���� �������� ���� �� ���� ���ӻ��� ���ƿ�.";
                durationTime = 2;
                ap_value = 1;
                mp_value = 8;
                hp_value = 0;
                break;

            case MienralCategory.Stone:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 5;
                }

                Name = "�ܴ��� ����";
                Detail_KR = "�ܴ��� �����Դϴ�. ưư�ؼ� �μ����� �ϳ� ���� ü���� �ʿ��� �ſ���. �������� ���Ǿ� �αⰡ �����ϴ�.";
                durationTime = 2;
                ap_value = 1;
                mp_value = 12;
                hp_value = 0;
                break;

            case MienralCategory.Iron:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 10;
                }

                Name = "���� ö����";
                Detail_KR = "������ ���� ö�� �����ϰ� �ִ� ö�����Դϴ�. ���⸦ ���� ���� ���̰� ���� ��ű����� ������.";
                durationTime = 2;
                ap_value = 1;
                mp_value = 13;
                hp_value = 0;
                break;

            case MienralCategory.Coal:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 3;
                }

                Name = "��ź";
                Detail_KR = "���� �� �ٴ� ��ź�Դϴ�. ������ ���� ��ź�� ȭ�µ� �����µ� �ξ� ��� ������ ���󿡼� ��� �����Դϴ�.";
                durationTime = 3;
                ap_value = 2;
                mp_value = 28;
                hp_value = 0;
                break;

            case MienralCategory.Diamond:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 2;
                }

                Name = "���̾Ƹ��";
                Detail_KR = "�ſ� ���� ������ ���̾Ƹ���Դϴ�. ������ ������ ���⶧���� ���������� �ٽ� �κ��̿���. �ſ� �ܴ��ϱ� ������ ĳ�⵵ ������.";
                durationTime = 5;
                ap_value = 3;
                mp_value = 75;
                hp_value = 0;
                break;
        }
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
            Cor_Facility = StartCoroutine(FacilityEvent(npc, durationTime, "���� ä����...", ap: ap_value, mp: changeMP, hp: hp_value));
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}�� �̺�Ʈ Ƚ������");
            return null;
        }
    }



    public override void MouseClickEvent()
    {
        if (Main.Instance.Management == false) return;

        var ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
        ui.SetText($"[{Name}] ö���ұ��? ");
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
