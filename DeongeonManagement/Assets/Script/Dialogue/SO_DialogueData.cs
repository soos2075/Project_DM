using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SO_DialogueData : ScriptableObject
{


    public string title;


    [SerializeField]
    [TextArea(5, 100)]
    public List<string> mainTextList = new List<string>();

    
    //public string mainText;





}
