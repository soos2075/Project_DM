using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Griffy : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Griffy");
        Trait_Original();

        if (GameManager.Monster.Check_Evolution("Griffin"))
        {
            StartCoroutine(Init_Evolution());
        }
    }
    public override void EvolutionMonster_Init()
    {
        Data = GameManager.Monster.GetData("Griffy");
        Trait_Original();
        Initialize_Status();
        EvolutionState = Evolution.Complete;
        EvolutionComplete();
    }
    public override void MonsterInit_Evolution()
    {
        Data = GameManager.Monster.GetData("Griffin");
        GameManager.Monster.ChangeSLA_New(this, "Griffin");
        GameManager.Monster.Regist_Evolution("Griffy");
    }


    void Trait_Original()
    {
        AddTrait(new Trait.Predation());
    }


    IEnumerator Init_Evolution()
    {
        yield return new WaitForEndOfFrame();
        if (EvolutionState == Evolution.None)
        {
            EvolutionState = Evolution.Ready;
        }
    }


    public override void ChangeValue_TraitCounter()
    {
        EvolutionCheck();
    }

    void EvolutionCheck()
    {
        if (EvolutionState == Evolution.Ready && LV >= 25 && traitCounter.KillCounter >= 100)
        {
            Evolution_Griffin();
        }
    }


    public void Evolution_Griffin()
    {
        EvolutionState = Evolution.Complete;
        var ui = Managers.UI.ShowPopUp<UI_StatusUp>("Monster/UI_StatusUp");
        ui.TargetMonster(this);
        ui.StateText = $"{GameManager.Monster.GetData("Griffy").labelName} ¡æ " +
            $"{GameManager.Monster.GetData("Griffin").labelName} {UserData.Instance.LocaleText("ÁøÈ­")}!!";
        EvolutionComplete();
    }
    void EvolutionComplete()
    {
        Data = GameManager.Monster.GetData("Griffin");
        Evolution_Status();
        GameManager.Monster.ChangeSLA_New(this, "Griffin");
        GameManager.Monster.Regist_Evolution("Griffy");
    }

}
