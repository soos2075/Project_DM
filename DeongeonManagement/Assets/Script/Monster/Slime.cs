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
            EvolutionComplete(false);
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
                    EvolutionComplete(true);
                }
                break;
        }
    }



    void EvolutionComplete(bool showUI)
    {
        string slime = Data.labelName;
        Data = GameManager.Monster.GetData("BloodySlime");
        string bloody = Data.labelName;

        if (showUI)
        {
            EvolutionUI(slime, bloody);
        }


        Initialize_Status();
        GameManager.Monster.ChangeSLA(this, "BloodyJelly");
        //Debug.Log("슬라임 진화완료");
    }

    void EvolutionUI(string _origin, string _Evolution)
    {
        var ui = Managers.UI.ShowPopUp<UI_StatusUp>("Monster/UI_StatusUp");
        ui.TargetMonster(this);
        ui.StateText = $"{_origin} → {_Evolution} {UserData.Instance.GetLocaleText("진화")}!!";
    }

}
