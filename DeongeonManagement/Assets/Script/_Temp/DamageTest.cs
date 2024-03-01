using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;
using System;

public class DamageTest : MonoBehaviour
{

    public DamageNumber damage;


    public Action myAction = null;


    int a = 10;

    int b;
    int AA { get { return a; } set { a = value; } }

    void CopyTest1()
    {
        b = AA;

        Debug.Log("1 =" + b);

        AA = 20;
        Debug.Log("2 =" + b);
    }


    class myClass
    {
        public List<int> A = new List<int> { 1, 2, 3 };

    }
    List<myClass> list1 = new List<myClass> { new myClass(), new myClass(), new myClass() };
    List<myClass> list2;
    void 얕은복사()
    {
        list2 = new List<myClass>(list1);
        list1[2].A[2] = 3333;
        Debug.Log(list2[2].A[2]);
    }
    void 깊은복사()
    {
        list2 = list1;
        list1[2].A[2] = 3333;
        Debug.Log(list2[2].A[2]);
    }
    void Start()
    {
        얕은복사();
        깊은복사();

        List<int> list1 = new List<int> { 1, 2, 3 };
        List<int> list2 = new List<int>(list1);
        list1[0] = 100; // list1을 변경하면 list2도 영향을 받음
        Debug.Log(list2[0]); // 출력 결과는 100
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
