using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventNPC : NPC
{
    public override List<BasementTile> PriorityList { get; set; }
    protected override Define.TileType[] AvoidTileType { get { return AvoidTile(); } set { } }

    Define.TileType[] AvoidTile()
    {
        switch (State)
        {
            case NPCState.Runaway:
            case NPCState.Return_Empty:
            case NPCState.Return_Satisfaction:
            case NPCState.Die:
                return new Define.TileType[] { Define.TileType.NPC, Define.TileType.Facility, Define.TileType.Monster };

            default:
                return new Define.TileType[] { Define.TileType.NPC };
        }
    }


    public override int RunawayHpRatio { get { return RunawayRatio(); } set { } }

    int RunawayRatio()
    {
        switch (EventDay)
        {
            case EventNPCType.Event_Day3:
            case EventNPCType.Event_Day8:
            case EventNPCType.Event_RetiredHero:
            case EventNPCType.Captine_A:
            case EventNPCType.Captine_B:
            case EventNPCType.Captine_C:
                return 999;

            case EventNPCType.A_Warrior:
            case EventNPCType.B_Warrior:
            case EventNPCType.A_Tanker:
            case EventNPCType.B_Tanker:
            case EventNPCType.A_Wizard:
            case EventNPCType.B_Wizard:
            case EventNPCType.A_Elf:
            case EventNPCType.B_Elf:
            case EventNPCType.Event_Goblin_Leader:
            case EventNPCType.Event_Goblin_Leader2:
            case EventNPCType.Event_Goblin:
                return 2;

            default:
                return 4;
        }
    }

    public override int KillGold { get { return SetKillGold(); } set { } }

    int SetKillGold()
    {
        switch (EventDay)
        {
            case EventNPCType.Event_Day3:
                return 50;
            case EventNPCType.Event_Day8:
                return 200;
            case EventNPCType.Event_RetiredHero:
                return 500;
            case EventNPCType.Captine_A:
            case EventNPCType.Captine_B:
            case EventNPCType.Captine_C:
                return 500;

            case EventNPCType.A_Warrior:
            case EventNPCType.B_Warrior:
            case EventNPCType.A_Tanker:
            case EventNPCType.B_Tanker:
            case EventNPCType.A_Wizard:
            case EventNPCType.B_Wizard:
            case EventNPCType.A_Elf:
            case EventNPCType.B_Elf:
                return Random.Range(500, 1000);

            default:
                return 100;
        }
    }



    public override void Departure(Vector3 startPoint, Vector3 endPoint)
    {
        base.Departure(startPoint, endPoint);



        switch (EventDay)
        {
            case EventNPCType.Event_Day3:
                StartCoroutine(EventCor($"Day3_Event"));
                break;

            case EventNPCType.Event_Day8:
                StartCoroutine(EventCor($"Day8_Event"));
                break;

            case EventNPCType.Event_RetiredHero:
                StartCoroutine(EventCor($"Day15_Event"));
                break;

            case EventNPCType.Event_Goblin_Leader:
                StartCoroutine(EventCor(DialogueName.Goblin_Appear));
                break;

            case EventNPCType.Event_Goblin_Leader2:
                StartCoroutine(EventCor(DialogueName.Goblin_Party));
                break;


            case EventNPCType.A_Warrior:
                StartCoroutine(EventCor($"Day20_Event"));
                break;
        }
    }

    //public enum EventNPCType
    //{
    //    Event_Day3 = 2000,
    //    Event_Day8,
    //    RetiredHero,

    //    Goblin_Leader,
    //    Goblin_Leader2,
    //    Goblin,


    //    A_Warrior,
    //    A_Tanker,
    //    A_Wizard,
    //    A_Elf,

    //    B_Warrior,
    //    B_Tanker,
    //    B_Wizard,
    //    B_Elf,

    //    Captine_A,
    //    Captine_B,
    //    Captine_C,

    //    Event_Soldier1,
    //    Event_Soldier2,
    //    Event_Soldier3,
    //}
    public EventNPCType EventDay { get { return (EventNPCType)EventID; } }

    protected override void SetRandomClothes()
    {
        var collection = characterBuilder.SpriteCollection;

        name = EventDay.ToString();

        switch (EventDay)
        {
            case EventNPCType.Event_Day3:
                KillGold = 50;
                //int ran = Random.Range(0, collection.Layers[9].Textures.Count);
                characterBuilder.Hair = "Hair4#0098DC/0:0:0";
                //string hexColor = Define.HairColors[Random.Range(0, 24)];
                //characterBuilder.Hair += hexColor;
                characterBuilder.Armor = "TravelerTunic#FFFFFF/0:0:0";
                characterBuilder.Weapon = "IronSword#FFFFFF/0:0:0";
                break;

            case EventNPCType.Event_Day8:
                KillGold = 200;
                characterBuilder.Hair = "Hair11#C42430/0:0:0";
                characterBuilder.Armor = "DemigodArmour";
                characterBuilder.Weapon = "Katana";
                break;

            case EventNPCType.Event_RetiredHero:
                KillGold = 500;
                characterBuilder.Hair = "Hair10#858585/0:0:0";
                characterBuilder.Armor = "HeavyKnightArmor";
                characterBuilder.Weapon = "Epee";
                characterBuilder.Shield = "KnightShield";
                break;


            case EventNPCType.Event_Goblin:
            case EventNPCType.Event_Goblin_Leader:
            case EventNPCType.Event_Goblin_Leader2:
                characterBuilder.Hair = "";
                characterBuilder.Head = "Goblin#FFFFFF/0:0:0";
                characterBuilder.Ears = "Goblin#FFFFFF/0:0:0";
                characterBuilder.Eyes = "Goblin#FFFFFF/0:0:0";
                characterBuilder.Body = "Goblin#FFFFFF/0:0:0";
                characterBuilder.Weapon = "RustedShovel#FFFFFF/0:0:0";
                characterBuilder.Back = "LargeBackpack#FFFFFF/0:0:0";
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
                characterBuilder.Helmet = "HeavyKnightHelmet#FFFFFF/0:0:0";
                characterBuilder.Armor = "IronKnight#FFFFFF/0:0:0";
                characterBuilder.Weapon = "RoyalLongsword#FFFFFF/0:0:0";
                break;

            case EventNPCType.Captine_C:
                characterBuilder.Helmet = "BlueKnightHelmet#FFFFFF/0:0:0";
                characterBuilder.Armor = "BlueKnight#FFFFFF/0:0:0";
                characterBuilder.Weapon = "MasterGreataxe#FFFFFF/0:0:0";
                characterBuilder.Cape = "Cape#FFFFFF/0:0:0";
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

            case EventNPCType.Event_Soldier3:
                characterBuilder.Helmet = "LegionaryHelmet#FFFFFF/0:0:0";
                characterBuilder.Armor = "LegionaryArmor#FFFFFF/0:0:0";
                characterBuilder.Weapon = "GuardianHalberd#FFFFFF/0:0:0";
                characterBuilder.Shield = "RoyalGreatShield#FFFFFF/0:0:0";
                break;
        }


        characterBuilder.Rebuild();
    }
    protected override void SetPriorityList()
    {
        if (PriorityList != null) PriorityList.Clear();

        switch (EventDay)
        {
            case EventNPCType.Event_Goblin:
            case EventNPCType.Event_Goblin_Leader:
            case EventNPCType.Event_Goblin_Leader2:
                {
                    var herb = GetPriorityPick(typeof(Herb));
                    AddList(herb, AddPos.Front);
                }
                {
                    var mineral = GetPriorityPick(typeof(Mineral));
                    AddList(mineral, AddPos.Front);
                }
                {
                    var treasure = GetPriorityPick(typeof(Treasure));
                    AddList(treasure, AddPos.Front);
                }
                {
                    var add_secret = GetPriorityPick(typeof(Entrance_Egg));
                    AddList(add_secret, AddPos.Back);
                }
                {
                    var add_egg = GetPriorityPick(typeof(SpecialEgg));
                    AddList(add_egg, AddPos.Back);
                }
                break;


            case EventNPCType.Event_Day3:
            case EventNPCType.Event_Day8:
            case EventNPCType.Event_RetiredHero:
            case EventNPCType.Captine_A:
            case EventNPCType.Captine_B:
            case EventNPCType.Captine_C:
                {
                    var list1 = GetFloorObjectsAll(Define.TileType.Monster);
                    AddList(list1);
                }
                {
                    var treasure = GetPriorityPick(typeof(Treasure));
                    AddList(treasure, AddPos.Back);
                }
                {
                    var add_secret = GetPriorityPick(typeof(Entrance_Egg));
                    AddList(add_secret, AddPos.Back);
                }
                {
                    var add_egg = GetPriorityPick(typeof(SpecialEgg));
                    AddList(add_egg, AddPos.Back);
                }
                break;



            case EventNPCType.A_Warrior:
            case EventNPCType.B_Warrior:
            case EventNPCType.A_Tanker:
            case EventNPCType.B_Tanker:
                {
                    var list1 = GetFloorObjectsAll(Define.TileType.Monster);
                    AddList(list1);
                }
                {
                    var add_egg = GetPriorityPick(typeof(SpecialEgg));
                    AddList(add_egg);
                }
                {
                    var add_secret = GetPriorityPick(typeof(Entrance_Egg));
                    AddList(add_secret, AddPos.Back);
                }
                {
                    var treasure = GetPriorityPick(typeof(Treasure));
                    AddList(treasure, AddPos.Front);
                }
                break;

            case EventNPCType.A_Wizard:
            case EventNPCType.B_Wizard:
                {
                    var list1 = GetFloorObjectsAll(Define.TileType.Monster);
                    AddList(list1);
                }
                {
                    var add_egg = GetPriorityPick(typeof(SpecialEgg));
                    AddList(add_egg);
                }
                {
                    var add_secret = GetPriorityPick(typeof(Entrance_Egg));
                    AddList(add_secret, AddPos.Back);
                }
                {
                    var mineral = GetPriorityPick(typeof(Mineral));
                    AddList(mineral, AddPos.Front);
                }
                break;

            case EventNPCType.A_Elf:
            case EventNPCType.B_Elf:
                {
                    var list1 = GetFloorObjectsAll(Define.TileType.Monster);
                    AddList(list1);
                }
                {
                    var add_egg = GetPriorityPick(typeof(SpecialEgg));
                    AddList(add_egg);
                }
                {
                    var add_secret = GetPriorityPick(typeof(Entrance_Egg));
                    AddList(add_secret);
                }
                {
                    var herb = GetPriorityPick(typeof(Herb));
                    AddList(herb, AddPos.Front);
                }
                break;


            case EventNPCType.Event_Soldier1:
            case EventNPCType.Event_Soldier2:
            case EventNPCType.Event_Soldier3:
                {
                    var list1 = GetFloorObjectsAll(Define.TileType.Monster);
                    AddList(list1);
                }
                {
                    var herb = GetPriorityPick(typeof(Herb));
                    AddList(herb, AddPos.Back);
                }
                {
                    var mineral = GetPriorityPick(typeof(Mineral));
                    AddList(mineral, AddPos.Back);
                }
                break;
        }
    }




    protected override void NPC_Die()
    {
        //Managers.Dialogue.ShowDialogueUI($"Day{8}_Event_Die", transform);
        StartCoroutine(DieEvent());

        AddCollectionPoint();
    }

    IEnumerator DieEvent()
    {
        Managers.Dialogue.ShowDialogueUI($"Day{Main.Instance.Turn}_Event_Die", transform);
        yield return null;
        yield return new WaitUntil(() => Managers.Dialogue.GetState() == DialogueManager.DialogueState.None);

        UI_EventBox.AddEventText($"◈{Name_Color} {UserData.Instance.LocaleText("Event_Defeat")}");

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

            case EventNPCType.Event_RetiredHero:
                Main.Instance.CurrentDay.AddDanger(50);
                Main.Instance.CurrentDay.AddPop(50);
                GuildManager.Instance.RemoveInstanceGuildNPC(GuildNPC_LabelName.RetiredHero);
                EventManager.Instance.RemoveQuestAction(1151);
                break;

            case EventNPCType.Event_Goblin_Leader:
                Managers.Dialogue.ShowDialogueUI(DialogueName.Goblin_Die, transform);
                break;


            case EventNPCType.A_Warrior:
            case EventNPCType.B_Warrior:
            case EventNPCType.A_Tanker:
            case EventNPCType.B_Tanker:
            case EventNPCType.A_Wizard:
            case EventNPCType.B_Wizard:
            case EventNPCType.A_Elf:
            case EventNPCType.B_Elf:
                Main.Instance.CurrentDay.AddDanger(10);
                Main.Instance.CurrentDay.AddPop(50);
                break;


            case EventNPCType.Captine_A:
            case EventNPCType.Captine_B:
            case EventNPCType.Captine_C:
                Main.Instance.CurrentDay.AddDanger(50);
                Main.Instance.CurrentDay.AddPop(10);
                break;


            case EventNPCType.Event_Soldier1:
            case EventNPCType.Event_Soldier2:
            case EventNPCType.Event_Soldier3:
                Main.Instance.CurrentDay.AddDanger(20);
                Main.Instance.CurrentDay.AddPop(10);
                break;
        }

        //Debug.Log(EventDay + "eventDay");
        //Debug.Log(Main.Instance.Turn + "Turn");
        Main.Instance.CurrentDay.AddGold(KillGold, Main.DayResult.EventType.Monster);
        GameManager.NPC.InactiveNPC(this);
    }




    protected override void NPC_Captive()
    {

    }
    protected override void NPC_Return_Empty()
    {
        Return();

        switch (EventDay)
        {
            case EventNPCType.Event_Goblin_Leader:
                Managers.Dialogue.ShowDialogueUI(DialogueName.Goblin_Empty, transform);
                break;
        }
    }
    protected override void NPC_Runaway()
    {
        Return();

        switch (EventDay)
        {
            case EventNPCType.Event_Goblin_Leader:
                Managers.Dialogue.ShowDialogueUI(DialogueName.Goblin_Empty, transform);
                break;
        }
    }
    protected override void NPC_Return_Satisfaction()
    {
        Return();

        switch (EventDay)
        {
            case EventNPCType.Event_Goblin_Leader:
                Managers.Dialogue.ShowDialogueUI(DialogueName.Goblin_Satisfiction, transform);
                break;
        }
    }
    protected override void NPC_Return_NonSatisfaction()
    {
        Return();

        switch (EventDay)
        {
            case EventNPCType.Event_Goblin_Leader:
                Managers.Dialogue.ShowDialogueUI(DialogueName.Goblin_Satisfiction, transform);
                break;
        }
    }






    void Return()
    {
        switch (EventDay)
        {
            case EventNPCType.Event_Day8:
                Managers.Dialogue.ShowDialogueUI($"Day{8}_ReturnEvent", transform);
                Main.Instance.CurrentDay.AddDanger(-10);
                Main.Instance.CurrentDay.AddPop(25);
                break;

            case EventNPCType.Event_RetiredHero:
                GuildManager.Instance.RemoveInstanceGuildNPC(GuildNPC_LabelName.RetiredHero);
                EventManager.Instance.RemoveQuestAction(1151);
                break;
        }
        

    }




}
