using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interface 
{
    //? ���彺���̽��� �����ϴ� ui�� ĵ���� ����
    public interface IWorldSpaceUI 
    {
        public void SetCanvasWorldSpace();
    }

    //? Basement�� ��ġ�� ������ ������Ʈ
    public interface IPlacementable
    {
        Define.PlacementType PlacementType { get; set; }
    }


}
