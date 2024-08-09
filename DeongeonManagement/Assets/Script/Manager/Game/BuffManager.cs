using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager
{
    public void Init()
    {
        CurrentBuff = new BuffList();
    }



    #region Normal Buff
    //? 이 항목은 따로 저장하지 않음. 그냥 모든 시설과 기타 등등으로 인해 새로시작, 로드 등으로 각자장소에서 해줘야함(중복방지)
    public int PortalBonus { get; set; }    //? 포탈 이용시 마나 보너스 - 정수
    public int FacilityBonus { get; set; }  //? 모든 시설 이용시 마나 보너스 - 정수

    public int HerbBonus { get; set; }      //? 모든 약초 채집시 마나 보너스 - 정수
    public int MineralBonus { get; set; }   //? 모든 광물 채굴시 마나 보너스 - 정수


    public int BattleBonus { get; set; }    //? 모든 전투시 마나 보너스 - 정수


    public int ManaBonus { get; set; }      //? 최종 마나 계산시 보너스 - 1당 1퍼센트
    public int GoldBonus { get; set; }      //? 최종 골드 계산시 보너스 - 1당 1퍼센트
    public int APBonus { get; set; }        //? 턴 시작시 행동력 보너스 - 정수


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
