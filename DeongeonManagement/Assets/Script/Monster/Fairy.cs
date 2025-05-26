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
    }

    public override void Load_EvolutionMonster()
    {
        Data = GameManager.Monster.GetData("Pixie");
        GameManager.Monster.ChangeSLA(this, "Pixie");
        GameManager.Monster.Regist_Evolution("Fairy");
    }

    public override void Create_EvolutionMonster_Init()
    {
        Data = GameManager.Monster.GetData("Fairy");
        Trait_Original();

        Initialize_Status();
        EvolutionState = Evolution.Complete;
        EvolutionComplete("Fairy", "Pixie");
    }
    IEnumerator Init_Evolution()
    {
        yield return new WaitForEndOfFrame();
        if (EvolutionState == Evolution.None)
        {
            EvolutionState = Evolution.Ready;
        }
    }

    protected override void EvolutionComplete(string _original_key, string _evolution_Key)
    {
        base.EvolutionComplete(_original_key, _evolution_Key);
        ChangeTrait_Evolution();
        UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.Pixie_First);
    }

    void ChangeTrait_Evolution()
    {
        List<ITrait> newTrait = new List<ITrait>();

        AddTrait_DisableList(TraitGroup.Friend);

        foreach (var item in TraitList) //? ������ ���� (����Ư���� ����)
        {
            if (item.ID == TraitGroup.Spirit)
            {
                newTrait.Add(new Trait.Spirit_V2());
                continue;
            }

            if (item.ID == TraitGroup.Friend)
            {
                newTrait.Add(new Trait.Friend_V2());
                continue;
            }
            if (item.ID == TraitGroup.VeteranB)
            {
                newTrait.Add(new Trait.VeteranA());
                continue;
            }
            if (item.ID == TraitGroup.EliteB)
            {
                newTrait.Add(new Trait.EliteA());
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



    //? �ڽ��� ������ ���� ���� 5���� �̻��� �ٸ� ������ ������� �� ����� ��ȭ (�ڽ�����6����)
    public override void TurnOver()
    {
        base.TurnOver();

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
        EvolutionComplete("Fairy", "Pixie");
    }


    //? ��ȭ ���� - ģ�� 5�� �̻�(���� �ٸ� ���� 5�� �̻�� ���� ���� ��ġ), ��������...�� ����
}
