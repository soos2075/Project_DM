using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager
{
    public void Init()
    {
        Init_LocalData();
        CurrentBuff = new BuffList();
    }


    #region SO_Buff (�����̻�)

    SO_BattleStatus[] so_data;
    public void Init_LocalData()
    {
        so_data = Resources.LoadAll<SO_BattleStatus>("Data/BattleStatus");

        foreach (var item in so_data)
        {
            string[] datas = Managers.Data.GetTextData_BattleStatus(item.id);

            if (datas == null)
            {
                Debug.Log($"{item.id} : CSV Data Not Exist");
                continue;
            }

            item.labelName = datas[0];
            item.detail = datas[1];
        }
    }


    public SO_BattleStatus GetData(BattleStatusLabel _keyName)
    {
        foreach (var item in so_data)
        {
            if (item.label == _keyName)
            {
                return item;
            }
        }
        Debug.Log($"RE_{_keyName}: Data Not Exist");
        return null;
    }



    #endregion





    #region Buff List
    //? �� �׸��� ���� �������� ����. �׳� ��� �ü��� ��Ÿ ������� ���� ���ν���, �ε� ������ ������ҿ��� �������(�ߺ�����)
    //! Add = ���� ���ϱ� (1 = 1)
    //! Up = �ۼ�Ʈ ���ϱ� (1 = 1%)

    //? ����
    public int ManaAdd_Portal { get; set; }    //? ��Ż �̿�� ���� ���ʽ� - ����
    public int ManaAdd_Facility { get; set; }  //? ��� �ü� �̿�� ���� ���ʽ� - ����

    public int ManaAdd_Herb { get; set; }      //? ��� ���� ä���� ���� ���ʽ� - ����
    public int ManaAdd_Mineral { get; set; }   //? ��� ���� ä���� ���� ���ʽ� - ����

    public int ManaUp_Herb          //? ��� ���� ä���� ���� ���ʽ� %
    { 
        get 
        {  
            int result = 0;
            if (CurrentBuff.Orb_green >= 1) result += 10;
            if (CurrentBuff.Orb_green >= 2) result += 10;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Herb_1).isActive) result += 10;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Herb_2).isActive) result += 20;
            return result;
        } 
    }   
    public int ManaUp_Mineral       //? ��� ���� ä���� ���� ���ʽ� %
    { 
        get
        {
            int result = 0;
            if (CurrentBuff.Orb_yellow >= 1) result += 10;
            if (CurrentBuff.Orb_yellow >= 2) result += 10;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Mineral_1).isActive) result += 10;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Mineral_2).isActive) result += 20;
            return result;
        }
    }   


    public int ManaAdd_Battle { get; set; }     //? ���� �� ���� ���� - ����
    public int ManaUp_Battle            //? ���� �� ���� ���� - %
    {
        get
        {
            int result = 0;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.StrongMonster_1).isActive) result += 10;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.StrongMonster_2).isActive) result += 10;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.StrongMonster_3).isActive) result += 10;
            return result;
        }
    }      

    public int ManaUp_Final { get; set; }      //? ���� ���� ���� ���ʽ� - 1�� 1�ۼ�Ʈ

    //? ���
    public int GoldUp_Final { get; set; }      //? ���� ��� ���� ���ʽ� - 1�� 1�ۼ�Ʈ


    //? �湮
    public int VisitAdd_Adv { get; set; }    //? �������� �湮 ����
    public int VisitAdd_Herb { get; set; }   //? ���� �湮 ����
    public int VisitAdd_Mineral { get; set; }//? ������ �湮 ����

    public int VisitUp_Adv          //? �������� �湮 ���� %
    {
        get
        {
            int result = 0;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Treasure_3).isActive) result += 30;
            return result;
        }
    }
    public int VisitUp_Herb         //? ���� �湮 ���� %
    {
        get
        {
            int result = 0;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Herb_3).isActive) result += 30;
            return result;
        }
    }   
    public int VisitUp_Mineral      //? ������ �湮 ���� %
    {
        get
        {
            int result = 0;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Mineral_3).isActive) result += 30;
            return result;
        }
    }


    public int VisitAdd_All         //? ��ü �湮�� �� +a (NPC �Ŵ����� ���� Value���� �����ָ� ��)
    {
        get
        {
            int result = 0;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Amenity_1).isActive) result += 5;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Amenity_2).isActive) result += 20;
            return result;
        }
    }
    public int VisitUp_All
    {
        get
        {
            int result = 0;
            return result;
        }
    }    //? ��ü �湮�� �� +%
    


    //? ����ġ
    public int ExpAdd_Battle { get; set; }       //? ������ ����ġ ���ʽ�


    //? ����
    public int HpAdd_Unit
    {
        get
        {
            int result = 0;
            return result;
        }
    }
    public int StatAdd_Unit
    {
        get
        {
            int result = 0;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.ManyMonster_1).isActive) result += 3;
            if (GameManager.Monster.Check_ExistUnit<Mastia>()) result += 2;

            return result;
        }
    }

    //public int HpUp_Unit            //? ���� ü�� ���ʽ� %
    //{
    //    get
    //    {
    //        int result = 0;
    //        result += (GameManager.Artifact.GetArtifact(ArtifactLabel.Cross).Count * 10);
    //        return result;
    //    }
    //}
    //public int StatUp_Unit          //? ���� �ý��� ���ʽ� %
    //{
    //    get
    //    {
    //        int result = 0;
    //        result += (GameManager.Artifact.GetArtifact(ArtifactLabel.Dice).Count * 5);
    //        return result;
    //    }
    //}


    public int HpAdd_Player
    {
        get
        {
            int result = 0;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Marauder_1).isActive) result += 20;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Marauder_2).isActive) result += 30;
            return result;
        }
    }
    public int StatAdd_Player
    {
        get
        {
            int result = 0;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Secret_1).isActive) result += 2;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Secret_2).isActive) result += 3;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Secret_3).isActive) result += 5;
            return result;
        }
    }


    public int HpAdd_NPC
    {
        get
        {
            int result = 0;
            return result;
        }
    }
    public int StatAdd_NPC
    {
        get
        {
            int result = 0;
            return result;
        }
    }



    //? ȿ�� ����
    public int EffectUp_Trap            //? ���� ȿ�� ���� %
    {
        get
        {
            int result = 0;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Trap_1).isActive) result += 10;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Trap_2).isActive) result += 20;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Trap_3).isActive) result += 30;
            return result;
        }
    }
    public int EffectUp_Treasure        //? ���� ȿ�� ���� %
    {
        get
        {
            int result = 0;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Treasure_1).isActive) result += 10;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Treasure_2).isActive) result += 20;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Treasure_3).isActive) result += 30;
            return result;
        }
    }



    #endregion




    #region Orb Buff
    public BuffList CurrentBuff { get; set; }

    public BuffList Save_Buff()
    {
        return CurrentBuff.DeepCopy();
    }

    public void Load_Buff(BuffList data)
    {
        if (data != null)
        {
            CurrentBuff = data.DeepCopy();
        }
    }
    #endregion
}

public class BuffList
{
    // bool������ �ص� ������, Ȥ�� ���׷��̵峪 �׷� ��Ȳ�� ����ؼ� �׳� int������. 0�� false, 1�̻���Ͱ� true

    public int Orb_green;
    public int Orb_blue;
    public int Orb_yellow;
    public int Orb_red;


    public BuffList()
    {

    }
    public BuffList DeepCopy()
    {
        var buff = new BuffList();
        buff.Orb_green = Orb_green;
        buff.Orb_blue = Orb_blue;
        buff.Orb_yellow = Orb_yellow;
        buff.Orb_red = Orb_red;

        return buff;
    }
}

public class BattleStatus
{
    Dictionary<BattleStatusLabel, int> currentStatus;

    GameObject parentObject;
    int sort;

    Anim_BattleStatus anim_BattleStatus;

    public BattleStatus(GameObject parent)
    {
        parentObject = parent;
        InitValue();
    }


    public void InitValue()
    {
        currentStatus = new Dictionary<BattleStatusLabel, int>();
        foreach (BattleStatusLabel item in System.Enum.GetValues(typeof(BattleStatusLabel)))
        {
            currentStatus.Add(item, 0);
        }
        sort = parentObject.GetComponentInChildren<SpriteRenderer>().sortingOrder;

        anim_BattleStatus = parentObject.GetComponentInChildren<Anim_BattleStatus>();
        if (anim_BattleStatus == null)
        {
            anim_BattleStatus = Managers.Resource.Instantiate($"Effect/Anim_BattleStatus", parentObject.transform)
                .GetComponent<Anim_BattleStatus>();
        }
        anim_BattleStatus.Anim_Reset();
        anim_BattleStatus.GetComponent<UnityEngine.Rendering.SortingGroup>().sortingOrder = sort + 1;
    }
    public void AddValue(BattleStatusLabel _label, int _value)
    {
        currentStatus[_label] += _value;

        //? ������ �� �ִ�ġ ����
        if (GameManager.Buff.GetData(_label).MaximumCount < currentStatus[_label])
        {
            currentStatus[_label] = GameManager.Buff.GetData(_label).MaximumCount;
        }

        //? 0���� ���� �� ����
        if (currentStatus[_label] < 0)
        {
            currentStatus[_label] = 0;
        }

        anim_BattleStatus.Update_BattleStatus(_label, currentStatus);
    }

    public void Die()
    {
        anim_BattleStatus.Anim_Reset();
    }



    public Dictionary<BattleStatusLabel, int> GetCurrentBattleStatus()
    {
        return currentStatus;
    }

    public Dictionary<BattleStatusLabel, int> GetCurrentBattleStatus_Active()
    {
        Dictionary<BattleStatusLabel, int> newDict = new Dictionary<BattleStatusLabel, int>();

        foreach (var item in currentStatus)
        {
            if (item.Value > 0)
            {
                newDict.Add(item.Key, item.Value);
            }
        }
        return newDict;
    }

    //? ���� add ��
    public int Get_Fixed_HP()
    {
        int value = 0;

        value += currentStatus[BattleStatusLabel.Heroism] > 0 ? 150 : 0;

        return value;
    }
    //? ���� add ��
    public int Get_Fixed_AllStat()
    {
        int value = 0;

        value += currentStatus[BattleStatusLabel.Heroism] > 0 ? 15 : 0;

        return value;
    }


    public float Get_HP_Stauts()
    {
        int value = 0;

        //? ��������
        value += currentStatus[BattleStatusLabel.Master] * 10;

        //? �÷���
        value += currentStatus[BattleStatusLabel.Robust] * 10;

        //? ���̳ʽ�
        value -= currentStatus[BattleStatusLabel.Wound] * 10;

        float ratio = value / 100f;
        return Mathf.Clamp(ratio, -1, ratio);
    }


    int Get_AllStat()
    {
        int value = 0;

        //? ��������
        value += currentStatus[BattleStatusLabel.Spiritual] * 7;
        value += currentStatus[BattleStatusLabel.Master] * 5;

        //? �ý��� ����
        value += currentStatus[BattleStatusLabel.Empower] * 5;
        value += currentStatus[BattleStatusLabel.Vigor] * 10;
        value += currentStatus[BattleStatusLabel.Blessing] * 20;

        //? �ý��� �����
        value -= currentStatus[BattleStatusLabel.Weaken] * 5;
        value -= currentStatus[BattleStatusLabel.Fatigue] * 10;
        value -= currentStatus[BattleStatusLabel.Decay] * 20;

        return value;
    }

    public float Get_ATK_Status()
    {
        int value = Get_AllStat();

        value += currentStatus[BattleStatusLabel.Sharp] * 5;
        value -= currentStatus[BattleStatusLabel.Wither] * 5;

        float ratio = value / 100f;
        return Mathf.Clamp(ratio, -1, ratio);
    }
    public float Get_DEF_Status()
    {
        int value = Get_AllStat();

        value += currentStatus[BattleStatusLabel.Guard] * 5;
        value -= currentStatus[BattleStatusLabel.Corrode] * 5;

        float ratio = value / 100f;
        return Mathf.Clamp(ratio, -1, ratio);
    }
    public float Get_AGI_Status()
    {
        int value = Get_AllStat();

        value += currentStatus[BattleStatusLabel.Haste] * 5;
        value -= currentStatus[BattleStatusLabel.Slow] * 5;

        float ratio = value / 100f;
        return Mathf.Clamp(ratio, -1, ratio);
    }
    public float Get_LUK_Status()
    {
        int value = Get_AllStat();

        value += currentStatus[BattleStatusLabel.Chance] * 5;
        value -= currentStatus[BattleStatusLabel.Jinx] * 5;

        float ratio = value / 100f;
        return Mathf.Clamp(ratio, -1, ratio);
    }
}

public enum BattleStatusLabel
{
    //? �������� �����̻� (Ư�� Ȥ�� Ư��)
    Spiritual = 100,
    Heroism = 101,

    Master = 200,



    //? ����
    Empower = 1000,
    Vigor = 1001,
    Blessing = 1002,

    Sharp = 1100,
    Guard = 1200,
    Haste = 1300,
    Chance = 1400,

    Robust = 1500,


    //? �����
    Weaken = 2000,
    Fatigue = 2001,
    Decay = 2002,

    Wither = 2100,
    Corrode = 2200,
    Slow = 2300,
    Jinx = 2400,

    Wound = 2500,
}
