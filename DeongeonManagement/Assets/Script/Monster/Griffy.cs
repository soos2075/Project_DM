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
    public override void Create_EvolutionMonster_Init()
    {
        Data = GameManager.Monster.GetData("Griffy");
        Trait_Original();

        Initialize_Status();
        EvolutionState = Evolution.Complete;
        EvolutionComplete("Griffy", "Griffin");
    }
    public override void Load_EvolutionMonster()
    {
        Data = GameManager.Monster.GetData("Griffin");
        GameManager.Monster.ChangeSLA(this, "Griffin");
        GameManager.Monster.Regist_Evolution("Griffy");
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
        ui.StateText = $"{GameManager.Monster.GetData("Griffy").labelName} → " +
            $"{GameManager.Monster.GetData("Griffin").labelName} {UserData.Instance.LocaleText("진화")}!!";
        EvolutionComplete("Griffy", "Griffin");
    }

    protected override void EvolutionComplete(string _original_key, string _evolution_Key)
    {
        base.EvolutionComplete(_original_key, _evolution_Key);

        ChangeTrait_Evolution();
    }

    void ChangeTrait_Evolution()
    {
        List<ITrait> newTrait = new List<ITrait>();

        AddTrait_DisableList(TraitGroup.Predation);

        foreach (var item in TraitList) //? 원래거 복사 (고유특성만 빼고)
        {
            if (item.ID == TraitGroup.Fierce)
            {
                newTrait.Add(new Trait.Fierce_V2());
                continue;
            }

            if (item.ID == TraitGroup.Predation)
            {
                newTrait.Add(new Trait.Predation_V2());
                continue;
            }
            newTrait.Add(item);
        }

        TraitList = newTrait;
    }

}
