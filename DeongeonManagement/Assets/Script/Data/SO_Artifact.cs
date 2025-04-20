using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "SO_Artifact", menuName = "SO_Artifact")]
public class SO_Artifact : ScriptableObject, I_SO_Collection
{

    [Header("Default")]
    public int id;
    public string keyName;


    [Header("Sprite_SLA")]
    public string SLA_category;
    public string SLA_label;


    [Header("Option")]
    public bool View_Store;
    public bool View_Collection;
    public int MaxCount;



    [Header("Type")]
    public ArtifactGroup artifactType;




    [Header("CSV(Localization)")] //? external CSV Data
    public string labelName;
    public string detail;
    public string tooltip_Effect;

}
