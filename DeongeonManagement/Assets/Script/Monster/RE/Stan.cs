using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stan : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Stan");

        Trait_Original();
    }



    //void Trait_Original()
    //{
    //    AddTrait(new Trait.LuckyPunch_V2());
    //    AddTrait(new Trait.Overwhelm());
    //}


}
