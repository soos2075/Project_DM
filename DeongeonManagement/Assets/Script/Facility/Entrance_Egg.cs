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
        Name = "비밀문";
        Detail_KR = "던전의 숨겨진 공간으로 이동할 수 있는 출입구입니다.";
        Name_prefab = this.GetType().Name;
    }

    public override Coroutine NPC_Interaction(NPC npc)
    {
        Cor_Facility = StartCoroutine(CustomEvent(npc, 1, "비밀방으로 이동중...", ap: 1, mp: 3, hp: 0));


        return Cor_Facility;
    }

    public override Coroutine NPC_Interaction_Portal(NPC npc, out int floor)
    {
        Cor_Facility = StartCoroutine(FacilityEvent(npc, 1, "비밀방으로 이동중...", ap: 1, mp: 3, hp: 0));
        floor = 3;

        return Cor_Facility;
    }


    protected IEnumerator CustomEvent(NPC npc, float durationTime, string text, int ap = 0, int mp = 0, int hp = 0)
    {
        UI_EventBox.AddEventText($"●{npc.Name_KR} (이)가 {PlacementInfo.Place_Floor.Name_KR}에서 {text}");

        yield return new WaitForSeconds(durationTime);

        int applyMana = Mathf.Clamp(mp, 0, npc.Mana); //? 높은 마나회수여도 npc가 가진 마나 이상으로 얻진 못함. - 앵벌이 방지용

        npc.ActionPoint -= ap;
        npc.Mana -= applyMana;
        npc.HP -= hp;

        GameManager.Placement.PlacementClear(npc);
        //? 입구에서 소환
        var info_Exit = new PlacementInfo(Main.Instance.Floor[3], Main.Instance.Floor[3].Exit.PlacementInfo.Place_Tile);
        GameManager.Placement.PlacementConfirm(npc, info_Exit);

        Cor_Facility = null;
    }

}
