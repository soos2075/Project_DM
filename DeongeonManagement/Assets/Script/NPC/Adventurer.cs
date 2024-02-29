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


    //? ����Ʈ ������� 
    //? 1. GetFloorObjectsAll �޼��� = TileType�� �ش��ϴ� ��� ������Ʈ ����Ʈ�� ��ȯ
    //? 2. GetPriorityPick �޼��� = typeof(target) �� �ش��ϴ� ��� ������Ʈ ����Ʈ�� ��ȯ / �ڵ�����(������ Ÿ�ٵ��� ������ ���� �׻� �����ؾ��� �ʿ�� �����Ƿ�)
    //? 3. AddList �޼��� = List<BasementTile>�� �޾Ƽ� PriorityList�� ������. �տ� ������ �ڿ� ������ enum���� ���ð���
    //? 4. �� �ϳ��� Ÿ�Ը� ã�´ٰ� �ص� �� AddList�� ����� PriorityList�� null�� ���� ����.
    //? 5. ����Ʈ�� ���ϴ°� �ʹ� �������� �׳� 1������ ��ã�Ƴ��� 2������ ������ �ֵ� �޾ƿµ� PriorityRemove�� 2���� �־ �����ϸ� ��. 

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

        UI_EventBox.AddEventText($"��{Name_KR} (��)�� ������");
        Main.Instance.CurrentDay.AddKill(1);
        Main.Instance.CurrentDay.AddGold(KillGold);

        Main.Instance.CurrentDay.AddDanger(Data.Rank);
        Main.Instance.ShowDM(Data.Rank, Main.TextType.danger, transform);
    }
    protected override void NPC_Captive()
    {
        UI_EventBox.AddEventText($"��{Name_KR} (��)�� ���η� ����");
        Main.Instance.CurrentDay.AddPrisoner(1);
        Main.Instance.CurrentDay.AddGold(KillGold * 2);

        Main.Instance.CurrentDay.AddDanger(Data.Rank);
        Main.Instance.ShowDM(Data.Rank, Main.TextType.danger, transform);
    }

}
