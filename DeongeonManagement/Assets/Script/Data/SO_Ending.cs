using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "SO_Data", menuName = "SO_Ending")]
public class SO_Ending : ScriptableObject, I_SO_Collection
{
    [Header("Default")]
    public int id;
    public string keyName;


    [Header("CutSceneInfo")]
    //public int page;
    public List<CutScene> cutSceneList;


    [Header("Collection")]
    public bool isCollected;



    [System.Serializable]
    public class CutScene
    {
        //public int index;
        public Sprite sprite;
        public string dialogueName;
        public UI_EndingCanvas.Preset preset;
    }
}
