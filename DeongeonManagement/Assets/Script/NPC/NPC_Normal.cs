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
            case NPC_Type_Normal.Herbalist0:
            case NPC_Type_Normal.Herbalist1:
            case NPC_Type_Normal.Herbalist2:
            case NPC_Type_Normal.Miner0:
            case NPC_Type_Normal.Miner1:
                KillGold = Data.Rank * Random.Range(10, 21);
                RunawayHpRatio = 2;
                break;

            case NPC_Type_Normal.Adventurer0:
            case NPC_Type_Normal.Adventurer1:
            case NPC_Type_Normal.Elf:
            case NPC_Type_Normal.Wizard:
                KillGold = Data.Rank * Random.Range(15, 31);
                RunawayHpRatio = 4;
                break;

            case NPC_Type_Normal.Goblin:
                KillGold = Data.Rank * Random.Range(20, 41);
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
                    case NPC_Type_Normal.Herbalist0:
                    case NPC_Type_Normal.Herbalist1:
                    case NPC_Type_Normal.Miner0:
                    case NPC_Type_Normal.Miner1:
                        return new Define.TileType[] { Define.TileType.NPC, Define.TileType.Monster };

                    case NPC_Type_Normal.Adventurer0:
                    case NPC_Type_Normal.Adventurer1:
                        return new Define.TileType[] { Define.TileType.NPC, Define.TileType.Facility };

                    case NPC_Type_Normal.Elf:
                    case NPC_Type_Normal.Wizard:
                        return new Define.TileType[] { Define.TileType.NPC };

                    case NPC_Type_Normal.Goblin:
                        return new Define.TileType[] { Define.TileType.NPC, Define.TileType.Monster };

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
            case NPC_Type_Normal.Herbalist0:
                characterBuilder.Armor = "ArcherTunic";
                characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_Herbalist);
                characterBuilder.Back = "SmallBackpack";
                characterBuilder.Helmet = "ArcherHood";
                break;

            case NPC_Type_Normal.Herbalist1:
                characterBuilder.Armor = "ArcherTunic";
                characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_Herbalist);
                characterBuilder.Back = "SmallBackpack";
                characterBuilder.Helmet = "FireWizardHood#FFFFFF/0:0:0";
                break;

            case NPC_Type_Normal.Herbalist2:
                characterBuilder.Armor = "ArcherTunic";
                characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_Herbalist);
                characterBuilder.Back = "SmallBackpack";
                characterBuilder.Helmet = "ClericHood#FFFFFF/0:0:0";
                break;

            case NPC_Type_Normal.Miner0:
                characterBuilder.Armor = "MinerArmour";
                characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_Miner);
                characterBuilder.Back = "LargeBackpack";
                characterBuilder.Helmet = "MinerHelment";
                break;

            case NPC_Type_Normal.Miner1:
                characterBuilder.Armor = "MinerArmour";
                characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_Miner);
                characterBuilder.Back = "LargeBackpack";
                characterBuilder.Helmet = "MinerHelment#FFFFFF/-45:0:0";
                break;

            //case NPC_Type_Normal.Miner2:
            //    characterBuilder.Armor = "MinerArmour";
            //    characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_Miner);
            //    characterBuilder.Back = "LargeBackpack";
            //    characterBuilder.Helmet = "MinerHelment#FFFFFF/180:0:0";
            //    break;

            case NPC_Type_Normal.Adventurer0:
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

            case NPC_Type_Normal.Adventurer1:
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

            case NPC_Type_Normal.Wizard:
                characterBuilder.Armor = "BlueWizardTunic";
                characterBuilder.Helmet = "BlueWizzardHat";
                characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_Magic);
                break;

            case NPC_Type_Normal.Goblin:
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
            case NPC_Type_Normal.Herbalist0:
                if (UserData.Instance.FileConfig.firstAppear_Herbalist == false)
                {
                    UserData.Instance.FileConfig.firstAppear_Herbalist = true;
                    StartCoroutine(EventCor(DialogueName.Herbalist0_Appear));
                }
                break;

            case NPC_Type_Normal.Miner0:
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


        List<BasementTile> main = null;
        List<BasementTile> sub1 = null;
        List<BasementTile> sub2 = null;

        switch (NPCType)
        {
            case NPC_Type_Normal.Herbalist0:
            case NPC_Type_Normal.Herbalist1:
            case NPC_Type_Normal.Herbalist2:
                main = GetPriorityPick(typeof(Herb));
                break;

            case NPC_Type_Normal.Miner0:
            case NPC_Type_Normal.Miner1:
                main = GetPriorityPick(typeof(Mineral));
                break;

            case NPC_Type_Normal.Adventurer0:
            case NPC_Type_Normal.Adventurer1:
                main = GetPriorityPick(typeof(Monster));
                sub1 = GetPriorityPick(typeof(Treasure));
                break;

            case NPC_Type_Normal.Elf:
                main = GetPriorityPick(typeof(Herb));
                sub1 = GetPriorityPick(typeof(Monster));
                break;

            case NPC_Type_Normal.Wizard:
                main = GetPriorityPick(typeof(Mineral));
                sub1 = GetPriorityPick(typeof(Monster));
                break;

            case NPC_Type_Normal.Goblin:
                var herb = GetPriorityPick(typeof(Herb));
                var mineral = GetPriorityPick(typeof(Mineral));
                herb.AddRange(mineral);
                main = herb;
                break;
        }



        //? 메인이랑 서브는 위에서 결정
        switch (option)
        {
            case PrioritySortOption.Random:
                break;
            case PrioritySortOption.SortByDistance:
                SortByDistance(main); 
                SortByDistance(sub1); 
                SortByDistance(sub2); 
                break;
        }

        AddList(main);
        AddList(sub1);
        AddList(sub2);


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
            case NPC_Type_Normal.Herbalist0:
            case NPC_Type_Normal.Herbalist1:
            case NPC_Type_Normal.Herbalist2:
            case NPC_Type_Normal.Miner0:
            case NPC_Type_Normal.Miner1:
                popValue = -Data.Rank;
                dangerValue = 0;
                break;

            case NPC_Type_Normal.Adventurer0:
            case NPC_Type_Normal.Adventurer1:
            case NPC_Type_Normal.Elf:
            case NPC_Type_Normal.Wizard:
                popValue = -(Data.Rank / 2);
                dangerValue = -(Data.Rank / 2);
                break;

            case NPC_Type_Normal.Goblin:
                popValue = -Data.Rank * 2;
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
    protected override void NPC_Return_Satisfaction()
    {
        int popValue = Data.Rank;
        int dangerValue = Data.Rank;

        switch (NPCType)
        {
            case NPC_Type_Normal.Herbalist0:
            case NPC_Type_Normal.Herbalist1:
            case NPC_Type_Normal.Herbalist2:
            case NPC_Type_Normal.Miner0:
            case NPC_Type_Normal.Miner1:
                popValue = Mathf.RoundToInt(Data.Rank * 1.6f);
                dangerValue = 0;
                break;

            case NPC_Type_Normal.Adventurer0:
            case NPC_Type_Normal.Adventurer1:
            case NPC_Type_Normal.Elf:
            case NPC_Type_Normal.Wizard:
                popValue = Data.Rank;
                dangerValue = 0;
                break;

            case NPC_Type_Normal.Goblin:
                popValue = Data.Rank * 2;
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
            case NPC_Type_Normal.Herbalist0:
            case NPC_Type_Normal.Herbalist1:
            case NPC_Type_Normal.Herbalist2:
            case NPC_Type_Normal.Miner0:
            case NPC_Type_Normal.Miner1:
                popValue = Mathf.RoundToInt(Data.Rank * 0.6f);
                dangerValue = 0;
                break;

            case NPC_Type_Normal.Adventurer0:
            case NPC_Type_Normal.Adventurer1:
            case NPC_Type_Normal.Elf:
            case NPC_Type_Normal.Wizard:
                popValue = Mathf.RoundToInt(Data.Rank * 0.6f);
                dangerValue = Mathf.RoundToInt(Data.Rank * 0.6f);
                break;

            case NPC_Type_Normal.Goblin:
                popValue = 0;
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
    protected override void NPC_Runaway()
    {
        int popValue = Data.Rank;
        int dangerValue = Data.Rank;

        switch (NPCType)
        {
            case NPC_Type_Normal.Herbalist0:
            case NPC_Type_Normal.Herbalist1:
            case NPC_Type_Normal.Herbalist2:
            case NPC_Type_Normal.Miner0:
            case NPC_Type_Normal.Miner1:
                popValue = 0;
                dangerValue = Data.Rank * 2;
                break;

            case NPC_Type_Normal.Adventurer0:
            case NPC_Type_Normal.Adventurer1:
            case NPC_Type_Normal.Elf:
            case NPC_Type_Normal.Wizard:
                popValue = (Data.Rank / 2);
                dangerValue = (Data.Rank / 2);
                break;

            case NPC_Type_Normal.Goblin:
                popValue = 0;
                dangerValue = Data.Rank;
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
            Main.Instance.CurrentDay.AddKill(-1);
        }
        else
        {
            UI_EventBox.AddEventText($"◈{Name_Color} {UserData.Instance.LocaleText("Event_Defeat")}");
        }

        int popValue = 0;
        int dangerValue = Data.Rank;

        switch (NPCType)
        {
            case NPC_Type_Normal.Herbalist0:
            case NPC_Type_Normal.Herbalist1:
            case NPC_Type_Normal.Herbalist2:
            case NPC_Type_Normal.Miner0:
            case NPC_Type_Normal.Miner1:
                dangerValue = Data.Rank * 3;
                break;

            case NPC_Type_Normal.Adventurer0:
            case NPC_Type_Normal.Adventurer1:
            case NPC_Type_Normal.Elf:
            case NPC_Type_Normal.Wizard:
                break;

            case NPC_Type_Normal.Goblin:
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


        int goldValue = isCaptive ? KillGold * 2 : KillGold;
        if (goldValue > 0)
        {
            Main.Instance.CurrentDay.AddGold(goldValue, Main.DayResult.EventType.Monster);
            Main.Instance.ShowDM(goldValue, Main.TextType.gold, transform);
        }
    }

}
