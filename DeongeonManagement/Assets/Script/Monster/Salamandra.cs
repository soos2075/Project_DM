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

    void Trait_Original()
    {
        AddTrait(new Trait.Overwhelm());
    }

    //? 일반 진화몹 로드할 때
    public override void MonsterInit_Evolution()
    {
        Data = GameManager.Monster.GetData("Salinu");
        GameManager.Monster.ChangeSLA_New(this, "Salinu");
        GameManager.Monster.Regist_Evolution("Salamandra");
    }

    //? 시작유닛으로 데려갈 떄
    public override void EvolutionMonster_Init()
    {
        Data = GameManager.Monster.GetData("Salamandra");
        Initialize_Status();
        EvolutionState = Evolution.Complete;
        EvolutionComplete();
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
            EvolutionComplete();
        }
    }

    void EvolutionComplete()
    {
        Data = GameManager.Monster.GetData("Salinu");
        Evolution_Status();
        GameManager.Monster.ChangeSLA_New(this, "Salinu");
        GameManager.Monster.Regist_Evolution("Salamandra");

        ChangeTrait_Evolution();

        UnitDialogueEvent.AddEvent(150600);
    }



    void ChangeTrait_Evolution()
    {
        List<ITrait> newTrait = new List<ITrait>();

        newTrait.Add(new Trait.Overwhelm());
        if (TraitCheck(TraitGroup.SurvivabilityA)) newTrait.Add(new Trait.SurvivabilityS());

        TraitList = newTrait;
    }
}
