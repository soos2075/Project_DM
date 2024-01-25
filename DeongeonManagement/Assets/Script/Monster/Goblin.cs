using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Monster
{
    public override MonsterType Type { get; set; }


    protected override void MonsterInit()
    {
        Type = MonsterType.Normal_Fixed;
    }

    protected override void Initialize_Status()
    {
        SetStatus(
            name: "½ºÄÌ·¹Åæ",
            lv: 1,
            hp: 25,
            atk: 5,
            def: 3,
            agi: 3,
            luk: 3);
    }


}
