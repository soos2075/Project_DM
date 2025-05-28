using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "MonsterData", menuName = "SO_Monster")]
public class SO_Monster : ScriptableObject, I_SO_Collection
{

    [Header("Default")]
    public int id;
    public string keyName;

    public string prefabPath;
    //public string spritePath;

    [Header("Sprite_SLA")]
    public string SLA_category;
    public string SLA_label;


    [Header("Option")]
    public bool View_Store;
    public bool View_Collection;

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
    public float ActionInterval;
    public float moveSpeed;

    [Header("Attack")]
    public AttackType attackType;
    public string effectPrefabName;


    [Header("CSV(Localization)")] //? external CSV Data
    public string labelName;
    public string detail;
    public string evolutionHint;
    public string evolutionDetail;


    [Header("Trait")]
    //? 고유 특성 - 시작부터 가지고있는 특성
    public List<TraitGroup> traitList_Original;

    //? 경험 특성 - 행동으로 얻는 특성
    public List<TraitGroup> traitList_Exp;

    //? 랜덤 특성 - 레벨업 혹은 특수한 방법으로 얻을 수 있는 특성
    public List<TraitGroup> traitList_Random;
}
