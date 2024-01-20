using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herb_Low : Facility
{
    public override void Interaction_NPC()
    {
        Debug.Log("1만큼의 마나를 소진, 약초가 1만큼 사라짐");
    }

}
