using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mineral : Facility
{
    public override FacilityType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }
    public override int OptionIndex { get { return ((int)mineralType); } set { mineralType = (MineralType)value; } }

    public override void FacilityInit()
    {
        Type = FacilityType.Mineral;
        Name_prefab = name;

        Init_Mineral();
    }

    public Sprite[] mineralSprites;

    public enum MineralType
    {
        Rock = 0,
        Sand = 1,
        Stone = 2,
        Iron = 3,
        Coal = 4,
        Diamond = 5,
    }
    public MineralType mineralType { get; set; } = MineralType.Rock;

    float durationTime;
    int ap_value;
    int mp_value;
    int hp_value;

    void Init_Mineral()
    {

        GetComponentInChildren<SpriteRenderer>().sprite = mineralSprites[(int)mineralType];

        switch (mineralType)
        {
            case MineralType.Rock:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 3;
                }

                Name = "���� ����";
                Detail_KR = "���� ������ �����Դϴ�. �׷����� ������ �����ʹ� ������ �ٸ� �����̿���.";
                durationTime = 2;
                ap_value = 1;
                mp_value = 3;
                hp_value = 0;
                break;

            case MineralType.Sand:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 4;
                }

                Name = "�𷡹���";
                Detail_KR = "���� �μ��� �𷡰� �Ǵ� ��������. ������ ������ �־� ���� �������� ���� �� ���� ���ӻ��� ���ƿ�.";
                durationTime = 1;
                ap_value = 1;
                mp_value = 4;
                hp_value = 0;
                break;

            case MineralType.Stone:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 5;
                }

                Name = "�ܴ��� ����";
                Detail_KR = "�ܴ��� �����Դϴ�. ưư�ؼ� �μ����� �ϳ� ���� ü���� �ʿ��� �ſ���. �������� ���Ǿ� �αⰡ �����ϴ�.";
                durationTime = 3;
                ap_value = 1;
                mp_value = 3;
                hp_value = 0;
                break;

            case MineralType.Iron:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 3;
                }

                Name = "���� ö����";
                Detail_KR = "������ ���� ö�� �����ϰ� �ִ� ö�����Դϴ�. ���⸦ ���� ���� ���̰� ���� ��ű����� ������.";
                durationTime = 5;
                ap_value = 2;
                mp_value = 15;
                hp_value = 0;
                break;

            case MineralType.Coal:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 5;
                }

                Name = "��ź";
                Detail_KR = "���� �� �ٴ� ��ź�Դϴ�. ������ ���� ��ź�� ȭ�µ� �����µ� �ξ� ��� ������ ���󿡼� ��� �����Դϴ�.";
                durationTime = 3;
                ap_value = 2;
                mp_value = 7;
                hp_value = 0;
                break;

            case MineralType.Diamond:
                if (InteractionOfTimes <= 0)
                {
                    InteractionOfTimes = 2;
                }

                Name = "���̾Ƹ��";
                Detail_KR = "�ſ� ���� ������ ���̾Ƹ���Դϴ�. ������ ������ ���⶧���� ���������� �ٽ� �κ��̿���. �ſ� �ܴ��ϱ� ������ ĳ�⵵ ������.";
                durationTime = 5;
                ap_value = 3;
                mp_value = 20;
                hp_value = 0;
                break;
        }
    }


    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(FacilityEvent(npc, durationTime, "���� ä����...", ap: ap_value, mp: mp_value, hp: hp_value));
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
