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
    Define.PlacementType PlacementType { get; set; }
    string Name_KR { get; }
    string Detail_KR { get; }

    PlacementInfo PlacementInfo { get; set; }

    GameObject GetObject();



}

public interface IDialogue
{
    public SO_DialogueData Data { get; set; }

    public void AddOption(GameObject button);
}