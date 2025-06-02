using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushBoy : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("MushBoy");
        Trait_Original();

        if (GameManager.Monster.Check_Evolution("MushGirl"))
        {
            StartCoroutine(Init_Evolution());
        }
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

    public override void Load_EvolutionMonster()
    {
        Data = GameManager.Monster.GetData("MushGirl");
        GameManager.Monster.ChangeSLA(this, "MushGirl");
        GameManager.Monster.Regist_Evolution("MushBoy");

        Trait_Original();
    }

    public override void Create_EvolutionMonster_Init()
    {
        Data = GameManager.Monster.GetData("MushBoy");
        Trait_Original();
        Initialize_Status();

        EvolutionState = Evolution.Complete;
        EvolutionComplete("MushBoy", "MushGirl");
    }



    public override void TurnOver()
    {
        base.TurnOver();
        EvolutionCheck();
    }

    void EvolutionCheck()
    {
        if (EvolutionState == Evolution.Ready && LV >= 25 && State == MonsterState.Placement && PlacementInfo.Place_Floor.FloorIndex == 4)
        {
            //? 진화진행
            EvolutionState = Evolution.Complete;
            var ui = Managers.UI.ShowPopUp<UI_StatusUp>("Monster/UI_StatusUp");
            ui.TargetMonster(this);
            ui.StateText = $"{GameManager.Monster.GetData("MushBoy").labelName} → " +
                $"{GameManager.Monster.GetData("MushGirl").labelName} {UserData.Instance.LocaleText("진화")}!!";
            EvolutionComplete("MushBoy", "MushGirl");
        }
    }


    protected override void EvolutionComplete(string _original_key, string _evolution_Key)
    {
        base.EvolutionComplete(_original_key, _evolution_Key);
        ChangeTrait_Evolution();
    }

    void ChangeTrait_Evolution()
    {
        List<ITrait> newTrait = new List<ITrait>();

        foreach (var item in TraitList) //? 원래거 복사 (고유특성은 업글)
        {
            if (item.ID == TraitGroup.Spore)
            {
                newTrait.Add(new Trait.Spore_V2());
                continue;
            }

            if (item.ID == TraitGroup.EliteC)
            {
                newTrait.Add(new Trait.EliteA());
                continue;
            }
            if (item.ID == TraitGroup.ShirkingC)
            {
                newTrait.Add(new Trait.ShirkingA());
                continue;
            }
            if (item.ID == TraitGroup.RuthlessC)
            {
                newTrait.Add(new Trait.RuthlessA());
                continue;
            }

            if (item.ID == TraitGroup.VeteranC)
            {
                newTrait.Add(new Trait.VeteranB());
                continue;
            }
            if (item.ID == TraitGroup.SurvivabilityC)
            {
                newTrait.Add(new Trait.SurvivabilityB());
                continue;
            }
            if (item.ID == TraitGroup.DiscreetC)
            {
                newTrait.Add(new Trait.DiscreetB());
                continue;
            }

            newTrait.Add(item);
        }

        TraitList = newTrait;
    }
}
