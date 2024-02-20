using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster
{

    public override MonsterData Data { get; set; }

    public override void MonsterInit()
    {
        Data = GameManager.Monster.GetMonsterData("Slime");
        //Mode = MoveType.Move_Hunting;
    }

    public override void TurnStart()
    {
        MoveSelf();
    }

    void MoveSelf()
    {
        Cor_Moving = StartCoroutine(MoveCor());
    }


    public override void MaxLevelQuest()
    {
        base.MaxLevelQuest();
        EventManager.Instance.GuildQuestAdd.Add(1100);
    }

}
