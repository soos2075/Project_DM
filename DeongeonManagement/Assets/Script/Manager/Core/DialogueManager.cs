using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{

    #region singleton
    private static DialogueManager _instance;
    public static DialogueManager Instance { get { Initialize(); return _instance; } }

    private static void Initialize()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<DialogueManager>();
        }
    }
    #endregion

    private void Awake()
    {
        if (FindObjectOfType<DialogueManager>())
        {
            Destroy(this);
        }
        else
        {
            DontDestroyOnLoad(this);
        }
    }



    //? �������� �޴°Ÿ� ����Ʈ�� �ް� ��ųʸ��� ����س��� �ɵ�
    public List<SO_DialogueData> so_Datas;



    public SO_DialogueData GetDialogue(string dialogueName)
    {
        return SearchData(dialogueName);
    }

    SO_DialogueData SearchData(string searchName)
    {
        foreach (var item in so_Datas)
        {
            if (item.dialogueName == searchName)
            {
                return item;
            }
        }
        Debug.Log($"{searchName} ������ ����");
        return null;
    }
}
