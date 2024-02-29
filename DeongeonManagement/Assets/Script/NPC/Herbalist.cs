using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herbalist : NPC
{
    [field: SerializeField]
    public override List<BasementTile> PriorityList { get; set; }
    protected override Define.TileType[] AvoidTileType { get { return AvoidTile(); } }

    Define.TileType[] AvoidTile()
    {
        if (ActionPoint <= 0 || Mana <= 0)
        {
            return new Define.TileType[] { Define.TileType.NPC, Define.TileType.Facility };
        }
        else
        {
            return new Define.TileType[] { Define.TileType.NPC, Define.TileType.Monster };
        }
    }

    protected override void SetRandomClothes()
    {
        var collection = characterBuilder.SpriteCollection;
        characterBuilder.Armor = "ArcherTunic";
        //characterBuilder.Weapon = "WoodenStuff";
        characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_Herbalist);
        characterBuilder.Helmet = "ArcherHood";
        characterBuilder.Back = "SmallBackpack";
        //{ // mask = 8
        //    int ran = Random.Range(-3, collection.Layers[8].Textures.Count);
        //    if (ran > 0)
        //    {
        //        characterBuilder.Mask = collection.Layers[8].Textures[ran].name;
        //    }
        //}
        if (Rank > 3)
        {
            characterBuilder.Helmet = "FireWizardHood#FFFFFF/0:0:0";
        }
        if (Rank > 7)
        {
            characterBuilder.Helmet = "ClericHood#FFFFFF/0:0:0";
        }

        characterBuilder.Rebuild();
    }


    protected override void SetPriorityList()
    {
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
        Main.Instance.CurrentDay.AddPop(-3);
        Main.Instance.ShowDM(-3, Main.TextType.pop, transform, 1);
    }
    protected override void NPC_Return_Satisfaction()
    {
        Main.Instance.CurrentDay.AddPop(Data.Rank + 1);
        Main.Instance.ShowDM(Data.Rank + 1, Main.TextType.pop, transform, 1);
    }
    protected override void NPC_Runaway()
    {
        Main.Instance.CurrentDay.AddDanger(Data.Rank + 3);

        Main.Instance.ShowDM(Data.Rank + 3, Main.TextType.danger, transform, 1);
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
        Main.Instance.ShowDM(5+ Data.Rank, Main.TextType.danger, transform);
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
