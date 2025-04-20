using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Romys : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Romys");

        Trait_Original();
    }



    //void Trait_Original()
    //{
    //    AddTrait(new Trait.Overwhelm_V2());
    //    AddTrait(new Trait.Predation_V2());
    //}


}
