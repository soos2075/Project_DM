using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster
{

    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Slime");
        StartCoroutine(Init_Evolution());
    }
    IEnumerator Init_Evolution()
    {
        yield return new WaitForEndOfFrame();
        //Debug.Log(EvolutionState);
        if (EvolutionState == Evolution.None)
        {
            EvolutionState = Evolution.Ready;
        }
        else if (EvolutionState == Evolution.Complete)
        {
            EvolutionComplete();
        }
    }

    public override void LevelUpEvent(LevelUpEventType levelUpType)
    {
        if (EvolutionState == Evolution.Ready && LV + 1 >= Data.maxLv)
        {
            EvolutionState = Evolution.Progress;
            EventManager.Instance.GuildQuestAdd.Add(1100);
        }
    }


    public override void BattleEvent(BattleField.BattleResult result, NPC npc)
    {
        base.BattleEvent(result, npc);
        switch (result)
        {
            case BattleField.BattleResult.Nothing:
                break;

            case BattleField.BattleResult.Monster_Die:
                break;

            case BattleField.BattleResult.NPC_Die:
                if (npc.name == $"Hunter_{name}")
                {
                    //? 진화진행
                    EvolutionState = Evolution.Complete;
                    EventManager.Instance.RemoveQuestAction(1100);
                    EvolutionComplete();
                }
                break;
        }
    }



    void EvolutionComplete()
    {
        var ui = Managers.UI.ShowPopUp<UI_StatusUp>("Monster/UI_StatusUp");
        ui.TargetMonster(this);
        ui.StateText = "슬라임 -> 블러디 슬라임 진화!!" +
            "";
        Data = GameManager.Monster.GetData("BloodySlime");
        Initialize_Status();
        GameManager.Monster.ChangeSLA(this, "BloodyJelly");
        //Debug.Log("슬라임 진화완료");
    }

}
