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
        //    Debug.Log("�̹� ��ϵ� ��ȭ�� ����");
        //}
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
            Debug.Log("����Ʈ �߰�");
            EventManager.Instance.Add_Special(1101);
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
                if (npc.name == $"Hunter_{name}")
                {
                    //? ��ȭ����
                    EvolutionState = Evolution.Complete;
                    EventManager.Instance.RemoveQuestAction(1101);
                    var ui = Managers.UI.ShowPopUp<UI_StatusUp>("Monster/UI_StatusUp");
                    ui.TargetMonster(this);
                    ui.StateText = "��� -> �÷��Ӱ� ��ȭ!!";
                    EvolutionComplete();
                }
                break;
        }
    }

    void EvolutionComplete()
    {
        Data = GameManager.Monster.GetData("FlameGolem");
        Initialize_Status();
        GameManager.Monster.ChangeSLA(this, "FlameGolem");

        GameManager.Monster.Regist_Evolution("EarthGolem");
    }
}
