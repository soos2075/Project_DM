using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "SO_Trait", menuName = "SO_Trait")]
public class SO_Trait : ScriptableObject, I_SO_Collection
{
    [Header("Default")]
    public int id;
    public string traitName;

    [Header("Trait")]
    public TraitRating rating;
    public TraitGroup trait;

    [Header("SLA")]
    public string SLA_Category;
    public string SLA_Label;


    [Header("Collection")]
    public bool isCollected;


    [Header("CSV(Localization)")] //? external CSV Data
    public string labelName;

    [TextArea(3, 10)]
    public string detail;

    [TextArea(3, 10)]
    public string Acquire;
}