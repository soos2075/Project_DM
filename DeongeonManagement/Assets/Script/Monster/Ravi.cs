using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ravi : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Ravi");
        Trait_Original();
    }



    void Trait_Original()
    {
        AddTrait(new Trait.AbsoluteShield());
        AddTrait(new Trait.SurvivabilityS());
    }


}
