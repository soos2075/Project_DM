using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elf : NPC
{
    [field: SerializeField]
    public override List<BasementTile> PriorityList { get; set; }
    protected override Define.TileType[] AvoidTileType { get { return AvoidTile(); } set { } }
    Define.TileType[] AvoidTile()
    {
        if (ActionPoint <= 0 || Mana <= 0)
        {
            return new Define.TileType[] { Define.TileType.NPC, Define.TileType.Facility, Define.TileType.Monster };
        }
        else
        {
            return new Define.TileType[] { Define.TileType.NPC };
        }
    }

    protected override void SetRandomClothes()
    {
        var collection = characterBuilder.SpriteCollection;
        { // hair = 9
            int ran = Random.Range(0, collection.Layers[9].Textures.Count);
            characterBuilder.Hair = collection.Layers[9].Textures[ran].name;
            string hexColor = Define.HairColors[Random.Range(0, 24)];
            characterBuilder.Hair += hexColor;
        }

        { // armor = 4
            characterBuilder.Armor = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Armor_Elf);
        }

        { // Weapon = 14
            characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_Bow);
        }

        characterBuilder.Rebuild();
    }


    public override void Departure(Vector3 startPoint, Vector3 endPoint)
    {
        base.Departure(startPoint, endPoint);

        if (UserData.Instance.FileConfig.firstAppear_Elf == false)
        {
            UserData.Instance.FileConfig.firstAppear_Elf = true;
            StartCoroutine(EventCor(DialogueName.Elf_Appear));
        }
    }


    protected override void SetPriorityList()
    {
        if (PriorityList != null) PriorityList.Clear();

        {
            var list1 = GetFloorObjectsAll(Define.TileType.Monster);
            AddList(list1);
        }

        {
            var list = GetPriorityPick(typeof(Herb));
            AddList(list, AddPos.Front);
        }

        {
            var add_egg = GetPriorityPick(typeof(SpecialEgg));
            AddList(add_egg);
        }

        PickToProbability(GetPriorityPick(typeof(Entrance_Egg)), (PlacementInfo.Place_Floor.FloorIndex + Rank) * 0.05f);
    }


    protected override void NPC_Return_Empty()
    {
        Main.Instance.CurrentDay.AddPop(-1);
        Main.Instance.ShowDM(-1, Main.TextType.pop, transform, 1);

        //Main.Instance.CurrentDay.AddDanger(-1);
        //Main.Instance.ShowDM(-1, Main.TextType.danger, transform, 1);
    }
    protected override void NPC_Return_Satisfaction()
    {
        Main.Instance.CurrentDay.AddPop(Data.Rank);
        Main.Instance.ShowDM(Data.Rank, Main.TextType.pop, transform, 1);

        Main.Instance.CurrentDay.AddDanger(Data.Rank - 1);
        Main.Instance.ShowDM(Data.Rank - 1, Main.TextType.danger, transform, 1);
    }
    protected override void NPC_Runaway()
    {
        Main.Instance.CurrentDay.AddDanger(Data.Rank + 1);
        Main.Instance.ShowDM(Data.Rank + 1, Main.TextType.danger, transform, 1);
    }
    protected override void NPC_Die()
    {
        Kill();
        GameManager.NPC.InactiveNPC(this);
    }
    protected override void NPC_Captive()
    {
        UI_EventBox.AddEventText($"¢Â{Name_Color} {UserData.Instance.LocaleText("Event_Prison")}");
        Main.Instance.CurrentDay.AddPrisoner(1);

        Main.Instance.CurrentDay.AddGold(KillGold * 2);
        Main.Instance.ShowDM(KillGold * 2, Main.TextType.gold, transform);

        Main.Instance.CurrentDay.AddDanger(Data.Rank);
        Main.Instance.ShowDM(Data.Rank, Main.TextType.danger, transform);
    }
    void Kill()
    {
        var prison = GameManager.Technical.Prison;
        if (prison != null)
        {
            var ran = Random.Range(0, 10);
            if (ran > 6)
            {
                NPC_Captive();
                return;
            }
        }

        UI_EventBox.AddEventText($"¢Â{Name_Color} {UserData.Instance.LocaleText("Event_Defeat")}");
        Main.Instance.CurrentDay.AddGold(KillGold);
        Main.Instance.ShowDM(KillGold, Main.TextType.gold, transform);

        Main.Instance.CurrentDay.AddDanger(Data.Rank);
        Main.Instance.ShowDM(Data.Rank, Main.TextType.danger, transform);
    }


}
