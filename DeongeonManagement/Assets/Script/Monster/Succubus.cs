using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Succubus : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Succubus");
        Trait_Original();

        //UnitDialogueEvent.AddEvent(102001);
    }



    void Trait_Original()
    {
        AddTrait(new Trait.LifeDrain());
    }


}
