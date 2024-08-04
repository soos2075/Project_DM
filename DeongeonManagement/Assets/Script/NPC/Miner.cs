using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : NPC
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
            return new Define.TileType[] { Define.TileType.NPC, Define.TileType.Monster };
        }
    }


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


        if (Rank > 3)
        {
            characterBuilder.Helmet = "MinerHelment#FFFFFF/-45:0:0";
        }
        if (Rank > 7)
        {
            characterBuilder.Helmet = "MinerHelment#FFFFFF/180:0:0";
        }

        characterBuilder.Rebuild();
    }

    public override void Departure(Vector3 startPoint, Vector3 endPoint)
    {
        base.Departure(startPoint, endPoint);

        if (UserData.Instance.FileConfig.firstAppear_Miner == false)
        {
            UserData.Instance.FileConfig.firstAppear_Miner = true;
            StartCoroutine(EventCor(DialogueName.Miner0_Appear));
        }
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
        Add_Wells();
    }


    protected override void NPC_Return_Empty()
    {
        Main.Instance.CurrentDay.AddPop(-Data.Rank);
        Main.Instance.ShowDM(-Data.Rank, Main.TextType.pop, transform, 1);
    }
    //protected override void NPC_Return_Satisfaction()
    //{
    //    Main.Instance.CurrentDay.AddPop(Data.Rank);
    //    Main.Instance.ShowDM(Data.Rank, Main.TextType.pop, transform, 1);
    //}
    protected override void NPC_Return_NonSatisfaction()
    {
        int value = Mathf.RoundToInt(Data.Rank * 0.6f);

        Main.Instance.CurrentDay.AddPop(value);
        Main.Instance.ShowDM(value, Main.TextType.pop, transform, 1);
    }
    protected override void NPC_Runaway()
    {
        Main.Instance.CurrentDay.AddDanger(Data.Rank * 2);

        Main.Instance.ShowDM(Data.Rank * 2, Main.TextType.danger, transform, 1);
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

        Main.Instance.CurrentDay.AddGold(KillGold * 2, Main.DayResult.EventType.Monster);
        Main.Instance.ShowDM(KillGold * 2, Main.TextType.gold, transform);

        Main.Instance.CurrentDay.AddDanger(5 + Data.Rank);
        Main.Instance.ShowDM(5 + Data.Rank, Main.TextType.danger, transform);
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

        Main.Instance.CurrentDay.AddGold(KillGold, Main.DayResult.EventType.Monster);
        Main.Instance.ShowDM(KillGold, Main.TextType.gold, transform);

        Main.Instance.CurrentDay.AddDanger(5 + Data.Rank);
        Main.Instance.ShowDM(5 + Data.Rank, Main.TextType.danger, transform);
    }
}
