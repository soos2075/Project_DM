using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Wells : Facility, IWall
{
    public override void Init_Personal()
    {
        Init();
    }

    void Init()
    {
        SLA = GetComponentInChildren<SpriteResolver>();
        Wells_SpriteSwap();
    }

    SpriteResolver SLA;
    void Wells_SpriteSwap()
    {
        if (InteractionOfTimes == 0)
        {
            SLA.SetCategoryAndLabel("Wells", "Wells_4");
        }
        else if (InteractionOfTimes < 5)
        {
            SLA.SetCategoryAndLabel("Wells", "Wells_3");
        }
        else if (InteractionOfTimes < 9)
        {
            SLA.SetCategoryAndLabel("Wells", "Wells_2");
        }
        else
        {
            SLA.SetCategoryAndLabel("Wells", "Wells_1");
        }
    }



    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (npc.isWellsCheck)
        {
            Debug.Log($"�̹� �칰 ���� ����");
            if (npc.gameObject.activeInHierarchy)
            {
                npc.SetPriorityListForPublic();
            }
            return null;
        }

        if (InteractionOfTimes > 0)
        {
            Debug.Log("�칰�̺�Ʈ �߻�");

            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(FacilityEvent_Wells(npc, durationTime, UserData.Instance.LocaleText("Event_Mineral"),
                ap: ap_value, mp: mp_value, hp: hp_value));


            Wells_SpriteSwap();
            npc.isWellsCheck = true;
            return Cor_Facility;
        }
        else
        {//? ���� �̿�Ƚ���� ���, �̹� �Ѱɷ� �ľ��� �ٽ� ���ư�. �ȱ׷� �칰�տ��� ��� �ӹ����Եȴ�.
            npc.isWellsCheck = true;
            if (npc.gameObject.activeInHierarchy)
            {
                npc.SetPriorityListForPublic();
            }
            Debug.Log($"{Name}�� �̺�Ʈ Ƚ������");
            return null;
        }
    }

    protected IEnumerator FacilityEvent_Wells(NPC npc, float durationTime, string text, int ap = 0, int mp = 0, int hp = 0)
    {
        UI_EventBox.AddEventText($"��{npc.Name_Color} {text}");
        //? �̿� �ο��� ���� ����
        //PlacementState = PlacementState.Busy;
        yield return new WaitForSeconds(durationTime);

        npc.ActionPoint -= ap;
        npc.Mana -= mp;
        npc.HP -= hp;

        Main.Instance.CurrentDay.AddMana(10);
        Main.Instance.ShowDM(10, Main.TextType.mana, transform);

        OverCor_Wells(npc);
    }

    void OverCor_Wells(NPC npc)
    {
        Cor_Facility = null;
        //PlacementState = PlacementState.Standby;
        if (npc.gameObject.activeInHierarchy)
        {
            npc.SetPriorityListForPublic();
        }
    }



    public override void MouseClickEvent()
    {
        if (Main.Instance.Management == false) return;


        if (InteractionOfTimes > 0) return;


        var ui = Managers.UI.ShowPopUpAlone<UI_Confirm>();
        ui.SetText($"[{Name}] {"���Ƚ���� �����ұ��?"}");
        StartCoroutine(WaitForAnswer(ui));
    }


    IEnumerator WaitForAnswer(UI_Confirm confirm)
    {
        yield return new WaitUntil(() => confirm.GetAnswer() != UI_Confirm.State.Wait);

        if (confirm.GetAnswer() == UI_Confirm.State.Yes)
        {
            InteractionOfTimes = 12;
            Wells_SpriteSwap();
            //GameManager.Facility.RemoveFacility(this);
        }
    }


}