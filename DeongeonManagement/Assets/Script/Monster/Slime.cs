using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster
{

    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Slime");
        Trait_Original();

        UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.Slime_First);

        if (GameManager.Monster.Check_Evolution("BloodySlime"))
        {
            StartCoroutine(Init_Evolution());
        }
        else
        {
            Debug.Log("이미 등록된 진화몹 있음");
        }
    }


    public override void Load_EvolutionMonster()
    {
        Data = GameManager.Monster.GetData("BloodySlime");
        GameManager.Monster.ChangeSLA_New(this, "Slime_Bloody");

        GameManager.Monster.Regist_Evolution("Slime");

        Trait_Original();
    }

    public override void Create_EvolutionMonster_Init()
    {
        Data = GameManager.Monster.GetData("Slime");
        Trait_Original();

        Initialize_Status();
        EvolutionState = Evolution.Complete;
        EvolutionComplete("Slime", "BloodySlime");
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



    public override void LevelUpEvent(LevelUpEventType levelUpType)
    {
        base.LevelUpEvent(levelUpType);
        if (EvolutionState == Evolution.Ready && LV + 1 >= Data.maxLv)
        {
            EvolutionState = Evolution.Progress;
            EventManager.Instance.Add_GuildQuest_Special(1100, false);
            UserData.Instance.FileConfig.Notice_Guild = true;
            FindAnyObjectByType<UI_Management>().OverlayImageReset();
        }
    }


    public override void BattleEvent(BattleField.BattleResult result, NPC npc)
    {
        base.BattleEvent(result, npc);

        if (EvolutionState != Evolution.Progress)
        {
            return;
        }

        switch (result)
        {
            case BattleField.BattleResult.Nothing:
                break;

            case BattleField.BattleResult.Monster_Die:
                break;

            case BattleField.BattleResult.NPC_Die:
                if (npc.EventID == (int)NPC_Type_Hunter.Hunter_Slime)
                {
                    //? 진화진행
                    EvolutionState = Evolution.Complete;
                    EventManager.Instance.ClearQuestAction(1100);
                    var ui = Managers.UI.ShowPopUp<UI_StatusUp>("Monster/UI_StatusUp");
                    ui.TargetMonster(this);
                    ui.StateText = $"{GameManager.Monster.GetData("Slime").labelName} → " +
                        $"{GameManager.Monster.GetData("BloodySlime").labelName} {UserData.Instance.LocaleText("진화")}!!";
                    EvolutionComplete("Slime", "BloodySlime");
                }
                break;
        }
    }

    protected override void EvolutionComplete(string _original_key, string _evolution_Key)
    {
        base.EvolutionComplete(_original_key, _evolution_Key);
        ChangeTrait_Evolution();
        UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.BloodySlime_First);
    }

    void ChangeTrait_Evolution()
    {
        List<ITrait> newTrait = new List<ITrait>();

        foreach (var item in TraitList) //? 원래거 복사 (고유특성만 빼고)
        {
            if (item.ID == TraitGroup.VeteranC)
            {
                newTrait.Add(new Trait.VeteranB());
                continue;
            }
            if (item.ID == TraitGroup.EliteC)
            {
                newTrait.Add(new Trait.EliteB());
                continue;
            }
            if (item.ID == TraitGroup.DiscreetC)
            {
                newTrait.Add(new Trait.DiscreetB());
                continue;
            }
            if (item.ID == TraitGroup.ShirkingC)
            {
                newTrait.Add(new Trait.ShirkingB());
                continue;
            }
            if (item.ID == TraitGroup.SurvivabilityC)
            {
                newTrait.Add(new Trait.SurvivabilityB());
                continue;
            }
            if (item.ID == TraitGroup.RuthlessC)
            {
                newTrait.Add(new Trait.RuthlessB());
                continue;
            }

            newTrait.Add(item);
        }

        TraitList = newTrait;
    }


}
