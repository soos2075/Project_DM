using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster
{

    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Slime");
        Trait_Original();


        if (GameManager.Monster.Check_Evolution("BloodySlime"))
        {
            StartCoroutine(Init_Evolution());
        }
        else
        {
            Debug.Log("이미 등록된 진화몹 있음");
        }
    }

    void Trait_Original()
    {
        //AddTrait(new Trait.Reconfigure());
        AddTrait(TraitGroup.Reconfigure);
    }


    public override void MonsterInit_Evolution()
    {
        Data = GameManager.Monster.GetData("BloodySlime");
        GameManager.Monster.ChangeSLA(this, "BloodyJelly");

        GameManager.Monster.Regist_Evolution("Slime");

        Trait_Original();
    }


    IEnumerator Init_Evolution()
    {
        yield return new WaitForEndOfFrame();
        //Debug.Log(EvolutionState);
        if (EvolutionState == Evolution.None)
        {
            EvolutionState = Evolution.Ready;
        }
    }



    public override void LevelUpEvent(LevelUpEventType levelUpType)
    {
        if (EvolutionState == Evolution.Ready && LV + 1 >= Data.maxLv)
        {
            EvolutionState = Evolution.Progress;
            EventManager.Instance.Add_Special(1100);
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

        GameManager.Monster.Regist_Evolution("Slime");
    }

    void EvolutionUI(string _origin, string _Evolution)
    {
        var ui = Managers.UI.ShowPopUp<UI_StatusUp>("Monster/UI_StatusUp");
        ui.TargetMonster(this);
        ui.StateText = $"{_origin} → {_Evolution} {UserData.Instance.LocaleText("진화")}!!";
    }

}
