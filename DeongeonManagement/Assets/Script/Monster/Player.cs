using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Monster
{
    protected override int HP_Bonus => GameManager.Buff.HpAdd_Player;
    protected override int AllStat_Bonus => GameManager.Buff.StatAdd_Player;

    public override SO_Monster Data { get; set; }
    public override void MonsterInit()
    {
        //PlacementType = PlacementType.Monster;
        //Data = GameManager.Monster.GetData("Player_lv1");
        //Initialize_Status();
        //LV = 1;
    }

    public void Player_FirstInit()
    {
        PlacementType = PlacementType.Monster;
        Data = GameManager.Monster.GetData("Player_lv1");
        Initialize_Status();
        LV = 1;

        //Hero_Buff();
    }


    protected override void BattleStatue_TurnStart()
    {
        if (GameManager.Artifact.GetArtifact(ArtifactLabel.ProofOfHero).Count > 0)
        {
            CurrentBattleStatus.AddValue(BattleStatusLabel.Heroism, 1);
        }

        if (GameManager.Artifact.GetArtifact(ArtifactLabel.Cross).Count > 0)
        {
            CurrentBattleStatus.AddValue(BattleStatusLabel.Master, GameManager.Artifact.GetArtifact(ArtifactLabel.Cross).Count);
        }
    }


    public void Player_DataLoad(Save_MonsterData _LoadData)
    {
        LV = _LoadData.LV;
        HP = _LoadData.HP;
        HP_MAX = _LoadData.HP_MAX;

        ATK = _LoadData.ATK;
        DEF = _LoadData.DEF;
        AGI = _LoadData.AGI;
        LUK = _LoadData.LUK;

        if (_LoadData.traitCounter != null)
        {
            traitCounter = _LoadData.traitCounter.DeepCopy();
            traitCounter.monster = this;
        }
        LoadTraitList(_LoadData.currentTraitList);
    }

    public void Player_RankUp(int dungeonLV)
    {
        while (LV < dungeonLV) //? 던전 레벨만큼 스탯업
        {
            LV++;

            HP_MAX += 50;
            HP += 50;

            ATK += 5;
            DEF += 2;
            AGI += 2;
            LUK += 2;
        }

    }
    //public void Level_Stat(int dungeonLV)
    //{
    //    switch (dungeonLV)
    //    {
    //        case 1:
    //            Data = GameManager.Monster.GetData("Player_lv1");
    //            break;

    //        case 2:
    //            Data = GameManager.Monster.GetData("Player_lv2");
    //            break;

    //        case 3:
    //            Data = GameManager.Monster.GetData("Player_lv3");
    //            break;
    //    }
    //    Initialize_Status();
    //    LV = dungeonLV;
    //}






    public override void TurnStart()
    {
        //HP_Damaged = 0;
        BattleStatue_TurnStart();
    }

    public override void MoveSelf()
    {
        //Cor_Moving = StartCoroutine(MoveCor());
    }


    public override void MouseClickEvent()
    {
        
    }
    public override void MouseDownEvent()
    {
        
    }
    public override void MouseExitEvent()
    {
        
    }
    public override void MouseMoveEvent()
    {
        
    }
    public override void MouseUpEvent()
    {
        
    }




}
