using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "FacilityData", menuName = "SO_Facility")]
public class SO_Facility : ScriptableObject, I_SO_Collection
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
    //public bool View_Store;
    public bool View_Collection;
    public int MaxCount;


    [Header("Interaction")]
    public Facility.FacilityEventType Type;
    //public int OptionIndex;

    public bool isClearable;
    public bool isOnlyOne;


    [Header("Status")]
    public int interactionOfTimes;
    public float durationTime;
    public int ap_value;
    public int hp_value;
    public int mp_value;
    public int gold_value;
    public int pop_value;
    public int danger_value;

    [Header("Status")]
    public BattleStatusLabel battleStatus;
    public int bs_Value;


    [Header("Category")]
    public InteractionGroup category;

    [Header("Target(TypeClassName)")]
    public string main;

    public string sub;

    public string weak;

    public string invalid;

    [Header("NPCType")]
    public List<TraitGroup> bonusTarget; //? 1.3배
    //public List<NPCGroup> subTarget;
    public List<TraitGroup> weakTarget; //? 0.7배
    public List<TraitGroup> invalidTarget; //? 무효(0.1배? 혹은 0)




    [Header("CSV(Localization)")] //? external CSV Data
    public string labelName;
    public string detail;




}
