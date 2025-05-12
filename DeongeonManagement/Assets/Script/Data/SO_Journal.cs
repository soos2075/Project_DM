using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Journal", menuName = "ScriptableObjects/Journal")]
public class SO_Journal : ScriptableObject
{
    [Header("Default")]
    public int ID;


    [Header("Option")]
    public bool fixedEvent;
    public int dayInfo;




    [Header("CSV(Localization)")]
    public string title;
    public string description;


}