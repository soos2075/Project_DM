using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthGolem : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("EarthGolem");
        Trait_Original();

        if (GameManager.Monster.Check_Evolution("FlameGolem"))
        {
            StartCoroutine(Init_Evolution());
        }
    }

    public override void Create_EvolutionMonster_Init()
    {
        Data = GameManager.Monster.GetData("EarthGolem");
        Trait_Original();

        Initialize_Status();
        EvolutionState = Evolution.Complete;
        EvolutionComplete("EarthGolem", "FlameGolem");
    }


    public override void Load_EvolutionMonster()
    {
        Data = GameManager.Monster.GetData("FlameGolem");
        GameManager.Monster.ChangeSLA(this, "FlameGolem");
        GameManager.Monster.Regist_Evolution("EarthGolem");
    }
    IEnumerator Init_Evolution()
    {
        yield return new WaitForEndOfFrame();

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
            Debug.Log("퀘스트 추가");
            EventManager.Instance.Add_GuildQuest_Special(1101, false);
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
                if (npc.EventID == (int)NPC_Type_Hunter.Hunter_EarthGolem)
                {
                    //? 진화진행
                    EvolutionState = Evolution.Complete;
                    EventManager.Instance.ClearQuestAction(1101);
                    var ui = Managers.UI.ShowPopUp<UI_StatusUp>("Monster/UI_StatusUp");
                    ui.TargetMonster(this);
                    ui.StateText = $"{GameManager.Monster.GetData("EarthGolem").labelName} → " +
                        $"{GameManager.Monster.GetData("FlameGolem").labelName} {UserData.Instance.LocaleText("진화")}!!";
                    EvolutionComplete("EarthGolem", "FlameGolem");
                }
                break;
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

        AddTrait_DisableList(TraitGroup.Vitality);

        foreach (var item in TraitList) //? 특성 업그레이드
        {
            if (item.ID == TraitGroup.Golem)
            {
                newTrait.Add(new Trait.Golem_V2());
                continue;
            }
            if (item.ID == TraitGroup.Vitality)
            {
                newTrait.Add(new Trait.Vitality_V2());
                continue;
            }
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
