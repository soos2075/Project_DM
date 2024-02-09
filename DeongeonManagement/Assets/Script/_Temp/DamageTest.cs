using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;

public class DamageTest : MonoBehaviour
{

    public DamageNumber damage;

    void Start()
    {
        
    }


    public void SpawnMesh()
    {
        int ran = Random.Range(1, 100);
        

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
