using System;
using UnityEngine;

//[CreateAssetMenu(fileName = "Data", menuName = "SO_Data")]
public class SO_Technical : ScriptableObject
{
    [Header("Default")]
    public int id;
    public string keyName;

    public string prefabPath;
    public string spritePath;


    [Header("Create")]
    public int UnlockRank;
    public int Mana;
    public int Gold;
    public int Ap;

    public Action action;


    [Header("CSV(Localization)")] //? external CSV Data
    public string labelName;
    public string detail;


}
