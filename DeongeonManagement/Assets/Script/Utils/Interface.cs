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
}