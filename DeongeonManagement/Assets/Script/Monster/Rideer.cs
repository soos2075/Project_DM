using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rideer : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Rideer");
        Trait_Original();
    }



}
