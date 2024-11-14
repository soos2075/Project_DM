using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bushmon : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Bushmon");
        Trait_Original();
    }


    void Trait_Original()
    {
        AddTrait(new Trait.IronSkin());
    }


}
