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


public interface I_AttackEffect
{
    public class AttackEffect
    {
        public AttackType attack_Type;
        public string effectName;
    }

    public AttackEffect AttackOption { get; set; }
    public GameObject GetGameObject { get; }
}


//public interface I_Projectile
//{
//    public class AttackEffect
//    {
//        public AttackType AttackAnim;
//        public string effectName;
//        public string projectile_Category;
//        public string projectile_Label;

//        public AttackEffect(AttackType type, string effect = "")
//        {

//        }

//        public void SetProjectile(AttackType type, string category, string label)
//        {
//            AttackAnim = type;
//            projectile_Category = category;
//            projectile_Label = label;
//        }
//    }
//    public enum AttackType
//    {
//        Normal,
//        Bow,
//        Magic,
//    }
//    public AttackEffect AttackOption { get; set; }
//}
public interface I_BattleStat
{
    int B_HP { get; }
    int B_HP_Max { get; }
    int B_ATK { get; }
    int B_DEF { get; }
    int B_AGI { get; }
    int B_LUK { get; }

    int HP { get; set; }

    int HP_Damaged { get; set; }
    int HP_normal { get; }
    int HP_Status { get; }

    BattleStatus CurrentBattleStatus { get; set; }


    //? 기본 수치 (에디터 및 계산용)
    int Base_HP_MAX { get; }
    int Base_ATK { get; }
    int Base_DEF { get; }
    int Base_AGI { get; }
    int Base_LUK { get; }

    //? 상태이상이 없을 떄 기본수치
    int ATK_normal { get; }
    int DEF_normal { get; }
    int AGI_normal { get; }
    int LUK_normal { get; }

    //? 현재 상태이상을 적용시킨 수치
    int ATK_Status { get; }
    int DEF_Status { get; }
    int AGI_Status { get; }
    int LUK_Status { get; }
}



public interface I_TraitSystem
{
    public bool TraitCheck(TraitGroup searchTrait);
    public void DoSomething(TraitGroup searchTrait);
    public int GetSomething<T>(TraitGroup searchTrait, T current);

}



//? Collection용 SO 데이터
public interface I_SO_Collection
{
    //T GetData<T>() where T : ScriptableObject;
}


//? Basement에 배치가 가능한 오브젝트
public interface IPlacementable
{
    PlacementInfo PlacementInfo { get; set; }
    PlacementType PlacementType { get; set; }
    PlacementState PlacementState { get; set; }

    string Name_Color { get; }
    string Detail_KR { get; }

    //? 게임오브젝트로 가져오기
    GameObject GetObject();

    //? 마우스 클릭했을 때 뜨게 할 이벤트
    void MouseClickEvent();
    void MouseMoveEvent();
    void MouseExitEvent();

    //? 길게 누르기, 떼기 이벤트
    void MouseDownEvent();
    void MouseUpEvent();
}


//? NPC가 지나갈 수 없는 타입. 길찾기에서 무조건적으로 걸러야함. 대신 타겟이 이속성을 가진 타일이라면 약간 다른 방식의 길찾기를 하면 댐
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