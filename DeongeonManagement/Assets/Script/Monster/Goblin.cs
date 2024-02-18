using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Monster
{
    public override MonsterData Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetMonsterData("Skeleton");
    }

}
