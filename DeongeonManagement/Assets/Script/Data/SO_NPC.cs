using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "NPCData", menuName = "SO_NPC")]
public class SO_NPC : ScriptableObject, I_SO_Collection
{
    [Header("Default")]
    public int id;
    public string keyName;
    public string prefabPath;

    [Header("Sprite_SLA")]
    public string SLA_category;
    public string SLA_label;


    [Header("Option")]
    //public bool View_Store;
    public bool View_Collection;

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



    [Header("Trait")]
    public List<TraitGroup> NPC_TraitList;

    [Header("Prefer")] //? �ൿ �켱����
    public List<InteractionGroup> PreferList;
    public List<InteractionGroup> Non_PreferList;

    //[Header("FacilityTag")] //? ��ȣ�ۿ�� ���ʽ�ȿ���� ���̳ʽ�ȿ��
    //public List<TagGroup> FacilityTagList;
}
