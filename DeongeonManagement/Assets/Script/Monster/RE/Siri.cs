using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Siri : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Siri");

        Trait_Original();
    }



    //void Trait_Original()
    //{
    //    AddTrait(new Trait.Vitality_V2());
    //    AddTrait(new Trait.Friend_V2());
    //}


}
