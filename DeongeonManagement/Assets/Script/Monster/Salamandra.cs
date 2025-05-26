using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salamandra : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Salamandra");
        Trait_Original();

        UnitDialogueEvent.AddEvent(100600);

        if (GameManager.Monster.Check_Evolution("Salinu"))
        {
            StartCoroutine(Init_Evolution());
        }
    }


    //? 일반 진화몹 로드할 때
    public override void Load_EvolutionMonster()
    {
        Data = GameManager.Monster.GetData("Salinu");
        GameManager.Monster.ChangeSLA(this, "Salinu");
        GameManager.Monster.Regist_Evolution("Salamandra");
    }

    //? 시작유닛으로 데려갈 떄
    public override void Create_EvolutionMonster_Init()
    {
        Data = GameManager.Monster.GetData("Salamandra");
        Trait_Original();

        Initialize_Status();
        EvolutionState = Evolution.Complete;
        EvolutionComplete("Salamandra", "Salinu");
    }



    IEnumerator Init_Evolution()
    {
        yield return new WaitForEndOfFrame();
        if (EvolutionState == Evolution.None)
        {
            EvolutionState = Evolution.Ready;
        }
    }



    public override void TurnOver()
    {
        base.TurnOver();

        EvolutionCheck();
    }

    void EvolutionCheck()
    {
        if (EvolutionState == Evolution.Ready && LV >= 20 && State == MonsterState.Placement && PlacementInfo.Place_Floor.FloorIndex == 5)
        {
            //? 진화진행
            EvolutionState = Evolution.Complete;
            var ui = Managers.UI.ShowPopUp<UI_StatusUp>("Monster/UI_StatusUp");
            ui.TargetMonster(this);
            ui.StateText = $"{GameManager.Monster.GetData("Salamandra").labelName} → " +
                $"{GameManager.Monster.GetData("Salinu").labelName} {UserData.Instance.LocaleText("진화")}!!";
            EvolutionComplete("Salamandra", "Salinu");
        }
    }

    protected override void EvolutionComplete(string _original_key, string _evolution_Key)
    {
        base.EvolutionComplete(_original_key, _evolution_Key);
        ChangeTrait_Evolution();
        UnitDialogueEvent.AddEvent(150600);
    }

    void ChangeTrait_Evolution()
    {
        List<ITrait> newTrait = new List<ITrait>();

        AddTrait_DisableList(TraitGroup.Overwhelm);

        foreach (var item in TraitList) //? 원래거 복사 (고유특성만 빼고)
        {
            if (item.ID == TraitGroup.ToughSkin)
            {
                newTrait.Add(new Trait.ToughSkin_V2());
                continue;
            }

            if (item.ID == TraitGroup.Overwhelm)
            {
                newTrait.Add(new Trait.Overwhelm_V2());
                continue;
            }
            if (item.ID == TraitGroup.SurvivabilityA)
            {
                newTrait.Add(new Trait.SurvivabilityS());
                continue;
            }
            newTrait.Add(item);
        }

        TraitList = newTrait;
    }
}
