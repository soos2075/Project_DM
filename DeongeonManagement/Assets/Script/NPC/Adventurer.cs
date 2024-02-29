using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adventurer : NPC
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
            return new Define.TileType[] { Define.TileType.NPC, Define.TileType.Facility };
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
            characterBuilder.Armor = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Armor_Warrior);
        }

        { // Weapon = 14
            //int ran = Random.Range(0, collection.Layers[14].Textures.Count);
            //characterBuilder.Weapon = collection.Layers[14].Textures[ran].name;
            //characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_OneHandSword);
            characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_BeginnerSword);
        }

        {
            characterBuilder.Shield = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_BeginnerShield);
        }

        if (Rank > 7)
        {
            characterBuilder.Weapon = GameManager.Pixel.GetRandomItem(GameManager.Pixel.Weapon_TwoHandSword);
            characterBuilder.Shield = "";
        }

        characterBuilder.Rebuild();
    }


    //? 리스트 설정방법 
    //? 1. GetFloorObjectsAll 메서드 = TileType에 해당하는 모든 오브젝트 리스트를 반환
    //? 2. GetPriorityPick 메서드 = typeof(target) 에 해당하는 모든 오브젝트 리스트를 반환 / 자동셔플(동일한 타겟들의 순서가 굳이 항상 일정해야할 필요는 없으므로)
    //? 3. AddList 메서드 = List<BasementTile>를 받아서 PriorityList에 더해줌. 앞에 더할지 뒤에 더할지 enum으로 선택가능
    //? 4. 단 하나의 타입만 찾는다고 해도 꼭 AddList를 해줘야 PriorityList가 null이 되지 않음.
    //? 5. 리스트가 더하는게 너무 많아지면 그냥 1번으로 다찾아놓고 2번으로 삭제할 애들 받아온뒤 PriorityRemove에 2번을 넣어서 삭제하면 됨. 

    protected override void SetPriorityList()
    {
        if (PriorityList != null) PriorityList.Clear();

        {
            var list1 = GetFloorObjectsAll(Define.TileType.Monster);
            AddList(list1);
        }

        {
            var list = GetPriorityPick(typeof(Treasure));
            AddList(list);
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

        Main.Instance.CurrentDay.AddDanger(Data.Rank);
        Main.Instance.ShowDM(Data.Rank, Main.TextType.danger, transform);
    }
    protected override void NPC_Captive()
    {
        UI_EventBox.AddEventText($"◈{Name_KR} (이)가 포로로 잡힘");
        Main.Instance.CurrentDay.AddPrisoner(1);
        Main.Instance.CurrentDay.AddGold(KillGold * 2);

        Main.Instance.CurrentDay.AddDanger(Data.Rank);
        Main.Instance.ShowDM(Data.Rank, Main.TextType.danger, transform);
    }

}
