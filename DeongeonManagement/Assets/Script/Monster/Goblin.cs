using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Monster
{
    public override MonsterType Type { get; set; }
    public override MonsterData Data { get; set; }

    protected override void MonsterInit()
    {
        Type = MonsterType.Normal_Fixed;
        Data = Managers.Monster.GetMonsterData("Skeleton");
    }

    protected override void Initialize_Status()
    {
        SetStatus(
            name: "���̷���",
            lv: 1,
            hp: 25,
            atk: 5,
            def: 3,
            agi: 3,
            luk: 3);
    }


}
