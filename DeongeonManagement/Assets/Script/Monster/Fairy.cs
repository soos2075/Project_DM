using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fairy : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Fairy");
        Trait_Original();

        UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.Fairy_First);
        if (GameManager.Monster.Check_Evolution("Pixie"))
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
        AddTrait(new Trait.Friend());
    }
    public override void MonsterInit_Evolution()
    {
        Data = GameManager.Monster.GetData("Pixie");
        GameManager.Monster.ChangeSLA_New(this, "Pixie");
        GameManager.Monster.Regist_Evolution("Fairy");
        Trait_Original();
    }

    public override void EvolutionMonster_Init()
    {
        Data = GameManager.Monster.GetData("Fairy");
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
    void EvolutionComplete()
    {
        Data = GameManager.Monster.GetData("Pixie");
        Evolution_Status();
        GameManager.Monster.ChangeSLA_New(this, "Pixie");
        GameManager.Monster.Regist_Evolution("Fairy");
        ChangeTrait_Evolution();

        UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.Pixie_First);
    }
    void ChangeTrait_Evolution()
    {
        List<ITrait> newTrait = new List<ITrait>();

        newTrait.Add(new Trait.Friend());
        if (TraitCheck(TraitGroup.VeteranB)) newTrait.Add(new Trait.VeteranA());
        if (TraitCheck(TraitGroup.EliteB)) newTrait.Add(new Trait.EliteA());
        if (TraitCheck(TraitGroup.DiscreetB)) newTrait.Add(new Trait.DiscreetA());
        if (TraitCheck(TraitGroup.ShirkingB)) newTrait.Add(new Trait.ShirkingA());
        if (TraitCheck(TraitGroup.SurvivabilityB)) newTrait.Add(new Trait.SurvivabilityA());
        if (TraitCheck(TraitGroup.RuthlessB)) newTrait.Add(new Trait.RuthlessA());

        TraitList = newTrait;
    }



    //? �ڽ��� ������ ���� ���� 5���� �̻��� �ٸ� ������ ������� �� ����� ��ȭ (�ڽ�����6����)
    public override void TurnOver()
    {
        if (EvolutionState == Evolution.Ready && State == MonsterState.Placement)
        {
            var friend = PlacementInfo.Place_Floor.GetFloorObjectList(Define.TileType.Monster);
            List<string> monList = new List<string>();
            foreach (var mon in friend)
            {
                if (mon.Original_Obj != null && monList.Contains(mon.Original_Obj.name) == false)
                {
                    monList.Add(mon.Original_Obj.name);
                }
            }

            if (monList.Count >= 6)
            {
                Managers.UI.Popup_Reservation(() => Evolution_Pixie());
            }
        }
    }

    public void Evolution_Pixie()
    {
        EvolutionState = Evolution.Complete;
        var ui = Managers.UI.ShowPopUp<UI_StatusUp>("Monster/UI_StatusUp");
        ui.TargetMonster(this);
        ui.StateText = $"{GameManager.Monster.GetData("Fairy").labelName} �� " +
            $"{GameManager.Monster.GetData("Pixie").labelName} {UserData.Instance.LocaleText("��ȭ")}!!";
        EvolutionComplete();
    }


    //? ��ȭ ���� - ģ�� 5�� �̻�(���� �ٸ� ���� 5�� �̻�� ���� ���� ��ġ), ��������...�� ����
}
