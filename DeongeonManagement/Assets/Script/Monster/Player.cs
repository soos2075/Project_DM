using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Monster
{
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
    }

    int HeroBuff(StatEnum stat)
    {
        if (GameManager.Artifact.GetArtifact(ArtifactLabel.ProofOfHero).Count > 0)
        {
            switch (stat)
            {
                case StatEnum.HP:
                    return 150;
                case StatEnum.ATK:
                    return 20;
                case StatEnum.DEF:
                    return 10;
                case StatEnum.AGI:
                    return 10;
                case StatEnum.LUK:
                    return 10;
            }
        }
        return 0;
    }

    public override int B_HP => base.B_HP + HeroBuff(StatEnum.HP);
    public override int B_HP_Max => base.B_HP_Max + HeroBuff(StatEnum.HP);
    public override int B_ATK => base.B_ATK + HeroBuff(StatEnum.ATK);
    public override int B_DEF => base.B_DEF + HeroBuff(StatEnum.DEF);
    public override int B_AGI => base.B_AGI + HeroBuff(StatEnum.AGI);
    public override int B_LUK => base.B_LUK + HeroBuff(StatEnum.LUK);


    public void Player_DataLoad(Save_MonsterData _LoadData)
    {
        LV = _LoadData.LV;
        HP = _LoadData.HP;
        HP_Max = _LoadData.HP_MAX;

        ATK = _LoadData.ATK;
        DEF = _LoadData.DEF;
        AGI = _LoadData.AGI;
        LUK = _LoadData.LUK;
    }

    public void Player_RankUp(int dungeonLV)
    {
        while (LV < dungeonLV) //? 던전 레벨만큼 스탯업
        {
            LV++;

            HP_Max += 50;
            HP += 50;

            ATK += 5;
            DEF += 2;
            AGI += 2;
            LUK += 2;
        }

    }
    public void Level_Stat(int dungeonLV)
    {
        switch (dungeonLV)
        {
            case 1:
                Data = GameManager.Monster.GetData("Player_lv1");
                break;

            case 2:
                Data = GameManager.Monster.GetData("Player_lv2");
                break;

            case 3:
                Data = GameManager.Monster.GetData("Player_lv3");
                break;
        }
        Initialize_Status();
        LV = dungeonLV;
    }




    public override void TurnStart()
    {
        //MoveSelf();
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
