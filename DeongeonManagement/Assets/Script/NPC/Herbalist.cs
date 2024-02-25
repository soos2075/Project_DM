using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herbalist : NPC
{
    [field: SerializeField]
    public override List<BasementTile> PriorityList { get; set; }
    protected override Define.TileType[] AvoidTileType { get; set; }

    protected override void SetRandomClothes()
    {
        var collection = characterBuilder.SpriteCollection;
        characterBuilder.Armor = "ArcherTunic";
        characterBuilder.Weapon = "WoodenStuff";
        characterBuilder.Helmet = "ArcherHood";
        characterBuilder.Back = "SmallBackpack";

        characterBuilder.Helmet += $"#FFFFFF/{Random.Range(-150, 150)}:{Random.Range(-80, 80)}:{Random.Range(-70, 70)}";
        { // mask = 8
            int ran = Random.Range(-3, collection.Layers[8].Textures.Count);
            if (ran > 0)
            {
                characterBuilder.Mask = collection.Layers[8].Textures[ran].name;
            }
        }

        characterBuilder.Rebuild();
    }

    void Init_AvoidType()
    {
        AvoidTileType = new Define.TileType[] { Define.TileType.NPC, Define.TileType.Monster };
    }

    protected override void SetPriorityList()
    {
        Init_AvoidType();

        if (PriorityList != null) PriorityList.Clear();

        var list_0 = GetPriorityPick(typeof(Herb));
        AddList(list_0);

        {
            var add_egg = GetPriorityPick(typeof(SpecialEgg));
            AddList(add_egg);
        }

        PickToProbability(GetPriorityPick(typeof(Entrance_Egg)), (PlacementInfo.Place_Floor.FloorIndex + Rank) * 0.04f);
    }


    protected override void NPC_Return_Empty()
    {
        Main.Instance.CurrentDay.AddPop(-1);
        Main.Instance.CurrentDay.AddDanger(-1);
    }
    protected override void NPC_Return_Satisfaction()
    {
        Main.Instance.CurrentDay.AddPop(2 + Data.Rank);
    }
    protected override void NPC_Runaway()
    {
        Main.Instance.CurrentDay.AddDanger(2 + Data.Rank);
    }
    protected override void NPC_Die()
    {
        var prison = GameManager.Technical.Prison;
        if (prison != null)
        {
            var ran = Random.Range(0, 10);
            if (ran > 3)
            {
                NPC_Captive();
                return;
            }
        }

        UI_EventBox.AddEventText($"◈{Name_KR} (이)가 쓰러짐");
        Main.Instance.CurrentDay.AddKill(1);

        Main.Instance.CurrentDay.AddDanger(1 + Data.Rank);
        //Main.Instance.CurrentDay.AddGold(Data.Rank * Random.Range(20, 30));
        Main.Instance.CurrentDay.AddGold(KillGold);
    }
    protected override void NPC_Captive()
    {
        UI_EventBox.AddEventText($"◈{Name_KR} (이)가 포로로 잡힘");
        Main.Instance.CurrentDay.AddPrisoner(1);

        Main.Instance.CurrentDay.AddDanger(1 + Data.Rank);
        //Main.Instance.CurrentDay.AddGold(Data.Rank * Random.Range(40, 60));
        Main.Instance.CurrentDay.AddGold(KillGold * 2);
    }
}
