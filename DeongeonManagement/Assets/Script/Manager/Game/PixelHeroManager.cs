using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelHeroManager
{
    public void Init()
    {

    }


    public string GetRandomItem(string[] _list)
    {
        int ranValue = Random.Range(0, _list.Length);

        return _list[ranValue];
    }



    public readonly string[] Armor_Elf = new string[]
    { "ArcherTunic","ElfTunic"};



    public readonly string[] Armor_Warrior = new string[]
        { "BlueKnight","CaptainArmor","DeerCostume","DemigodArmour","GuardianTunic","HeavyKnightArmor","HornsKnight","IronKnight"
            ,"LegionaryArmor","MilitiamanArmor","Overalls"};



    public readonly string[] Weapon_BeginnerShield = new string[]
        { "SteelShield", "TowerShield", "WoodenBuckler", "IronBuckler"};


    public readonly string[] Weapon_BeginnerSword = new string[]
        { "AssaultSword","BastardSword","Broadsword","IronSword","MarderDagger","RustedShortSword","Saber","Sword","Tanto"};


    public readonly string[] Weapon_Herbalist = new string[]
        { "Branch","CurveBranch","Fork","GoldenSkepter","Rake","RustedShovel","SmallPitchfork","Stick","WoodenStuff","Sickle" };

    public readonly string[] Weapon_Miner = new string[]
        { "LargePickaxe","Pickaxe","RustedPickaxe","RustedShovel","Rake"};

    public readonly string[] Weapon_Bow = new string[]
        { "Bow","ShortBow","LongBow","CurvedBow","BattleBow" };

    public readonly string[] Weapon_Priest = new string[]
        { "BishopStaff","Mace","Morgenstern","RoundMace","Shestoper","PriestWand"};

    public readonly string[] Weapon_TwoHandSword = new string[]
        { "Butcher","Crusher","GiantBlade","GiantSword","Greatsword"};

    public readonly string[] Weapon_LongDistance = new string[]
        { "Bident","DeathScythe","GuardianHalberd","Halberd","LargeScythe","Pitchfork","Scythe"};

    public readonly string[] Weapon_OneHandSword = new string[]
        {"AssaultSword","BastardSword","Blade","Broadsword","Cutlass","Dagger","Epee","HunterKnife","IronSword","Katana","Knife","LongKatana","Longsword"
            ,"MarderDagger","RedKatana","RoyalLongsword","Saber","ShortDagger","Sword","Tanto","WideDagger"};

    public readonly string[] Weapon_AXE = new string[]
        {"Axe","BattleAxe","BattleHammer","BlacksmithHammer","Executioner","Greataxe","GreatHammer","Hammer","MasterGreataxe","Pan","RustedHammer"
            ,"SmallAxe","SmallPitchfork","SpikedClub","WoodcutterAxe"};

    public readonly string[] Weapon_Magic = new string[]
        {"AmurWand","ArchStaff","CrystalWand","ElderStaff","FireWand","FlameStaff","GoldenSkullWand","HermitStaff","MagicWand","MasterWand"
            ,"NatureWand","NecromancerStaff","SkullWand","StormStaff","WaterWand"};

}

