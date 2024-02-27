using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : NPC
{
    [field: SerializeField]
    public override List<BasementTile> PriorityList { get; set; }
    protected override Define.TileType[] AvoidTileType { get; set; } = new Define.TileType[] { Define.TileType.NPC, Define.TileType.Monster };

    protected override void SetRandomClothes()
    {
        var collection = characterBuilder.SpriteCollection;
        characterBuilder.Armor = "MinerArmour";
        //characterBuilder.Weapon = "Pickaxe";
        characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_Miner);
        characterBuilder.Helmet = "MinerHelment";
        characterBuilder.Back = "LargeBackpack";

        //characterBuilder.Armor += $"#FFFFFF/{Random.Range(-100, 100)}:{Random.Range(-70, 70)}:{Random.Range(-50, 50)}";
        //characterBuilder.Helmet += $"#FFFFFF/{Random.Range(-50, 50)}:0:0";

        characterBuilder.Rebuild();
    }
    void Init_AvoidType()
    {
        AvoidTileType = new Define.TileType[] { Define.TileType.NPC, Define.TileType.Monster };
    }

    protected override void SetPriorityList()
    {
        if (PriorityList != null) PriorityList.Clear();


        var list1 = GetPriorityPick(typeof(Mineral));
        AddList(list1);


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

        Main.Instance.ShowDM(-1, Main.TextType.pop, transform, 1);
        Main.Instance.ShowDM(-1, Main.TextType.danger, transform, 1);
    }
    protected override void NPC_Return_Satisfaction()
    {
        Main.Instance.CurrentDay.AddPop(2 + Data.Rank);
        Main.Instance.ShowDM(2 + Data.Rank, Main.TextType.pop, transform, 1);
    }
    protected override void NPC_Runaway()
    {
        Main.Instance.CurrentDay.AddDanger(2 + Data.Rank);

        Main.Instance.ShowDM(2 + Data.Rank, Main.TextType.danger, transform, 1);
    }
    protected override void NPC_Die()
    {
        GameManager.NPC.InactiveNPC(this);

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
        Main.Instance.CurrentDay.AddGold(KillGold);

        Main.Instance.CurrentDay.AddDanger(5 + Data.Rank);
        Main.Instance.ShowDM(5 + Data.Rank, Main.TextType.danger, transform);
    }
    protected override void NPC_Captive()
    {
        UI_EventBox.AddEventText($"◈{Name_KR} (이)가 포로로 잡힘");
        Main.Instance.CurrentDay.AddPrisoner(1);
        Main.Instance.CurrentDay.AddGold(KillGold * 2);

        Main.Instance.CurrentDay.AddDanger(5 + Data.Rank);
        Main.Instance.ShowDM(5 + Data.Rank, Main.TextType.danger, transform);
    }
}
