using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SO_DialogueData : ScriptableObject
{

    public string dialogueName;

    //public string title;

    //[SerializeField]
    //[TextArea(5, 100)]
    //public List<string> mainTextList = new List<string>();


    [SerializeField]
    public List<TextData> TextDataList = new List<TextData>();


    [System.Serializable]
    public class TextData
    {
        public Sprite sprite;
        public string name;
        [TextArea(5, 100)]
        public string mainText;
    }
}
