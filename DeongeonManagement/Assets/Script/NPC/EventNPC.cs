using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventNPC : NPC
{
    public override List<BasementTile> PriorityList { get; set; }
    protected override Define.TileType[] AvoidTileType { get; set; } = new Define.TileType[] { Define.TileType.NPC, Define.TileType.Facility };

    public override int RunawayHpRatio { get; set; } = 999;

    public override void Departure(Vector3 startPoint, Vector3 endPoint)
    {
        base.Departure(startPoint, endPoint);

        StartCoroutine(EventCor());
    }
    IEnumerator EventCor()
    {
        yield return new WaitForSeconds(5);

        Managers.Dialogue.ShowDialogueUI($"Day{8}_Event", transform);
    }


    protected override void SetRandomClothes()
    {
        var collection = characterBuilder.SpriteCollection;
        if (Main.Instance.Turn == 1)
        {
            KillGold = 200;
            characterBuilder.Hair = "Hair11#C42430/0:0:0";
            characterBuilder.Armor = "DemigodArmour";
            characterBuilder.Weapon = "Katana";
            characterBuilder.Shield = "GoldenEagle";
        }

        if (Main.Instance.Turn == 15)
        {

        }

        characterBuilder.Rebuild();
    }
    protected override void SetPriorityList()
    {
        if (PriorityList != null) PriorityList.Clear();

        var list1 = GetFloorObjectsAll(Define.TileType.Monster);
        AddList(list1);

        {
            var add_egg = GetPriorityPick(typeof(SpecialEgg));
            AddList(add_egg);
        }

        {
            var add_secret = GetPriorityPick(typeof(Entrance_Egg));
            AddList(add_secret);
        }
    }




    protected override void NPC_Die()
    {
        //Managers.Dialogue.ShowDialogueUI($"Day{8}_Event_Die", transform);
        StartCoroutine(DieEvent());
    }

    IEnumerator DieEvent()
    {
        Managers.Dialogue.ShowDialogueUI($"Day{8}_Event_Die", transform);
        yield return null;
        yield return new WaitUntil(() => Managers.Dialogue.GetState() == DialogueManager.DialogueState.None);

        //UI_EventBox.AddEventText($"◈{Name_KR} (이)가 {PlacementInfo.Place_Floor.Name_KR}에서 쓰러짐");
        Main.Instance.CurrentDay.AddDanger(50);
        Main.Instance.CurrentDay.AddPop(50);

        GameManager.NPC.InactiveNPC(this);
    }

    protected override void NPC_Captive()
    {

    }
    protected override void NPC_Return_Empty()
    {
        
    }
    protected override void NPC_Return_Satisfaction()
    {
        
    }
    protected override void NPC_Runaway()
    {
        
    }

    void Return()
    {
        Managers.Dialogue.ShowDialogueUI($"Day{8}_Event_Return", transform);
        Main.Instance.CurrentDay.AddDanger(-50);
    }


}
