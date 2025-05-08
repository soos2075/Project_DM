using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_BattleStatus", menuName = "ScriptableObjects/SO_BattleStatus")]
public class SO_BattleStatus : ScriptableObject, I_SO_Collection
{

    [Header("Default")]
    public int id;


    [Header("Content")]
    public BattleStatusLabel label;
    public int MaximumCount;


    [Header("CSV(Localization)")] //? external CSV Data
    public string labelName;
    public string detail;
}
