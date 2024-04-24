using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Data", menuName = "SO_ContentsData")]
public class SO_Contents : ScriptableObject
{
    [Header("Default")]
    public int id;
    public string keyName;
    public string spritePath;


    [Header("Create")]
    public int UnlockRank;
    public int Mana;
    public int Gold;
    public int Ap;


    [Header("Unique")]
    public bool isOnlyOne;


    [Header("Contents")]
    //public UI_Floor.BuildMode mode;
    public Facility_Priority priority;
    public Define.Boundary Boundary_All;
    public List<Option> Options;

    public Action action;



    [Header("CSV(Localization)")] //? external CSV Data
    public string labelName;
    public string detail;
    public string boundary;

    [System.Serializable]
    public class Option
    {
        public Define.Boundary Boundary;
        public string FacilityKeyName;
    }
}


