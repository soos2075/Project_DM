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
            delay: 0.3f);

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

        //var list0 = GetFloorObjectsAll(Define.TileType.Facility);
        var list1 = GetFloorObjectsAll(Define.TileType.Monster);
        var list2 = GetFacilityPick(Facility.FacilityType.RestZone);
        var list3 = GetFacilityPick(Facility.FacilityType.Treasure);
        var list4 = GetFacilityPick(Facility.FacilityType.Artifact);
        var list5 = GetFacilityPick(Facility.FacilityType.Event);

        AddList(list1);
        AddList(list2);
        AddList(list3);
        AddList(list4);
        AddList(list5);

        PickToProbability(GetPriorityPick(typeof(Entrance_Egg)), PlacementInfo.Place_Floor.FloorIndex * 0.1f);


    }




    void PickToProbability(List<BasementTile> pick, float probability, AddPos pos = AddPos.Back)
    {
        float randomValue = Random.value;
        if (randomValue < probability)
        {
            AddList(pick);
        }
    }
    void RemoveToProbability(List<BasementTile> pick, float probability)
    {
        float randomValue = Random.value;
        if (randomValue < probability)
        {
            foreach (var item in pick)
            {
                PriorityRemove(item);
            }
        }
    }
}
