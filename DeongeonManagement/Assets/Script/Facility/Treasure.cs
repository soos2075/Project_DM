using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class Treasure : Facility
{
    public override FacilityEventType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }
    public override int OptionIndex { get { return ((int)treasureType); } set { treasureType = (TreasureCategory)value; } }

    public enum TreasureCategory
    {
        Sword,
        Ring,
        Hat,
        Scroll,
        Coin,
        Crown,

    }
    public TreasureCategory treasureType;

    float durationTime;
    int ap_value;
    int mp_value;
    int gold_value;
    int hp_value;
    int pop_value;
    int danger_value;


    public override void FacilityInit()
    {
        Type = FacilityEventType.NPC_Interaction;
        Name_prefab = name;
        InteractionOfTimes = 1;
        durationTime = 5;
        ap_value = 3;
        mp_value = 60;
        gold_value = 0;
        hp_value = 0;
        pop_value = 2;
        danger_value = 0;

        CategorySelect();
    }

    void CategorySelect()
    {
        var SLA = GetComponentInChildren<SpriteResolver>();
        SLA.SetCategoryAndLabel(treasureType.ToString(), "Entry");

        switch (treasureType)
        {
            case TreasureCategory.Sword:
                Name = "��� �ҵ�";
                Detail_KR = "���谡���� ������ Ž���ϴ� ���� ū ������ �Ǵ� �����Դϴ�.";
                ap_value = 3;
                mp_value = 60;
                pop_value = 2;
                break;

            case TreasureCategory.Ring:
                Name = "���� ��";
                Detail_KR = "���谡���� ������ Ž���ϴ� ���� ū ������ �Ǵ� �����Դϴ�.";
                ap_value = 5;
                mp_value = 100;
                pop_value = 8;
                break;

            case TreasureCategory.Hat:
                Name = "�������� ����";
                Detail_KR = "���谡���� ������ Ž���ϴ� ���� ū ������ �Ǵ� �����Դϴ�.";
                ap_value = 5;
                mp_value = 200;
                pop_value = 5;
                break;

            case TreasureCategory.Scroll:
                Name = "���� ��ũ��";
                Detail_KR = "���谡���� ������ Ž���ϴ� ���� ū ������ �Ǵ� �����Դϴ�.";
                ap_value = 5;
                mp_value = 200;
                pop_value = 10;
                danger_value = 5;
                break;

            case TreasureCategory.Coin:
                Name = "���� ��ȭ";
                Detail_KR = "���谡���� ������ Ž���ϴ� ���� ū ������ �Ǵ� �����Դϴ�.";
                ap_value = 2;
                mp_value = 150;
                break;

            case TreasureCategory.Crown:
                Name = "Ȳ�� �հ�";
                Detail_KR = "���谡���� ������ Ž���ϴ� ���� ū ������ �Ǵ� �����Դϴ�.";
                ap_value = 7;
                mp_value = 300;
                pop_value = 5;
                break;
        }
    }




    public override Coroutine NPC_Interaction(NPC npc)
    {
        if (InteractionOfTimes > 0)
        {
            InteractionOfTimes--;
            Cor_Facility = StartCoroutine(TreasureEvent(npc));
            return Cor_Facility;
        }
        else
        {
            Debug.Log($"{Name}�� �̺�Ʈ Ƚ������");
            return null;
        }
    }

    protected IEnumerator TreasureEvent(NPC npc)
    {
        UI_EventBox.AddEventText($"��{npc.Name_KR} (��)�� {PlacementInfo.Place_Floor.Name_KR}���� {"���� Ž����..."}");
        PlacementState = PlacementState.Busy;

        bool isLastInteraction = false;
        if (InteractionOfTimes <= 0)
        {
            isLastInteraction = true;
        }

        yield return new WaitForSeconds(durationTime);

        int changeMP = mp_value;
        int changeGold = gold_value;
        int changePop = pop_value;
        int changeDanger = danger_value;

        if (npc.GetType() != typeof(Adventurer))
        {
            changeMP = (int)(mp_value * 0.3f);
            changeGold = (int)(gold_value * 0.3f);
            changePop = (int)(pop_value * 0.3f);
            changeDanger = (int)(danger_value * 0.3f);
        }


        int applyMana = Mathf.Clamp(changeMP, 0, npc.Mana); //? ���� ����ȸ������ npc�� ���� ���� �̻����� ���� ����. - �޹��� ������

        npc.ActionPoint -= ap_value;
        npc.Mana -= applyMana;
        npc.HP -= hp_value;


        if (applyMana > 0)
        {
            Main.Instance.CurrentDay.AddMana(applyMana);
            var dm = Main.Instance.dm_small.Spawn(transform.position, $"+{applyMana} mana");
            dm.SetColor(Color.blue);
        }

        Main.Instance.CurrentDay.AddGold(changeGold);
        Main.Instance.CurrentDay.AddPop(changePop);
        Main.Instance.CurrentDay.AddDanger(changeDanger);

        OverCor(npc, isLastInteraction);
    }
}
