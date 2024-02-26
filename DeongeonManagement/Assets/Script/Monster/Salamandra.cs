using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salamandra : Monster
{
    public override MonsterData Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetMonsterData("Salamandra");
    }


}
