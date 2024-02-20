using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance_Egg : Facility
{
    public override FacilityType Type { get; set; }
    public override int InteractionOfTimes { get; set; }
    public override string Name { get; set; }

    public override void FacilityInit()
    {
        InteractionOfTimes = 10000;
        Type = FacilityType.Portal;
        Name = "��й�";
        Detail_KR = "������ ������ �������� �̵��� �� �ִ� ���Ա��Դϴ�.";
        Name_prefab = this.GetType().Name;
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        Cor_Facility = StartCoroutine(CustomEvent(npc, 1, "��й����� �̵���...", ap: 1, mp: 3, hp: 0));


        return Cor_Facility;
    }

    public override Coroutine NPC_Interaction_Portal(NPC npc, out int floor)
    {
        Cor_Facility = StartCoroutine(FacilityEvent(npc, 1, "��й����� �̵���...", ap: 1, mp: 3, hp: 0));
        floor = 3;

        return Cor_Facility;
    }


    protected IEnumerator CustomEvent(NPC npc, float durationTime, string text, int ap = 0, int mp = 0, int hp = 0)
    {
        UI_EventBox.AddEventText($"��{npc.Name_KR} (��)�� {PlacementInfo.Place_Floor.Name_KR}���� {text}");

        yield return new WaitForSeconds(durationTime);

        int applyMana = Mathf.Clamp(mp, 0, npc.Mana); //? ���� ����ȸ������ npc�� ���� ���� �̻����� ���� ����. - �޹��� ������

        npc.ActionPoint -= ap;
        npc.Mana -= applyMana;
        npc.HP -= hp;

        GameManager.Placement.PlacementClear(npc);
        //? �Ա����� ��ȯ
        var info_Exit = new PlacementInfo(Main.Instance.Floor[3], Main.Instance.Floor[3].Exit.PlacementInfo.Place_Tile);
        GameManager.Placement.PlacementConfirm(npc, info_Exit);

        Cor_Facility = null;
    }

}
