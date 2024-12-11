using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Normal : NPC
{
    NPC_Type_Normal NPCType { get { return (NPC_Type_Normal)EventID; } }

    [field: SerializeField]
    public override List<BasementTile> PriorityList { get; set; }


    protected override void Start_Setting()
    {
        //? 공격타입과 투사체 세팅
        switch (NPCType)
        {
            case NPC_Type_Normal.Elf:
                AttackOption.SetProjectile(AttackType.Bow, "LegucyElf", "ElfA");
                break;
            case NPC_Type_Normal.Wizard:
                AttackOption.SetProjectile(AttackType.Magic, "Fireball", "4");
                break;
        }

        //? 킬골드와 도망치는 조건 등등의 세팅
        switch (NPCType)
        {
            case NPC_Type_Normal.Herbalist1:
            case NPC_Type_Normal.Herbalist2:
            case NPC_Type_Normal.Herbalist3:
            case NPC_Type_Normal.Miner1:
            case NPC_Type_Normal.Miner2:
            case NPC_Type_Normal.Miner3:
                KillGold = Data.Rank * Random.Range(10, 16);
                RunawayHpRatio = 2;
                break;

            case NPC_Type_Normal.Adventurer1:
            case NPC_Type_Normal.Adventurer2:
            case NPC_Type_Normal.Elf:
            case NPC_Type_Normal.Wizard:
                KillGold = Data.Rank * Random.Range(20, 31);
                RunawayHpRatio = 4;
                break;

            case NPC_Type_Normal.DarkElf:
            case NPC_Type_Normal.Vampire:
                KillGold = Data.Rank * Random.Range(30, 46);
                RunawayHpRatio = 5;
                break;

            case NPC_Type_Normal.Normal_Goblin:
                KillGold = Data.Rank * Random.Range(15, 31);
                RunawayHpRatio = 3;
                break;
        }
    }


    protected override Define.TileType[] AvoidTileType { get { return AvoidTile(); } }

    Define.TileType[] AvoidTile() //? 어차피 제일 처음 길찾기 조건이고, 길을 못찾으면 다시 찾을 땐 이거 없이 찾으니까 괜찮
    {
        switch (State)
        {
            case NPCState.Runaway:
            case NPCState.Return_Satisfaction:
                return new Define.TileType[] { Define.TileType.Facility, Define.TileType.Monster };

            default:
                switch (NPCType)
                {
                    case NPC_Type_Normal.Herbalist1:
                    case NPC_Type_Normal.Herbalist2:
                    case NPC_Type_Normal.Herbalist3:
                    case NPC_Type_Normal.Miner1:
                    case NPC_Type_Normal.Miner2:
                    case NPC_Type_Normal.Miner3:
                    case NPC_Type_Normal.Normal_Goblin:
                        return new Define.TileType[] { Define.TileType.NPC, Define.TileType.Monster };

                    case NPC_Type_Normal.Adventurer1:
                    case NPC_Type_Normal.Adventurer2:
                        return new Define.TileType[] { Define.TileType.NPC, Define.TileType.Facility };

                    case NPC_Type_Normal.Elf:
                    case NPC_Type_Normal.Wizard:
                    case NPC_Type_Normal.DarkElf:
                    case NPC_Type_Normal.Vampire:
                        return new Define.TileType[] { Define.TileType.NPC };


                    default:
                        return new Define.TileType[] { Define.TileType.NPC };
                }
        }
    }

    protected override void SetRandomClothes() //? 복장
    {
        var collection = characterBuilder.SpriteCollection;

        switch (NPCType)
        {
            case NPC_Type_Normal.Herbalist1:
                characterBuilder.Armor = "ArcherTunic";
                characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_Herbalist);
                characterBuilder.Back = "SmallBackpack";
                characterBuilder.Helmet = "ArcherHood";
                break;

            case NPC_Type_Normal.Herbalist2:
                characterBuilder.Armor = "ArcherTunic";
                characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_Herbalist);
                characterBuilder.Back = "SmallBackpack";
                characterBuilder.Helmet = "FireWizardHood#FFFFFF/0:0:0";
                break;

            case NPC_Type_Normal.Herbalist3:
                characterBuilder.Armor = "ArcherTunic";
                characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_Herbalist);
                characterBuilder.Back = "SmallBackpack";
                characterBuilder.Helmet = "ClericHood#FFFFFF/0:0:0";
                break;

            case NPC_Type_Normal.Miner1:
                characterBuilder.Armor = "MinerArmour";
                characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_Miner);
                characterBuilder.Back = "LargeBackpack";
                characterBuilder.Helmet = "MinerHelment";
                break;

            case NPC_Type_Normal.Miner2:
                characterBuilder.Armor = "MinerArmour";
                characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_Miner);
                characterBuilder.Back = "LargeBackpack";
                characterBuilder.Helmet = "MinerHelment#FFFFFF/-45:0:0";
                break;

            case NPC_Type_Normal.Miner3:
                characterBuilder.Armor = "MinerArmour";
                characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_Miner);
                characterBuilder.Back = "LargeBackpack";
                characterBuilder.Helmet = "MinerHelment#FFFFFF/180:0:0";
                break;

            case NPC_Type_Normal.Adventurer1:
                { // hair = 9
                    int ran = Random.Range(0, collection.Layers[9].Textures.Count);
                    characterBuilder.Hair = collection.Layers[9].Textures[ran].name;
                    string hexColor = Define.HairColors[Random.Range(0, 24)];
                    characterBuilder.Hair += hexColor;
                }
                characterBuilder.Armor = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Armor_Warrior);
                characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_BeginnerSword);
                characterBuilder.Shield = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_BeginnerShield);
                break;

            case NPC_Type_Normal.Adventurer2:
                { // hair = 9
                    int ran = Random.Range(0, collection.Layers[9].Textures.Count);
                    characterBuilder.Hair = collection.Layers[9].Textures[ran].name;
                    string hexColor = Define.HairColors[Random.Range(0, 24)];
                    characterBuilder.Hair += hexColor;
                }
                characterBuilder.Armor = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Armor_Warrior);
                characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_TwoHandSword);
                characterBuilder.Shield = "";
                break;

            case NPC_Type_Normal.Elf:
                { // hair = 9
                    int ran = Random.Range(0, collection.Layers[9].Textures.Count);
                    characterBuilder.Hair = collection.Layers[9].Textures[ran].name;
                    string hexColor = Define.HairColors[Random.Range(0, 24)];
                    characterBuilder.Hair += hexColor;
                }
                characterBuilder.Armor = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Armor_Elf);
                characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_Bow);
                break;

            case NPC_Type_Normal.DarkElf:
                characterBuilder.Hair = "";
                characterBuilder.Head = "DarkElf#FFFFFF/0:0:0";
                characterBuilder.Ears = "DarkElf#FFFFFF/0:0:0";
                characterBuilder.Eyes = "DarkElf#FFFFFF/0:0:0";
                characterBuilder.Body = "DarkElf#FFFFFF/0:0:0";
                characterBuilder.Weapon = "HunterKnife#FFFFFF/0:0:0";
                characterBuilder.Armor = "HolowKnight#FFFFFF/0:0:0";
                break;

            case NPC_Type_Normal.Wizard:
                characterBuilder.Armor = "BlueWizardTunic";
                characterBuilder.Helmet = "BlueWizzardHat";
                characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_Magic);
                break;

            case NPC_Type_Normal.Vampire:
                characterBuilder.Hair = "";
                characterBuilder.Head = "Vampire#FFFFFF/0:0:0";
                characterBuilder.Ears = "Vampire#FFFFFF/0:0:0";
                characterBuilder.Eyes = "Vampire#FFFFFF/0:0:0";
                characterBuilder.Body = "Vampire#FFFFFF/0:0:0";
                characterBuilder.Weapon = "ElderStaff#FFFFFF/0:0:0";
                characterBuilder.Armor = "DeathRobe#FFFFFF/0:0:0";
                break;

            case NPC_Type_Normal.Normal_Goblin:
                characterBuilder.Hair = "";
                characterBuilder.Head = "Goblin#FFFFFF/0:0:0";
                characterBuilder.Ears = "Goblin#FFFFFF/0:0:0";
                characterBuilder.Eyes = "Goblin#FFFFFF/0:0:0";
                characterBuilder.Body = "Goblin#FFFFFF/0:0:0";
                characterBuilder.Weapon = "RustedShovel#FFFFFF/0:0:0";
                characterBuilder.Back = "LargeBackpack#FFFFFF/0:0:0";
                break;
        }

        characterBuilder.Rebuild();
    }

    public override void Departure(Vector3 startPoint, Vector3 endPoint) //? 던전에 도착했을 때
    {
        base.Departure(startPoint, endPoint);

        switch (NPCType)
        {
            case NPC_Type_Normal.Herbalist1:
                if (UserData.Instance.FileConfig.firstAppear_Herbalist == false)
                {
                    UserData.Instance.FileConfig.firstAppear_Herbalist = true;
                    StartCoroutine(EventCor(DialogueName.Herbalist0_Appear));
                }
                break;

            case NPC_Type_Normal.Miner1:
                if (UserData.Instance.FileConfig.firstAppear_Miner == false)
                {
                    UserData.Instance.FileConfig.firstAppear_Miner = true;
                    StartCoroutine(EventCor(DialogueName.Miner0_Appear));
                }
                break;

            case NPC_Type_Normal.Elf:
                if (UserData.Instance.FileConfig.firstAppear_Elf == false)
                {
                    UserData.Instance.FileConfig.firstAppear_Elf = true;
                    StartCoroutine(EventCor(DialogueName.Elf_Appear));
                }
                break;

            case NPC_Type_Normal.Wizard:
                if (UserData.Instance.FileConfig.firstAppear_Wizard == false)
                {
                    UserData.Instance.FileConfig.firstAppear_Wizard = true;
                    StartCoroutine(EventCor(DialogueName.Wizard_Appear));
                }
                break;
        }
    }






    protected override void SetPriorityList(PrioritySortOption option)
    {
        if (PriorityList != null) PriorityList.Clear();

        switch (NPCType)
        {
            case NPC_Type_Normal.Herbalist1:
            case NPC_Type_Normal.Herbalist2:
            case NPC_Type_Normal.Herbalist3:
                AddPriorityList(GetPriorityPick(typeof(Herb)), AddPos.Front, option);
                break;

            case NPC_Type_Normal.Miner1:
            case NPC_Type_Normal.Miner2:
            case NPC_Type_Normal.Miner3:
                AddPriorityList(GetPriorityPick(typeof(Mineral)), AddPos.Front, option);
                break;

            case NPC_Type_Normal.Adventurer1:
            case NPC_Type_Normal.Adventurer2:
            case NPC_Type_Normal.DarkElf:
            case NPC_Type_Normal.Vampire:
                AddPriorityList(GetPriorityPick(typeof(Monster)), AddPos.Front, option);
                AddPriorityList(GetPriorityPick(typeof(Treasure)), AddPos.Front, option);
                break;

            case NPC_Type_Normal.Elf:
                AddPriorityList(GetPriorityPick(typeof(Monster)), AddPos.Front, option);
                AddPriorityList(GetPriorityPick(typeof(Herb)), AddPos.Front, option);
                break;

            case NPC_Type_Normal.Wizard:
                AddPriorityList(GetPriorityPick(typeof(Monster)), AddPos.Front, option);
                AddPriorityList(GetPriorityPick(typeof(Mineral)), AddPos.Front, option);
                break;

            case NPC_Type_Normal.Normal_Goblin:
                
                var herb = GetPriorityPick(typeof(Herb));
                var mineral = GetPriorityPick(typeof(Mineral));
                herb.AddRange(mineral);
                AddPriorityList(herb, AddPos.Front, option);
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
            PickToProbability(GetPriorityPick(typeof(Entrance_Egg)), (PlacementInfo.Place_Floor.FloorIndex + Rank) * 0.04f); //0.04f
        }
    }



    protected override void NPC_Return_Empty()
    {
        int popValue = -Data.Rank;
        int dangerValue = -Data.Rank;

        switch (NPCType)
        {
            case NPC_Type_Normal.Herbalist1:
            case NPC_Type_Normal.Herbalist2:
            case NPC_Type_Normal.Herbalist3:
            case NPC_Type_Normal.Miner1:
            case NPC_Type_Normal.Miner2:
            case NPC_Type_Normal.Miner3:
            case NPC_Type_Normal.Normal_Goblin:
                popValue = -Data.Rank;
                dangerValue = 0;
                break;

            case NPC_Type_Normal.Adventurer1:
            case NPC_Type_Normal.Adventurer2:
            case NPC_Type_Normal.Elf:
            case NPC_Type_Normal.Wizard:
            case NPC_Type_Normal.DarkElf:
            case NPC_Type_Normal.Vampire:
                popValue = -(Data.Rank / 2);
                dangerValue = -(Data.Rank / 2);
                break;

        }

        if (popValue != 0)
        {
            Main.Instance.CurrentDay.AddPop(popValue);
            Main.Instance.ShowDM(popValue, Main.TextType.pop, transform, 1);
        }
        if (dangerValue != 0)
        {
            Main.Instance.CurrentDay.AddDanger(dangerValue);
            Main.Instance.ShowDM(dangerValue, Main.TextType.danger, transform, 1);
        }
    }
    protected override void NPC_Return_Satisfaction()
    {
        int popValue = Data.Rank;
        int dangerValue = Data.Rank;

        switch (NPCType)
        {
            case NPC_Type_Normal.Herbalist1:
            case NPC_Type_Normal.Herbalist2:
            case NPC_Type_Normal.Herbalist3:
            case NPC_Type_Normal.Miner1:
            case NPC_Type_Normal.Miner2:
            case NPC_Type_Normal.Miner3:
            case NPC_Type_Normal.Normal_Goblin:
                popValue = Mathf.RoundToInt(Data.Rank * 1.6f);
                dangerValue = 0;
                break;

            case NPC_Type_Normal.Adventurer1:
            case NPC_Type_Normal.Adventurer2:
            case NPC_Type_Normal.Elf:
            case NPC_Type_Normal.Wizard:
            case NPC_Type_Normal.DarkElf:
            case NPC_Type_Normal.Vampire:
                popValue = Data.Rank;
                dangerValue = 0;
                break;

        }

        if (popValue != 0)
        {
            Main.Instance.CurrentDay.AddPop(popValue);
            Main.Instance.ShowDM(popValue, Main.TextType.pop, transform, 1);
        }
        if (dangerValue != 0)
        {
            Main.Instance.CurrentDay.AddDanger(dangerValue);
            Main.Instance.ShowDM(dangerValue, Main.TextType.danger, transform, 1);
        }
    }
    protected override void NPC_Return_NonSatisfaction()
    {
        int popValue = Data.Rank;
        int dangerValue = Data.Rank;

        switch (NPCType)
        {
            case NPC_Type_Normal.Herbalist1:
            case NPC_Type_Normal.Herbalist2:
            case NPC_Type_Normal.Herbalist3:
            case NPC_Type_Normal.Miner1:
            case NPC_Type_Normal.Miner2:
            case NPC_Type_Normal.Miner3:
            case NPC_Type_Normal.Normal_Goblin:
                popValue = Mathf.RoundToInt(Data.Rank * 0.6f);
                dangerValue = 0;
                break;

            case NPC_Type_Normal.Adventurer1:
            case NPC_Type_Normal.Adventurer2:
            case NPC_Type_Normal.Elf:
            case NPC_Type_Normal.Wizard:
            case NPC_Type_Normal.DarkElf:
            case NPC_Type_Normal.Vampire:
                popValue = Mathf.RoundToInt(Data.Rank * 0.6f);
                dangerValue = Mathf.RoundToInt(Data.Rank * 0.6f);
                break;
        }

        if (popValue != 0)
        {
            Main.Instance.CurrentDay.AddPop(popValue);
            Main.Instance.ShowDM(popValue, Main.TextType.pop, transform, 1);
        }
        if (dangerValue != 0)
        {
            Main.Instance.CurrentDay.AddDanger(dangerValue);
            Main.Instance.ShowDM(dangerValue, Main.TextType.danger, transform, 1);
        }
    }
    protected override void NPC_Runaway()
    {
        int popValue = Data.Rank;
        int dangerValue = Data.Rank;

        switch (NPCType)
        {
            case NPC_Type_Normal.Herbalist1:
            case NPC_Type_Normal.Herbalist2:
            case NPC_Type_Normal.Herbalist3:
            case NPC_Type_Normal.Miner1:
            case NPC_Type_Normal.Miner2:
            case NPC_Type_Normal.Miner3:
            case NPC_Type_Normal.Normal_Goblin:
                popValue = 0;
                dangerValue = Data.Rank;
                break;

            case NPC_Type_Normal.Adventurer1:
            case NPC_Type_Normal.Adventurer2:
            case NPC_Type_Normal.Elf:
            case NPC_Type_Normal.Wizard:
            case NPC_Type_Normal.DarkElf:
            case NPC_Type_Normal.Vampire:
                //popValue = (Data.Rank / 2);
                popValue = 0;
                dangerValue = (Data.Rank / 2);
                break;
        }

        if (popValue != 0)
        {
            Main.Instance.CurrentDay.AddPop(popValue);
            Main.Instance.ShowDM(popValue, Main.TextType.pop, transform, 1);
        }
        if (dangerValue != 0)
        {
            Main.Instance.CurrentDay.AddDanger(dangerValue);
            Main.Instance.ShowDM(dangerValue, Main.TextType.danger, transform, 1);
        }
    }
    protected override void NPC_Die()
    {
        bool isCaptive = false;
        var prison = GameManager.Technical.Prison;
        if (prison != null)
        {
            var ran = Random.Range(0, 10);
            if (ran > 6)
            {
                isCaptive = true;
            }
        }
        KillorCaptive(isCaptive);
        GameManager.NPC.InactiveNPC(this);
    }

    void KillorCaptive(bool isCaptive = false)
    {
        if (isCaptive)
        {
            UI_EventBox.AddEventText($"◈{Name_Color} {UserData.Instance.LocaleText("Event_Prison")}");
            Main.Instance.CurrentDay.AddPrisoner(1);
            //Main.Instance.CurrentDay.AddDefeatNPC(-1);
        }
        else
        {
            UI_EventBox.AddEventText($"◈{Name_Color} {UserData.Instance.LocaleText("Event_Defeat")}");
        }

        int popValue = 0;
        int dangerValue = Data.Rank;

        switch (NPCType)
        {
            case NPC_Type_Normal.Herbalist1:
            case NPC_Type_Normal.Herbalist2:
            case NPC_Type_Normal.Herbalist3:
            case NPC_Type_Normal.Miner1:
            case NPC_Type_Normal.Miner2:
            case NPC_Type_Normal.Miner3:
            case NPC_Type_Normal.Normal_Goblin:
                dangerValue = Data.Rank * 2;
                break;

            case NPC_Type_Normal.Adventurer1:
            case NPC_Type_Normal.Adventurer2:
            case NPC_Type_Normal.Elf:
            case NPC_Type_Normal.Wizard:
            case NPC_Type_Normal.DarkElf:
            case NPC_Type_Normal.Vampire:
                break;
        }

        if (popValue != 0)
        {
            Main.Instance.CurrentDay.AddPop(popValue);
            Main.Instance.ShowDM(popValue, Main.TextType.pop, transform, 1);
        }
        if (dangerValue != 0)
        {
            Main.Instance.CurrentDay.AddDanger(dangerValue);
            Main.Instance.ShowDM(dangerValue, Main.TextType.danger, transform, 1);
        }


        //int goldValue = isCaptive ? KillGold * 2 : KillGold;
        //if (goldValue > 0)
        //{
        //    Main.Instance.CurrentDay.AddGold(goldValue, Main.DayResult.EventType.Monster);
        //    Main.Instance.ShowDM(goldValue, Main.TextType.gold, transform);
        //}
        int goldValue = KillGold;

        if (isCaptive)
        {
            Main.Instance.CurrentDay.AddGold(goldValue, Main.DayResult.EventType.Technical);
            Main.Instance.ShowDM(goldValue, Main.TextType.gold, transform);
        }

        Main.Instance.CurrentDay.AddGold(goldValue, Main.DayResult.EventType.Monster);
        Main.Instance.ShowDM(goldValue, Main.TextType.gold, transform);
    }

}
