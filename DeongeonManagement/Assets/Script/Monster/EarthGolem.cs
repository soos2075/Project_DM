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
        //else
        //{
        //    Debug.Log("이미 등록된 진화몹 있음");
        //}
    }

    public override void EvolutionMonster_Init()
    {
        Data = GameManager.Monster.GetData("EarthGolem");
        Initialize_Status();
        EvolutionState = Evolution.Complete;
        EvolutionComplete();
    }

    void Trait_Original()
    {
        AddTrait(new Trait.Vitality());
    }
    public override void MonsterInit_Evolution()
    {
        Data = GameManager.Monster.GetData("FlameGolem");
        GameManager.Monster.ChangeSLA(this, "FlameGolem");

        GameManager.Monster.Regist_Evolution("EarthGolem");

        Trait_Original();
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
                    EvolutionComplete();
                }
                break;
        }
    }

    void EvolutionComplete()
    {
        Data = GameManager.Monster.GetData("FlameGolem");
        Evolution_Status();
        GameManager.Monster.ChangeSLA(this, "FlameGolem");
        GameManager.Monster.Regist_Evolution("EarthGolem");

        ChangeTrait_Evolution();
    }

    void ChangeTrait_Evolution()
    {
        List<ITrait> newTrait = new List<ITrait>();

        newTrait.Add(new Trait.Vitality());
        if (TraitCheck(TraitGroup.VeteranC)) newTrait.Add(new Trait.VeteranB());
        if (TraitCheck(TraitGroup.EliteC)) newTrait.Add(new Trait.EliteB());
        if (TraitCheck(TraitGroup.DiscreetC)) newTrait.Add(new Trait.DiscreetB());
        if (TraitCheck(TraitGroup.ShirkingC)) newTrait.Add(new Trait.ShirkingB());
        if (TraitCheck(TraitGroup.SurvivabilityC)) newTrait.Add(new Trait.SurvivabilityB());
        if (TraitCheck(TraitGroup.RuthlessC)) newTrait.Add(new Trait.RuthlessB());

        TraitList = newTrait;
    }
}
