using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;
using System;

public class DamageTest : MonoBehaviour
{

    public DamageNumber damage;


    public Action myAction = null;
    void Start()
    {
        Debug.Log(myAction);

        myAction -= () => Debug.Log("�ȵǱ⸸�غ� ��¥ ������.");

        myAction += () => Debug.Log("�ȵǱ⸸�غ� ��¥ ������.");
        myAction += () => Debug.Log("���� ���� �ȵƴµ� �ù� ����???.");

        myAction.Invoke();
    }


    public void SpawnMesh()
    {
        int ran = UnityEngine.Random.Range(1, 100);
        

        if (ran > 75)
        {
            DamageNumber dn = damage.Spawn(transform.position, ran);
            dn.SetScale(1.2f);
        }
        else if (ran < 25)
        {
            DamageNumber dn = damage.Spawn(transform.position, "Miss");
        }
        else
        {
            DamageNumber dn = damage.Spawn(transform.position, ran);
        }
    }




}
