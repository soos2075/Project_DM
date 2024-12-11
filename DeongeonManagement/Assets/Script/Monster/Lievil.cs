using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lievil : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Lievil");
        Trait_Original();
    }



    void Trait_Original()
    {
        AddTrait(new Trait.Reaper());
        AddTrait(new Trait.SurvivabilityS());
    }


}
