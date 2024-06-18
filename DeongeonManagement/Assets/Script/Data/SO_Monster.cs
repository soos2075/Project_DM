using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "MonsterData", menuName = "SO_Monster")]
public class SO_Monster : ScriptableObject
{

    [Header("Default")]
    public int id;
    public string keyName;

    public string prefabPath;
    public string spritePath;


    [Header("Summon")]
    public int manaCost;
    public int unlockRank;
    public int startLv;
    public int maxLv;


    [Header("Status")]
    public int hp;
    public int atk;
    public int def;
    public int agi;
    public int luk;


    [Header("LevelUp")]
    public float up_hp;
    public float up_atk;
    public float up_def;
    public float up_agi;
    public float up_luk;

    [Header("Battle")]
    public int battleAp;
    public int maxBattleCount;
    public float battleInterval;


    [Header("CSV(Localization)")] //? external CSV Data
    public string labelName;
    public string detail;
    public string evolutionHint;
    public string evolutionDetail;
}
