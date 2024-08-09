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
    //? �� �׸��� ���� �������� ����. �׳� ��� �ü��� ��Ÿ ������� ���� ���ν���, �ε� ������ ������ҿ��� �������(�ߺ�����)
    public int PortalBonus { get; set; }    //? ��Ż �̿�� ���� ���ʽ� - ����
    public int FacilityBonus { get; set; }  //? ��� �ü� �̿�� ���� ���ʽ� - ����

    public int HerbBonus { get; set; }      //? ��� ���� ä���� ���� ���ʽ� - ����
    public int MineralBonus { get; set; }   //? ��� ���� ä���� ���� ���ʽ� - ����


    public int BattleBonus { get; set; }    //? ��� ������ ���� ���ʽ� - ����


    public int ManaBonus { get; set; }      //? ���� ���� ���� ���ʽ� - 1�� 1�ۼ�Ʈ
    public int GoldBonus { get; set; }      //? ���� ��� ���� ���ʽ� - 1�� 1�ۼ�Ʈ
    public int APBonus { get; set; }        //? �� ���۽� �ൿ�� ���ʽ� - ����


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
