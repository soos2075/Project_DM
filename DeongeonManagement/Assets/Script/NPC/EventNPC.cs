using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventNPC : NPC
{
    public override List<BasementTile> PriorityList { get; set; }
    protected override Define.TileType[] AvoidTileType { get; } = new Define.TileType[] { Define.TileType.NPC, Define.TileType.Facility };

    public override int RunawayHpRatio { get; set; } = 999;

    public override void Departure(Vector3 startPoint, Vector3 endPoint)
    {
        base.Departure(startPoint, endPoint);

        StartCoroutine(EventCor());
    }
    IEnumerator EventCor()
    {
        yield return new WaitForSeconds(5);

        Managers.Dialogue.ShowDialogueUI($"Day{Main.Instance.Turn}_Event", transform);
    }

    public enum EventNPCType
    {
        Event_Day3 = 2000,
        Event_Day8,
        Event_Day15,
        Event_Day23,
        Event_Day30,
    }
    public EventNPCType EventDay { get { return (EventNPCType)EventID; } }

    protected override void SetRandomClothes()
    {
        var collection = characterBuilder.SpriteCollection;

        switch (EventDay)
        {
            case EventNPCType.Event_Day3:
                KillGold = 50;
                int ran = Random.Range(0, collection.Layers[9].Textures.Count);
                characterBuilder.Hair = collection.Layers[9].Textures[ran].name;
                string hexColor = Define.HairColors[Random.Range(0, 24)];
                characterBuilder.Hair += hexColor;
                characterBuilder.Armor = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Armor_Warrior);
                characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_BeginnerSword);
                break;

            case EventNPCType.Event_Day8:
                KillGold = 200;
                characterBuilder.Hair = "Hair11#C42430/0:0:0";
                characterBuilder.Armor = "DemigodArmour";
                characterBuilder.Weapon = "Katana";
                break;

            case EventNPCType.Event_Day15:
                KillGold = 500;
                characterBuilder.Hair = "Hair10#858585/0:0:0";
                characterBuilder.Armor = "HeavyKnightArmor";
                characterBuilder.Weapon = "Epee";
                characterBuilder.Shield = "KnightShield";
                break;

            case EventNPCType.Event_Day23:
                break;

            case EventNPCType.Event_Day30:
                break;
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
        Managers.Dialogue.ShowDialogueUI($"Day{Main.Instance.Turn}_Event_Die", transform);
        yield return null;
        yield return new WaitUntil(() => Managers.Dialogue.GetState() == DialogueManager.DialogueState.None);

        UI_EventBox.AddEventText($"◈{Name_Color} (이)가 {PlacementInfo.Place_Floor.Name_KR}에서 쓰러짐");

        switch (EventDay)
        {
            case EventNPCType.Event_Day3:
                Main.Instance.CurrentDay.AddDanger(5);
                Main.Instance.CurrentDay.AddPop(5);
                break;

            case EventNPCType.Event_Day8:
                Main.Instance.CurrentDay.AddDanger(25);
                Main.Instance.CurrentDay.AddPop(25);
                break;

            case EventNPCType.Event_Day15:
                Main.Instance.CurrentDay.AddDanger(50);
                Main.Instance.CurrentDay.AddPop(50);
                break;

            case EventNPCType.Event_Day23:
                break;

            case EventNPCType.Event_Day30:
                break;
        }
        Debug.Log(EventDay + "eventDay");
        Debug.Log(Main.Instance.Turn + "Turn");
        Main.Instance.CurrentDay.AddKill(1);
        Main.Instance.CurrentDay.AddGold(KillGold);
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
        Managers.Dialogue.ShowDialogueUI($"Day{Main.Instance.Turn}_Event_Return", transform);
        Main.Instance.CurrentDay.AddDanger(-50);
    }


}
