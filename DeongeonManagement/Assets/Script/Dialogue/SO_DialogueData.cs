using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SO_DialogueData : ScriptableObject
{

    public string dialogueName;

    [SerializeField]
    public List<TextData> TextDataList = new List<TextData>();


    [System.Serializable]
    public class TextData
    {
        [TextArea(1, 100)]
        public string optionString;
        [TextArea(5, 100)]
        public string mainText;
    }
}
