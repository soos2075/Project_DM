using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreyHound : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("GreyHound");
        Trait_Original();
        UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.GreyHound_First);


        if (GameManager.Monster.Check_Evolution("HellHound"))
        {
            StartCoroutine(Init_Evolution());
        }
    }
    public override void EvolutionMonster_Init()
    {
        Data = GameManager.Monster.GetData("GreyHound");
        Trait_Original();
        Initialize_Status();
        EvolutionState = Evolution.Complete;
        EvolutionComplete();
    }
    public override void MonsterInit_Evolution()
    {
        Data = GameManager.Monster.GetData("HellHound");
        GameManager.Monster.ChangeSLA_New(this, "HellHound");
        GameManager.Monster.Regist_Evolution("GreyHound");
    }


    void Trait_Original()
    {
        AddTrait(new Trait.GaleForce());
    }


    public override void LevelUpEvent(LevelUpEventType levelUpType)
    {
        if (EvolutionState != Evolution.Ready) return;

        if (LV + 1 >= 20)
        {
            UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.GreyHound_Lv20);
        }
    }


    IEnumerator Init_Evolution()
    {
        yield return new WaitForEndOfFrame();
        if (EvolutionState == Evolution.None)
        {
            EvolutionState = Evolution.Ready;
        }
        yield return null;
        EvolutionCheck();
    }


    void EvolutionCheck()
    {
        if (EvolutionState == Evolution.Ready && LV >= 20 && GameManager.Artifact.GetArtifact(ArtifactLabel.BananaBone).Count > 0)
        {
            //? 진화퀘스트 등록
            UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.GreyHound_Evolution);
        }
    }

    public void Evolution_Hound()
    {
        EvolutionState = Evolution.Complete;
        var ui = Managers.UI.ShowPopUp<UI_StatusUp>("Monster/UI_StatusUp");
        ui.TargetMonster(this);
        ui.StateText = $"{GameManager.Monster.GetData("GreyHound").labelName} → " +
            $"{GameManager.Monster.GetData("HellHound").labelName} {UserData.Instance.LocaleText("진화")}!!";
        EvolutionComplete();
    }
    void EvolutionComplete()
    {
        Data = GameManager.Monster.GetData("HellHound");
        Evolution_Status();
        GameManager.Monster.ChangeSLA_New(this, "HellHound");
        GameManager.Monster.Regist_Evolution("GreyHound");

        ChangeTrait_Evolution();
    }


    void ChangeTrait_Evolution()
    {
        List<ITrait> newTrait = new List<ITrait>();

        newTrait.Add(new Trait.GaleForce());
        if (TraitCheck(TraitGroup.VeteranB)) newTrait.Add(new Trait.VeteranA());
        if (TraitCheck(TraitGroup.DiscreetB)) newTrait.Add(new Trait.DiscreetA());
        if (TraitCheck(TraitGroup.ShirkingB)) newTrait.Add(new Trait.ShirkingA());
        if (TraitCheck(TraitGroup.SurvivabilityB)) newTrait.Add(new Trait.SurvivabilityA());
        if (TraitCheck(TraitGroup.RuthlessB)) newTrait.Add(new Trait.RuthlessA());

        TraitList = newTrait;
    }


}
