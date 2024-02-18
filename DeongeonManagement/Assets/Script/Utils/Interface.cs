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
    Define.PlacementType PlacementType { get; set; }
    PlacementInfo PlacementInfo { get; set; }

    string Name_KR { get; }
    string Detail_KR { get; }

    //? ���ӿ�����Ʈ�� ��������
    GameObject GetObject();

    //? ���콺 Ŭ������ �� �߰� �� �̺�Ʈ
    void MouseClickEvent();

}

public interface IDialogue
{
    public SO_DialogueData Data { get; set; }
    public float TextDelay { get; set; }

    public void AddOption(GameObject button);
}