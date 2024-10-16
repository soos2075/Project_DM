using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interface 
{



}





public interface IWorldSpaceUI
{
    public void SetCanvasWorldSpace();
}


public interface I_Projectile
{
    public class AttackEffect
    {
        public AttackType AttackAnim;
        public string effectName;
        public string projectile_Category;
        public string projectile_Label;

        public AttackEffect(AttackType type, string effect = "")
        {

        }

        public void SetProjectile(AttackType type, string category, string label)
        {
            AttackAnim = type;
            projectile_Category = category;
            projectile_Label = label;
        }
    }
    public enum AttackType
    {
        Normal,
        Bow,
        Magic,
    }
    public AttackEffect AttackOption { get; set; }
}
public interface I_BattleStat
{
    int B_HP { get; }
    int B_HP_Max { get; }
    int B_ATK { get; }
    int B_DEF { get; }
    int B_AGI { get; }
    int B_LUK { get; }

    int HP { get; set; }
}

public interface I_TraitSystem
{
    public bool TraitCheck(TraitGroup searchTrait);
    public void DoSomething(TraitGroup searchTrait);
    public int GetSomething<T>(TraitGroup searchTrait, T current);

}



//? Collection�� SO ������
public interface I_SO_Collection
{
    //T GetData<T>() where T : ScriptableObject;
}


//? Basement�� ��ġ�� ������ ������Ʈ
public interface IPlacementable
{
    PlacementInfo PlacementInfo { get; set; }
    PlacementType PlacementType { get; set; }
    PlacementState PlacementState { get; set; }

    string Name_Color { get; }
    string Detail_KR { get; }

    //? ���ӿ�����Ʈ�� ��������
    GameObject GetObject();

    //? ���콺 Ŭ������ �� �߰� �� �̺�Ʈ
    void MouseClickEvent();
    void MouseMoveEvent();
    void MouseExitEvent();

    //? ��� ������, ���� �̺�Ʈ
    void MouseDownEvent();
    void MouseUpEvent();
}


//? NPC�� ������ �� ���� Ÿ��. ��ã�⿡�� ������������ �ɷ�����. ��� Ÿ���� �̼Ӽ��� ���� Ÿ���̶�� �ణ �ٸ� ����� ��ã�⸦ �ϸ� ��
public interface IWall
{

}


public enum PlacementState
{
    Standby,
    Busy,
    Inactive,
}
public enum PlacementType
{
    Facility,
    Monster,
    NPC,
}

public interface IDialogue
{
    public DialogueData Data { get; set; }
    public float TextDelay { get; set; }

    public void AddOption(GameObject button);

    public void CloseOptionBox();
}