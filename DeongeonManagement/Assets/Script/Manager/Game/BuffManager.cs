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


    #region SO_Buff (상태이상)

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
    //? 이 항목은 따로 저장하지 않음. 그냥 모든 시설과 기타 등등으로 인해 새로시작, 로드 등으로 각자장소에서 해줘야함(중복방지)
    //! Add = 정수 더하기 (1 = 1)
    //! Up = 퍼센트 곱하기 (1 = 1%)

    //? 마나
    public int ManaAdd_Portal { get; set; }    //? 포탈 이용시 마나 보너스 - 정수
    public int ManaAdd_Facility { get; set; }  //? 모든 시설 이용시 마나 보너스 - 정수

    public int ManaAdd_Herb { get; set; }      //? 모든 약초 채집시 마나 보너스 - 정수
    public int ManaAdd_Mineral { get; set; }   //? 모든 광물 채굴시 마나 보너스 - 정수

    public int ManaUp_Herb          //? 모든 약초 채집시 마나 보너스 %
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
    public int ManaUp_Mineral       //? 모든 광물 채굴시 마나 보너스 %
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


    public int ManaAdd_Battle { get; set; }     //? 전투 시 마나 증가 - 정수
    public int ManaUp_Battle            //? 전투 시 마나 증가 - %
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

    public int ManaUp_Final { get; set; }      //? 최종 마나 계산시 보너스 - 1당 1퍼센트

    //? 골드
    public int GoldUp_Final { get; set; }      //? 최종 골드 계산시 보너스 - 1당 1퍼센트


    //? 방문
    public int VisitAdd_Adv { get; set; }    //? 전투직군 방문 증가
    public int VisitAdd_Herb { get; set; }   //? 허브류 방문 증가
    public int VisitAdd_Mineral { get; set; }//? 광물류 방문 증가

    public int VisitUp_Adv          //? 전투직군 방문 증가 %
    {
        get
        {
            int result = 0;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Treasure_3).isActive) result += 30;
            return result;
        }
    }
    public int VisitUp_Herb         //? 허브류 방문 증가 %
    {
        get
        {
            int result = 0;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Herb_3).isActive) result += 30;
            return result;
        }
    }   
    public int VisitUp_Mineral      //? 광물류 방문 증가 %
    {
        get
        {
            int result = 0;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Mineral_3).isActive) result += 30;
            return result;
        }
    }


    public int VisitAdd_All         //? 전체 방문자 수 +a (NPC 매니저의 최종 Value값에 더해주면 댐)
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
    }    //? 전체 방문자 수 +%
    


    //? 경험치
    public int ExpAdd_Battle { get; set; }       //? 전투시 경험치 보너스


    //? 스탯
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

    //public int HpUp_Unit            //? 유닛 체력 보너스 %
    //{
    //    get
    //    {
    //        int result = 0;
    //        result += (GameManager.Artifact.GetArtifact(ArtifactLabel.Cross).Count * 10);
    //        return result;
    //    }
    //}
    //public int StatUp_Unit          //? 유닛 올스탯 보너스 %
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



    //? 효과 증가
    public int EffectUp_Trap            //? 함정 효과 증가 %
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
    public int EffectUp_Treasure        //? 보물 효과 증가 %
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
    // bool값으로 해도 되지만, 혹시 업그레이드나 그런 상황을 대비해서 그냥 int값으로. 0이 false, 1이상부터가 true

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

        //? 데이터 상 최대치 제한
        if (GameManager.Buff.GetData(_label).MaximumCount < currentStatus[_label])
        {
            currentStatus[_label] = GameManager.Buff.GetData(_label).MaximumCount;
        }

        //? 0보다 작을 순 없음
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

    //? 고정 add 값
    public int Get_Fixed_HP()
    {
        int value = 0;

        value += currentStatus[BattleStatusLabel.Heroism] > 0 ? 150 : 0;

        return value;
    }
    //? 고정 add 값
    public int Get_Fixed_AllStat()
    {
        int value = 0;

        value += currentStatus[BattleStatusLabel.Heroism] > 0 ? 15 : 0;

        return value;
    }


    public float Get_HP_Stauts()
    {
        int value = 0;

        //? 오리지널
        value += currentStatus[BattleStatusLabel.Master] * 10;

        //? 플러스
        value += currentStatus[BattleStatusLabel.Robust] * 10;

        //? 마이너스
        value -= currentStatus[BattleStatusLabel.Wound] * 10;

        float ratio = value / 100f;
        return Mathf.Clamp(ratio, -1, ratio);
    }


    int Get_AllStat()
    {
        int value = 0;

        //? 오리지널
        value += currentStatus[BattleStatusLabel.Spiritual] * 7;
        value += currentStatus[BattleStatusLabel.Master] * 5;

        //? 올스탯 버프
        value += currentStatus[BattleStatusLabel.Empower] * 5;
        value += currentStatus[BattleStatusLabel.Vigor] * 10;
        value += currentStatus[BattleStatusLabel.Blessing] * 20;

        //? 올스탯 디버프
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
    //? 오리지널 상태이상 (특수 혹은 특성)
    Spiritual = 100,
    Heroism = 101,

    Master = 200,



    //? 버프
    Empower = 1000,
    Vigor = 1001,
    Blessing = 1002,

    Sharp = 1100,
    Guard = 1200,
    Haste = 1300,
    Chance = 1400,

    Robust = 1500,


    //? 디버프
    Weaken = 2000,
    Fatigue = 2001,
    Decay = 2002,

    Wither = 2100,
    Corrode = 2200,
    Slow = 2300,
    Jinx = 2400,

    Wound = 2500,
}
