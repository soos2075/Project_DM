using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class UI_SaveLoad : UI_PopUp
{
    void Start()
    {
        Init();
    }


    enum Slot
    {
        SaveSlot_1,
        SaveSlot_2,
        SaveSlot_3,
        SaveSlot_4,
        SaveSlot_5,
        SaveSlot_6,
        AutoSave,
    }

    public enum Buttons
    {
        Close,
        Save,
        Load,
    }

    public Buttons State;

    public Sprite button_Down;
    public Sprite button_Up;


    public override void Init()
    {
        base.Init();


        Bind<Image>(typeof(Slot));
        Bind<Button>(typeof(Buttons));

        if (State == Buttons.Load)
        {
            GetButton(((int)Buttons.Save)).gameObject.SetActive(false);
            //GetButton(((int)Buttons.Load)).gameObject.SetActive(false);
            GetButton(((int)Buttons.Load)).GetComponent<Image>().sprite = button_Down;
            GetButton(((int)Buttons.Load)).GetComponentInChildren<TextMeshProUGUI>().margin = new Vector4(0, 12, 0, 0);

        }
        if (State == Buttons.Save)
        {
            SaveButton();
        }

        GetButton(((int)Buttons.Close)).gameObject.AddUIEvent((data) => ClosePopUp());

        GetButton(((int)Buttons.Save)).gameObject.AddUIEvent((data) => SaveButton());
        GetButton(((int)Buttons.Load)).gameObject.AddUIEvent((data) => LoadButton());



        for (int i = 0; i < Enum.GetNames(typeof(Slot)).Length; i++)
        {
            GetImage(i).gameObject.AddUIEvent((data) => SlotClickEvent(data.pointerCurrentRaycast.gameObject.transform.GetSiblingIndex()));
        }

        ShowDataInfo();
    }

    public void SetMode(Buttons saveMode)
    {
        State = saveMode;
    }


    void SaveButton()
    {
        State = Buttons.Save;
        GetButton(((int)Buttons.Save)).GetComponent<Image>().sprite = button_Down;
        GetButton(((int)Buttons.Save)).GetComponentInChildren<TextMeshProUGUI>().margin = new Vector4(0, 12, 0, 0);

        GetButton(((int)Buttons.Load)).GetComponent<Image>().sprite = button_Up;
        GetButton(((int)Buttons.Load)).GetComponentInChildren<TextMeshProUGUI>().margin = Vector4.zero;
    }
    void LoadButton()
    {
        State = Buttons.Load;
        GetButton(((int)Buttons.Load)).GetComponent<Image>().sprite = button_Down;
        GetButton(((int)Buttons.Load)).GetComponentInChildren<TextMeshProUGUI>().margin = new Vector4(0, 12, 0, 0);

        GetButton(((int)Buttons.Save)).GetComponent<Image>().sprite = button_Up;
        GetButton(((int)Buttons.Save)).GetComponentInChildren<TextMeshProUGUI>().margin = Vector4.zero;
    }


    void SlotClickEvent(int index)
    {
        index += 1;
        switch (State)
        {
            case Buttons.Save:
                if (index == 7)
                {
                    return;
                }
                Managers.Data.SaveToJson($"DM_Save_{index}", index);
                ShowDataInfo();
                break;

            case Buttons.Load:
                if (index == 7)
                {
                    if (Managers.Data.GetData($"AutoSave") == null)
                    {
                        return;
                    }
                    else
                    {
                        Managers.Scene.AddLoadAction_OneTime(() => Main.Instance.Default_Init());
                        Managers.Scene.AddLoadAction_OneTime(() => Managers.Data.LoadGame($"AutoSave"));
                        Managers.Scene.LoadSceneAsync(SceneName._2_Management);
                        return;
                    }
                }

                if (Managers.Data.GetData($"DM_Save_{index}") == null)
                {
                    return;
                }
                //ClosePopUp();
                Managers.Scene.AddLoadAction_OneTime(() => LoadAction(index));
                Managers.Scene.LoadSceneAsync(SceneName._2_Management);
                break;
        }
    }


    void ShowDataInfo()
    {
        for (int i = 1; i <= 6; i++)
        {
            var data = Managers.Data.GetData($"DM_Save_{i}");

            if (data != null)
            {
                GetImage(i - 1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{i}번 슬롯";
                GetImage(i - 1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{data.dateTime}";
                GetImage(i - 1).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = 
                    $"{data.turn}일차\n유명도 : {data.FameOfDungeon} / 위험도 : {data.DangerOfDungeon}";
            }
            else
            {
                GetImage(i - 1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{i}번 슬롯";
                GetImage(i - 1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"";
                GetImage(i - 1).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"데이터 없음";
            }
        }
        ShowAutoInfo();
    }
    void ShowAutoInfo()
    {
        var autodata = Managers.Data.GetData($"AutoSave");
        if (autodata != null)
        {
            GetImage(((int)Slot.AutoSave)).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"자동저장";
            GetImage(((int)Slot.AutoSave)).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{autodata.dateTime}";
            GetImage(((int)Slot.AutoSave)).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                $"{autodata.turn}일차\n유명도 : {autodata.FameOfDungeon} / 위험도 : {autodata.DangerOfDungeon}";
        }
        else
        {
            GetImage(((int)Slot.AutoSave)).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"자동저장";
            GetImage(((int)Slot.AutoSave)).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"";
            GetImage(((int)Slot.AutoSave)).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"데이터 없음";
        }

    }


    void LoadAction(int index)
    {
        Main.Instance.Default_Init();
        Managers.Data.LoadGame($"DM_Save_{index}");
    }
}
