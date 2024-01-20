using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interface 
{
    //? 월드스페이스에 존재하는 ui들 캔버스 설정
    public interface IWorldSpaceUI 
    {
        public void SetCanvasWorldSpace();
    }

    //? Basement에 배치가 가능한 오브젝트
    public interface IPlacementable
    {
        Define.PlacementType PlacementType { get; set; }
    }


}
