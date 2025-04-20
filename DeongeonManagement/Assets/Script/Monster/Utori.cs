using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utori : Monster
{
    public override SO_Monster Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetData("Utori");
        Trait_Original();

        UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.Utori_First);

        UnitDialogueEvent.AddEvent(UnitDialogueEventLabel.Utori_ArtifactBuy, true);
    }


}
