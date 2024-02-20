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

        myAction -= () => Debug.Log("안되기만해봐 진짜 뒤졌다.");

        myAction += () => Debug.Log("안되기만해봐 진짜 뒤졌다.");
        myAction += () => Debug.Log("머지 원래 안됐는데 시발 머임???.");

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
