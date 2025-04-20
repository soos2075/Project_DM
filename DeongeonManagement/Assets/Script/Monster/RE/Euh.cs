using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Euh : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Euh");

        Trait_Original();
    }



    //void Trait_Original()
    //{
    //    AddTrait(new Trait.Powerful_V2());
    //    AddTrait(new Trait.DivineForce());
    //}



}
