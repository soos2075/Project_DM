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
        SetStatus("Goblin", 1, 15, 5, 3);
    }


}
