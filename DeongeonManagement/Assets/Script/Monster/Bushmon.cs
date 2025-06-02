using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bushmon : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Bushmon");
        Trait_Original();

        if (GameManager.Monster.Check_Evolution("Treemon"))
        {
            StartCoroutine(Init_Evolution());
        }
    }

    IEnumerator Init_Evolution()
    {
        yield return new WaitForEndOfFrame();
        if (EvolutionState == Evolution.None)
        {
            EvolutionState = Evolution.Ready;
        }
    }

    public override void Load_EvolutionMonster()
    {
        Data = GameManager.Monster.GetData("Treemon");
        GameManager.Monster.ChangeSLA(this, "Treemon");
        GameManager.Monster.Regist_Evolution("Bushmon");

        Trait_Original();
    }

    public override void Create_EvolutionMonster_Init()
    {
        Data = GameManager.Monster.GetData("Bushmon");
        Trait_Original();
        Initialize_Status();

        EvolutionState = Evolution.Complete;
        EvolutionComplete("Bushmon", "Treemon");
    }




    public override void BattleEvent(BattleField.BattleResult result, NPC npc)
    {
        base.BattleEvent(result, npc);

        if (EvolutionState != Evolution.Ready)
        {
            return;
        }

        if (result == BattleField.BattleResult.Monster_Die)
        {
            return;
        }

        if (B_DEF >= 50)
        {
            //? 진화진행
            EvolutionState = Evolution.Complete;
            var ui = Managers.UI.ShowPopUp<UI_StatusUp>("Monster/UI_StatusUp");
            ui.TargetMonster(this);
            ui.StateText = $"{GameManager.Monster.GetData("Bushmon").labelName} → " +
                $"{GameManager.Monster.GetData("Treemon").labelName} {UserData.Instance.LocaleText("진화")}!!";
            EvolutionComplete("Bushmon", "Treemon");
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
            if (item.ID == TraitGroup.ThornyVine)
            {
                newTrait.Add(new Trait.ThornyVine_V2());
                continue;
            }
            if (item.ID == TraitGroup.IronSkin)
            {
                newTrait.Add(new Trait.IronSkin_V2());
                continue;
            }

            if (item.ID == TraitGroup.EliteB)
            {
                newTrait.Add(new Trait.EliteA());
                continue;
            }
            if (item.ID == TraitGroup.ShirkingB)
            {
                newTrait.Add(new Trait.ShirkingA());
                continue;
            }
            if (item.ID == TraitGroup.RuthlessB)
            {
                newTrait.Add(new Trait.RuthlessA());
                continue;
            }
            if (item.ID == TraitGroup.DiscreetB)
            {
                newTrait.Add(new Trait.DiscreetA());
                continue;
            }

            newTrait.Add(item);
        }

        TraitList = newTrait;
    }

}
