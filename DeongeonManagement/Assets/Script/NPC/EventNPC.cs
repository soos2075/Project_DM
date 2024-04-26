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

        switch (EventDay)
        {
            case EventNPCType.Event_Day3:
                Managers.Dialogue.ShowDialogueUI($"Day3_Event", transform);
                break;
            case EventNPCType.Event_Day8:
                Managers.Dialogue.ShowDialogueUI($"Day8_Event", transform);
                break;
            case EventNPCType.Event_Day15:
                Managers.Dialogue.ShowDialogueUI($"Day15_Event", transform);
                break;
            case EventNPCType.A_Warrior:
                Managers.Dialogue.ShowDialogueUI($"Day20_Event", transform);
                break;
            //case EventNPCType.B_Warrior:
            //    Managers.Dialogue.ShowDialogueUI($"Day{Main.Instance.Turn}_Event", transform);
            //    break;


            case EventNPCType.Captine_A:
                Managers.Dialogue.ShowDialogueUI($"Day25_Event", transform);
                break;
            case EventNPCType.Captine_B:
                break;
            case EventNPCType.Captine_C:
                Managers.Dialogue.ShowDialogueUI($"Day30_Event", transform);
                break;


    
        }
    }

    public enum EventNPCType
    {
        Event_Day3 = 2000,
        Event_Day8,
        Event_Day15,


        A_Warrior,
        A_Tanker,
        A_Wizard,
        A_Elf,

        B_Warrior,
        B_Tanker,
        B_Wizard,
        B_Elf,

        Captine_A,
        Captine_B,
        Captine_C,

        Event_Soldier1,
        Event_Soldier2,
    }
    public EventNPCType EventDay { get { return (EventNPCType)EventID; } }

    protected override void SetRandomClothes()
    {
        var collection = characterBuilder.SpriteCollection;

        name = EventDay.ToString();

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




                //? 여기부터 killgold같은거 추가로 설정해야함

            case EventNPCType.A_Warrior:
            case EventNPCType.B_Warrior:
                characterBuilder.Hair = "Hair15#C42430/0:0:0";
                characterBuilder.Armor = "DemigodArmour#FFFFFF/0:0:0";
                characterBuilder.Weapon = "RedKatana#FFFFFF/0:0:0";
                characterBuilder.Cape = "Cape#FFFFFF/0:0:0";
                break;

            case EventNPCType.A_Tanker:
            case EventNPCType.B_Tanker:
                characterBuilder.Head = "Demon#FFFFFF/0:0:0";
                characterBuilder.Ears = "Demon#FFFFFF/0:0:0";
                characterBuilder.Eyes = "Demon#FFFFFF/0:0:0";
                characterBuilder.Body = "Demon#FFFFFF/0:0:0";

                characterBuilder.Hair = "";

                characterBuilder.Armor = "GuardianTunic#FFFFFF/0:0:0";
                characterBuilder.Weapon = "Lance#FFFFFF/0:0:0";
                characterBuilder.Shield = "Dreadnought#FFFFFF/0:0:0";
                break;

            case EventNPCType.A_Wizard:
            case EventNPCType.B_Wizard:
                characterBuilder.Helmet = "BlueWizzardHat#FFFFFF/128:0:0";
                characterBuilder.Armor = "FireWizardRobe#FFFFFF/0:0:0";
                characterBuilder.Weapon = "ElderStaff#FFFFFF/0:0:0";
                break;

            case EventNPCType.A_Elf:
            case EventNPCType.B_Elf:
                characterBuilder.Head = "Elf#FFFFFF/0:0:0";
                characterBuilder.Ears = "Elf#FFFFFF/0:0:0";
                characterBuilder.Eyes = "Elf#FFFFFF/0:0:0";
                characterBuilder.Body = "Elf#FFFFFF/0:0:0";

                characterBuilder.Hair = "Hair9#C42430/0:0:0";
                characterBuilder.Armor = "FemaleSwimmingSuit#FFFFFF/0:0:0";
                characterBuilder.Weapon = "CurvedBow#FFFFFF/0:0:0";
                break;



            case EventNPCType.Captine_A:
            case EventNPCType.Captine_B:
            case EventNPCType.Captine_C:
                characterBuilder.Helmet = "HeavyKnightHelmet#FFFFFF/0:0:0";
                characterBuilder.Armor = "IronKnight#FFFFFF/0:0:0";
                characterBuilder.Weapon = "RoyalLongsword#FFFFFF/0:0:0";
                break;

            case EventNPCType.Event_Soldier1:
                characterBuilder.Helmet = "MilitiamanHelmet#FFFFFF/0:0:0";
                characterBuilder.Armor = "HornsKnight#FFFFFF/0:0:0";
                characterBuilder.Weapon = "IronSword#FFFFFF/0:0:0";
                characterBuilder.Shield = "IronBuckler#FFFFFF/0:0:0";
                break;

            case EventNPCType.Event_Soldier2:
                characterBuilder.Helmet = "CaptainHelmet#FFFFFF/0:0:0";
                characterBuilder.Armor = "HeavyKnightArmor#FFFFFF/0:0:0";
                characterBuilder.Weapon = "Epee#FFFFFF/0:0:0";
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

        UI_EventBox.AddEventText($"◈{Name_Color} {UserData.Instance.GetLocaleText("Event_Defeat")}");

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



            case EventNPCType.A_Warrior:
                break;

            case EventNPCType.A_Tanker:
                break;

            case EventNPCType.A_Wizard:
                break;

            case EventNPCType.A_Elf:
                break;



            case EventNPCType.Captine_A:
                break;

            case EventNPCType.Event_Soldier1:
                break;

            case EventNPCType.Event_Soldier2:
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
