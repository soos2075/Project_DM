using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : NPC
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

        {
            characterBuilder.Armor = "BlueWizardTunic";
            characterBuilder.Helmet = "BlueWizzardHat";
        }

        { // Weapon = 14
            characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_Magic);
        }

        characterBuilder.Rebuild();
    }


    public override void Departure(Vector3 startPoint, Vector3 endPoint)
    {
        base.Departure(startPoint, endPoint);

        if (UserData.Instance.FileConfig.firstAppear_Wizard == false)
        {
            UserData.Instance.FileConfig.firstAppear_Wizard = true;
            StartCoroutine(EventCor(DialogueName.Wizard_Appear));
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
            var list = GetPriorityPick(typeof(Mineral));
            AddList(list, AddPos.Front);
        }

        {
            var add_egg = GetPriorityPick(typeof(SpecialEgg));
            AddList(add_egg);
        }

        PickToProbability(GetPriorityPick(typeof(Entrance_Egg)), (PlacementInfo.Place_Floor.FloorIndex + Rank) * 0.05f);
        Add_Wells();
    }


    protected override void NPC_Return_Empty()
    {
        int value = (Data.Rank / 2);

        Main.Instance.CurrentDay.AddPop(-value);
        Main.Instance.ShowDM(-value, Main.TextType.pop, transform, 1);

        Main.Instance.CurrentDay.AddDanger(-value);
        Main.Instance.ShowDM(-value, Main.TextType.danger, transform, 1);
    }

    //protected override void NPC_Return_Satisfaction()
    //{
    //    Main.Instance.CurrentDay.AddPop(Data.Rank);
    //    Main.Instance.ShowDM(Data.Rank, Main.TextType.pop, transform, 1);

    //    Main.Instance.CurrentDay.AddDanger(Data.Rank / 2);
    //    Main.Instance.ShowDM(Data.Rank / 2, Main.TextType.danger, transform, 1);
    //}

    protected override void NPC_Runaway()
    {
        Main.Instance.CurrentDay.AddDanger(Data.Rank);
        Main.Instance.ShowDM(Data.Rank, Main.TextType.danger, transform, 1);
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

        Main.Instance.CurrentDay.AddGold(KillGold, Main.DayResult.EventType.Monster);
        Main.Instance.ShowDM(KillGold, Main.TextType.gold, transform);

        Main.Instance.CurrentDay.AddDanger(Data.Rank);
        Main.Instance.ShowDM(Data.Rank, Main.TextType.danger, transform);
    }

}
