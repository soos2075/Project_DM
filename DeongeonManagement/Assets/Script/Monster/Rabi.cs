using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabi : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Rabi");
        Trait_Original();
    }



    void Trait_Original()
    {
        AddTrait(new Trait.LifeDrain());
    }


}
