using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : Facility
{
    public override FacilityEventType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }
    public override int OptionIndex { get { return ((int)treasureType); } set { treasureType = (TreasureType)value; } }


    public Sprite[] treasureSprites;
    public enum TreasureType
    {
        sword,
        ring,
        coin,
        scroll,
    }
    public TreasureType treasureType;

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
        InteractionOfTimes = 1;
        Name = "����";
        Detail_KR = "���谡���� ������ Ž���ϴ� ���� ū ������ �Ǵ� �����Դϴ�.";

        if (InteractionOfTimes <= 0)
        {
            InteractionOfTimes = 1;
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
        yield return new WaitForSeconds(durationTime);

        int changeMP = mp_value;
        int changePop = pop_value;

        if (npc.GetType() != typeof(Adventurer))
        {
            changeMP = (int)(mp_value * 0.3f);
            changePop = (int)(pop_value * 0.3f);
        }


        int applyMana = Mathf.Clamp(changeMP, 0, npc.Mana); //? ���� ����ȸ������ npc�� ���� ���� �̻����� ���� ����. - �޹��� ������

        npc.ActionPoint -= ap_value;
        npc.Mana -= applyMana;
        npc.HP -= hp_value;


        if (applyMana > 0)
        {
            Main.Instance.CurrentDay.AddMana(applyMana);
            var dm = Main.Instance.dmMesh_dungeon.Spawn(transform.position, $"+{applyMana} mana");
            dm.SetColor(Color.blue);
        }

        Main.Instance.CurrentDay.AddGold(gold_value);
        Main.Instance.CurrentDay.AddPop(pop_value);
        Main.Instance.CurrentDay.AddDanger(danger_value);

        OverCor(npc);
    }
}
