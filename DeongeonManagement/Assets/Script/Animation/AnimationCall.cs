using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCall : MonoBehaviour
{

    public void Call_Mash()
    {
        Debug.Log("npc ����");

        GetComponentInParent<BattleField>().Call_Mash();
    }
}