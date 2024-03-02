using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Herb : Facility
{
    public override FacilityEventType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }
    public override int OptionIndex { get { return ((int)herbType); } set { herbType = (HerbType)value; } }

    public override void FacilityInit()
    {
        Type = FacilityEventType.NPC_Interaction;
        Name_prefab = name;

        Init_Herb();
    }

    public enum HerbType
    {
        Low = 0,
        Pumpkin = 1,
        High = 2,
    }
    public HerbType herbType { get; set; } = HerbType.Low;

    float durationTime;
    int ap_value;
    int mp_value;
    int hp_value;


    void Init_Herb()
    {
        var SLA = GetComponentInChildren<SpriteResolver>();
        SLA.SetCategoryAndLabel(herbType.ToString(), "Entry");

        switch (herbType)
        {
            case HerbType.Low:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 1;
                }
                Name = "�ϱ� ����";
                Detail_KR = "���� �����Դϴ�. �ҷ������� ������ �ӱݰ� �ְ� ���������� ���� �� �־� �α�� �����ϴ�.";
                durationTime = 3;
                ap_value = 1;
                mp_value = 9;
                hp_value = 0;
                break;

            case HerbType.Pumpkin:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 3;
                }
                Name = "���� ��Ų";
                Detail_KR = "������ ���� ȣ���Դϴ�. �ְ�� ������ ���Ǹ� ��Ÿ ���� �ռ����� ���˴ϴ�.";
                durationTime = 2;
                ap_value = 1;
                mp_value = 12;
                hp_value = 0;
                break;

            case HerbType.High:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 3;
                }
                Name = "��� ����";
                Detail_KR = "�� ���� �����Դϴ�. ��� ������ ���� ���ȴٰ� �ϳ׿�.";
                durationTime = 3;
                ap_value = 1;
                mp_value = 35;
                hp_value = 0;
                break;
        }
    }



    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            int changeMP = mp_value;

            if (npc.GetType() == typeof(Herbalist))
            {
                changeMP = mp_value;
            }
            else if(npc.GetType() == typeof(Miner))
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
