using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heroine : Monster, I_Projectile
{
    public override SO_Monster Data { get; set; }
    public I_Projectile.AttackEffect AttackOption { get; set; } = new I_Projectile.AttackEffect(I_Projectile.AttackType.Normal);

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Heroine");
        Trait_Original();
        AttackOption.SetProjectile(I_Projectile.AttackType.Normal, "LegucyElf", "ElfA");

        UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.Heroin_First);
    }

    void Trait_Original()
    {
        AddTrait(new Trait.Nimble());
    }

    public override void ChangeValue_TraitCounter()
    {
        if (traitCounter.TrainingCounter >= 3)
        {
            UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.Heroin_Training);
        }
        if (traitCounter.InjuryCounter >= 1)
        {
            UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.Heroin_Defeat);
        }


        if (traitCounter.Days >= 3)
        {
            UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.Heroin_Day3);
        }
        if (traitCounter.Days >= 6)
        {
            UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.Heroin_Day6);
        }
        if (traitCounter.Days >= 9)
        {
            UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.Heroin_Day9);
        }
        if (traitCounter.Days >= 15)
        {
            UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.Heroin_Day15);
        }
    }

    public override void LevelUpEvent(LevelUpEventType levelUpType)
    {
        if (LV + 1 >= 5)
        {
            UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.Heroin_Lv5);
        }
        if (LV + 1 >= 10)
        {
            UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.Heroin_Lv10);
        }
        if (LV + 1 >= 15)
        {
            UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.Heroin_Lv15);
        }
        if (LV + 1 >= 18)
        {
            UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.Heroin_Lv18);
        }
    }

    public override void TurnOver()
    {
        if (UnitDialogueEvent.ClearEventList.Contains((int)UnitDialogueEventLabel.Heroin_Day3) &&
            UnitDialogueEvent.ClearEventList.Contains((int)UnitDialogueEventLabel.Heroin_Day6) &&
            UnitDialogueEvent.ClearEventList.Contains((int)UnitDialogueEventLabel.Heroin_Day9) &&
            UnitDialogueEvent.ClearEventList.Contains((int)UnitDialogueEventLabel.Heroin_Lv5) &&
            UnitDialogueEvent.ClearEventList.Contains((int)UnitDialogueEventLabel.Heroin_Lv10) &&
            UnitDialogueEvent.ClearEventList.Contains((int)UnitDialogueEventLabel.Heroin_Lv15) &&
            UnitDialogueEvent.ClearEventList.Contains((int)UnitDialogueEventLabel.Heroin_Training) &&
            UnitDialogueEvent.ClearEventList.Contains((int)UnitDialogueEventLabel.Heroin_First))
        {
            UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.Heroin_EndingRoot);
        }
        EvolutionCheck();
    }

    void EvolutionCheck()
    {
        if (EvolutionState == Evolution.Complete) return;

        if (CustomName == "Rena" || CustomName == "·¹³ª" || CustomName == "«ì«Ê" || CustomName == "ÖôÑÖ")
        {
            //? ÁøÈ­ÁøÇà
            UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.Heroin_CallName);
        }
    }

    public void Evolution_Rena()
    {
        EvolutionState = Evolution.Complete;
        var ui = Managers.UI.ShowPopUp<UI_StatusUp>("Monster/UI_StatusUp");
        ui.TargetMonster(this);
        ui.StateText = $"{GameManager.Monster.GetData("Heroine").labelName} ¡æ " +
            $"{GameManager.Monster.GetData("Rena").labelName} {UserData.Instance.LocaleText("ÁøÈ­")}!!";
        EvolutionComplete();
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
