using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager
{
    public void Init()
    {
        guild = Util.FindChild<Transform>(Main.Instance.gameObject, "Guild");
        dungeonEntrance = Util.FindChild<Transform>(Main.Instance.gameObject, "Dungeon");
    }


    public Transform guild;
    public Transform dungeonEntrance;

    int Max_NPC { get; set; }
    public List<NPC> Current_NPCList;


    public void TurnStart()
    {
        Calculation_MaxNPC();
        Debug.Log($"�ִ� �� ���� = {Max_NPC}");

        Calculation_Rank();
        Debug.Log($"�ִ� �� ��ũ = {rankList.Count}");

        Current_NPCList = new List<NPC>();

        for (int i = 0; i < Max_NPC; i++)
        {
            InstantiateNPC((NPCType)WeightRandomPicker());
        }


        if (Current_NPCList.Count <= 5)
        {
            for (int i = 0; i < 5; i++)
            {
                float ranValue = Random.Range(1f, 5f);
                Main.Instance.StartCoroutine(ActiveNPC(i, ranValue));
            }
        }
        else if (Current_NPCList.Count <= 15)
        {
            for (int i = 0; i < Current_NPCList.Count; i++)
            {
                float ranValue = Random.Range(6f, 10f);
                Main.Instance.StartCoroutine(ActiveNPC(i, ranValue));
            }
        }
        else
        {
            for (int i = 0; i < Current_NPCList.Count; i++)
            {
                float ranValue = Random.Range(11f, 15f);
                Main.Instance.StartCoroutine(ActiveNPC(i, ranValue));
            }
        }

    }

    IEnumerator ActiveNPC(int index, float delay)
    {
        yield return new WaitForSeconds(delay);
        ActiveNPC(index);
    }


    enum NPCType
    {
        Herbalist = 0,
        Miner = 1,
        Adventurer = 2,
    }


    void InstantiateNPC(NPCType rank)
    {
        switch (rank)
        {
            case NPCType.Herbalist:
                var herb = Managers.Placement.CreatePlacementObject($"NPC/Herbalist", null, Define.PlacementType.NPC);
                Current_NPCList.Add(herb as NPC);
                break;

            case NPCType.Miner:
                var miner = Managers.Placement.CreatePlacementObject($"NPC/Miner", null, Define.PlacementType.NPC);
                Current_NPCList.Add(miner as NPC);
                break;

            case NPCType.Adventurer:
                var adv = Managers.Placement.CreatePlacementObject($"NPC/Adventurer", null, Define.PlacementType.NPC);
                Current_NPCList.Add(adv as NPC);
                break;
        }
    }



    public void ActiveNPC(int index)
    {
        Current_NPCList[index].Departure(guild.position, dungeonEntrance.position);
    }

    public void InactiveNPC(NPC npc)
    {
        Managers.Placement.PlacementClear(npc);
        Current_NPCList.Remove(npc);

        Managers.Resource.Destroy(npc.gameObject);

        TurnOverCheck();
    }

    void TurnOverCheck()
    {
        if (Current_NPCList.Count == 0)
        {
            Debug.Log("��� npc�� ��Ȱ��ȭ��");
            Main.Instance.DayChange();
        }
    }




    [System.Obsolete]
    public void TestCreate(string name)
    {
        var adv = Managers.Placement.CreatePlacementObject($"NPC/{name}", null, Define.PlacementType.NPC);
        var npc = adv as NPC;
        npc.Departure(guild.position, dungeonEntrance.position);
    }








    #region Calculation
    void Calculation_MaxNPC()
    {
        int ofFame = Main.Instance.FameOfDungeon / 10;

        Max_NPC = Mathf.Clamp(Main.Instance.Turn + ofFame, 5, Main.Instance.Turn + ofFame);
    }



    int[] rankWeightedList = new int[] { 20, 20, 60, 100, 200, 300 };
    List<int> rankList;


    void Calculation_Rank() //? ���赵�� ���� ���� �� �ִ� ������ �޶���. �ƹ��� ���赵�� ���Ƶ� ���� ���� ������� ��. �ٸ� ���� �پ���.
    {
        rankList = new List<int>();

        int _danger = Mathf.Clamp(Main.Instance.DangerOfDungeon, 40, Main.Instance.DangerOfDungeon);

        for (int i = 0; i < rankWeightedList.Length; i++)
        {
            _danger -= rankWeightedList[i];
            rankList.Add(rankWeightedList[i]);
            if (_danger <= 0)
            {
                rankList[i] += _danger;
                break;
            }
        }
    } 

    int WeightRandomPicker() //? 0~1�� �������� ��ü ����ġ�� ���� ������. �׸��� �װ����� ���ϸ� ��. ��ȯ���� ��ũ �ܰ�
    {
        int weightMax = 0;
        foreach (var item in rankList)
        {
            weightMax += item;
        }

        float randomValue = Random.value * weightMax;
        int currentWeight = 0;


        for (int i = 0; i < rankList.Count; i++)
        {
            currentWeight += rankList[i];
            if (currentWeight > randomValue)
            {
                return i;
            }
        }
        Debug.Log("�߸��� ��ũ");
        return 0;
    }
    #endregion
}
