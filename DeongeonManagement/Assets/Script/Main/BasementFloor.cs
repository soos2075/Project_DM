using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasementFloor : MonoBehaviour
{

    public string Floor { get; set; }

    public string Name_KR;

    public int Size { get; set; } = 1;

    public BoxCollider2D boxCollider;

    public List<NPC> npcList;

    public Monster placementMonster;

    public Facility placementFacility;


    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        Floor = gameObject.name;

    }

    void Update()
    {
        
    }



}
