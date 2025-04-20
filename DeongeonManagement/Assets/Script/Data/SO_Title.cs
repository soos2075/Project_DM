using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "SO_Data", menuName = "SO_Content")]
public class SO_Title : ScriptableObject, I_SO_Collection
{
    [Header("Default")]
    public int id;
    public string keyName;



    [Header("Collection")]
    public bool isCollected;


    [Header("CSV(Localization)")] //? external CSV Data
    public string Title;

    [TextArea(3, 10)]
    public string Detail;

    [TextArea(3, 10)]
    public string Effect;

    [TextArea(3, 10)]
    public string Acquire;

}
