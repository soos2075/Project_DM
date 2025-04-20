using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager
{
    public void Init()
    {
        CurrentBuff = new BuffList();
    }




    #region Buff List
    //? �� �׸��� ���� �������� ����. �׳� ��� �ü��� ��Ÿ ������� ���� ���ν���, �ε� ������ ������ҿ��� �������(�ߺ�����)
    //! Add = ���� ���ϱ� (1 = 1)
    //! Up = �ۼ�Ʈ ���ϱ� (1 = 1%)

    //? ����
    public int ManaAdd_Portal { get; set; }    //? ��Ż �̿�� ���� ���ʽ� - ����
    public int ManaAdd_Facility { get; set; }  //? ��� �ü� �̿�� ���� ���ʽ� - ����

    public int ManaAdd_Herb { get; set; }      //? ��� ���� ä���� ���� ���ʽ� - ����
    public int ManaAdd_Mineral { get; set; }   //? ��� ���� ä���� ���� ���ʽ� - ����

    public int ManaUp_Herb          //? ��� ���� ä���� ���� ���ʽ� %
    { 
        get 
        {  
            int result = 0;
            if (CurrentBuff.Orb_green >= 1) result += 10;
            if (CurrentBuff.Orb_green >= 2) result += 10;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Herb_1).isActive) result += 10;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Herb_2).isActive) result += 20;
            return result;
        } 
    }   
    public int ManaUp_Mineral       //? ��� ���� ä���� ���� ���ʽ� %
    { 
        get
        {
            int result = 0;
            if (CurrentBuff.Orb_yellow >= 1) result += 10;
            if (CurrentBuff.Orb_yellow >= 2) result += 10;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Mineral_1).isActive) result += 10;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Mineral_2).isActive) result += 20;
            return result;
        }
    }   


    public int ManaAdd_Battle { get; set; }     //? ���� �� ���� ���� - ����
    public int ManaUp_Battle            //? ���� �� ���� ���� - %
    {
        get
        {
            int result = 0;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.StrongMonster_1).isActive) result += 10;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.StrongMonster_2).isActive) result += 10;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.StrongMonster_3).isActive) result += 10;
            return result;
        }
    }      

    public int ManaUp_Final { get; set; }      //? ���� ���� ���� ���ʽ� - 1�� 1�ۼ�Ʈ

    //? ���
    public int GoldUp_Final { get; set; }      //? ���� ��� ���� ���ʽ� - 1�� 1�ۼ�Ʈ


    //? �湮
    public int VisitAdd_Adv { get; set; }    //? �������� �湮 ����
    public int VisitAdd_Herb { get; set; }   //? ���� �湮 ����
    public int VisitAdd_Mineral { get; set; }//? ������ �湮 ����

    public int VisitUp_Adv          //? �������� �湮 ���� %
    {
        get
        {
            int result = 0;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Treasure_3).isActive) result += 30;
            return result;
        }
    }
    public int VisitUp_Herb         //? ���� �湮 ���� %
    {
        get
        {
            int result = 0;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Herb_3).isActive) result += 30;
            return result;
        }
    }   
    public int VisitUp_Mineral      //? ������ �湮 ���� %
    {
        get
        {
            int result = 0;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Mineral_3).isActive) result += 30;
            return result;
        }
    }


    public int VisitAdd_All         //? ��ü �湮�� �� +a (NPC �Ŵ����� ���� Value���� �����ָ� ��)
    {
        get
        {
            int result = 0;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Amenity_1).isActive) result += 5;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Amenity_2).isActive) result += 20;
            return result;
        }
    }
    public int VisitUp_All
    {
        get
        {
            int result = 0;
            return result;
        }
    }    //? ��ü �湮�� �� +%
    


    //? ����ġ
    public int ExpAdd_Battle { get; set; }       //? ������ ����ġ ���ʽ�


    //? ����
    public int HpAdd_Unit
    {
        get
        {
            int result = 0;
            return result;
        }
    }
    public int StatAdd_Unit
    {
        get
        {
            int result = 0;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.ManyMonster_1).isActive) result += 3;
            if (GameManager.Monster.Check_ExistUnit<Mastia>()) result += 2;

            return result;
        }
    }

    public int HpUp_Unit            //? ���� ü�� ���ʽ� %
    {
        get
        {
            int result = 0;
            result += (GameManager.Artifact.GetArtifact(ArtifactLabel.Cross).Count * 10);
            return result;
        }
    }
    public int StatUp_Unit          //? ���� �ý��� ���ʽ� %
    {
        get
        {
            int result = 0;
            result += (GameManager.Artifact.GetArtifact(ArtifactLabel.Dice).Count * 5);
            return result;
        }
    }


    public int HpAdd_Player
    {
        get
        {
            int result = 0;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Marauder_1).isActive) result += 20;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Marauder_2).isActive) result += 30;
            return result;
        }
    }
    public int StatAdd_Player
    {
        get
        {
            int result = 0;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Secret_1).isActive) result += 2;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Secret_2).isActive) result += 3;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Secret_3).isActive) result += 5;
            return result;
        }
    }


    public int HpAdd_NPC
    {
        get
        {
            int result = 0;
            return result;
        }
    }
    public int StatAdd_NPC
    {
        get
        {
            int result = 0;
            return result;
        }
    }



    //? ȿ�� ����
    public int EffectUp_Trap            //? ���� ȿ�� ���� %
    {
        get
        {
            int result = 0;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Trap_1).isActive) result += 10;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Trap_2).isActive) result += 20;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Trap_3).isActive) result += 30;
            return result;
        }
    }
    public int EffectUp_Treasure        //? ���� ȿ�� ���� %
    {
        get
        {
            int result = 0;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Treasure_1).isActive) result += 10;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Treasure_2).isActive) result += 20;
            if (GameManager.Title.Get_InstanceTitle(TitleGroup.Treasure_3).isActive) result += 30;
            return result;
        }
    }



    #endregion




    #region Orb Buff
    public BuffList CurrentBuff { get; set; }

    public BuffList Save_Buff()
    {
        return CurrentBuff.DeepCopy();
    }

    public void Load_Buff(BuffList data)
    {
        if (data != null)
        {
            CurrentBuff = data.DeepCopy();
        }
    }
    #endregion
}

public class BuffList
{
    // bool������ �ص� ������, Ȥ�� ���׷��̵峪 �׷� ��Ȳ�� ����ؼ� �׳� int������. 0�� false, 1�̻���Ͱ� true

    public int Orb_green;
    public int Orb_blue;
    public int Orb_yellow;
    public int Orb_red;


    public BuffList()
    {

    }
    public BuffList DeepCopy()
    {
        var buff = new BuffList();
        buff.Orb_green = Orb_green;
        buff.Orb_blue = Orb_blue;
        buff.Orb_yellow = Orb_yellow;
        buff.Orb_red = Orb_red;

        return buff;
    }
}
