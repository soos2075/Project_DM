using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu]
[System.Obsolete]
public class SO_DialogueData : ScriptableObject
{
    public enum DialogueType
    {
        Quest,
        Bubble,
    }    


    public string dialogueName;
    public DialogueType Type = DialogueType.Quest;


    [SerializeField]
    public List<TextData> TextDataList = new List<TextData>();


    [System.Serializable]
    public class TextData
    {
        [TextArea(1, 100)]
        public string optionString;
        [TextArea(3, 100)]
        public string mainText;
    }
}
