using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager
{
    public void Init()
    {
        CurrentBuff = new BuffList();
    }






    public BuffList CurrentBuff { get; set; }




    public BuffList Save_Buff()
    {
        return CurrentBuff.Clone();
    }

    public void Load_Buff(BuffList data)
    {
        if (data != null)
        {
            CurrentBuff = data.Clone();
        }
    }
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
    public BuffList Clone()
    {
        var buff = new BuffList();
        buff.Orb_green = Orb_green;
        buff.Orb_blue = Orb_blue;
        buff.Orb_yellow = Orb_yellow;
        buff.Orb_red = Orb_red;

        return buff;
    }
}
