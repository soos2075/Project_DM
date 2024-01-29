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

        SetStatus("모험가",
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


    //? 리스트 설정방법 
    //? 1. GetFloorObjectsAll 메서드 = TileType에 해당하는 모든 오브젝트 리스트를 반환
    //? 2. GetPriorityPick 메서드 = typeof(target) 에 해당하는 모든 오브젝트 리스트를 반환 / 자동셔플(동일한 타겟들의 순서가 굳이 항상 일정해야할 필요는 없으므로)
    //? 3. AddList 메서드 = List<BasementTile>를 받아서 PriorityList에 더해줌. 앞에 더할지 뒤에 더할지 enum으로 선택가능
    //? 4. 단 하나의 타입만 찾는다고 해도 꼭 AddList를 해줘야 PriorityList가 null이 되지 않음.
    //? 5. 리스트가 더하는게 너무 많아지면 그냥 1번으로 다찾아놓고 2번으로 삭제할 애들 받아온뒤 PriorityRemove에 2번을 넣어서 삭제하면 됨. 

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
