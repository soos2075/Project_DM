using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Succubus : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Succubus");
        Trait_Original();

        UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.Succubus_First);

        if (GameManager.Monster.Check_Evolution("Lilith"))
        {
            StartCoroutine(Init_Evolution());
        }
    }

    public override void Load_EvolutionMonster()
    {
        Data = GameManager.Monster.GetData("Lilith");
        GameManager.Monster.ChangeSLA_New(this, "Lilith");
        GameManager.Monster.Regist_Evolution("Succubus");
    }

    public override void Create_EvolutionMonster_Init()
    {
        Data = GameManager.Monster.GetData("Succubus");
        Trait_Original();

        Initialize_Status();
        EvolutionState = Evolution.Complete;
        EvolutionComplete();

        UnitDialogueEvent.ClearEvent(UnitDialogueEventLabel.Succubus_First);
    }
    IEnumerator Init_Evolution()
    {
        yield return new WaitForEndOfFrame();
        if (EvolutionState == Evolution.None)
        {
            EvolutionState = Evolution.Ready;
        }
    }

    void EvolutionComplete()
    {
        Data = GameManager.Monster.GetData("Lilith");
        Evolution_Status();
        GameManager.Monster.ChangeSLA_New(this, "Lilith");
        GameManager.Monster.Regist_Evolution("Succubus");

        ChangeTrait_Evolution();

        UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.Lilith_First);
    }
    void ChangeTrait_Evolution()
    {
        List<ITrait> newTrait = new List<ITrait>();

        AddTrait_DisableList(TraitGroup.LifeDrain);

        foreach (var item in TraitList) //? 원래거 복사 (고유특성만 빼고)
        {
            if (item.ID == TraitGroup.LifeDrain)
            {
                newTrait.Add(new Trait.LifeDrain_V2());
                continue;
            }
            if (item.ID == TraitGroup.SurvivabilityB)
            {
                newTrait.Add(new Trait.SurvivabilityS());
                continue;
            }
            newTrait.Add(item);
        }

        TraitList = newTrait;
    }





    //? 진화 조건 ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
    public override void ChangeValue_TraitCounter()
    {
        if (EvolutionState == Evolution.Ready && traitCounter.CustomValueCounter >= 1000)
        {
            UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.Succubus_Evolution1);
        }
    }

    public void Evolution_Lilith()
    {
        EvolutionState = Evolution.Complete;
        var ui = Managers.UI.ShowPopUp<UI_StatusUp>("Monster/UI_StatusUp");
        ui.TargetMonster(this);
        ui.StateText = $"{GameManager.Monster.GetData("Succubus").labelName} → " +
            $"{GameManager.Monster.GetData("Lilith").labelName} {UserData.Instance.LocaleText("진화")}!!";
        EvolutionComplete();
    }

    public void Refeat_Evolution()
    {
        Debug.Log("반복호출");
        UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.Succubus_Evolution2, true);
    }
}
