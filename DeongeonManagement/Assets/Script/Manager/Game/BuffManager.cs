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
    // bool값으로 해도 되지만, 혹시 업그레이드나 그런 상황을 대비해서 그냥 int값으로. 0이 false, 1이상부터가 true

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
