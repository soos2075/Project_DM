using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fairy : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Fairy");
    }


}
