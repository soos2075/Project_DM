using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuildHelper : MonoBehaviour
{
    private static GuildHelper _instance;
    public static GuildHelper Instance { get { return _instance; } }


    private void Awake()
    {
        _instance = this;
    }


    public Camera mainCam;
    public Camera VIPCam;

    public void CamChange_ToVIP()
    {
        var fade = Managers.UI.ShowPopUp<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 4);

        mainCam.enabled = false;
        VIPCam.enabled = true;
    }
    public void CamChange_ToMain()
    {
        var fade = Managers.UI.ShowPopUp<UI_Fade>();
        fade.SetFadeOption(UI_Fade.FadeMode.BlackIn, 2);

        mainCam.enabled = true;
        VIPCam.enabled = false;
    }

    public Transform VIP_Room;
    public Transform Chair_Left;
    public Transform Chair_Right;

    public void VIP_Room_Talk(GameObject _Left, GameObject _Right, DialogueName dialogueName)
    {
        CamChange_ToVIP();

        var left = Make_Clone(_Left, Chair_Left);
        var right = Make_Clone(_Right, Chair_Right);
        right.GetComponentInChildren<SpriteRenderer>().flipX = true;

        //? 대화 끝나면 자동삭제하거나 정리하는 로직 필요
        Managers.Dialogue.ShowDialogueUI(dialogueName);
        FindAnyObjectByType<UI_DialogueBubble>().transform.localScale = Vector3.one * 0.015f;

        Managers.Dialogue.ActionReserve(() => {
            CamChange_ToMain();
            Managers.Resource.Destroy(left);
            Managers.Resource.Destroy(right);
        });
    }

    GameObject Make_Clone(GameObject origin, Transform pos)
    {
        if (origin.name == "Player")
        {
            return Managers.Resource.Instantiate("Player_Clone", pos);
        }
        else
        {
            GameObject go = Instantiate(origin, pos);
            go.transform.localPosition = Vector3.zero;
            go.GetComponentInChildren<BoxCollider2D>().enabled = false;
            go.GetComponentInChildren<Interaction_Guild>().enabled = false;
            go.GetComponentInChildren<SpriteOutline>().outlineSize = 0;
            return go;
        }
    }
 

    public GameObject Get_Current_Guild_NPC(GuildNPC_LabelName label)
    {
        var list = FindObjectsByType<Interaction_Guild>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (var item in list)
        {
            if (item.label == label)
            {
                return item.gameObject;
            }
        }
        return null;
    }




    public Sprite board_empty;
    public Sprite board_little;
    public Sprite board_many;


    public GameObject eventUI;
    public Sprite[] event_Icon;


    public enum Icon
    {
        Question_Yellow,
        Question_Red,
        Question_Blue,
        Question_Gray,

        Bang_Y,
        Bang_R,
        Bang_B,
        Bang_Gray,

        Exit,
        Dialogue,
    }

    public GameObject GetIcon(Icon icon)
    {
        GameObject ui = Instantiate(eventUI);
        ui.GetComponentInChildren<SpriteRenderer>().sprite = event_Icon[(int)icon];

        return ui;
    }
    public Sprite GetIconSprite(Icon icon)
    {
        return event_Icon[(int)icon];
    }


    public Transform[] posList;

    public enum Pos
    {
        Start,
        Exit,
        Board,
        Table1,
        Table2,
        Table3,
        Table4,

        Hero,
        DeathMagician,

        Center_Left,
        Center_Right,
    }

    public Transform GetPos(Pos pos)
    {
        return posList[(int)pos];
    }

}
