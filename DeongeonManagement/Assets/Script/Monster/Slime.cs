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


        if (GameManager.Monster.Check_Evolution("BloodySlime"))
        {
            StartCoroutine(Init_Evolution());
        }
        else
        {
            Debug.Log("�̹� ��ϵ� ��ȭ�� ����");
        }
    }

    void Trait_Original()
    {
        //AddTrait(new Trait.Reconfigure());
        AddTrait(new Trait.Reconfigure());
    }


    public override void MonsterInit_Evolution()
    {
        Data = GameManager.Monster.GetData("BloodySlime");
        GameManager.Monster.ChangeSLA(this, "BloodyJelly");

        GameManager.Monster.Regist_Evolution("Slime");

        Trait_Original();
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
        switch (result)
        {
            case BattleField.BattleResult.Nothing:
                break;

            case BattleField.BattleResult.Monster_Die:
                break;

            case BattleField.BattleResult.NPC_Die:
                if (npc.EventID == (int)NPC_Type_Hunter.Hunter_Slime)
                {
                    //? ��ȭ����
                    EvolutionState = Evolution.Complete;
                    EventManager.Instance.ClearQuestAction(1100);
                    var ui = Managers.UI.ShowPopUp<UI_StatusUp>("Monster/UI_StatusUp");
                    ui.TargetMonster(this);
                    ui.StateText = $"{GameManager.Monster.GetData("Slime").labelName} �� " +
                        $"{GameManager.Monster.GetData("BloodySlime").labelName} {UserData.Instance.LocaleText("��ȭ")}!!";
                    EvolutionComplete();
                }
                break;
        }
    }


    void EvolutionComplete()
    {
        Data = GameManager.Monster.GetData("BloodySlime");
        Evolution_Status();
        GameManager.Monster.ChangeSLA(this, "BloodyJelly");
        GameManager.Monster.Regist_Evolution("Slime");

        ChangeTrait_Evolution();
    }

    void ChangeTrait_Evolution()
    {
        List<ITrait> newTrait = new List<ITrait>();

        newTrait.Add(new Trait.Reconfigure());
        if (TraitCheck(TraitGroup.VeteranC)) newTrait.Add(new Trait.VeteranB());
        if (TraitCheck(TraitGroup.EliteC)) newTrait.Add(new Trait.EliteB());
        if (TraitCheck(TraitGroup.DiscreetC)) newTrait.Add(new Trait.DiscreetB());
        if (TraitCheck(TraitGroup.ShirkingC)) newTrait.Add(new Trait.ShirkingB());
        if (TraitCheck(TraitGroup.SurvivabilityC)) newTrait.Add(new Trait.SurvivabilityB());
        if (TraitCheck(TraitGroup.RuthlessC)) newTrait.Add(new Trait.RuthlessB());

        TraitList = newTrait;
    }



}
