using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimation : MonoBehaviour
{


    public void Call_Mash()
    {
        Debug.Log("몬스터 어택");

        GetComponentInParent<BattleField>().Call_Mash();
    }

}
