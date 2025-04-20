using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Karen : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Karen");

        Trait_Original();
    }



    //void Trait_Original()
    //{
    //    AddTrait(new Trait.Burn());
    //    AddTrait(new Trait.EagleEye());
    //}



}
