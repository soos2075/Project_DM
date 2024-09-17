using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heroine : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Heroine");
        Trait_Original();
    }

    void Trait_Original()
    {
        AddTrait(new Trait.Nimble());
    }



    public override void TurnOver()
    {
        EvolutionCheck();
    }

    void EvolutionCheck()
    {
        if (EvolutionState != Evolution.Complete && CustomName == "Rena" || CustomName == "레나" || CustomName == "レナ")
        {
            //? 진화진행
            EvolutionState = Evolution.Complete;
            var ui = Managers.UI.ShowPopUp<UI_StatusUp>("Monster/UI_StatusUp");
            ui.TargetMonster(this);
            ui.StateText = $"{GameManager.Monster.GetData("Heroine").labelName} → " +
                $"{GameManager.Monster.GetData("Rena").labelName} {UserData.Instance.LocaleText("진화")}!!";
            EvolutionComplete();
        }
    }

    void EvolutionComplete()
    {
        Data = GameManager.Monster.GetData("Rena");
        Evolution_Status();
        //GameManager.Monster.ChangeSLA_New(this, "Salinu");
        //GameManager.Monster.Regist_Evolution("Salamandra");
    }

    public override void MonsterInit_Evolution()
    {
        Data = GameManager.Monster.GetData("Rena");
        //GameManager.Monster.ChangeSLA_New(this, "Salinu");
        //GameManager.Monster.Regist_Evolution("Salamandra");
    }
}
