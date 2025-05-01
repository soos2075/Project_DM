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
    public override void Create_EvolutionMonster_Init()
    {
        Data = GameManager.Monster.GetData("GreyHound");
        Trait_Original();

        Initialize_Status();
        EvolutionState = Evolution.Complete;
        EvolutionComplete("GreyHound", "HellHound");
    }
    public override void Load_EvolutionMonster()
    {
        Data = GameManager.Monster.GetData("HellHound");
        GameManager.Monster.ChangeSLA_New(this, "HellHound");
        GameManager.Monster.Regist_Evolution("GreyHound");
    }



    public override void LevelUpEvent(LevelUpEventType levelUpType)
    {
        base.LevelUpEvent(levelUpType);
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
        EvolutionComplete("GreyHound", "HellHound");
    }

    protected override void EvolutionComplete(string _original_key, string _evolution_Key)
    {
        base.EvolutionComplete(_original_key, _evolution_Key);

        ChangeTrait_Evolution();
    }


    void ChangeTrait_Evolution()
    {
        List<ITrait> newTrait = new List<ITrait>();

        AddTrait_DisableList(TraitGroup.GaleForce);

        foreach (var item in TraitList) //? 원래거 복사 (고유특성만 빼고)
        {
            if (item.ID == TraitGroup.GaleForce)
            {
                newTrait.Add(new Trait.GaleForce_V2());
                continue;
            }
            if (item.ID == TraitGroup.VeteranB)
            {
                newTrait.Add(new Trait.VeteranA());
                continue;
            }
            if (item.ID == TraitGroup.DiscreetB)
            {
                newTrait.Add(new Trait.DiscreetA());
                continue;
            }
            if (item.ID == TraitGroup.ShirkingB)
            {
                newTrait.Add(new Trait.ShirkingA());
                continue;
            }
            if (item.ID == TraitGroup.SurvivabilityB)
            {
                newTrait.Add(new Trait.SurvivabilityA());
                continue;
            }
            if (item.ID == TraitGroup.RuthlessB)
            {
                newTrait.Add(new Trait.RuthlessA());
                continue;
            }

            newTrait.Add(item);
        }

        TraitList = newTrait;
    }


}
