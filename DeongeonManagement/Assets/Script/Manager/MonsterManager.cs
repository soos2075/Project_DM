using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager
{
    public void Init()
    {
        Monsters = new Monster[7];

        Init_Data();
    }

    #region ���� �ν���Ʈ
    public Monster[] Monsters { get; set; }
    public int TrainingCount { get; set; } = 2;


    public bool MaximumCheck()
    {
        foreach (var monster in Monsters)
        {
            if (monster == null)
            {
                return true;
            }
        }

        Debug.Log("Monster Maximum");
        return false;
    }
    public void AddMonster(Monster mon)
    {
        for (int i = 0; i < Monsters.Length; i++)
        {
            if (Monsters[i] == null)
            {
                Monsters[i] = mon;
                mon.MonsterID = i;
                break;
            }
        }
    }

    public void ReleaseMonster(int monsterID)
    {
        if (Monsters[monsterID] != null)
        {
            var mon = Monsters[monsterID];
            Managers.Placement.PlacementClear(mon);
            Managers.Resource.Destroy(mon.gameObject);
            Monsters[monsterID] = null;
        }
    }


    #endregion

    #region ��ȯ ������
    public List<MonsterData> MonsterDatas { get; } = new List<MonsterData>();

    public MonsterData GetMonsterData(string monsterName)
    {
        foreach (var item in MonsterDatas)
        {
            if (item.Name == monsterName)
            {
                return item;
            }
        }
        Debug.Log($"{monsterName} �����͸� ã�� ����");
        return null;
    }

    void Init_Data()
    {
        {
            MonsterData monster = new MonsterData();
            monster.Name_KR = "������";
            monster.Name = "Slime";
            monster.prefabPath = "Monster/Slime";
            monster.detail = "�ǿܷ� �Ϳ��� �������Դϴ�. ���ϱ� ������ �׷��ٰ� �������� �����ǰǰ���? �и� ������ Ű��� ��������ſ���!";
            monster.sprite = Managers.Sprite.GetSprite("Monster/Slime");

            monster.ManaCost = 30;

            monster.LV = 1;
            monster.MAXLV = 25;
            monster.HP = 15;
            monster.ATK = 2;
            monster.DEF = 4;
            monster.AGI = 2;
            monster.LUK = 3;

            monster.HP_chance= 1.2f;
            monster.ATK_chance = 0.3f;
            monster.DEF_chance = 0.25f;
            monster.AGI_chance = 0.15f;
            monster.LUK_chance = 0.1f;

            MonsterDatas.Add(monster);
        }

        {
            MonsterData monster = new MonsterData();
            monster.Name_KR = "���̷���";
            monster.Name = "Skeleton";
            monster.prefabPath = "Monster/Skeleton";
            monster.detail = "������ ������ ���̷����Դϴ�. ưư�ϰ� ���ؼ� ���谡���� ���� �����̿���. ������ ���� �Ѱ谡 ��������?";
            monster.sprite = Managers.Sprite.GetSprite("Monster/Skeleton");

            monster.ManaCost = 100;

            monster.LV = 3;
            monster.MAXLV = 20;
            monster.HP = 30;
            monster.ATK = 5;
            monster.DEF = 4;
            monster.AGI = 4;
            monster.LUK = 4;

            monster.HP_chance = 1.5f;
            monster.ATK_chance = 0.4f;
            monster.DEF_chance = 0.15f;
            monster.AGI_chance = 0.1f;
            monster.LUK_chance = 0.08f;

            MonsterDatas.Add(monster);
        }

    }



    #endregion
}

public class MonsterData
{
    public int ManaCost { get; set; }
    public string Name_KR { get; set; }

    public string Name { get; set; }



    public int LV { get; set; }
    public int MAXLV { get; set; }

    public int HP { get; set; }

    public int ATK { get; set; }
    public int DEF { get; set; }
    public int AGI { get; set; }
    public int LUK { get; set; }


    public float HP_chance { get; set; }
    public float ATK_chance { get; set; }
    public float DEF_chance { get; set; }
    public float AGI_chance { get; set; }
    public float LUK_chance { get; set; }


    public string prefabPath;

    public string detail;
    public Sprite sprite;

}


