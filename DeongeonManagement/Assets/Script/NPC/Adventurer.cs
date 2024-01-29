using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adventurer : NPC
{
    public override List<BasementTile> PriorityList { get; set; }
    protected override Define.TileType[] AvoidTileType { get; set; }

    void Init_AvoidType()
    {
        AvoidTileType = new Define.TileType[] { Define.TileType.NPC };
    }

    protected override void Initialize_Status()
    {
        int index = Random.Range(0, 100);
        Name_Index = index;

        SetStatus("���谡",
            lv: 1,
            atk: 8,
            def: 6,
            agi: 5,
            luk: 5,
            hp: 20,
            ap: 10,
            mp: 100,
            speed: 2f,
            delay: 0.8f);

        Init_AvoidType();
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

        var list0 = GetFloorObjectsAll(Define.TileType.Facility);
        var list1 = GetFloorObjectsAll(Define.TileType.Monster);
        //var list2 = GetPriorityPick(typeof(Entrance_Egg));
        //var list3 = GetPriorityPick(typeof(Treasure_Base));
        //var list4 = GetPriorityPick(typeof(Campfire));
        //var list5 = GetPriorityPick(typeof(SpecialEgg));

        AddList(list1);
        AddList(list0);
        //AddList(list2);
        //AddList(list3, AddPos.Front);
        //AddList(list4);
        //AddList(list5, AddPos.Back);


        var remove1 = GetPriorityPick(typeof(Mineral_Low));
        var remove2 = GetPriorityPick(typeof(Mineral_High));
        var remove3 = GetPriorityPick(typeof(Herb_Low));
        var remove4 = GetPriorityPick(typeof(Herb_High));

        foreach (var item in remove1)
        {
            PriorityRemove(item);
        }
        foreach (var item in remove2)
        {
            PriorityRemove(item);
        }
        foreach (var item in remove3)
        {
            PriorityRemove(item);
        }
        foreach (var item in remove4)
        {
            PriorityRemove(item);
        }
    }
}
