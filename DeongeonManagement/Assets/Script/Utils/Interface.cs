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

//? Basement에 배치가 가능한 오브젝트
public interface IPlacementable
{
    PlacementInfo PlacementInfo { get; set; }
    PlacementType PlacementType { get; set; }
    PlacementState PlacementState { get; set; }

    string Name_KR { get; }
    string Detail_KR { get; }

    //? 게임오브젝트로 가져오기
    GameObject GetObject();

    //? 마우스 클릭했을 때 뜨게 할 이벤트
    void MouseClickEvent();
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
    public SO_DialogueData Data { get; set; }
    public float TextDelay { get; set; }

    public void AddOption(GameObject button);
}