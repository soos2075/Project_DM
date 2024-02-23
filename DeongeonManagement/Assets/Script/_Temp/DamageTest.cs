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

    }


    public void Spawn_Mana()
    {
        int ran = UnityEngine.Random.Range(1, 10);
        DamageNumber dn = damage.Spawn(transform.position, $"+{ran} mana");
        dn.SetColor(Color.blue);
    }
    public void Spawn_gold()
    {
        int ran = UnityEngine.Random.Range(1, 10);
        DamageNumber dn = damage.Spawn(transform.position, $"+{ran} gold");
        dn.SetColor(Color.yellow);
    }
    public void Spawn_fame()
    {
        int ran = UnityEngine.Random.Range(1, 10);
        DamageNumber dn = damage.Spawn(transform.position, $"+{ran} pop");
        DamageNumber dn2 = damage.Spawn(transform.position, $"+{ran} danger");
        dn.SetColor(Color.green);
        dn2.SetColor(Color.red);
    }

    public void SpawnRandomMesh()
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
