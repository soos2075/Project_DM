using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heroine : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Heroine");
        Trait_Original();
    }

    void Trait_Original()
    {
        AddTrait(new Trait.Nimble());
    }

}