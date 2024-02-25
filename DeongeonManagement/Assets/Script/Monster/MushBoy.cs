using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushBoy : Monster
{
    public override MonsterData Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetMonsterData("MushBoy");
    }




    public override void MaxLevelQuest()
    {
        base.MaxLevelQuest();
        //EventManager.Instance.GuildQuestAdd.Add(1100);
    }

}
