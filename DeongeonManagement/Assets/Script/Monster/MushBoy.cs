using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushBoy : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("MushBoy");
        Trait_Original();
    }


    void Trait_Original()
    {
        AddTrait(new Trait.Predation());

        //AddTrait(new Trait.Predation());
        //AddTrait(new Trait.Overwhelm());
        //AddTrait(new Trait.Friend());
        //AddTrait(new Trait.Vitality());
    }




}
