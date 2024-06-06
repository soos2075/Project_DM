using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "NPCData", menuName = "SO_NPC")]
public class SO_NPC : ScriptableObject
{
    [Header("Default")]
    public int id;
    public string keyName;
    public string prefabPath;

    [Header("Sprite_SLA")]
    public string SLA_category;
    public string SLA_label;


    [Header("Spawn")]
    public int Rank;
    public int MP;
    public int AP;
    public int HP;


    [Header("Status")]
    public int ATK;
    public int DEF;
    public int AGI;
    public int LUK;


    [Header("NPC")]
    public float groundSpeed;
    public float actionDelay;


    [Header("CSV(Localization)")] //? external CSV Data
    public string labelName;
    public string detail;


}
