using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimation : MonoBehaviour
{


    public void Call_Mash()
    {
        Debug.Log("���� ����");

        GetComponentInParent<BattleField>().Call_Mash();
    }

}
