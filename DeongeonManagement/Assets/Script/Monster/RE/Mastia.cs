using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mastia : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Mastia");

        Trait_Original();
    }



    //void Trait_Original()
    //{
    //    AddTrait(new Trait.Harmony());
    //    AddTrait(new Trait.Blindness());
    //}


}
