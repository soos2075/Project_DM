using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_MainEvent : NPC
{
    NPC_Type_MainEvent NPCType { get { return (NPC_Type_MainEvent)EventID; } }

    [field: SerializeField]
    public override List<BasementTile> PriorityList { get; set; }


    protected override void TurnOverEventSetting()
    {
        switch (NPCType)
        {
            case NPC_Type_MainEvent.EM_FirstAdventurer:
                break;
            case NPC_Type_MainEvent.EM_RedHair:
                break;
            case NPC_Type_MainEvent.Event_Goblin:
                break;
            case NPC_Type_MainEvent.Event_Goblin_Leader:
                break;
            case NPC_Type_MainEvent.Event_Goblin_Leader2:
                break;
            case NPC_Type_MainEvent.EM_Catastrophe:
                break;
            case NPC_Type_MainEvent.EM_RetiredHero:
                EventManager.Instance.AddTurnOverEventReserve(() => 
                {
                    GuildManager.Instance.RemoveInstanceGuildNPC(GuildNPC_LabelName.RetiredHero);
                    EventManager.Instance.RemoveQuestAction(1151);
                    EventManager.Instance.ReservationToQuest(3, 7010);
                    EventManager.Instance.ReservationToQuest(13, 7020);
                    EventManager.Instance.AddDayEvent(DayEventLabel.Guild_Raid_1, 0, 20, 0);
                    EventManager.Instance.AddDayEvent(DayEventLabel.Guild_Raid_2, 0, 30, 0);
                });
                break;

            case NPC_Type_MainEvent.EM_Blood_Warrior_A:
                EventManager.Instance.AddTurnOverEventReserve(() =>
                {
                    Managers.Dialogue.ShowDialogueUI(DialogueName.BloodSong_Return, Main.Instance.Player);
                });

                break;

            case NPC_Type_MainEvent.EM_Blood_Warrior_B:
                break;

            case NPC_Type_MainEvent.EM_Captine_A:
                break;
            case NPC_Type_MainEvent.EM_Captine_B:
                break;
            case NPC_Type_MainEvent.EM_Captine_BlueKnight:
                break;
        }
    }


    protected override void Start_Setting()
    {
        ////? 공격타입과 투사체 세팅
        //switch (NPCType)
        //{
        //    case NPC_Type_Normal.Elf:
        //        AttackOption.SetProjectile(AttackType.Bow, "LegucyElf", "ElfA");
        //        break;
        //    case NPC_Type_Normal.Wizard:
        //        AttackOption.SetProjectile(AttackType.Magic, "Fireball", "4");
        //        break;
        //}

        //? 도망 체력 세팅
        switch (NPCType)
        {
            case NPC_Type_MainEvent.EM_FirstAdventurer:
            case NPC_Type_MainEvent.EM_RedHair:
            case NPC_Type_MainEvent.EM_Catastrophe:
            case NPC_Type_MainEvent.EM_RetiredHero:
            case NPC_Type_MainEvent.EM_Captine_A:
            case NPC_Type_MainEvent.EM_Captine_B:
            case NPC_Type_MainEvent.EM_Captine_BlueKnight:
                RunawayHpRatio = 10000;
                break;

            case NPC_Type_MainEvent.EM_Blood_Warrior_A:
            case NPC_Type_MainEvent.EM_Blood_Warrior_B:
            case NPC_Type_MainEvent.EM_Blood_Tanker_A:
            case NPC_Type_MainEvent.EM_Blood_Tanker_B:
            case NPC_Type_MainEvent.EM_Blood_Wizard_A:
            case NPC_Type_MainEvent.EM_Blood_Wizard_B:
            case NPC_Type_MainEvent.EM_Blood_Elf_A:
            case NPC_Type_MainEvent.EM_Blood_Elf_B:
            case NPC_Type_MainEvent.Event_Goblin_Leader:
            case NPC_Type_MainEvent.Event_Goblin_Leader2:
            case NPC_Type_MainEvent.Event_Goblin:
                RunawayHpRatio = 2;
                break;

            default:
                RunawayHpRatio = 4;
                break;
        }

        //? 킬골드 세팅
        switch (NPCType)
        {
            case NPC_Type_MainEvent.EM_FirstAdventurer:
                KillGold = 50;
                break;

            case NPC_Type_MainEvent.EM_RedHair:
                KillGold = 100;
                break;

            case NPC_Type_MainEvent.Event_Goblin:
                KillGold = 100;
                break;

            case NPC_Type_MainEvent.Event_Goblin_Leader:
            case NPC_Type_MainEvent.Event_Goblin_Leader2:
                KillGold = 200;
                break;

            case NPC_Type_MainEvent.EM_Catastrophe:
                KillGold = 200;
                break;

            case NPC_Type_MainEvent.EM_RetiredHero:
                KillGold = 500;
                break;

            case NPC_Type_MainEvent.EM_Blood_Warrior_A:
            case NPC_Type_MainEvent.EM_Blood_Tanker_A:
            case NPC_Type_MainEvent.EM_Blood_Wizard_A:
            case NPC_Type_MainEvent.EM_Blood_Elf_A:
            case NPC_Type_MainEvent.EM_Blood_Warrior_B:
            case NPC_Type_MainEvent.EM_Blood_Tanker_B:
            case NPC_Type_MainEvent.EM_Blood_Wizard_B:
            case NPC_Type_MainEvent.EM_Blood_Elf_B:
                KillGold = 500;
                break;

            case NPC_Type_MainEvent.EM_Captine_A:
            case NPC_Type_MainEvent.EM_Captine_B:
                KillGold = 300;
                break;

            case NPC_Type_MainEvent.EM_Captine_BlueKnight:
                KillGold = 1000;
                break;

            case NPC_Type_MainEvent.EM_Soldier1:
            case NPC_Type_MainEvent.EM_Soldier2:
            case NPC_Type_MainEvent.EM_Soldier3:
                KillGold = 100;
                break;

            default:
                KillGold = Data.Rank * Random.Range(10, 30);
                break;
        }
    }






    protected override Define.TileType[] AvoidTileType { get { return AvoidTile(); } }

    Define.TileType[] AvoidTile()
    {
        switch (State)
        {
            case NPCState.Runaway:
            case NPCState.Return_Satisfaction:
                return new Define.TileType[] { Define.TileType.Facility, Define.TileType.Monster };

            default:
                switch (NPCType)
                {
                    case NPC_Type_MainEvent.Event_Goblin:
                    case NPC_Type_MainEvent.Event_Goblin_Leader:
                    case NPC_Type_MainEvent.Event_Goblin_Leader2:
                        return new Define.TileType[] { Define.TileType.NPC, Define.TileType.Monster };

                    case NPC_Type_MainEvent.EM_Blood_Warrior_A:
                    case NPC_Type_MainEvent.EM_Blood_Tanker_A:
                    case NPC_Type_MainEvent.EM_Blood_Wizard_A:
                    case NPC_Type_MainEvent.EM_Blood_Elf_A:
                    case NPC_Type_MainEvent.EM_Blood_Warrior_B:
                    case NPC_Type_MainEvent.EM_Blood_Tanker_B:
                    case NPC_Type_MainEvent.EM_Blood_Wizard_B:
                    case NPC_Type_MainEvent.EM_Blood_Elf_B:
                    case NPC_Type_MainEvent.EM_FirstAdventurer:
                    case NPC_Type_MainEvent.EM_RetiredHero:
                    case NPC_Type_MainEvent.EM_Captine_A:
                    case NPC_Type_MainEvent.EM_Captine_B:
                    case NPC_Type_MainEvent.EM_Captine_BlueKnight:
                        return new Define.TileType[] { Define.TileType.NPC, Define.TileType.Facility };

                    default:
                        return new Define.TileType[] { Define.TileType.NPC };
                }
        }
    }



    public override void Departure(Vector3 startPoint, Vector3 endPoint)
    {
        base.Departure(startPoint, endPoint);

        switch (NPCType)
        {
            case NPC_Type_MainEvent.EM_FirstAdventurer:
                StartCoroutine(EventCor(DialogueName.FirstAdvAppear));
                break;

            case NPC_Type_MainEvent.EM_RedHair:
                StartCoroutine(EventCor(DialogueName.RedHair_Appear));
                break;

            case NPC_Type_MainEvent.EM_RetiredHero:
                StartCoroutine(EventCor(DialogueName.RetiredHero_Appear));
                break;

            case NPC_Type_MainEvent.EM_Catastrophe:
                StartCoroutine(EventCor(DialogueName.Catastrophe_Appear));
                break;

            case NPC_Type_MainEvent.Event_Goblin_Leader:
                StartCoroutine(EventCor(DialogueName.Goblin_Appear));
                break;

            case NPC_Type_MainEvent.Event_Goblin_Leader2:
                StartCoroutine(EventCor(DialogueName.Goblin_Party));
                break;

            case NPC_Type_MainEvent.EM_Blood_Warrior_A:
                StartCoroutine(EventCor(DialogueName.BloodSong_Appear, () => 
                {
                    Dialogue_Highlight(NPC_Type_MainEvent.EM_Blood_Elf_A.ToString());
                    Dialogue_Highlight(NPC_Type_MainEvent.EM_Blood_Wizard_A.ToString());
                    Dialogue_Highlight(NPC_Type_MainEvent.EM_Blood_Tanker_A.ToString());
                }));
                break;
        }
    }

    protected override void SetRandomClothes()
    {
        var collection = characterBuilder.SpriteCollection;

        switch (NPCType)
        {
            case NPC_Type_MainEvent.EM_FirstAdventurer:
                characterBuilder.Hair = "Hair4#0098DC/0:0:0";
                characterBuilder.Armor = "TravelerTunic#FFFFFF/0:0:0";
                characterBuilder.Weapon = "IronSword#FFFFFF/0:0:0";
                break;

            case NPC_Type_MainEvent.EM_RedHair:
                characterBuilder.Hair = "Hair11#C42430/0:0:0";
                characterBuilder.Armor = "DemigodArmour";
                characterBuilder.Weapon = "Katana";
                break;

            case NPC_Type_MainEvent.EM_Catastrophe:
                characterBuilder.Hair = "";
                characterBuilder.Head = "Demigod#FFFFFF/0:0:0";
                characterBuilder.Ears = "Demigod#FFFFFF/0:0:0";
                characterBuilder.Eyes = "Demigod#FFFFFF/0:0:0";
                characterBuilder.Body = "Demigod#FFFFFF/0:0:0";
                characterBuilder.Weapon = "DeathScythe#FFFFFF/0:0:0";
                characterBuilder.Helmet = "ChumDoctorHelmet#FFFFFF/0:0:0";
                break;

            case NPC_Type_MainEvent.EM_RetiredHero:
                characterBuilder.Hair = "Hair10#858585/0:0:0";
                characterBuilder.Armor = "HeavyKnightArmor";
                characterBuilder.Weapon = "Epee";
                characterBuilder.Shield = "KnightShield";
                break;

            case NPC_Type_MainEvent.Event_Goblin:
            case NPC_Type_MainEvent.Event_Goblin_Leader:
            case NPC_Type_MainEvent.Event_Goblin_Leader2:
                characterBuilder.Hair = "";
                characterBuilder.Helmet = "";
                characterBuilder.Armor = "";

                characterBuilder.Head = "Goblin#FFFFFF/0:0:0";
                characterBuilder.Ears = "Goblin#FFFFFF/0:0:0";
                characterBuilder.Eyes = "Goblin#FFFFFF/0:0:0";
                characterBuilder.Body = "Goblin#FFFFFF/0:0:0";

                characterBuilder.Weapon = "RustedShovel#FFFFFF/0:0:0";
                characterBuilder.Back = "LargeBackpack#FFFFFF/0:0:0";
                break;

            case NPC_Type_MainEvent.EM_Blood_Warrior_A:
            case NPC_Type_MainEvent.EM_Blood_Warrior_B:
                characterBuilder.Hair = "Hair15#C42430/0:0:0";
                characterBuilder.Armor = "DemigodArmour#FFFFFF/0:0:0";
                characterBuilder.Weapon = "RedKatana#FFFFFF/0:0:0";
                characterBuilder.Cape = "Cape#FFFFFF/0:0:0";
                break;

            case NPC_Type_MainEvent.EM_Blood_Tanker_A:
            case NPC_Type_MainEvent.EM_Blood_Tanker_B:
                characterBuilder.Head = "Demon#FFFFFF/0:0:0";
                characterBuilder.Ears = "Demon#FFFFFF/0:0:0";
                characterBuilder.Eyes = "Demon#FFFFFF/0:0:0";
                characterBuilder.Body = "Demon#FFFFFF/0:0:0";
                characterBuilder.Hair = "";
                characterBuilder.Armor = "GuardianTunic#FFFFFF/0:0:0";
                characterBuilder.Weapon = "Lance#FFFFFF/0:0:0";
                characterBuilder.Shield = "Dreadnought#FFFFFF/0:0:0";
                break;

            case NPC_Type_MainEvent.EM_Blood_Wizard_A:
            case NPC_Type_MainEvent.EM_Blood_Wizard_B:
                characterBuilder.Helmet = "BlueWizzardHat#FFFFFF/128:0:0";
                characterBuilder.Armor = "FireWizardRobe#FFFFFF/0:0:0";
                characterBuilder.Weapon = "ElderStaff#FFFFFF/0:0:0";
                break;

            case NPC_Type_MainEvent.EM_Blood_Elf_A:
            case NPC_Type_MainEvent.EM_Blood_Elf_B:
                characterBuilder.Head = "Elf#FFFFFF/0:0:0";
                characterBuilder.Ears = "Elf#FFFFFF/0:0:0";
                characterBuilder.Eyes = "Elf#FFFFFF/0:0:0";
                characterBuilder.Body = "Elf#FFFFFF/0:0:0";
                characterBuilder.Hair = "Hair9#C42430/0:0:0";
                characterBuilder.Armor = "FemaleSwimmingSuit#FFFFFF/0:0:0";
                characterBuilder.Weapon = "CurvedBow#FFFFFF/0:0:0";
                break;

            case NPC_Type_MainEvent.EM_Captine_A:
            case NPC_Type_MainEvent.EM_Captine_B:
                characterBuilder.Helmet = "HeavyKnightHelmet#FFFFFF/0:0:0";
                characterBuilder.Armor = "IronKnight#FFFFFF/0:0:0";
                characterBuilder.Weapon = "RoyalLongsword#FFFFFF/0:0:0";
                break;

            case NPC_Type_MainEvent.EM_Captine_BlueKnight:
                characterBuilder.Helmet = "BlueKnightHelmet#FFFFFF/0:0:0";
                characterBuilder.Armor = "BlueKnight#FFFFFF/0:0:0";
                characterBuilder.Weapon = "MasterGreataxe#FFFFFF/0:0:0";
                characterBuilder.Cape = "Cape#FFFFFF/0:0:0";
                break;

            case NPC_Type_MainEvent.EM_Soldier1:
                characterBuilder.Helmet = "MilitiamanHelmet#FFFFFF/0:0:0";
                characterBuilder.Armor = "HornsKnight#FFFFFF/0:0:0";
                characterBuilder.Weapon = "IronSword#FFFFFF/0:0:0";
                characterBuilder.Shield = "IronBuckler#FFFFFF/0:0:0";
                break;

            case NPC_Type_MainEvent.EM_Soldier2:
                characterBuilder.Helmet = "CaptainHelmet#FFFFFF/0:0:0";
                characterBuilder.Armor = "HeavyKnightArmor#FFFFFF/0:0:0";
                characterBuilder.Weapon = "Epee#FFFFFF/0:0:0";
                break;

            case NPC_Type_MainEvent.EM_Soldier3:
                characterBuilder.Helmet = "LegionaryHelmet#FFFFFF/0:0:0";
                characterBuilder.Armor = "LegionaryArmor#FFFFFF/0:0:0";
                characterBuilder.Weapon = "GuardianHalberd#FFFFFF/0:0:0";
                characterBuilder.Shield = "RoyalGreatShield#FFFFFF/0:0:0";
                break;
        }


        characterBuilder.Rebuild();
    }
    protected override void SetPriorityList(PrioritySortOption option)
    {
        if (PriorityList != null) PriorityList.Clear();

        switch (NPCType)
        {
            case NPC_Type_MainEvent.Event_Goblin:
            case NPC_Type_MainEvent.Event_Goblin_Leader:
            case NPC_Type_MainEvent.Event_Goblin_Leader2:
                {
                    var herb = GetPriorityPick(typeof(Herb));
                    var mineral = GetPriorityPick(typeof(Mineral));
                    herb.AddRange(mineral);
                    AddPriorityList(herb, AddPos.Front, option);
                    AddPriorityList(GetPriorityPick(typeof(Treasure)), AddPos.Back, option);
                }
                break;

            case NPC_Type_MainEvent.EM_Soldier1:
            case NPC_Type_MainEvent.EM_Soldier2:
            case NPC_Type_MainEvent.EM_Soldier3:
            case NPC_Type_MainEvent.EM_Catastrophe:
                {
                    AddPriorityList(GetPriorityPick(typeof(Monster)), AddPos.Front, option);
                    AddPriorityList(GetPriorityPick(typeof(Treasure)), AddPos.Back, option);
                    AddPriorityList(GetPriorityPick(typeof(Herb)), AddPos.Back, option);
                    AddPriorityList(GetPriorityPick(typeof(Mineral)), AddPos.Back, option);
                }
                break;

            case NPC_Type_MainEvent.EM_FirstAdventurer:
            case NPC_Type_MainEvent.EM_RedHair:
            case NPC_Type_MainEvent.EM_RetiredHero: //? 얘는 나중에 위에 솔져로 바꿔야할지도 모르겠음
            case NPC_Type_MainEvent.EM_Captine_A:
            case NPC_Type_MainEvent.EM_Captine_B:
            case NPC_Type_MainEvent.EM_Captine_BlueKnight:
                AddPriorityList(GetPriorityPick(typeof(Monster)), AddPos.Front, option);
                //AddPriorityList(GetPriorityPick(typeof(Treasure)), AddPos.Front, option);
                break;

            case NPC_Type_MainEvent.EM_Blood_Warrior_A:
            case NPC_Type_MainEvent.EM_Blood_Warrior_B:
            case NPC_Type_MainEvent.EM_Blood_Tanker_A:
            case NPC_Type_MainEvent.EM_Blood_Tanker_B:
            case NPC_Type_MainEvent.EM_Blood_Wizard_A:
            case NPC_Type_MainEvent.EM_Blood_Wizard_B:
            case NPC_Type_MainEvent.EM_Blood_Elf_A:
            case NPC_Type_MainEvent.EM_Blood_Elf_B:
                AddPriorityList(GetPriorityPick(typeof(Monster)), AddPos.Front, option);
                AddPriorityList(GetPriorityPick(typeof(Treasure)), AddPos.Front, option);
                break;
        }

        {//? 우물 등 모험가 유용 이벤트
            Add_Wells();
        }
        {//? 에그서치
            var add_egg = GetPriorityPick(typeof(SpecialEgg));
            AddList(add_egg);
        }
        {//? 전이진 서치
            var portal = GetPriorityPick(typeof(Entrance_Egg));
            AddList(portal);
        }
    }



    void AddValue(int value, Main.TextType type)
    {
        if (value == 0)
        {
            return;
        }

        switch (type)
        {
            case Main.TextType.pop:
                Main.Instance.CurrentDay.AddPop(value);
                Main.Instance.ShowDM(value, Main.TextType.pop, transform, 1);
                break;

            case Main.TextType.danger:
                Main.Instance.CurrentDay.AddDanger(value);
                Main.Instance.ShowDM(value, Main.TextType.danger, transform, 1);
                break;

            case Main.TextType.mana:
                break;
            case Main.TextType.gold:
                break;
            case Main.TextType.exp:
                break;
            case Main.TextType.hp:
                break;
        }
    }



    protected override void NPC_Return_Empty()
    {
        Return();

        switch (NPCType)
        {
            case NPC_Type_MainEvent.Event_Goblin_Leader:
                Managers.Dialogue.ShowDialogueUI(DialogueName.Goblin_Empty, transform);
                break;
        }
    }
    protected override void NPC_Runaway()
    {
        Return();

        switch (NPCType)
        {
            case NPC_Type_MainEvent.Event_Goblin_Leader:
                Managers.Dialogue.ShowDialogueUI(DialogueName.Goblin_Empty, transform);
                break;

            case NPC_Type_MainEvent.EM_Soldier1:
            case NPC_Type_MainEvent.EM_Soldier2:
            case NPC_Type_MainEvent.EM_Soldier3:
                AddValue(5, Main.TextType.danger);
                break;
        }
    }
    protected override void NPC_Return_Satisfaction()
    {
        Return();

        switch (NPCType)
        {
            case NPC_Type_MainEvent.EM_RedHair:
                AddValue(100, Main.TextType.pop);
                break;

            case NPC_Type_MainEvent.Event_Goblin_Leader:
                Managers.Dialogue.ShowDialogueUI(DialogueName.Goblin_Satisfiction, transform);
                AddValue(30, Main.TextType.pop);
                break;

            case NPC_Type_MainEvent.Event_Goblin:
            case NPC_Type_MainEvent.Event_Goblin_Leader2:
                AddValue(15, Main.TextType.pop);
                break;

            case NPC_Type_MainEvent.EM_Blood_Warrior_A:
            case NPC_Type_MainEvent.EM_Blood_Warrior_B:
            case NPC_Type_MainEvent.EM_Blood_Tanker_A:
            case NPC_Type_MainEvent.EM_Blood_Tanker_B:
            case NPC_Type_MainEvent.EM_Blood_Wizard_A:
            case NPC_Type_MainEvent.EM_Blood_Wizard_B:
            case NPC_Type_MainEvent.EM_Blood_Elf_A:
            case NPC_Type_MainEvent.EM_Blood_Elf_B:
                AddValue(25, Main.TextType.pop);
                break;


            case NPC_Type_MainEvent.EM_Captine_A:
            case NPC_Type_MainEvent.EM_Captine_B:
            case NPC_Type_MainEvent.EM_Captine_BlueKnight:
                AddValue(50, Main.TextType.pop);
                break;


            case NPC_Type_MainEvent.EM_Soldier1:
            case NPC_Type_MainEvent.EM_Soldier2:
            case NPC_Type_MainEvent.EM_Soldier3:
                AddValue(10, Main.TextType.pop);
                break;

            default:
                AddValue(10, Main.TextType.pop);
                break;
        }
    }
    protected override void NPC_Return_NonSatisfaction()
    {
        Return();

        switch (NPCType)
        {
            case NPC_Type_MainEvent.EM_RedHair:
                AddValue(75, Main.TextType.pop);
                break;

            case NPC_Type_MainEvent.Event_Goblin_Leader:
                Managers.Dialogue.ShowDialogueUI(DialogueName.Goblin_Satisfiction, transform);
                Main.Instance.CurrentDay.AddPop(15);
                break;

            case NPC_Type_MainEvent.Event_Goblin:
            case NPC_Type_MainEvent.Event_Goblin_Leader2:
                AddValue(10, Main.TextType.pop);
                break;

            case NPC_Type_MainEvent.EM_Blood_Warrior_A:
            case NPC_Type_MainEvent.EM_Blood_Warrior_B:
            case NPC_Type_MainEvent.EM_Blood_Tanker_A:
            case NPC_Type_MainEvent.EM_Blood_Tanker_B:
            case NPC_Type_MainEvent.EM_Blood_Wizard_A:
            case NPC_Type_MainEvent.EM_Blood_Wizard_B:
            case NPC_Type_MainEvent.EM_Blood_Elf_A:
            case NPC_Type_MainEvent.EM_Blood_Elf_B:
                AddValue(10, Main.TextType.pop);
                break;


            case NPC_Type_MainEvent.EM_Captine_A:
            case NPC_Type_MainEvent.EM_Captine_B:
            case NPC_Type_MainEvent.EM_Captine_BlueKnight:
                AddValue(20, Main.TextType.pop);
                break;


            case NPC_Type_MainEvent.EM_Soldier1:
            case NPC_Type_MainEvent.EM_Soldier2:
            case NPC_Type_MainEvent.EM_Soldier3:
                AddValue(5, Main.TextType.pop);
                break;

            default:
                AddValue(5, Main.TextType.pop);
                break;
        }
    }



    void Return()
    {
        AddCollectionPoint();

        switch (NPCType)
        {
            case NPC_Type_MainEvent.EM_RedHair:
                Managers.Dialogue.ShowDialogueUI(DialogueName.RedHair_Return, transform);
                break;

            case NPC_Type_MainEvent.Event_Goblin_Leader2:
                EventManager.Instance.RemoveQuestAction(1150);
                break;

            case NPC_Type_MainEvent.EM_Catastrophe:
                if (UserData.Instance.FileConfig.firstReturn_Catastrophe == false)
                {
                    UserData.Instance.FileConfig.firstReturn_Catastrophe = true;
                    Managers.Dialogue.ShowDialogueUI(DialogueName.Catastrophe_Return_First, transform);
                }
                else
                {
                    Managers.Dialogue.ShowDialogueUI(DialogueName.Catastrophe_Return, transform);
                }
                break;
        }
    }


    protected override void NPC_Die()
    {
        Defeat();

        switch (NPCType)
        {
            case NPC_Type_MainEvent.EM_FirstAdventurer:
                AddValue(5, Main.TextType.danger);
                break;

            case NPC_Type_MainEvent.EM_RedHair:
                AddValue(75, Main.TextType.danger);
                break;

            case NPC_Type_MainEvent.Event_Goblin:
                AddValue(15, Main.TextType.danger);
                break;

            case NPC_Type_MainEvent.Event_Goblin_Leader:
            case NPC_Type_MainEvent.Event_Goblin_Leader2:
                AddValue(30, Main.TextType.danger);
                break;

            case NPC_Type_MainEvent.EM_Catastrophe:
                AddValue(25, Main.TextType.danger);
                break;

            case NPC_Type_MainEvent.EM_RetiredHero:
                AddValue(50, Main.TextType.danger);
                break;

            case NPC_Type_MainEvent.EM_Blood_Warrior_A:
            case NPC_Type_MainEvent.EM_Blood_Tanker_A:
            case NPC_Type_MainEvent.EM_Blood_Wizard_A:
            case NPC_Type_MainEvent.EM_Blood_Elf_A:
            case NPC_Type_MainEvent.EM_Blood_Warrior_B:
            case NPC_Type_MainEvent.EM_Blood_Tanker_B:
            case NPC_Type_MainEvent.EM_Blood_Wizard_B:
            case NPC_Type_MainEvent.EM_Blood_Elf_B:
                AddValue(25, Main.TextType.danger);
                break;

            case NPC_Type_MainEvent.EM_Captine_A:
            case NPC_Type_MainEvent.EM_Captine_B:
                AddValue(20, Main.TextType.danger);
                break;

            case NPC_Type_MainEvent.EM_Captine_BlueKnight:
                AddValue(100, Main.TextType.danger);
                break;

            default:
                AddValue(10, Main.TextType.danger);
                break;
        }
    }

    void Defeat()
    {
        AddCollectionPoint();
        switch (NPCType)
        {
            case NPC_Type_MainEvent.EM_RedHair:
                Managers.Dialogue.ShowDialogueUI(DialogueName.RedHair_Defeat, transform);
                break;

            case NPC_Type_MainEvent.EM_RetiredHero:
                Managers.Dialogue.ShowDialogueUI(DialogueName.RetiredHero_Defeat, transform);
                break;

            case NPC_Type_MainEvent.Event_Goblin_Leader:
                Managers.Dialogue.ShowDialogueUI(DialogueName.Goblin_Die, transform);
                break;

            case NPC_Type_MainEvent.Event_Goblin_Leader2:
                EventManager.Instance.RemoveQuestAction(1150);
                break;

            case NPC_Type_MainEvent.EM_Catastrophe:
                if (UserData.Instance.FileConfig.firstReturn_Catastrophe == false)
                {
                    UserData.Instance.FileConfig.firstReturn_Catastrophe = true;
                    Managers.Dialogue.ShowDialogueUI(DialogueName.Catastrophe_Return_First, transform);
                }
                else
                {
                    Managers.Dialogue.ShowDialogueUI(DialogueName.Catastrophe_Return, transform);
                }
                break;
        }

        Managers.Dialogue.ActionReserve(() => InactiveCallback());
    }


    void InactiveCallback()
    {
        UI_EventBox.AddEventText($"◈{Name_Color} {UserData.Instance.LocaleText("Event_Defeat")}");
        Main.Instance.CurrentDay.AddGold(KillGold, Main.DayResult.EventType.Monster);
        GameManager.NPC.InactiveNPC(this);
    }

}
